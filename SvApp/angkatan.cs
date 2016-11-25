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
        public int AngkatanID { get; set; }

        [DataMember]
        public string Angkatan { get; set; }
    }
}
