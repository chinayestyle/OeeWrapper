using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OeeWrapper.Model;
//using Interop.OeeCli;

namespace OeeWrapper.Controller
{
    public class OeeCliCounterWrapper
    {
        public static List<OeeCounter> GetCounters()
        {
            try
            {
                List<OeeCounter> counters = new List<OeeCounter>();               // The cluster types
                OEECli.IOEERuntime oeecli_rt = new OEECli.OEERuntime();     // OEECli

                // Declare some variables 
                //
                object instanceId = null;
                int instance_timeout_ms = 500;
                OEECli.OEE_RETURN_VALUE rv = OEECli.OEE_RETURN_VALUE.OEE_SUCCESS;

                // Initialize OEECli
                //
                rv = oeecli_rt.ResizeFreeSet(1);
                if (rv != OEECli.OEE_RETURN_VALUE.OEE_SUCCESS)
                    throw new Exception("internal error - no more memory"); // return; // handle error - no more memory
                rv = oeecli_rt.GetInstance(out instanceId, instance_timeout_ms);
                if (rv != OEECli.OEE_RETURN_VALUE.OEE_SUCCESS)
                    throw new Exception("internal error - all instances are busy, try to increase ");  //return; // handle error - all instances are busy, try to increase 


                // Retrieve counters
                //
                object counters_obj = null;
                rv = oeecli_rt.GetCounterList(instanceId, out counters_obj, OEECli.OEE_VERSION.OEE_V4);
                if (rv != OEECli.OEE_RETURN_VALUE.OEE_SUCCESS)
                    throw new Exception("internal error - handle error ");  //return; // handle error

                // Convert result object to List<Counter>
                //
                object[] variantArray = (object[])counters_obj;
                if (variantArray.Length == 0)
                    throw new Exception("internal error - no data ");  //return; // handle error - no data
                object[] data = (object[])(variantArray[0]);

                counters.Capacity = data.Length;
                for (int i = 0; data != null && i < data.Length; ++i)
                {
                    OeeCounter elem = new OeeCounter();

                    object[] elem_obj = (object[])data[i];
                    int j = 0;
                    elem.id = (int)elem_obj[j++];
                    elem.name = (String)elem_obj[j++];
                    elem.applyToAll = (bool)elem_obj[j++];
                    elem.description = (String)elem_obj[j++];
                    elem.type = (int)elem_obj[j++];
                    elem.energyType = (int)elem_obj[j++];
                    elem.materialLink = (String)elem_obj[j++];
                    elem.materialLinkType = (int)elem_obj[j++];

                    counters.Add(elem);
                }

                // Do something with counters ..
                //
                /*
                foreach (var ctr in counters)
                {
                    Console.WriteLine(String.Format("Counter id:{0} name:\"{1}\"", ctr.id, ctr.name));
                }
                */

                oeecli_rt = null;
                return counters == null ? new List<OeeCounter>() : counters;
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }

        public static void AcquireCounterSample(CounterRecord ctrRecord, double dCtrValue, short qualityValue)
        {
            try
            {
                string strError;

                // Create OEECli
                OEECli.IOEECollector oeecli_collector = new OEECli.OEECollector();

                // Register to OeeCli Collector Interface
                long acquisitionProviderInfo = 2; // Acquisition Gsi
                OEECli.OEE_RETURN_VALUE oeeRetVal = oeecli_collector.Register(acquisitionProviderInfo, OEECli.OEE_VERSION.OEE_V1);

                if (oeeRetVal != OEECli.OEE_RETURN_VALUE.OEE_SUCCESS)
                {
                    oeecli_collector.OEEGetLastError(out strError);
                    throw new Exception(strError);
                }

                ArrayList eqItem = new ArrayList();
                ArrayList ctrItem = new ArrayList();
                ArrayList sampleItem = new ArrayList();

                // build custom field list
                ArrayList customList = new ArrayList();
                foreach (CustomFieldValueEntity customFieldEntity in ctrRecord.CustomFields)
                {
                    customList.Add(customFieldEntity.CustomFieldName);
                    customList.Add(customFieldEntity.Value);
                }

                double startTime = ctrRecord.StartTime.ToOADate();
                double endTime = (ctrRecord.EndTime.HasValue) ? ctrRecord.EndTime.Value.ToOADate() : 0;

                eqItem.Add(ctrRecord.EquipmentId);
                eqItem.Add(0); // LongIdType

                ctrItem.Add(1); // 1=ctr  2=dsp
                ctrItem.Add(ctrRecord.CtrId);
                ctrItem.Add(0); // LongIdType

                sampleItem.Add(eqItem.ToArray());
                sampleItem.Add(ctrItem.ToArray()); // idItem to be filled only for ctr dsp
                sampleItem.Add(startTime); // startTime
                sampleItem.Add(endTime);  // endTime
                sampleItem.Add(dCtrValue); // ctr value
                sampleItem.Add(customList.ToArray()); // custom field list
                sampleItem.Add(qualityValue); // quality

                // Call ProcessSample method
                OEECli.OEE_RETURN_VALUE returnValue = oeecli_collector.ProcessSample(sampleItem.ToArray(), OEECli.OEE_EXECUTION_MODE.OEE_SYNC, OEECli.OEE_VERSION.OEE_V1);

                if (returnValue != OEECli.OEE_RETURN_VALUE.OEE_SUCCESS)
                {
                    oeecli_collector.OEEGetLastError(out strError);
                    throw new Exception(strError);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
