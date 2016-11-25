using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class praktikan
    {
        [DataMember]
        public string NRP { get; set; }

        [DataMember]
        public string Nama { get; set; }

        [DataMember]
        public int AngkatanID { get; set; }

        [DataMember]
        public int JurusanID { get; set; }

        [DataMember]
        public Byte[] Foto { get; set; }

        [DataMember]
        public string Notes { get; set; }
    }
}
