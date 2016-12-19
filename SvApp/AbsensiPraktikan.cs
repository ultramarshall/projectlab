using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class AbsensiPraktikan
    {
        [DataMember]
        public int id_absensi { get; set; }

        [DataMember]
        public pertemuan Pertemuan { get; set; }

        [DataMember]
        public jadwalPraktikan JadwalPraktikan { get; set; }

        [DataMember]
        public Staff staff { get; set; }

        [DataMember]
        public int Nilai { get; set; }

        [DataMember]
        public DateTime waktu_absensi { get; set; }
    }
}
