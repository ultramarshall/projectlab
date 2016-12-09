using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class Staff
    {
        [DataMember]
        public string id_staff { get; set; } 
    }
}
