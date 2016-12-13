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
        public jurusan jurusan { get; set; }

        [DataMember]
        public angkatan angkatan { get; set; }

        [DataMember]
        public Byte[] Foto { get; set; }

        [DataMember]
        public string Notes { get; set; }
    }
}
