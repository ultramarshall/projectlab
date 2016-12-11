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
            catch (Exception)
            {
                throw;
            }
        }

        public static void ComboBoxEditAdd(string option ,ComboBoxEdit comboBoxEdit)
        {
            IadmClient service = new IadmClient();
            comboBoxEdit.Properties.Items.Clear();
            if (option == "Jurusan")
            {
                var jurusan = service.GetJurusan();
                for (int i = 0; i < jurusan.Count(); i++) // Add Jurusan
                {
                    comboBoxEdit.Properties.Items.Add(jurusan[i].NamaJurusan);
                }
            }
            if (option == "Angkatan")
            {
                var angkatan = service.GetAngkatan();
                for (int i = 0; i < angkatan.Count(); i++) // Add Angkatan
                {
                    comboBoxEdit.Properties.Items.Add(angkatan[i].TahunAngkatan);
                }
            }
            if (option == "Periode")
            {
                var _periode = service.viewPeriode().Select(z => new { start = z.awalSemester.ToString("yyyy"), finish = z.akhirSemester.ToString("yyyy") }).Distinct().ToList();
                for (int i = 0; i < _periode.Count(); i++)
                {
                    comboBoxEdit.Properties.Items.Add(string.Format("{0:yyyy}/{1:yyyy}",
                                                           _periode[i].start,
                                                           _periode[i].finish));
                }
            }
            if (option == "Semester")
            {
                var _semester = service.viewPeriode().Select(z => z.semester).Distinct().ToList();

                for (int i = 0; i < _semester.Count(); i++)
                {
                    comboBoxEdit.Properties.Items.Add(_semester[i]);
                }
            }
            comboBoxEdit.SelectedIndex = 0;
            service.Close();
        }

        public static void CariPraktikan(ComboBoxEdit ComboBoxEdit1, ComboBoxEdit ComboBoxEdit2, GridControl gridcontrol, GridView Gridview)
        {
            try
            {
                IadmClient service = new IadmClient();
                ComboBoxEdit1.SelectedIndex = 0;
                ComboBoxEdit2.SelectedIndex = 0;
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
            catch(Exception)
            {
                //XtraMessageBox.Show(err.ToString());
                //XtraMessageBox.Show("data pencarian tidak lengkap");
                throw;
            }

        }

       
    }
}
