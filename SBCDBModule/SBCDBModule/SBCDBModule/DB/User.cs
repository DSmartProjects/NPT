using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBCDBModule.DB
{
  public  class User 
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
        public string Password { get; set; }
        public string Passwordsalt { get; set; }
    }

    public class PairedDevice
    {

    }

    public class DeviceType
    {
        public int ID { get; set; }
        public string DeviceTypeName { get; set; }
        public string Description { get; set; }
        public string CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string Modifydate { get; set; }
        public string ModifyUser { get; set; }
        public string DeviceName { get; set; }
        public string CharacteristicAttributeid { get; set; }
        public string ServiceID { get; set; }
        
    }
}
