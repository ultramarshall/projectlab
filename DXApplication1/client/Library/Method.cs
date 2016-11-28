using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using client.lab;
using DevExpress.XtraEditors;

namespace client.Library
{
    public static class Method
    {
        public static List<jurusan> ListJurusan = new List<jurusan>();
        public static int PostLogin(string username, string password)
        {
            try
            {
                IadmClient service = new IadmClient();
                List<akun> data = new List<akun>();

                akun user = new akun()
                {
                    Username = username,
                    Password = password
                };
                int roles = service.GetLogin(user);
                service.Close();
                return roles;
            } 
            catch (Exception error)
            {
                throw;
            }
        }

        public static void ComboBoxEditAdd(string JurusanOrAngkatan ,ComboBoxEdit ComboBoxEdit)
        {
            IadmClient service = new IadmClient();
            if(JurusanOrAngkatan == "Jurusan")
            {
                ComboBoxEdit.Properties.Items.Clear();
                var jurusan = service.GetJurusan();
                for (int i = 0; i < jurusan.Count(); i++) // Add Jurusan
                {
                    ComboBoxEdit.Properties.Items.Add(jurusan[i].NamaJurusan);
                }
            }
            if (JurusanOrAngkatan == "Angkatan")
            {
                ComboBoxEdit.Properties.Items.Clear();
                var angkatan = service.GetAngkatan();
                for (int i = 0; i < angkatan.Count(); i++) // Add Angkatan
                {
                    ComboBoxEdit.Properties.Items.Add(angkatan[i].TahunAngkatan);
                }
            }
            service.Close();
        }
    }
}
