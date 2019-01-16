using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OeeWrapper.Model
{
    public class Alarm
    {
        public int id;
        public String Description;
        public Int64 instanceId;
        public Guid EquipmentGUID;
        public String EquipmentPath;
    }
}
