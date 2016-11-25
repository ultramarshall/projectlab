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
        public int JurusanID { get; set; }

        [DataMember]
        public string Jurusan { get; set; }
    }
}
