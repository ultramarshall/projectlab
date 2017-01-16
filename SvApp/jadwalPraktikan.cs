using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class jadwalPraktikan
    {
        [DataMember]
        public int id_jadwal_praktikan { get; set; }

        [DataMember]
        public string nrp { get; set; }

        [DataMember]
        public praktikan praktikan { get; set; }

        [DataMember]
        public jadwal_umum id_jadwal_umum { get; set; }

        [DataMember]
        public AbsensiPraktikan absen { get; set; }
    }
}