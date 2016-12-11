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

        [DataMember]
        public string nama { get; set; }

        [DataMember]
        public byte[] foto { get; set; }

        [DataMember]
        public string no_hp { get; set; }

        [DataMember]
        public string alamat { get; set; }
    }
}
