using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;

namespace client.Library
{
    public static class Method
    {
        public static List<jurusan> ListJurusan = new List<jurusan>();
        public static string PostLogin(string username, string password)
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
                string roles = service.GetLogin(user).TrimEnd();
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

        public static void CariPraktikan(ComboBoxEdit ComboBoxEdit1, ComboBoxEdit ComboBoxEdit2, GridControl gridcontrol, GridView Gridview)
        {
            try
            {
                IadmClient service = new IadmClient();
                
                string nmAngkatan = ComboBoxEdit1.SelectedItem.ToString();
                string nmJurusan = ComboBoxEdit2.SelectedItem.ToString();
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.TahunAngkatan == nmAngkatan);
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.NamaJurusan == nmJurusan);

                praktikan data = new praktikan()
                {
                    KodeAngkatan = angkatan.KodeAngkatan,
                    KodeJurusan = jurusan.KodeJurusan
                };
                gridcontrol.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama, x.KodeAngkatan, x.KodeJurusan}).ToList();
                Gridview.RowHeight = 60;
                Gridview.Columns["Foto"].Width = 70;
                Gridview.Columns["NRP"].Width = 150;
               
                Gridview.Columns["NRP"].Caption = "NO MAHASISWA";
                Gridview.Columns["Foto"].Caption = "FOTO";
                Gridview.Columns["Nama"].Caption = "NAMA";
                Gridview.Columns["KodeAngkatan"].Caption = "ANGKATAN";
                Gridview.Columns["KodeJurusan"].Caption = "JURUSAN";

                for (int i = 0; i < Gridview.Columns.Count; i++)
                {
                    Gridview.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                }
                
                    
                service.Close();
            }
            catch(Exception err)
            {
                XtraMessageBox.Show(err.ToString());
                XtraMessageBox.Show("data pencarian tidak lengkap");
            }

        }
    }
}
