using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;

namespace SvApp
{
    [ServiceContract]
    public interface Iadm
    {
        [OperationContract]
        string GetLogin(akun data);

        [OperationContract]
        List<jadwalPraktikan> getTimeLogin(jadwalPraktikan data);

        [OperationContract]
        matkul getPraktikanPraktikum(jadwalPraktikan data);

        [OperationContract]
        praktikan getProfilePraktikan(praktikan data);

        [OperationContract]
        List<jurusan> GetJurusan();

        [OperationContract]
        List<angkatan> GetAngkatan();

        [OperationContract]
        List<praktikan> GetPraktikan(praktikan data);

        [OperationContract]
        int InsertMultiplePraktikan(List<praktikan> data);

        [OperationContract]
        List<jadwal_umum> ViewJadwalUmum(jadwal_umum data);

        [OperationContract]
        List<matkul> GetMatKul();

        [OperationContract]
        List<kelas> GetKelas();

        [OperationContract]
        List<Shift> GetShift();

        [OperationContract]
        int InsertJadwal(List<jadwal_umum> data);

        [OperationContract]
        int DeleteJadwal(jadwal_umum data);

        [OperationContract]
        List<Staff> getStaffID();

        [OperationContract]
        Staff getProfileStaff(Staff data);

        [OperationContract]
        List<jadwalStaff> GetStaffJadwal(string data);

        [OperationContract]
        List<jadwalStaff> jadwalUmumStaff(periode data);

        [OperationContract]
        int addPeriode(periode data);

        [OperationContract]
        List<periode> viewPeriode();
    }
}
