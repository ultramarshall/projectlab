using System.Runtime.Serialization;

namespace SvApp
{
    [DataContract]
    public class jurusan
    {
        [DataMember]
        public string KodeJurusan { get; set; }

        [DataMember]
        public string NamaJurusan { get; set; }
    }
}