using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp
{
    [DataContract]
    public class upload_file
    {
        [DataMember]
        public int id_file { get; set; }

        [DataMember]
        public int id_absensi { get; set; }

        [DataMember]
        public string nama_file { get; set; }

        [DataMember]
        public string lokasi_file { get; set; }

        [DataMember]
        public byte[] data_file { get; set; }
    }
}
