using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class jadwalStaff
    {
        [DataMember]
        public string hari { get; set; }

        [DataMember]
        public string mata_kuliah { get; set; }

        [DataMember]
        public string kelas { get; set; }

        [DataMember]
        public string shift { get; set; }

        [DataMember]
        public string waktu { get; set; }
    }
}
