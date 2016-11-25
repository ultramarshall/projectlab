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
        int GetLogin(akun data);

        [OperationContract]
        List<jurusan> GetJurusan();

        [OperationContract]
        List<angkatan> GetAngkatan();

        [OperationContract]
        List<praktikan> GetPraktikan(praktikan data);
    }
}
