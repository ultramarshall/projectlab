using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using client.lab;
using static client.Library.border;
using static client.Library.Method;
namespace client
{
    public partial class FrmTambahJadwalUmum : XtraForm
    {
        public FrmTambahJadwalUmum()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            ComboBoxEditAdd("Semester", comboBoxEdit1);
            ComboBoxEditAdd("Periode", comboBoxEdit2);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new DataTable();


            jadwal.Columns.Add("Hari", typeof(string));
            jadwal.Columns.Add("Shit", typeof(string));
            jadwal.Columns.Add("jam", typeof(string));
            jadwal.Columns.Add("Praktikum", typeof(string));
            jadwal.Columns.Add("Kelas", typeof(string));


            string[] hari = { "Senin", "Selasa", "Rabu", "Kamis", "Jumat" };
            string[] shift = { "I", "II", "III", "IV" };
            string[] jam = { "08:00 - 09:40", "09:50 - 11:30", "11:40 - 13:20", "13:30 - 15:10" };
            foreach (string t in hari)
            {
                for (var s = 0; s < shift.Length; s++)
                {
                    jadwal.Rows.Add(t, shift[s], jam[s], null, null);
                }
            }

            gridControl1.DataSource = jadwal;
            gridView1.OptionsView.AllowCellMerge = true;
            gridView1.Columns["Praktikum"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            gridView1.Columns["Kelas"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            gridControl1.ForceInitialize();

            var cmbbxPraktikum = new RepositoryItemComboBox()
            {
                TextEditStyle = TextEditStyles.DisableTextEditor
            };
            var cmbbxMk = new RepositoryItemComboBox()
            {
                TextEditStyle = TextEditStyles.DisableTextEditor
            };
            var mk = service.GetMatKul();
            var praktikum = new string[mk.Length];
            for (var i = 0; i < mk.Length; i++)
            {
                praktikum[i] = mk[i].mata_kuliah;
            }
            cmbbxPraktikum.Items.AddRange(praktikum);

            var kls = service.GetKelas();
            var kelas = new string[kls.Length];
            var count = kls.Length;
            for (var i = 0; i < count; i++)
            {
                kelas[i] = kls[i].Kelas;
            }
            cmbbxMk.Items.AddRange(kelas);

            gridControl1.RepositoryItems.Add(cmbbxPraktikum);
            gridControl1.RepositoryItems.Add(cmbbxMk);


            gridView1.Columns["Praktikum"].ColumnEdit = cmbbxPraktikum;
            gridView1.Columns["Kelas"].ColumnEdit = cmbbxMk;
            
        }

        private void Batal(object sender, EventArgs e)
        {
            Close();
        }

        private void Simpan(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new List<jadwal_umum>();

            

            var idPeriode = service.viewPeriode().FirstOrDefault(
                q => q.awalSemester.ToString("yyyy") == comboBoxEdit2.SelectedItem.ToString().Substring(0, 4)
                && q.akhirSemester.ToString("yyyy") == comboBoxEdit2.SelectedItem.ToString().Substring(5, 4)
                && q.semester == comboBoxEdit1.SelectedItem.ToString());

            if (idPeriode == null)
            {
                XtraMessageBox.Show(
                    $"Semester {comboBoxEdit1.SelectedItem} Periode {comboBoxEdit2.SelectedItem} belum ada");
            }
            else
            {
                //ambil data dari tabel
                for (var i = 0; i < gridView1.RowCount; i++)
                {
                    DataRow row = gridView1.GetDataRow(i);

                    if (row[3].ToString() == string.Empty || row[4].ToString() == string.Empty) continue;
                    var idKelas = service.GetKelas().FirstOrDefault(q => q.Kelas == row[4].ToString());
                    var kdMk = service.GetMatKul().FirstOrDefault(q => q.mata_kuliah == row[3].ToString());

                    Debug.Assert(idKelas != null, "idKelas != null");
                    Debug.Assert(kdMk != null, "kdMk != null");
                    var listJadwal = new jadwal_umum()
                    {
                        hari = row[0].ToString(),
                        id_kelas = idKelas.id_kelas,
                        id_periode = idPeriode.id_periode,
                        id_shift = row[1].ToString(),
                        kode_mk = kdMk.kode_mk
                    };
                    jadwal.Add(listJadwal);
                }

                //ubah List<jadwal_umum> ke jadwal_umum[]
                var jadd = new jadwal_umum[jadwal.Count];
                for (var i = 0; i < jadwal.Count; i++)
                {
                    var jadum = new jadwal_umum()
                    {
                        hari = jadwal[i].hari,
                        id_kelas = jadwal[i].id_kelas,
                        id_periode = jadwal[i].id_periode,
                        id_shift = jadwal[i].id_shift,
                        kode_mk = jadwal[i].kode_mk
                    };
                    jadd[i] = jadum;
                }

                try
                {
                    service.InsertJadwal(jadd);
                } catch(Exception)
                {
                    //XtraMessageBox.Show(error.ToString());
                    XtraMessageBox.Show("Tidak ada jadwal yg ditambahkan");
                }
            }



        }
    }
}
