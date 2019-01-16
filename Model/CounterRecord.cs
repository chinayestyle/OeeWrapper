using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OeeWrapper.Model
{
    public class CounterRecord
    {

        public DateTime StartTime;
        public Nullable<DateTime> EndTime;
        public int EquipmentId;
        public int CtrId;

        public String ProductId;
        public List<CustomFieldValueEntity> CustomFields;

        public int EngineeringVersionId;
        public int ObjectId;
        public String Comment;
        public int FlagValid;
        public int FlagActions;

        public Alarm rootAlarm;

        public CounterRecord(int equipmentId, int ctrId, string productId, int versionID, int objectId, string comment, int flagValid, int flagActions)
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            EquipmentId = equipmentId;
            CtrId = ctrId;
            ProductId = productId;
            EngineeringVersionId = versionID;
            ObjectId = objectId;
            Comment = comment;
            FlagActions = flagActions;
            FlagValid = flagValid;
            CustomFields = new List<CustomFieldValueEntity>();
        }
    }
}
