using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class frmTambahJadwalUmum : XtraForm
    {
        public frmTambahJadwalUmum()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            ComboBoxEditAdd("Semester", comboBoxEdit1);
            ComboBoxEditAdd("Periode", comboBoxEdit2);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            IadmClient service = new IadmClient();
            DataTable jadwal = new DataTable();


            jadwal.Columns.Add("Hari", typeof(string));
            jadwal.Columns.Add("Shit", typeof(string));
            jadwal.Columns.Add("jam", typeof(string));
            jadwal.Columns.Add("Praktikum", typeof(string));
            jadwal.Columns.Add("Kelas", typeof(string));


            string[] hari = { "Senin", "Selasa", "Rabu", "Kamis", "Jumat" };
            string[] shift = { "I", "II", "III", "IV" };
            string[] jam = { "08:00 - 09:40", "09:50 - 11:30", "11:40 - 13:20", "13:30 - 15:10" };
            for (int h = 0; h < hari.Length; h++)
            {
                for (int s = 0; s < shift.Length; s++)
                {
                    jadwal.Rows.Add(hari[h], shift[s], jam[s], null, null);
                }
            }

            gridControl1.DataSource = jadwal;
            gridView1.OptionsView.AllowCellMerge = true;
            gridView1.Columns["Praktikum"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            gridView1.Columns["Kelas"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

            gridControl1.ForceInitialize();

            RepositoryItemComboBox cmbbxPraktikum = new RepositoryItemComboBox()
            {
                TextEditStyle = TextEditStyles.DisableTextEditor
            };
            RepositoryItemComboBox cmbbxMk = new RepositoryItemComboBox()
            {
                TextEditStyle = TextEditStyles.DisableTextEditor
            };
            var mk = service.GetMatKul();
            string[] praktikum = new string[mk.Count()];
            for (int i = 0; i < mk.Count(); i++)
            {
                praktikum[i] = mk[i].mata_kuliah;
            }
            cmbbxPraktikum.Items.AddRange(praktikum);

            var kls = service.GetKelas();
            string[] kelas = new string[kls.Count()];
            for (int i = 0; i < kls.Count(); i++)
            {
                kelas[i] = kls[i].Kelas;
            }
            cmbbxMk.Items.AddRange(kelas);

            gridControl1.RepositoryItems.Add(cmbbxPraktikum);
            gridControl1.RepositoryItems.Add(cmbbxMk);


            gridView1.Columns["Praktikum"].ColumnEdit = cmbbxPraktikum;
            gridView1.Columns["Kelas"].ColumnEdit = cmbbxMk;
            
        }

        private void batal(object sender, EventArgs e)
        {
            Close();
        }

        private void simpan(object sender, EventArgs e)
        {
            IadmClient service = new IadmClient();
            List<jadwal_umum> jadwal = new List<jadwal_umum>();

            

            var _idPeriode = service.viewPeriode().FirstOrDefault(
                q => q.awalSemester.ToString("yyyy") == comboBoxEdit2.SelectedItem.ToString().Substring(0, 4)
                && q.akhirSemester.ToString("yyyy") == comboBoxEdit2.SelectedItem.ToString().Substring(5, 4)
                && q.semester == comboBoxEdit1.SelectedItem.ToString());

            if (_idPeriode == null)
            {
                XtraMessageBox.Show(string.Format("Semester {0} Periode {1} belum ada",
                                     comboBoxEdit1.SelectedItem, 
                                     comboBoxEdit2.SelectedItem));
            }
            else
            {
                //ambil data dari tabel
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    DataRow row = gridView1.GetDataRow(i);

                    if (row[3].ToString() != string.Empty && row[4].ToString() != string.Empty)
                    {
                        var _idKelas = service.GetKelas().FirstOrDefault(q => q.Kelas == row[4].ToString());
                        var _KdMk = service.GetMatKul().FirstOrDefault(q => q.mata_kuliah == row[3].ToString());

                        jadwal_umum listJadwal = new jadwal_umum()
                        {
                            hari = row[0].ToString(),
                            id_kelas = _idKelas.id_kelas,
                            id_periode = _idPeriode.id_periode,
                            id_shift = row[1].ToString(),
                            kode_mk = _KdMk.kode_mk
                        };
                        jadwal.Add(listJadwal);
                    }
                }

                //ubah List<jadwal_umum> ke jadwal_umum[]
                jadwal_umum[] jadd = new jadwal_umum[jadwal.Count];
                for (int i = 0; i < jadwal.Count; i++)
                {
                    jadwal_umum jadum = new jadwal_umum()
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
