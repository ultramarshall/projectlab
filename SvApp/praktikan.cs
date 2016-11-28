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
        public string KodeJurusan { get; set; }

        [DataMember]
        public string KodeAngkatan { get; set; }

        [DataMember]
        public Byte[] Foto { get; set; }

        [DataMember]
        public string Notes { get; set; }
    }
}
