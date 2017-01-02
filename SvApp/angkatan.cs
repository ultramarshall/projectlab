using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class angkatan
    {
        [DataMember]
        public string KodeAngkatan { get; set; }

        [DataMember]
        public string TahunAngkatan { get; set; }
    }
}