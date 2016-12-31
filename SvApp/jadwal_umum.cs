using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SvApp {

    [DataContract] public class jadwal_umum {

        [DataMember] public int id_jadwal_umum { get; set; }

        [DataMember] public string hari { get; set; }

        [DataMember] public int id_kelas { get; set; }

        [DataMember] public int id_periode { get; set; }

        [DataMember] public string id_shift { get; set; }

        [DataMember] public string kode_mk { get; set; }

        [DataMember] public Shift fk_jadwalUmum_Shift { get; set; }

        [DataMember] public matkul fk_jadwalUmum_matakuliah { get; set; }

        [DataMember] public kelas fk_jadwalUmum_kelas { get; set; }

        [DataMember] public periode fk_jadwalUmum_periode { get; set; }

    }

}