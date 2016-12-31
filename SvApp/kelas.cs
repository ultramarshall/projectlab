using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp {

    [DataContract] public class kelas {

        [DataMember] public int id_kelas { get; set; }

        [DataMember] public string Kelas { get; set; }

    }

}