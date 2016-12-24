using System;
using System.Collections.Generic;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Text.RegularExpressions;

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
            var service = new IadmClient();
            comboBoxEdit.Properties.Items.Clear();
            if (option == "Jurusan")
            {
                var jurusan = service.GetJurusan();
                for (var i = 0; i < jurusan.Count(); i++) // Add Jurusan
                {
                    comboBoxEdit.Properties.Items.Add(jurusan[i].NamaJurusan);
                }
            }
            if (option == "Angkatan")
            {
                var angkatan = service.GetAngkatan();
                for (var i = 0; i < angkatan.Count(); i++) // Add Angkatan
                {
                    comboBoxEdit.Properties.Items.Add(angkatan[i].TahunAngkatan);
                }
            }
            if (option == "Periode")
            {
                var periode = service.viewPeriode().Select(z => new { start = z.awalSemester.ToString("yyyy"), finish = z.akhirSemester.ToString("yyyy") }).Distinct().ToList();
                for (int i = 0; i < periode.Count(); i++)
                {
                    comboBoxEdit.Properties.Items.Add($"{periode[i].start:yyyy}/{periode[i].finish:yyyy}");
                }
            }
            if (option == "Semester")
            {
                var semester = service.viewPeriode().Select(z => z.semester).Distinct().ToList();

                for (var i = 0; i < semester.Count(); i++)
                {
                    comboBoxEdit.Properties.Items.Add(semester[i]);
                }
            }
            comboBoxEdit.SelectedIndex = 0;
            service.Close();
        }

        public static void CariPraktikan(ComboBoxEdit comboBoxEdit1, ComboBoxEdit comboBoxEdit2, GridControl gridcontrol, GridView gridview)
        {
            try
            {
                var service = new IadmClient();
                comboBoxEdit1.SelectedIndex = 0;
                comboBoxEdit2.SelectedIndex = 0;
                var nmAngkatan = comboBoxEdit1.SelectedItem.ToString();
                var nmJurusan = comboBoxEdit2.SelectedItem.ToString();
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.TahunAngkatan == nmAngkatan);
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.NamaJurusan == nmJurusan);

                praktikan data = new praktikan()
                {
                    angkatan= new angkatan() { KodeAngkatan = angkatan?.KodeAngkatan },
                    jurusan = new jurusan() { KodeJurusan = jurusan?.KodeJurusan }
                };
                gridcontrol.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama, KodeAngkatan = x.angkatan.KodeAngkatan, KodeJurusan = x.jurusan.KodeJurusan}).ToList();
                gridview.RowHeight = 60;
                gridview.Columns["Foto"].Width = 70;
                gridview.Columns["NRP"].Width = 150;
               
                gridview.Columns["NRP"].Caption = @"NO MAHASISWA";
                gridview.Columns["Foto"].Caption = @"FOTO";
                gridview.Columns["Nama"].Caption = @"NAMA";
                gridview.Columns["KodeAngkatan"].Caption = @"ANGKATAN";
                gridview.Columns["KodeJurusan"].Caption = @"JURUSAN";

                for (int i = 0; i < gridview.Columns.Count; i++)
                {
                    gridview.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
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

        public static bool Like(this string toSearch, string toFind)
        {
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }
    }
}
