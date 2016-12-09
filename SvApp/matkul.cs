using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class matkul
    {
        [DataMember]
        public string kode_mk { get; set; }
        [DataMember]
        public string mata_kuliah { get; set; }
    }
}
