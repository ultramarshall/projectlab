using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using static System.IO.Directory;
using System.Linq;
using client.lab;
using client.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using static client.Library.Method;
using static client.Library.border;
using static client.Library.ConvertFromTo;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace client
{
    public partial class FrmMain : XtraForm
    {
        private readonly List<praktikan> _dataPraktikan = new List<praktikan>();
        private readonly List<Staff> _dataStaff = new List<Staff>();
        private jadwalPraktikan _jadwalPraktikan = new jadwalPraktikan();

        public FrmMain()
        {
            InitializeComponent();
            Disablelayoutcontrolmenu();
            DisableStart(); // kill start menu
            CekKoneksi();
            timer1.Start();
            
        }

        private void CekKoneksi()
        {
            koneksiserver.Start();
        }
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, int lParam);
        private void DisableStart()
        {
            //int hwnd;
            //hwnd = FindWindow("Progman", null);
            //PostMessage(hwnd, /*WM_QUIT*/ 0x12, 0, 0);
            //return;
        }

        private void EnableStart()
        {
           // Process.Start("explorer.exe");
        }

        private void Disablelayoutcontrolmenu()
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

        private void SettingGridView(GridView gridView)
        {
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsBehavior.Editable = false;
            gridView.Appearance.SelectedRow.BackColor = Color.Aqua;
            gridView.Appearance.FocusedRow.BackColor = Color.Aqua;
            gridView.OptionsMenu.EnableColumnMenu = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            for ( var i = 0; i < gridView.Columns.Count; i++ )
            {
                gridView.Columns[i].OptionsColumn.AllowFocus = false;
                gridView.Columns[i].OptionsColumn.AllowMove = false;
            }
        }

        private void CekJadwal()
        {
            var service = new IadmClient();
            var periode = service.viewPeriode().FirstOrDefault(x => DateTime.Now >= x.awalSemester &&
                                                                    DateTime.Now <= x.akhirSemester);
            var data = new jadwalStaff()
            {
                staff = new Staff() { id_staff = _dataStaff[0].id_staff },
                jadwal_umum = new jadwal_umum()
                {
                    id_periode = periode.id_periode
                }
            };
            var jadwal = service.GetJadwalAsisten(data);

            if ( jadwal.Count() > 0 )
            {
                accordionControlElement17.Enabled = false;
            }
            else
            {
                accordionControlElement17.Enabled = true;
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            listBoxControl1.Items.Clear();
            try
            {
                var service = new IadmClient();
                var roles = PostLogin(username.Text, password.Text);
                var culture = new CultureInfo("id-ID"); // waktu format indonesia
                var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek);

                //menampilkan hari ini/ ex: Senin, Selasa, Rabu, Kamis, Jumat, Sabtu
                var now = service.ServerTime();
                if (roles == "Praktikan")
                {
                    try
                    {
                        //periode.id_periode
                        var periode = service.viewPeriode().FirstOrDefault(
                            x => x.awalSemester < DateTime.Now &&
                                 x.akhirSemester > DateTime.Now
                            );

                        //shift.id_shift
                        var shift = service.GetShift().FirstOrDefault(x => x.mulai.TimeOfDay < now.TimeOfDay &&
                                                                           x.selesai.TimeOfDay > now.TimeOfDay
                            );
                        var jadwal = service.getPraktikanPraktikum(username.Text, shift.id_shift, periode.id_periode);
                        _jadwalPraktikan = jadwal[0];
                        if (jadwal.Length != 1) return;
                        {
                            if (day == jadwal[0].id_jadwal_umum.hari)
                            {
                                var timeStart = jadwal[0].id_jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay;
                                var timeEnd = jadwal[0].id_jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay;
                                if (now.TimeOfDay <= timeStart) // praktikum belum mulai
                                    XtraMessageBox.Show("tidak bisa login. praktikum belum di mulai");
                                else if (now.TimeOfDay >= timeEnd) // praktikum sudah lewat
                                    XtraMessageBox.Show("tidak bisa login. praktikum sudah selesai");
                                else
                                {
                                    layoutControlItem6.Text = @"Praktikum " +
                                                              jadwal[0].id_jadwal_umum.fk_jadwalUmum_matakuliah
                                                                  .mata_kuliah;
                                    var data = new jadwalPraktikan
                                    {
                                        id_jadwal_praktikan = jadwal[0].id_jadwal_praktikan
                                    };
                                    var praktikumTersedia = service.GetPertemuan(data).Select(x => x.id_jenis_pertemuan);
                                    listBoxControl1.Items.AddRange(praktikumTersedia.ToArray());
                                    Login_Button.Enabled = false;
                                    Jadwal.SelectedPage = Jadwal_Tersedia;
                                }
                            }
                            else XtraMessageBox.Show("hari ini tidak ada jadwal praktikum");
                        }
                    }
                    catch (Exception)
                    {
                        XtraMessageBox.Show("tidak ada praktikum saat ini");
                    }
                }
                else if (roles == "Asisten") //aslab
                {
                    Interface.SelectedPage = InterfaceStaff;
                    var data = new Staff //kirim id_staff 
                    {
                        id_staff = username.Text
                    };
                    if (_dataStaff != null)
                    {
                        _dataStaff.Add(service.getProfileStaff(data));
                        simpleLabelItem11.Text = _dataStaff[0].id_staff;
                        simpleLabelItem12.Text = _dataStaff[0].nama;
                        simpleLabelItem13.Text = _dataStaff[0].no_hp;
                        simpleLabelItem14.Text = _dataStaff[0].alamat;
                        var foto = new MemoryStream(_dataStaff[0].foto);
                        pictureEdit2.Image = Image.FromStream(foto);
                        ViewJadwalProfile(sender, e);
                        CekJadwal();
                    }
                    service.Close();
                }
                else if (roles == "Admin")
                {
                    Interface.SelectedPage = InterfaceAdmin;
                    EnableStart();
                }
                else if (roles == "Koordinator") //kordinator
                {
                    Interface.SelectedPage = InterfaceKoordinator;
                    var data = new Staff //kirim id_staff 
                    {
                        id_staff = username.Text
                    };
                    _dataStaff.Add(service.getProfileStaff(data));
                    simpleLabelItem30.Text = _dataStaff[0].id_staff;
                    simpleLabelItem27.Text = _dataStaff[0].nama;
                    simpleLabelItem28.Text = _dataStaff[0].no_hp;
                    simpleLabelItem29.Text = _dataStaff[0].alamat;
                    var foto = new MemoryStream(_dataStaff[0].foto);
                    pictureEdit4.Image = Image.FromStream(foto);

                    //gridControl6.DataSource = service.GetStaffJadwal(_dataStaff[0].id_staff);
                    K_Info_Click(sender, e);
                    service.Close();
                }
                else //username ga ada
                    XtraMessageBox.Show("username tidak terdaftar");
            }
            catch ( Exception err )
            {
                XtraMessageBox.Show(err.ToString());
            }
        }

        private void CloseLogin(object sender, EventArgs e)
        {
            username.Text = "";
            password.Text = "";
            Login_Button.Enabled = true;
            Jadwal.SelectedPage = Jadwal_Blank;
        }

        private void gridView6_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if ( e.Clicks == 2 )
            {
                AbsensiPraktikanSearch(sender, e);
            }
        }

        private void MulaiButton_Click(object sender, EventArgs e)
        {
            Jadwal.SelectedPage = Jadwal_Blank;
            var service = new IadmClient();
            var p =
                service.ListPertemuan()
                    .FirstOrDefault(x => x.id_jenis_pertemuan == listBoxControl1.SelectedItem.ToString());
            var key = new AbsensiPraktikan
            {
                JadwalPraktikan = new jadwalPraktikan
                {
                    id_jadwal_praktikan = _jadwalPraktikan.id_jadwal_praktikan
                },
                Pertemuan = new pertemuan
                {
                    id_pertemuan = p.id_pertemuan
                }
            };
            service.PostAbsenPraktikan(key);
            var data = new praktikan { NRP = username.Text };
            _dataPraktikan.Add(service.getProfilePraktikan(data));
            simpleLabelItem22.Text = _dataPraktikan[0].NRP;
            simpleLabelItem19.Text = _dataPraktikan[0].Nama;
            simpleLabelItem20.Text = _dataPraktikan[0].jurusan.NamaJurusan;
            simpleLabelItem21.Text = _dataPraktikan[0].angkatan.TahunAngkatan;
            var img = new MemoryStream(_dataPraktikan[0].Foto);
            pictureEdit3.Image = Image.FromStream(img);
            Interface.SelectedPage = InterfacePraktikan;
            EnableStart();
        }

        private void Staff(object sender, EventArgs e)
        {
            // tampil layar Praktikan
            a_.SelectedPage = a_staff;
            var service = new IadmClient();
            gridControl2.DataSource = service.getStaffID().Select(x => new
            {
                x.id_staff,
                x.nama,
                x.users.status
            });
            SettingGridView(gridView2);
        }

        private void Cari_Praktikan_Button(object sender, EventArgs e)
        {
            try
            {
                var service = new IadmClient();
                var nmAngkatan = comboBoxEdit1.SelectedItem.ToString();
                var nmJurusan = comboBoxEdit2.SelectedItem.ToString();
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.TahunAngkatan == nmAngkatan);
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.NamaJurusan == nmJurusan);
                var data = new praktikan
                {
                    angkatan = new angkatan { KodeAngkatan = angkatan.KodeAngkatan },
                    jurusan = new jurusan { KodeJurusan = jurusan.KodeJurusan }
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama }).ToList();
                SettingGridView(gridView1);
                gridView1.Columns[0].OptionsColumn.AllowMerge = DefaultBoolean.False;
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;
                service.Close();
            }
            catch (Exception error)
            {
                XtraMessageBox.Show(error.ToString());
                //XtraMessageBox.Show("Belum ada data praktikan yang di input");
            }
        }

        private void Admin_Logout_Button(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
            username.Text = "";
            password.Text = "";
        }

        private void Praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_praktikan;
            ComboBoxEditAdd("Angkatan", comboBoxEdit1);
            ComboBoxEditAdd("Jurusan", comboBoxEdit2);
            Cari_Praktikan_Button(sender, e);
        }

        private void AddPraktikanCick(object sender, EventArgs e)
        {
            ComboBoxEditAdd("Angkatan", AngkatanPraktikan);
            ComboBoxEditAdd("Jurusan", JurusanPraktikan);
            pictureEdit1.Image = Resources.Screenshot_6;
            NamaPraktikan.Text = string.Empty;
            PasswordPraktikan.Text = string.Empty;
            NrpPraktikan.Text = string.Empty;
            JurusanPraktikan.SelectedIndex = -1;
            AngkatanPraktikan.SelectedIndex = -1;
            a_.SelectedPage = a_praktikan_profile;
        }

        private void EditPraktikanClick(object sender, EventArgs e)
        {
            try
            {
                ComboBoxEditAdd("Angkatan", AngkatanPraktikan);
                ComboBoxEditAdd("Jurusan", JurusanPraktikan);
                var service = new IadmClient();
                var data = new praktikan
                {
                    NRP = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[1])
                };
                var p = service.getProfilePraktikan(data);
                NrpPraktikan.Text = p.NRP;
                PasswordPraktikan.Text = p.Users.password;
                NamaPraktikan.Text = p.Nama;
                a_.SelectedPage = a_praktikan_profile;
                AngkatanPraktikan.Text = p.angkatan.TahunAngkatan;
                JurusanPraktikan.Text = p.jurusan.NamaJurusan;
            }
            catch ( Exception )
            {
                XtraMessageBox.Show("belum pilih praktikan");
            }
        }

        private void SaveProfilPraktikanClick(object sender, EventArgs e)
        {
            try
            {
                var service = new IadmClient();
                var data = new praktikan { NRP = NrpPraktikan.Text };
                var profilepraktikan = service.getProfilePraktikan(data);
                var jurusan =
                    service.GetJurusan().FirstOrDefault(x => x.NamaJurusan == JurusanPraktikan.SelectedItem.ToString());
                var angkatan =
                    service.GetAngkatan()
                        .FirstOrDefault(x => x.TahunAngkatan == AngkatanPraktikan.SelectedItem.ToString());
                var nrp = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[1]);
                var datapraktikan = new praktikan
                {
                    NRP = NrpPraktikan.Text,
                    Nama = NamaPraktikan.Text,
                    Foto = ImageToByteArray(pictureEdit1.Image),
                    jurusan = new jurusan { KodeJurusan = jurusan.KodeJurusan },
                    angkatan = new angkatan { KodeAngkatan = angkatan.KodeAngkatan },
                    Users = new Users
                    {
                        password = PasswordPraktikan.Text,
                        status = "Praktikan"
                    }
                };
                if ( profilepraktikan.NRP == null ) //simpan
                    service.AddPraktikan(datapraktikan);
                else //edit
                    service.EditPraktikan(nrp, datapraktikan);
                a_.SelectedPage = a_praktikan;
                Cari_Praktikan_Button(sender, e);
            }
            catch ( Exception error )
            {
                XtraMessageBox.Show(error.ToString());
            }
        }

        private void import_excel(object sender, EventArgs e)
        {
            var frm = new FrmImportExcel();
            frm.ShowDialog();
        }

        private void jadwal_umum(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_umum;
            var service = new IadmClient();
            AddPeriode(comboBoxEdit5);
            var jadwal = new jadwal_umum { id_periode = PeriodeId(comboBoxEdit5) };
            gridControl3.DataSource = ToDataTable(service.ViewJadwalUmum(jadwal).Select(x => new
            {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    string.Format("{0:HH:mm} - {1:HH:mm}",
                    x.fk_jadwalUmum_Shift.mulai,
                    x.fk_jadwalUmum_Shift.selesai),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            }).ToList());
            SettingGridView(gridView5);
        }

        private void lihat_jadwal(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new jadwal_umum { id_periode = PeriodeId(comboBoxEdit5) };
            gridControl3.DataSource = ToDataTable(service.ViewJadwalUmum(jadwal).Select(x => new
            {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    string.Format("{0:HH:mm} - {1:HH:mm}",
                    x.fk_jadwalUmum_Shift.mulai,
                    x.fk_jadwalUmum_Shift.selesai),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            }).ToList());
            SettingGridView(gridView5);
        }

        private void BuatJadwalUmum(object sender, EventArgs e)
        {
            var frm = new FrmTambahJadwalUmum();
            frm.ShowDialog();
        }

        private void hapus_jadwal(object sender, EventArgs e)
        {
            var service = new IadmClient();
            try
            {
                var jadwal = new jadwal_umum { id_periode = PeriodeId(comboBoxEdit5) };
                service.DeleteJadwal(jadwal);
            }
            catch ( Exception )
            {
                XtraMessageBox.Show(string.Format("Jadwal tidak dapat dihapus !{0}Periode sudah berlangsung", Environment.NewLine));
            }
            lihat_jadwal(sender, e);
        }

        private void Tambah_Periode_Click(object sender, EventArgs e)
        {
            var frm = new FrmPeriode();
            frm.ShowDialog();
        }

        private void StaffLogout_ButtonClick(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
            _dataStaff.Clear();
            username.Text = "";
            password.Text = "";
            DisableStart();
        }

        private void ViewJadwalProfile(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffAccount;
            try
            {
                var service = new IadmClient();

                //periode.id_periode
                var periode = service.viewPeriode().FirstOrDefault(
                    x => x.awalSemester < DateTime.Now &&
                         x.akhirSemester > DateTime.Now
                    );
                Debug.Assert(periode != null, "periode != null");
                var jadwal = new jadwalStaff
                {
                    staff = new Staff { id_staff = _dataStaff[0].id_staff },
                    jadwal_umum = new jadwal_umum { id_periode = periode.id_periode }
                };
                gridControl6.DataSource =
                    ToDataTable(service.GetStaffJadwal(jadwal).Select(
                        x => new
                        {
                            x.jadwal_umum.hari,
                            shift = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                            waktu =
                                string.Format("{0} - {1}",
                                x.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay,
                                x.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay),
                            praktikum = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                            kelas = x.jadwal_umum.fk_jadwalUmum_kelas.Kelas
                        }).ToList());
                SettingGridView(gridView6);
                service.Close();
            }
            catch ( Exception )
            {
            }
        }

        private void P_Logout(object sender, EventArgs e)
        {
            _dataPraktikan.Clear();
            CloseLogin(sender, e);
            Interface.SelectedPage = InterfaceLogin;
            P_.SelectedPage = P_Info;
            LogFileUpload.Items.Clear();
            sourceFileName.Text = string.Empty;
        }

        private void P_Info_Click(object sender, EventArgs e)
        {
            P_.SelectedPage = P_Info;
        }

        private void P_Modul_Click(object sender, EventArgs e)
        {
            P_.SelectedPage = P_Modul;
            comboBoxEdit3.Properties.Items.Clear();
            var service = new IadmClient();
            var kodemk = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == layoutControlItem6.Text.Replace("Praktikum ", String.Empty));
            var data = new modul()
            {
                matkul = new matkul()
                {
                    kode_mk = kodemk.kode_mk
                }
            };
            var b = service.GetListModul(data).Select(x => x.file_modul).ToList();
            for ( var i = 0; i < b.Count; i++ )
            {
                comboBoxEdit3.Properties.Items.Add(b[i]);
            }

        }

        private void K_Info_Click(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Info;
        }

        private void JadwalStaffKPraktikumClick(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Praktikum;
            AddPeriode(comboBoxEdit8);
            LihatJadwalStaff(sender, e);
        }

        private void K_Logout_Click(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
        }

        private void LihatJadwalStaff(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new periode { id_periode = PeriodeId(comboBoxEdit8) };
            gridControl8.DataSource = ToDataTable(service.jadwalUmumStaff(jadwal).Select(x => new
            {
                HARI = x.jadwal_umum.hari,
                SHIFT = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    string.Format("{0:HH:mm} - {1:HH:mm}",
                    x.jadwal_umum.fk_jadwalUmum_Shift.mulai,
                    x.jadwal_umum.fk_jadwalUmum_Shift.selesai),
                PRAKTIKUM = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                PENGAJAR = x.staff.nama
            }).ToList());
            SettingGridView(gridView8);
            gridView8.OptionsView.AllowCellMerge = true;
            gridView8.Columns["PENGAJAR"].OptionsColumn.AllowMerge = DefaultBoolean.False;

        }

        private void AbsensiPraktikanSearch(object sender, EventArgs e)
        {
            var shift = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[1]);
            var mk = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[3]);
            viewStaff.SelectedPage = viewStaffAbsensi;
            var service = new IadmClient();
            var periode = service.viewPeriode().FirstOrDefault(
                x => x.awalSemester < DateTime.Now &&
                     x.akhirSemester > DateTime.Now
                );
            var data = new AbsensiPraktikan
            {
                Pertemuan = new pertemuan
                {
                    id_jenis_pertemuan = comboBoxEdit9.SelectedItem.ToString()
                },
                JadwalPraktikan = new jadwalPraktikan
                {
                    id_jadwal_umum = new jadwal_umum
                    {
                        fk_jadwalUmum_periode = new periode
                        {
                            id_periode = periode.id_periode
                        },
                        fk_jadwalUmum_Shift = new Shift
                        {
                            id_shift = shift
                        },
                        fk_jadwalUmum_matakuliah = new matkul()
                        {
                            mata_kuliah = mk
                        }
                    }
                }
            };
            gridControl5.DataSource = ToDataTable(service.GetAbsensiPraktikans(data).Select(x => new
            {
                //foto = x.Foto,
                nrp = x.NRP,
                nama = x.Nama,
                nilai = x.absen.Nilai,
                hapus = new RepositoryItemButtonEdit()
            }).ToList());
            try
            {
                //gridView5.Columns["foto"].OptionsColumn.AllowEdit = false;
                gridView5.Columns["nrp"].OptionsColumn.AllowEdit = false;
                gridView5.Columns["nama"].OptionsColumn.AllowEdit = false;
                //gridView5.Columns["foto"].OptionsColumn.AllowFocus = false;
                gridView5.Columns["nrp"].OptionsColumn.AllowFocus = false;
                gridView5.Columns["nama"].OptionsColumn.AllowFocus = false;
                gridView5.Columns["nilai"].OptionsColumn.AllowEdit = true;
                gridView5.Appearance.FocusedRow.BackColor = Color.Aqua;
                var nilai = new RepositoryItemTextEdit();
                //var photo = new RepositoryItemPictureEdit();
                var cancel = new RepositoryItemButtonEdit()
                {
                    TextEditStyle = TextEditStyles.HideTextEditor
                };
                nilai.EditValueChanged += new EventHandler(nilai_EditValueChanged);
                cancel.Click += new EventHandler(cancel_ButtonClick);
                gridControl5.RepositoryItems.Add(nilai);
                //gridControl5.RepositoryItems.Add(photo);
                gridControl5.RepositoryItems.Add(cancel);

                //gridView5.Columns["foto"].ColumnEdit = photo;
                gridView5.Columns["nilai"].ColumnEdit = nilai;
                gridView5.Columns["hapus"].ColumnEdit = cancel;
                //gridView5.Columns["nilai"].OptionsColumn.ReadOnly = false;
                gridView5.RowHeight = 60;
                gridControl5.ForceInitialize();
            }
            catch (Exception) { }
        }

        private void cancel_ButtonClick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var nil = sender as TextEdit;
            string a = nil.Text;
            var row = gridView5.GetDataRow(gridView5.FocusedRowHandle);
            var nrp = row[1].ToString();
            var shift = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[1]);
            var periode = service.viewPeriode().FirstOrDefault(
                x => x.awalSemester < DateTime.Now &&
                     x.akhirSemester > DateTime.Now
                );
            var data = new AbsensiPraktikan()
            {
                Pertemuan = new pertemuan() { id_jenis_pertemuan = comboBoxEdit9.SelectedItem.ToString() },
                JadwalPraktikan = new jadwalPraktikan()
                {
                    nrp = nrp,
                    id_jadwal_umum = new jadwal_umum()
                    {
                        id_shift = shift,
                        id_periode = periode.id_periode
                    },
                },
            };
            service.HapusAbsensi(data);
            AbsensiPraktikanSearch(sender, e);
        }

        private void nilai_EditValueChanged(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var nil = sender as TextEdit;
            string a = nil.Text;
            var row = gridView5.GetDataRow(gridView5.FocusedRowHandle);
            var nrp = row[1].ToString();
            var shift = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[1]);
            var periode = service.viewPeriode().FirstOrDefault(
                x => x.awalSemester < DateTime.Now &&
                     x.akhirSemester > DateTime.Now
                );
            if ( nil.Text == "" )
            {
                a = "0";
            }

            var data = new AbsensiPraktikan()
            {
                Nilai = int.Parse(a),
                staff = new Staff() { id_staff = _dataStaff[0].id_staff },
                Pertemuan = new pertemuan() { id_jenis_pertemuan = comboBoxEdit9.SelectedItem.ToString() },
                JadwalPraktikan = new jadwalPraktikan()
                {
                    nrp = nrp,
                    id_jadwal_umum = new jadwal_umum()
                    {
                        id_shift = shift,
                        id_periode = periode.id_periode
                    },
                },
            };

            service.KonfirmasiAbsensi(data);
        }

        private void JadwalAsistenClick(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_staff_asisten;
            AddPeriode(comboBoxEdit10);
            ViewJadwallStaffClick(sender, e);
        }

        private void ViewJadwallStaffClick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new periode { id_periode = PeriodeId(comboBoxEdit10) };
            gridControl4.DataSource = ToDataTable(service.jadwalUmumStaff(jadwal).Select(x => new
            {
                HARI = x.jadwal_umum.hari,
                SHIFT = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    string.Format("{0:HH:mm} - {1:HH:mm}",
                    x.jadwal_umum.fk_jadwalUmum_Shift.mulai,
                    x.jadwal_umum.fk_jadwalUmum_Shift.selesai),
                PRAKTIKUM = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                PENGAJAR = x.staff.nama
            }).ToList());
            //gridView4.OptionsView.AllowCellMerge = true;
            gridView4.Columns["WAKTU"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            gridView4.Columns["PRAKTIKUM"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            gridView4.Columns["PENGAJAR"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            gridView4.OptionsCustomization.AllowFilter = false;
            gridView4.OptionsBehavior.Editable = false;
            gridView4.Appearance.SelectedRow.BackColor = Color.Aqua;
            gridView4.Appearance.FocusedRow.BackColor = Color.Aqua;
            gridView4.OptionsMenu.EnableColumnMenu = false;
            gridView4.OptionsView.ShowIndicator = false;
            gridView4.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            for (var i = 0; i < gridView4.Columns.Count; i++)
            {
                gridView4.Columns[i].OptionsColumn.AllowFocus = false;
                gridView4.Columns[i].OptionsColumn.AllowMove = false;
            }
        }

        private void PrintJadwalStaffClick(object sender, EventArgs e)
        {
            gridView4.ShowPrintPreview();
        }

        private void EditStaffClick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var idStaff = gridView2.GetRowCellDisplayText(gridView2.FocusedRowHandle, gridView2.Columns[0]);
            var data = new Staff { id_staff = idStaff };
            var staff = service.getProfileStaff(data);
            textEdit4.Text = staff.id_staff;
            textEdit5.Text = staff.users.password;
            textEdit6.Text = staff.nama;
            textEdit7.Text = staff.alamat;
            textEdit8.Text = staff.no_hp;
            var foto = new MemoryStream(staff.foto);
            pictureEdit5.Image = Image.FromStream(foto);
            comboBoxEdit11.Text = staff.users.status;
            a_.SelectedPage = a_staff_profile;
        }

        private void TambahStaffClick(object sender, EventArgs e)
        {
            textEdit4.Text = string.Empty;
            textEdit5.Text = string.Empty;
            textEdit6.Text = string.Empty;
            textEdit7.Text = string.Empty;
            textEdit8.Text = string.Empty;
            comboBoxEdit11.SelectedIndex = -1;
            a_.SelectedPage = a_staff_profile;
        }

        private void SaveProfilStaffClick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var data = new Staff { id_staff = textEdit4.Text };
            var profileStaff = service.getProfileStaff(data);
            try
            {
                var idStaff = gridView2.GetRowCellDisplayText(gridView2.FocusedRowHandle, gridView2.Columns[0]);
                var datastaff = new Staff
                {
                    id_staff = textEdit4.Text,
                    nama = textEdit6.Text,
                    alamat = textEdit7.Text,
                    no_hp = textEdit8.Text,
                    foto = ImageToByteArray(pictureEdit5.Image),
                    users = new Users
                    {
                        password = textEdit5.Text,
                        status = comboBoxEdit11.SelectedItem.ToString()
                    }
                };
                if ( profileStaff.id_staff == null ) //simpan
                    service.AddStaff(datastaff);
                else //edit
                    service.EditStaff(idStaff, datastaff);
                Staff(sender, e);
            }
            catch
            {
                // ignored
            }
        }

        private void simpleButton19_Click(object sender, EventArgs e)
        {
            a_.SelectedPage = a_praktikan;
            Cari_Praktikan_Button(sender, e);
        }

        private void PrintJadwalUmumClick(object sender, EventArgs e)
        {
            gridView3.ShowPrintPreview();
        }

        private void simpleButton21_Click(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffAccount;
        }

        private void EditProfileStaff(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            viewStaff.SelectedPage = viewStaffEditProfile;
            staffUsername.Text = _dataStaff[0].id_staff;
            staffPassword.Text = _dataStaff[0].users.password;
            staffNama.Text = _dataStaff[0].nama;
            staffAlamat.Text = _dataStaff[0].alamat;
            staffNoHp.Text = _dataStaff[0].no_hp;
            staffFoto.Image = Image.FromStream(new MemoryStream(_dataStaff[0].foto));
        }

        private void EditDataProfileAsisten(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var datastaff = new Staff
            {
                id_staff = staffUsername.Text,
                nama = staffNama.Text,
                alamat = staffAlamat.Text,
                no_hp = staffNoHp.Text,
                foto = ImageToByteArray(staffFoto.Image),
                users = new Users
                {
                    password = staffPassword.Text,
                    status = "Asisten"
                }
            };
            service.EditStaff(staffUsername.Text, datastaff);
            service.Close();

            _dataStaff[0].foto = datastaff.foto;
            _dataStaff[0].nama = datastaff.nama;
            _dataStaff[0].users.password = datastaff.users.password;
            _dataStaff[0].no_hp = datastaff.no_hp;
            _dataStaff[0].alamat = datastaff.alamat;

            pictureEdit2.Image = Image.FromStream(new MemoryStream(_dataStaff[0].foto));
            simpleLabelItem12.Text = _dataStaff[0].nama;
            simpleLabelItem13.Text = _dataStaff[0].no_hp;
            simpleLabelItem14.Text = _dataStaff[0].alamat;
            viewStaff.SelectedPage = viewStaffAccount;
        }

        private void JadwalPraktikan_DoubleClick(object sender, RowCellClickEventArgs e)
        {
            if ( e.Clicks == 2 )
            {
                var service = new IadmClient();
                var nrp = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[1]);
                var nama = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[2]);
                var periode = service.viewPeriode().FirstOrDefault(x => DateTime.Now >= x.awalSemester &&
                                                                        DateTime.Now <= x.akhirSemester);
                var data = new jadwalPraktikan() { nrp = nrp, id_jadwal_umum = new jadwal_umum() { id_periode = periode.id_periode } };
                var jadwal = service.GetJadwalPraktikan(data);

                if ( jadwal.Count() > 0 )
                {
                    var form = new FormResetJadwal(nrp);
                    form.ShowDialog();
                }
                else
                {
                    var form = new FrmTambahJadwalPraktikan(nrp, nama);
                    form.ShowDialog();
                }
                service.Close();
            }
        }

        private void TambahTahunAngkatan(object sender, EventArgs e)
        {
            var Form = new FormAngkatan();
            Form.ShowDialog();
        }

        private void LoadModulPraktikum(object sender, EventArgs e)
        {
            var praktikum = layoutControlItem6.Text.Replace("Praktikum ", String.Empty);
            var service = new IadmClient();
            var p = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == praktikum);
            var data = new modul()
            {
                matkul = new matkul() { kode_mk = p.kode_mk },
                file_modul = comboBoxEdit3.SelectedItem.ToString()
            };
            var source = service.GetModul(data).lokasi_modul;
            var file = new MemoryStream(source);
            pdfViewer1.NavigationPaneInitialVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Hidden;
            pdfViewer1.LoadDocument(file);
            service.Close();
        }

        private string _Dir;
        private void LoadFile(object sender, EventArgs e)
        {
            var open = new OpenFileDialog()
            {
                Title = "Browse Files",
                InitialDirectory = @"C:\",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if ( open.ShowDialog() == DialogResult.OK )
            {
                var Info = new FileInfo(open.FileName);
                _Dir = Info.Name;
                sourceFileName.Text = open.FileName;
                LogFileUpload.Items.Add("File yang di pilih " + _Dir);
            }
        }

        private void UploadFile(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var periode = service.viewPeriode()
                .FirstOrDefault(x => DateTime.Now >= x.awalSemester &&
                                     DateTime.Now <= x.akhirSemester);
            var shift = service.GetShift()
                .FirstOrDefault(x => DateTime.Now.TimeOfDay >= x.mulai.TimeOfDay &&
                                     DateTime.Now.TimeOfDay <= x.selesai.TimeOfDay);

            var praktikum = layoutControlItem6.Text.Replace("Praktikum ", String.Empty);
            var p = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == praktikum);
            var pertemuan = listBoxControl1.SelectedItem.ToString();
            var data = new AbsensiPraktikan()
            {
                JadwalPraktikan = new jadwalPraktikan {
                    nrp = _dataPraktikan[0].NRP,
                    id_jadwal_umum = new jadwal_umum()
                    {
                        id_periode = periode.id_periode,
                        id_shift = shift.id_shift,
                        kode_mk = p.kode_mk
                    },
                },
                Pertemuan = new pertemuan() { id_jenis_pertemuan = pertemuan }
            };

            var absenID = service.GetIDAbsensiPraktikan(data);
            var FileLocation = _dataPraktikan[0].NRP + "\\" + periode.semester + " " + periode.awalSemester.ToString("yyyy") + " - " + periode.akhirSemester.ToString("yyyy") + "\\" + listBoxControl1.SelectedItem.ToString();
            var f = new upload_file()
            {
                id_absensi = absenID,
                nama_file = _Dir,
                lokasi_file = FileLocation,
                data_file = File.ReadAllBytes(@sourceFileName.Text)
            };
            var ex = false;
            try
            {
                service.GetUpLoadFile(f);
            }
            catch ( Exception error )
            {
                ex = true;
                LogFileUpload.Items.Add("Maksimal upload file 500 MB");
                XtraMessageBox.Show(error.ToString());
            }
            //XtraMessageBox.Show( absenID.ToString( ) );

            if ( ex == false )
            {
                LogFileUpload.Items.Add(string.Format("'{0}' upload file berhasil", _Dir));
            }
            service.Close();
        }

        private void Upload_Click(object sender, EventArgs e)
        {
            P_.SelectedPage = P_Upload;
        }

        private void PraktikanUbahPassword(object sender, LinkLabelLinkClickedEventArgs e)
        {
            XtraMessageBox.Show("ganti password");
        }

        private void AmbilJadwal(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffJadwal;
            var service = new IadmClient();

            var periode = service.viewPeriode().FirstOrDefault(
                x =>
                DateTime.Now >= x.awalSemester &&
                DateTime.Now <= x.akhirSemester);

            var data = new jadwal_umum() { id_periode = periode.id_periode };
            gridControl7.DataSource = service.ViewJadwalUmum(data).Select(x => new {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                  string.Format("{0:HH:mm} - {1:HH:mm}",
                  x.fk_jadwalUmum_Shift.mulai,
                  x.fk_jadwalUmum_Shift.selesai),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            });

            // add checkbox and enable multi select rows
            gridView7.OptionsSelection.MultiSelect = true;
            gridView7.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect; ;
            // disable focused row and move column
            for ( var i = 0; i < gridView7.Columns.Count; i++ )
            {
                gridView7.Columns[i].OptionsColumn.AllowFocus = false;
                gridView7.Columns[i].OptionsColumn.AllowMove = false;
            }
            service.Close();
        }

        private void SaveJadwalAsisten(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new List<jadwalStaff>();
            var values = gridView7.GetSelectedRows();
            var periode = service.viewPeriode().FirstOrDefault(
                x =>
                DateTime.Now >= x.awalSemester &&
                DateTime.Now <= x.akhirSemester);
            var data = new jadwal_umum() { id_periode = periode.id_periode };

            var listjadwal = new List<jadwalStaff>();
            for ( var i = 0; i < values.Count(); i++ )
            {
                var shift = gridView7.GetRowCellValue(values[i], gridView7.Columns[1]).ToString();
                var hari = gridView7.GetRowCellValue(values[i], gridView7.Columns[0]).ToString();
                var matkul = gridView7.GetRowCellValue(values[i], gridView7.Columns[3]).ToString();
                var id = service.ViewJadwalUmum(data).FirstOrDefault(x =>
             (x.fk_jadwalUmum_Shift.id_shift == shift && x.hari == hari) &&
              x.fk_jadwalUmum_matakuliah.mata_kuliah == matkul);


                var jadwalstaff = new jadwalStaff()
                {
                    staff = new Staff() { id_staff = _dataStaff[0].id_staff },
                    jadwal_umum = new jadwal_umum() { id_jadwal_umum = id.id_jadwal_umum }
                };
                listjadwal.Add(jadwalstaff);
            }

            var xception = false;
            try
            {
                service.AddJadwalStaffAsisten(listjadwal.ToArray());
            }
            catch ( Exception )
            {
                xception = true;
                XtraMessageBox.Show("Tidak ada jadwal di pilih");
            }

            if ( xception == false )
            {
                CekJadwal();
                ViewJadwalProfile(sender, e);
                viewStaff.SelectedPage = viewStaffAccount;
            }
            service.Close();
        }

        private void Modul_click(object sender, EventArgs e)
        {
            layoutControlGroup30.Expanded = false;
            a_.SelectedPage = a_modul;
            var service = new IadmClient();
            var matkul = service.GetMatKul().Select(x => x.mata_kuliah).ToList();
            comboBoxEdit4.Properties.Items.AddRange(matkul);
            comboBoxEdit4.SelectedIndex = 0;
            service.Close();
        }

        private void Praktikum_SelectedValueChanged(object sender, EventArgs e)
        {
            listBoxControl2.Items.Clear();
            var service = new IadmClient();
            var kodemk = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == comboBoxEdit4.SelectedItem.ToString());
            var data = new modul()
            {
                matkul = new matkul()
                {
                    kode_mk = kodemk.kode_mk
                }
            };
            listBoxControl2.Items.AddRange(service.GetListModul(data).Select(x => x.file_modul).ToArray());
            comboBoxEdit6.Properties.Items.Clear();
            for ( var i = 0; i < 16; i++ )
            {

                comboBoxEdit6.Properties.Items.Add(string.Format("BAB {0:D2}", i + 1));
            }

            try
            {
                var modul = listBoxControl2.Items;
                for ( var i = 0; i < modul.Count; i++ )
                {
                    comboBoxEdit6.Properties.Items.Remove(modul[i].ToString());
                }
            }
            catch ( Exception )
            {
                return;
            }
            service.Close();
        }
        public void fileModule(modul data)
        {
            listBoxControl2.Items.Add(data.file_modul);
        }
        string filesName = String.Empty;
        private void simpleButton28_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog()
            {
                Title = "Browse Files",
                InitialDirectory = @"C:\",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Files PDF|*.pdf",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if ( open.ShowDialog() == DialogResult.OK )
            {
                var info = new FileInfo(open.FileName);
                textEdit1.Text = open.FileName;
                filesName = info.Name;
            }
        }

        private void addFileModul(object sender, EventArgs e)
        {
            layoutControlGroup30.Expanded = true;
            textEdit1.Text = "";
            comboBoxEdit6.SelectedIndex = -1;
        }

        private void SaveModul(object sender, EventArgs e)
        {
            layoutControlGroup30.Expanded = false;
            var service = new IadmClient();
            var kdmk = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == comboBoxEdit4.SelectedItem.ToString());
            var data = new modul()
            {
                matkul = new matkul() { kode_mk = kdmk.kode_mk },
                file_modul = comboBoxEdit6.SelectedItem.ToString(),
                lokasi_modul = File.ReadAllBytes(@textEdit1.Text),
                modul_file = filesName
            };
            try { service.UploadModul(data); } catch ( Exception ee ) { XtraMessageBox.Show(ee.ToString()); }
            Praktikum_SelectedValueChanged(sender, e);
            service.Close();
        }

        private void CloseAddFileModul(object sender, EventArgs e)
        {
            layoutControlGroup30.Expanded = false;
        }

        private void Nilai_click(object sender, EventArgs e)
        {
            a_.SelectedPage = a_nilai;
            comboBoxEdit7.Properties.Items.Clear();
            var service = new IadmClient();
            AddPeriode(comboBoxEdit7);
            comboBoxEdit13.Properties.Items.AddRange(service.GetMatKul().Select(x => x.mata_kuliah).ToArray());
            comboBoxEdit13.SelectedIndex = 0;
            service.Close();
        }
        private class nilaimhs
        {
            public string nrp { get; set; }
            public int nilai { get; set; }
            public int pertemuan { get; set; }
        }
        private class listNilaimhs
        {
            public string nrp { get; set; }
            public string Nama { get; set; }
            public int P01 { get;set; }
            public int P02 { get;set; }
            public int P03 { get;set; }
            public int P04 { get;set; }
            public int P05 { get;set; }
            public int P06 { get;set; }
            public int P07 { get;set; }
            public int P08 { get;set; }
            public int P09 { get;set; }
            public int P10 { get;set; }
            public int P11 { get;set; }
            public int P12 { get;set; }
            public int P13 { get;set; }
            public int P14 { get;set; }
            public int P15 { get;set; }
            public int P16 { get;set; }
            public int jumlah_pertemuan { get; set; }
            public float nilai_akhir { get; set; }
        }
        
        List<listNilaimhs> listNilaiMahasiswa = new List<listNilaimhs>();
        List<nilaimhs> NilaiMahasiswa = new List<nilaimhs>();
        private void ViewNilaiPraktikan(object sender, EventArgs e)
        {
            listNilaiMahasiswa.Clear();
            NilaiMahasiswa.Clear();

            var service = new IadmClient();
            var mk = service.GetMatKul().FirstOrDefault(x => x.mata_kuliah == comboBoxEdit13.SelectedItem.ToString());
            var dm = new modul()
            {
                matkul = new matkul() { kode_mk = mk.kode_mk }
            };
            var jum_pertemuan = service.jumPraktikum(dm);

            //XtraMessageBox.Show(jum_pertemuan.ToString());
            var p_data = new jadwal_umum()
            {
                id_periode = PeriodeId(comboBoxEdit7),
                fk_jadwalUmum_matakuliah = new matkul()
                {
                    mata_kuliah = comboBoxEdit13.SelectedItem.ToString()
                }
            };
            var p = service.ListPraktikanPraktikum(p_data).ToList();
            for ( int i = 0; i < p.Count; i++ )
            {
                listNilaiMahasiswa.Add(new listNilaimhs() { nrp = p[i].NRP, Nama = p[i].Nama, jumlah_pertemuan = jum_pertemuan });
            }

            XtraMessageBox.Show(PeriodeId(comboBoxEdit7).ToString());
            var dnm = new AbsensiPraktikan()
            {
                JadwalPraktikan = new jadwalPraktikan()
                {
                    id_jadwal_umum = new jadwal_umum()
                    {
                        id_periode = PeriodeId(comboBoxEdit7),
                        fk_jadwalUmum_matakuliah = new matkul()
                        {
                            mata_kuliah = comboBoxEdit13.SelectedItem.ToString()
                        }
                    }
                }
            };
            var nm = service.ambilNilaiPraktikan(dnm).ToList();
            for ( var i = 0; i < listNilaiMahasiswa.Count; i++)
            {
                //cari index di listNilaiMahasiswa
                var index = listNilaiMahasiswa.FindIndex(x => x.nrp == nm[i].JadwalPraktikan.nrp);

                //add nilai to listNilaiMahasiswa
                if (nm[i].Pertemuan.id_pertemuan == 1)
                {
                    listNilaiMahasiswa[index].P01 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 2)
                {
                    listNilaiMahasiswa[index].P02 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 3)
                {
                    listNilaiMahasiswa[index].P03 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 4)
                {
                    listNilaiMahasiswa[index].P04 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 5)
                {
                    listNilaiMahasiswa[index].P05 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 6)
                {
                    listNilaiMahasiswa[index].P06 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 7)
                {
                    listNilaiMahasiswa[index].P07 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 8)
                {
                    listNilaiMahasiswa[index].P08 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 9)
                {
                    listNilaiMahasiswa[index].P09 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 10)
                {
                    listNilaiMahasiswa[index].P10 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 11)
                {
                    listNilaiMahasiswa[index].P11 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 12)
                {
                    listNilaiMahasiswa[index].P12 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 13)
                {
                    listNilaiMahasiswa[index].P13 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 14)
                {
                    listNilaiMahasiswa[index].P14 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 15)
                {
                    listNilaiMahasiswa[index].P15 = nm[i].Nilai;
                }
                if (nm[i].Pertemuan.id_pertemuan == 16)
                {
                    listNilaiMahasiswa[index].P16 = nm[i].Nilai;
                }
            }


            for (var i = 0; i < listNilaiMahasiswa.Count; i++)
            {
                listNilaiMahasiswa[i].nilai_akhir = (listNilaiMahasiswa[i].P01 + listNilaiMahasiswa[i].P02 + listNilaiMahasiswa[i].P03 + listNilaiMahasiswa[i].P04 + listNilaiMahasiswa[i].P05 + listNilaiMahasiswa[i].P06 + listNilaiMahasiswa[i].P07 + listNilaiMahasiswa[i].P08 + listNilaiMahasiswa[i].P09 + listNilaiMahasiswa[i].P10 + listNilaiMahasiswa[i].P11 + listNilaiMahasiswa[i].P12 + listNilaiMahasiswa[i].P13 + listNilaiMahasiswa[i].P14 + listNilaiMahasiswa[i].P15 + listNilaiMahasiswa[i].P16) / listNilaiMahasiswa[i].jumlah_pertemuan;
            }

                //var data = new jadwal_umum()
                //{
                //    id_periode = PeriodeId(comboBoxEdit7)
                //};
                //var jadwal = service.ViewJadwalUmum(data)
                //    .Where(x => x.fk_jadwalUmum_matakuliah.mata_kuliah == comboBoxEdit13.SelectedItem.ToString()).ToList();
                //for (var i = 0; i < jadwal.Count; i++)
                //{
                //    var count = i + 1;
                //    for (int j = 0; j < i; j++)
                //    {
                //        var id = new jadwal_umum() { id_jadwal_umum = jadwal[i].id_jadwal_umum };
                //        var nilai = service.Nilai(id).ToList();
                //        for (var k = 0; k < nilai.Count; k++)
                //        {
                //            var n = new nilaimhs()
                //            {
                //                nrp = nilai[k].JadwalPraktikan.nrp.TrimEnd(),
                //                nilai = nilai[k].Nilai,
                //                pertemuan = nilai[k].Pertemuan.id_pertemuan
                //            };
                //            NilaiMahasiswa.Add(n);
                //        }
                //    }
                //}
                ////XtraMessageBox.Show(NilaiMahasiswa.Count().ToString());
                //for (int i = 0; i < NilaiMahasiswa.Count; i++)
                //{
                //    var index = NilaiMahasiswa.FindIndex(x => x.nrp == NilaiMahasiswa[i].nrp);
                //    var nn = listNilaiMahasiswa.FindIndex(x => x.nrp == NilaiMahasiswa[i].nrp);
                //    if (nn == -1)
                //    {
                //        //XtraMessageBox.Show("ga ada");
                //        listNilaiMahasiswa.Add(new listNilaimhs() { nrp = NilaiMahasiswa[i].nrp });
                //        if (NilaiMahasiswa[i].pertemuan == 1)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P01 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 2)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P02 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 3)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P03 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 4)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P04 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 5)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P05 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 6)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P06 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 7)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P07 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 8)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P08 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 9)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P09 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 10)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P10 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 11)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P11 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 12)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P12 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 13)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P13 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 14)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P14 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 15)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P15 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 16)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P16 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (NilaiMahasiswa[i].pertemuan == 1)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P01 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 2)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P02 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 3)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P03 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 4)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P04 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 5)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P05 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 6)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P06 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 7)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P07 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 8)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P08 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 9)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P09 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 10)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P10 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 11)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P11 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 12)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P12 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 13)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P13 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 14)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P14 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 15)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P15 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //        else if (NilaiMahasiswa[i].pertemuan == 16)
                //        {
                //            foreach (var items in listNilaiMahasiswa.Where(w => w.nrp == NilaiMahasiswa[i].nrp))
                //            {
                //                items.P16 = NilaiMahasiswa[i].nilai;
                //            }
                //        }
                //    }
                //}

                gridControl9.DataSource = listNilaiMahasiswa.Select(x => new {
                NRP     = x.nrp,
                NAMA    = x.Nama,
                x.P01,
                x.P02,
                x.P03,
                x.P04,
                x.P05,
                x.P06,
                x.P07,
                x.P08,
                x.P09,
                x.P10,
                x.P11,
                x.P12,
                x.P13,
                x.P14,
                x.P15,
                x.P16,
                x.jumlah_pertemuan,
                x.nilai_akhir 
            });

            try {
                gridView9.Columns[0].Width = 120;
                gridView9.Columns[1].Width = 180;
                gridView9.Columns[2].Width = 50;
                gridView9.Columns[3].Width = 50;
                gridView9.Columns[4].Width = 50;
                gridView9.Columns[5].Width = 50;
                gridView9.Columns[6].Width = 50;
                gridView9.Columns[7].Width = 50;
                gridView9.Columns[8].Width = 50;
                gridView9.Columns[9].Width = 50;
                gridView9.Columns[10].Width = 50;
                gridView9.Columns[11].Width = 50;
                gridView9.Columns[12].Width = 50;
                gridView9.Columns[13].Width = 50;
                gridView9.Columns[14].Width = 50;
                gridView9.Columns[15].Width = 50;
                gridView9.Columns[16].Width = 50;
                gridView9.Columns[17].Width = 50;
                gridView9.Columns[18].Width = 90;
            }
            catch (Exception)
            {
                //buat nelen erornya
            }

            try {
                gridView9.Columns[2].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[3].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[4].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[5].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[6].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[7].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[8].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[9].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[10].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[11].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[12].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[13].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[14].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[15].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[16].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                gridView9.Columns[17].AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
            }
            catch (Exception)
            {

            }
            

            SettingGridView(gridView9);
            gridControl9.RefreshDataSource();
            service.Close();
        }

        private void simpleButton31_Click(object sender, EventArgs e)
        {
            gridView9.ShowPrintPreview();
        }

        private void gridView9_PrintInitialize(object sender, DevExpress.XtraGrid.Views.Base.PrintInitializeEventArgs e)
        {
            var pb = e.PrintingSystem as PrintingSystemBase;
            pb.PageSettings.Landscape = true;
        }

        private void accordionControlElement30_Click(object sender, EventArgs e)
        {
            comboBoxEdit12.Properties.Items.Clear();
            a_.SelectedPage = a_file;
            var service = new IadmClient();

            AddPeriode(comboBoxEdit15);
            var mk = service.GetMatKul().ToList();
            for ( int i = 0; i < mk.Count; i++ )
            {
                comboBoxEdit12.Properties.Items.Add(mk[i].mata_kuliah);
            }
            comboBoxEdit12.SelectedIndex = 0;
            comboBoxEdit14.SelectedIndex = 0;
            service.Close();
        }

        private void accordionControlElement31_Click(object sender, EventArgs e)
        {
            a_.SelectedPage = a_praktikum;
            ViewMatkul();
        }

        private void accordionControlElement32_Click(object sender, EventArgs e)
        {
            a_.SelectedPage = a_kelas;
            ViewKelas();
        }

        private void simpleButton33_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog() == DialogResult.OK )
            {
                var savePath = fbd.SelectedPath;
                var service = new IadmClient();
                var data = new jadwalPraktikan()
                {
                    id_jadwal_umum = new jadwal_umum()
                    {
                        id_periode = PeriodeId(comboBoxEdit15),
                        fk_jadwalUmum_matakuliah = new matkul()
                        {
                            mata_kuliah = comboBoxEdit12.SelectedItem.ToString()
                        }
                    },
                    absen = new AbsensiPraktikan()
                    {
                        Pertemuan = new pertemuan()
                        {
                            id_jenis_pertemuan = comboBoxEdit14.SelectedItem.ToString()
                        }
                    }
                };
                var files = service.GetFileUjian(data).Select(x => new
                {
                    x.jadwal.JadwalPraktikan.praktikan.NRP,
                    x.jadwal.JadwalPraktikan.praktikan.Nama,
                    x.data_file,
                    x.nama_file
                }).ToList();
                var ujian = comboBoxEdit14.SelectedItem.ToString();
                var praktikum = comboBoxEdit12.SelectedItem.ToString();
                var periode = comboBoxEdit15.SelectedItem.ToString();
                for ( int i = 0; i < files.Count; i++ )
                {
                    var dir = savePath + ujian + " " + praktikum + "//" + files[i].NRP + " - " + files[i].Nama;
                    if ( Exists(dir) )
                        File.WriteAllBytes(dir + "//" + files[i].nama_file, files[i].data_file);
                    else
                    {
                        CreateDirectory(dir);
                        File.WriteAllBytes(dir + "//" + files[i].nama_file, files[i].data_file);
                    }
                }
                service.Close();
            }
        }

        private void simpleButton35_Click(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var data = new jadwalPraktikan()
            {
                id_jadwal_umum = new jadwal_umum()
                {
                    id_periode = PeriodeId(comboBoxEdit15),
                    fk_jadwalUmum_matakuliah = new matkul()
                    {
                        mata_kuliah = comboBoxEdit12.SelectedItem.ToString()
                    }
                },
                absen = new AbsensiPraktikan()
                {
                    Pertemuan = new pertemuan()
                    {
                        id_jenis_pertemuan = comboBoxEdit14.SelectedItem.ToString()
                    }
                }
            };
            gridControl10.DataSource = service.GetFileUjian(data).Select(x => new {
                x.jadwal.JadwalPraktikan.praktikan.NRP,
                x.jadwal.JadwalPraktikan.praktikan.Nama
            }).ToList();
            service.Close();
        }
        private void ViewMatkul()
        {
            var service = new IadmClient();
            gridControl11.DataSource = service.GetMatKul().ToList();
            service.Close();
        }
        private void simpleButton34_Click(object sender, EventArgs e)
        {
            var kode_mk = textEdit3.Text;
            var matkul = textEdit9.Text;
            var service = new IadmClient();

            var data = new matkul()
            {
                kode_mk = kode_mk,
                mata_kuliah = matkul,
            };
            try
            {
                service.InputMatkul(data);
            } catch(Exception)
            {
                XtraMessageBox.Show("input tidak boleh kosong");
            }
            ViewMatkul();
            service.Close();
        }


        private void ViewKelas()
        {
            listBoxControl3.Items.Clear();
            var service = new IadmClient();
            listBoxControl3.Items.AddRange(service.GetKelas().Select(x => x.Kelas).ToArray());
            service.Close();
        }
        private void simpleButton36_Click(object sender, EventArgs e)
        {
            
            var kelas = textEdit10.Text;
            var service = new IadmClient();
            try
            {
                var data = new kelas() { Kelas = kelas };
                service.InputKelas(data);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("tidak ada kelas yang di tambahkan");
            }
            service.Close();
            ViewKelas();
        }

        private void simpleButton37_Click(object sender, EventArgs e)
        {
            var data = new Users
            {
                username = gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[1])
            };

            try
            {
                var service = new IadmClient();
                service.HapusPraktikan(data);
                Cari_Praktikan_Button(sender, e);
            } catch(Exception error)
            {
                XtraMessageBox.Show(error.ToString());
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            EnableStart();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {


        }
        int waktucekkoneksi = 0;
        private void koneksiserver_Tick(object sender, EventArgs e)
        {
            waktucekkoneksi = waktucekkoneksi + 1;
            if(waktucekkoneksi == 180)
            {
                koneksiserver.Stop();
            }
            Ping ping = new Ping();
            PingReply pingresult = ping.Send("127.0.0.1");
            if (pingresult.Status.ToString() == "Success")
            {
                labelControl2.ForeColor = Color.Green;
                labelControl2.Text = "Connected";
            }
            else
            {
                labelControl2.ForeColor = Color.Red;
                labelControl2.Text = "Disconnect";
            }
        }

        private void gridControl4_Click(object sender, EventArgs e)
        {

        }

        private void simpleButton39_Click(object sender, EventArgs e)
        {
            try
            {
                var service = new IadmClient(); //gridView1.GetRowCellDisplayText(gridView1.FocusedRowHandle, gridView1.Columns[1]
                var periode = PeriodeId(comboBoxEdit10);
                var praktikum = gridView4.GetRowCellDisplayText(gridView4.FocusedRowHandle, gridView4.Columns[3]);
                var shift = gridView4.GetRowCellDisplayText(gridView4.FocusedRowHandle, gridView4.Columns[1]);
                var hari = gridView4.GetRowCellDisplayText(gridView4.FocusedRowHandle, gridView4.Columns[0]);
                var nama = gridView4.GetRowCellDisplayText(gridView4.FocusedRowHandle, gridView4.Columns[4]);
                var periodeID = new jadwal_umum() { id_periode = periode };
                var jadwal = service.ViewJadwalUmum(periodeID).ToList()
                    .FirstOrDefault(x => x.fk_jadwalUmum_Shift.id_shift == shift &&
                                         x.fk_jadwalUmum_matakuliah.mata_kuliah == praktikum &&
                                         x.hari == hari);
                var staffID = service.getStaffID().FirstOrDefault(x => x.nama == nama);
                var data = new jadwalStaff()
                {
                    staff = new Staff()
                    {
                        id_staff = staffID.id_staff
                    },
                    jadwal_umum = new jadwal_umum()
                    {
                        id_jadwal_umum = jadwal.id_jadwal_umum
                    }
                };
                service.HapusPJadwalAsisten(data); 
                ViewJadwallStaffClick(sender, e);
                service.Close();
            } catch (Exception error)
            {
                XtraMessageBox.Show(error.ToString());
            }
        }

        private void simpleButton38_Click(object sender, EventArgs e)
        {
            var JadwalAsisten = new FormTambahJadwalAsisten();
            JadwalAsisten.ShowDialog();
        }

        private void simpleButton40_Click(object sender, EventArgs e)
        {
            var kode_mk = gridView11.GetRowCellDisplayText(gridView11.FocusedRowHandle, gridView11.Columns[0]);
            var matkul = gridView11.GetRowCellDisplayText(gridView11.FocusedRowHandle, gridView11.Columns[1]);
            var service = new IadmClient();

            var data = new matkul()
            {
                kode_mk = kode_mk,
                mata_kuliah = matkul,
            };
            service.HapusMataKuliah(data);
            ViewMatkul();
            service.Close();
        }

        private void simpleButton41_Click(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var data = new kelas() { Kelas = listBoxControl3.SelectedItem.ToString()};
            service.HapusKelas(data);
            ViewKelas();
            service.Close();
        }
        [DllImport("user32.dll")]

        [return: MarshalAs(UnmanagedType.Bool)]

        static extern bool SetForegroundWindow(IntPtr hWnd);
        private void timer1_Tick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            labelControl4.Text = service.ServerTime().ToString("HH:mm:ss tt");
            //SetForegroundWindow(this.Handle);
        }

        private void Jadwal_Blank_Paint(object sender, PaintEventArgs e)
        {

        }
        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            EnableStart();
        }

        private void gridControl3_Click(object sender, EventArgs e)
        {

        }

        private void gridControl9_Click(object sender, EventArgs e)
        {

        }

        

    }
}