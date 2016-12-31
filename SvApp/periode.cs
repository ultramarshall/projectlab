using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp {

    [DataContract] public class periode {

        [DataMember] public int id_periode { get; set; }

        [DataMember] public string semester { get; set; }

        [DataMember] public DateTime awalSemester { get; set; }

        [DataMember] public DateTime akhirSemester { get; set; }

    }

}