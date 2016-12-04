using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class jadwal_umum
    {
        [DataMember]
        public int id_jadwal_umum { get; set; }

        [DataMember]
        public string hari { get; set; }

        [DataMember]
        public string semester { get; set; }

        [DataMember]
        public string kelas { get; set; }

        [DataMember]
        public string tahun_akademik { get; set; }


        [DataMember]
        public int id_shift { get; set; }


        [DataMember]
        public string kode_mk { get; set; }
    }
}
