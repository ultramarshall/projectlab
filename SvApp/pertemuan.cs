using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp {

    [DataContract] public class pertemuan {

        [DataMember] public int id_pertemuan { get; set; }

        [DataMember] public string id_jenis_pertemuan { get; set; }

    }

}