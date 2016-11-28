using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class jurusan
    {
        [DataMember]
        public string KodeJurusan { get; set; }

        [DataMember]
        public string NamaJurusan { get; set; }
    }
}
