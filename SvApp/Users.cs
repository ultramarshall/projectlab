using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp {

    [DataContract] public class Users {

        [DataMember] public string username { get; set; }

        [DataMember] public string password { get; set; }

        [DataMember] public string status { get; set; }

    }

}