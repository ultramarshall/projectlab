using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class Shift
    {
        [DataMember]
        public string id_shift { get; set; }

        [DataMember]
        public string waktu { get; set; }

        [DataMember]
        public DateTime mulai { get; set; }

        [DataMember]
        public DateTime selesai { get; set; }
    }
}