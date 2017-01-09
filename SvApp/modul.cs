using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class modul
    {
        [DataMember]
        public int id_modul { get; set; }

        [DataMember]
        public matkul matkul { get; set; }

        [DataMember]
        public string file_modul { get; set; }

        [DataMember]
        public byte[] lokasi_modul { get; set; }
    }
}
