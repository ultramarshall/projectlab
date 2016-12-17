using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using static client.Library.Method;
using static client.Library.border;
using static client.Library.convertFromTo;
using System.Windows.Forms;
using System.Data;
using System.Globalization;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;

namespace client
{
    public partial class frmMain : XtraForm
    {
        List<Staff> data_staff = new List<Staff>();
        List<praktikan> data_praktikan = new List<praktikan>();
        

        public frmMain()
        {
            InitializeComponent();
            disablelayoutcontrolmenu();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

        }

        void disablelayoutcontrolmenu()
        {

            layoutControl1.AllowCustomization = false;
            layoutControl2.AllowCustomization = false;
            layoutControl3.AllowCustomization = false;
            layoutControl4.AllowCustomization = false;
            layoutControl5.AllowCustomization = false;
            layoutControl6.AllowCustomization = false;
            layoutControl7.AllowCustomization = false;
            layoutControl8.AllowCustomization = false;
            layoutControl9.AllowCustomization = false;

        }

        DateTime m = new DateTime();
        DateTime s = new DateTime();
        

        private void LoginButton_Click(object sender, EventArgs e)
        {
            listBoxControl1.Items.Clear();
            
            try
            {
                IadmClient service = new IadmClient();
                string roles = PostLogin(username.Text, password.Text);

                var culture = new CultureInfo("id-ID");// waktu format indonesia
                var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek); //menampilkan hari ini/ ex: Senin, Selasa, Rabu, Kamis, Jumat, Sabtu

                jadwalPraktikan nrp = new jadwalPraktikan() { nrp = username.Text };
                var at = service.getTimeLogin(nrp).Select(x => new { hari = x.id_jadwal_umum.hari, waktu = x.id_jadwal_umum.fk_jadwalUmum_Shift.waktu }).ToList();

                var now = DateTime.Now;
                 
                var TimeNow = now.ToString("dd/MM/yyyy hh:mm:ss");
                int year = int.Parse(TimeNow.Substring(6, 4));
                int month = int.Parse(TimeNow.Substring(3, 2));
                int days = int.Parse(TimeNow.Substring(0, 2));


                if (roles == "Praktikan")
                {
                    if(at.Count > 0) // punya jadwal
                    {
                        try
                        {
                            var h = at.SingleOrDefault(q => q.hari == day);
                            if (h.hari == day)
                            {
                                string v = string.Empty;
                                bool result_waktu = false;
                                
                                for (int i = 0; i < at.Count; i++)
                                {
                                    int mJam = int.Parse(at[i].waktu.Substring(0, 2));
                                    int mMenit = int.Parse(at[i].waktu.Substring(3, 2));
                                    int sJam = int.Parse(at[i].waktu.Substring(8, 2));
                                    int sMenit = int.Parse(at[i].waktu.Substring(11, 2));

                                    DateTime mulai = new DateTime(year, month, days, mJam, mMenit, 0);
                                    DateTime selesai = new DateTime(year, month, days, sJam, sMenit, 0);
                                    if ( ((now >= mulai)&&(now<=selesai)) && at[i].hari == day )
                                    {
                                        result_waktu = true;
                                        v = at[i].waktu;
                                        break;
                                    }
                                }
                                if (result_waktu == true)
                                {
                                    jadwalPraktikan data = new jadwalPraktikan()
                                    {
                                        nrp = username.Text,
                                        id_jadwal_umum = new jadwal_umum() { fk_jadwalUmum_Shift = new Shift() { waktu = v } }
                                    };
                                    var c = service.getPraktikanPraktikum(data);
                                    listBoxControl1.Items.Add(c.mata_kuliah);
                                    Jadwal.SelectedPage = Jadwal_Tersedia;
                                }
                                else
                                {
                                    XtraMessageBox.Show("bukan shift praktikum anda");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            XtraMessageBox.Show("tidak ada jadwal hari ini");
                        }
                    }
                    else // ga punya jadwal
                    {
                        XtraMessageBox.Show("belum memiliki jadwal, hubungi admin pak wasro");
                    }
                }
                else if (roles == "Asisten") //aslab
                {
                    Interface.SelectedPage = InterfaceStaff;
                    Staff data = new Staff() //kirim id_staff 
                    {
                        id_staff = username.Text
                    };
                    data_staff.Add(service.getProfileStaff(data));
                    simpleLabelItem11.Text = data_staff[0].id_staff;
                    simpleLabelItem12.Text = data_staff[0].nama;
                    simpleLabelItem13.Text = data_staff[0].no_hp;
                    simpleLabelItem14.Text = data_staff[0].alamat;
                    MemoryStream foto = new MemoryStream(data_staff[0].foto);
                    pictureEdit2.Image = Image.FromStream(foto);
                    gridControl6.DataSource = service.GetStaffJadwal(data_staff[0].id_staff);
                    service.Close();
                }
                else if (roles == "Admin")
                {
                    Interface.SelectedPage = InterfaceAdmin;
                }
                else if (roles == "Koordinator") //kordinator
                {
                    Interface.SelectedPage = InterfaceKoordinator;
                    Staff data = new Staff() //kirim id_staff 
                    {
                        id_staff = username.Text
                    };
                    data_staff.Add(service.getProfileStaff(data));
                    simpleLabelItem30.Text = data_staff[0].id_staff;
                    simpleLabelItem27.Text = data_staff[0].nama;
                    simpleLabelItem28.Text = data_staff[0].no_hp;
                    simpleLabelItem29.Text = data_staff[0].alamat;
                    MemoryStream foto = new MemoryStream(data_staff[0].foto);
                    pictureEdit4.Image = Image.FromStream(foto);
                    //gridControl6.DataSource = service.GetStaffJadwal(data_staff[0].id_staff);
                    K_Info_Click(sender,e);
                    service.Close();
                }
                else //username ga ada
                {
                    XtraMessageBox.Show("username tidak terdaftar");
                }

            }
            catch (Exception err)
            {

                XtraMessageBox.Show(err.ToString());
            }

        }private void MulaiButton_Click(object sender, EventArgs e)
        {
            Jadwal.SelectedPage = Jadwal_Blank;

            IadmClient service = new IadmClient();

            praktikan data = new praktikan() { NRP = username.Text };
            data_praktikan.Add(service.getProfilePraktikan(data));
            simpleLabelItem22.Text = data_praktikan[0].NRP.ToString();
            simpleLabelItem19.Text = data_praktikan[0].Nama.ToString();
            simpleLabelItem20.Text = data_praktikan[0].jurusan.NamaJurusan.ToString();
            simpleLabelItem21.Text = data_praktikan[0].angkatan.TahunAngkatan.ToString();
            MemoryStream img = new MemoryStream(data_praktikan[0].Foto);
            pictureEdit3.Image = Image.FromStream(img);
            Interface.SelectedPage = InterfacePraktikan; 
        }

        private void staff(object sender, EventArgs e)
        {
            // tampil layar praktikan
            a_.SelectedPage = a_staff;
        }

        private void Cari_Praktikan_Button(object sender, EventArgs e)
        {
            try
            {
                IadmClient service = new IadmClient();
                
                string nmAngkatan = comboBoxEdit1.SelectedItem.ToString();
                string nmJurusan = comboBoxEdit2.SelectedItem.ToString();
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.TahunAngkatan == nmAngkatan);
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.NamaJurusan == nmJurusan);
                XtraMessageBox.Show(jurusan.KodeJurusan);
                praktikan data = new praktikan()
                {
                    angkatan = new angkatan() { KodeAngkatan = angkatan.KodeAngkatan },
                    jurusan = new jurusan() { KodeJurusan = jurusan.KodeJurusan }
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama }).ToList();
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;
                 //gridView1.Columns["NRP"].Caption = "NO MAHASISWA";
                //gridView1.Columns["Foto"].Caption = "FOTO";
                //gridView1.Columns["Nama"].Caption = "NAMA";
                
                //for (int i = 0; i < gridView1.Columns.Count; i++)
                //{
                //    gridView1.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                //}

                service.Close();
            } catch (Exception err)
            {
                XtraMessageBox.Show(err.ToString());
            }
        }

        private void Admin_Logout_Button(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
        }

        private void praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_praktikan;
            ComboBoxEditAdd("Angkatan", comboBoxEdit1);
            ComboBoxEditAdd("Jurusan", comboBoxEdit2);
            Cari_Praktikan_Button(sender, e);
        }

        private void AddPraktikan(object sender, EventArgs e)
        {
            ComboBoxEditAdd("Angkatan", comboBoxEdit3);
            ComboBoxEditAdd("Jurusan", comboBoxEdit4);
            a_.SelectedPage = a_add_user;
        }

        private void import_excel(object sender, EventArgs e)
        {
            frmImportExcel frm = new frmImportExcel();
            frm.ShowDialog();
        }

        private void jadwal_umum(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_umum;

            IadmClient service = new IadmClient();

            
            ComboBoxEditAdd("Semester", comboBoxEdit5);
            ComboBoxEditAdd("Periode", comboBoxEdit6);

            jadwal_umum jadwal = new jadwal_umum()
            {
                //semester = comboBoxEdit5.Text.TrimEnd(),
                //tahun_akademik = comboBoxEdit6.Text.TrimEnd()
            };
            //gridControl3.DataSource = service.ViewJadwalUmum(jadwal).Select(x => new { x.hari, x.id_shift, x.waktu, x.mata_kuliah, x.kelas }).ToList();
        }

        private void jadwal_praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_praktikan;
        }

        private void lihat_jadwal(object sender, EventArgs e)
        {
            try
            {
                IadmClient service = new IadmClient();
                int start = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(0, 4));
                int finish = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(5, 4));

                jadwal_umum jadwal = new jadwal_umum()
                {
                    fk_jadwalUmum_periode = new periode() {
                        semester = comboBoxEdit5.Text.TrimEnd(),
                        awalSemester = new DateTime(start, 1, 1),
                        akhirSemester = new DateTime(finish, 1, 1)
                    },
                };
                gridControl3.DataSource = service.ViewJadwalUmum(jadwal).Select(x => new {
                    x.hari,
                    x.fk_jadwalUmum_Shift.id_shift,
                    x.fk_jadwalUmum_Shift.waktu,
                    x.fk_jadwalUmum_matakuliah.mata_kuliah,
                    x.fk_jadwalUmum_kelas.Kelas
                }).ToList();

                gridView3.Columns["id_shift"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                gridView3.Columns["waktu"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                gridView3.Columns["mata_kuliah"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
                gridView3.Columns["Kelas"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;

                //gridView3.Columns["id_shift"].Caption = "shift";
                //gridView3.Columns["mata_kuliah"].Caption = "praktikum";

                service.Close();
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(err.ToString());
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            frmTambahJadwalUmum frm = new frmTambahJadwalUmum();
            frm.ShowDialog();
        }

        private void hapus_jadwal(object sender, EventArgs e)
        {
            IadmClient service = new IadmClient();
            int start = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(0, 4));
            int finish = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(5, 4));

            jadwal_umum jadwal = new jadwal_umum()
            {
                fk_jadwalUmum_periode = new periode()
                {
                    semester = comboBoxEdit5.Text.TrimEnd(),
                    awalSemester = new DateTime(start, 1, 1),
                    akhirSemester = new DateTime(finish, 1, 1)
                },
            };
            service.DeleteJadwal(jadwal);
            lihat_jadwal(sender, e);
        }

        private void Tambah_Periode_Click(object sender, EventArgs e)
        {
            frmPeriode frm = new frmPeriode();
            frm.ShowDialog();
        }

        private void StaffLofout_ButtonClick(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
            data_staff.Clear();
        }

        private void ViewJadwalProfile(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffAccount;
            try
            {
                IadmClient service = new IadmClient();
                gridControl6.DataSource = service.GetStaffJadwal(data_staff[0].id_staff);
            }
            catch (Exception)
            {
                return;
            }}

        private void cekAbsensi_ButtonClick(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffJadwal;
        }

        private void P_Logout(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
        }

        private void P_Info_Click(object sender, EventArgs e)
        {
            P_.SelectedPage = P_Info;
        }

        private void P_Modul_Click(object sender, EventArgs e)
        {
            P_.SelectedPage = P_Modul;
        }

        private void K_Info_Click(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Info;
        }

        private void K_Praktikum_Click(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Praktikum;
            IadmClient service = new IadmClient();
            ComboBoxEditAdd("Periode",comboBoxEdit7);
            ComboBoxEditAdd("Semester",comboBoxEdit8);
            int awalSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(0, 4));
            int akhirSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(5, 4));
            try
            {

                periode data = new periode()
                {
                    semester = comboBoxEdit8.SelectedItem.ToString(),
                    awalSemester = new DateTime(awalSemester, 1, 1, 1, 1, 1),
                    akhirSemester = new DateTime(akhirSemester, 1, 1, 1, 1, 1)
                };

                gridControl8.DataSource = ToDataTable(service.jadwalUmumStaff(data).Select(r => new
                {
                    hari = r.jadwal_umum.hari,
                    shift = r.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                    waktu = r.jadwal_umum.fk_jadwalUmum_Shift.waktu,
                    praktikum = r.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                    nama = r.staff.nama,
                } ).ToList());
                
                /*
                 *  seting column merge
                 */gridView8.OptionsView.AllowCellMerge = true;
                gridView8.Columns["nama"].OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;



                RepositoryItemComboBox nama = new RepositoryItemComboBox()
                {
                    TextEditStyle = TextEditStyles.DisableTextEditor,
                    AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.True,
                };
                
                nama.SelectedIndexChanged += new EventHandler(nama_SelectedIndexChanged);
                var a = service.getStaffID().Select(n=>n.nama).ToList();
                nama.Items.AddRange(a);
                
                gridControl8.RepositoryItems.Add(nama);
                gridView8.Columns["nama"].ColumnEdit = nama;
                gridView8.Columns["nama"].OptionsColumn.AllowEdit = true;
                gridView8.Columns["nama"].OptionsColumn.ReadOnly = false;
                

                gridControl8.ForceInitialize();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
        }
        private void nama_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            IadmClient service = new IadmClient();

            //get id_staff by name, to show s.id_staff
            var s = service.getStaffID().FirstOrDefault(id => id.nama == editor.SelectedItem.ToString());

            // get value all column in selected rows
            DataRow row = gridView8.GetDataRow(gridView8.FocusedRowHandle);

            var p = comboBoxEdit7.SelectedItem.ToString();
            var awS = int.Parse(p.Substring(0, 4));
            var akS = int.Parse(p.Substring(5, 4));
            jadwalStaff data = new jadwalStaff()
            {
                jadwal_umum = new jadwal_umum()
                {
                    fk_jadwalUmum_periode = new periode()
                    {
                        awalSemester = new DateTime(awS,1,1,1,1,1),
                        akhirSemester = new DateTime(akS,1,1,1,1,1),
                        semester = comboBoxEdit8.SelectedItem.ToString()
                    },
                    hari = row[0].ToString(),
                    fk_jadwalUmum_Shift = new Shift()
                    {
                        id_shift = row[1].ToString(),
                        waktu = row[2].ToString()
                    },
                    fk_jadwalUmum_matakuliah = new matkul()
                    {
                        mata_kuliah = row[3].ToString()
                    },
                },
                staff = new Staff()
                {
                    id_staff = s.id_staff
                }
            };

        }


        private void K_Logout_Click(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
        }
    }
}
