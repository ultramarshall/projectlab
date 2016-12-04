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
        List<jurusan> GetJurusan();

        [OperationContract]
        List<angkatan> GetAngkatan();

        [OperationContract]
        List<praktikan> GetPraktikan(praktikan data);

        [OperationContract]
        int InsertMultiplePraktikan(List<praktikan> data);

        [OperationContract]
        List<jadwal_umum> ViewJadwalUmum(List<jadwal_umum> data);
    }
}
