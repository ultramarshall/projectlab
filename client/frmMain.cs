using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
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
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
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

        private static void SettingGridView(GridView gridView)
        {
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsBehavior.Editable = false;
            gridView.Appearance.SelectedRow.BackColor = Color.Aqua;
            gridView.Appearance.FocusedRow.BackColor = Color.Aqua;
            gridView.OptionsMenu.EnableColumnMenu = false;
            gridView.OptionsView.ShowIndicator = false;
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            for (var i = 0; i < gridView.Columns.Count; i++)
            {
                gridView.Columns[i].OptionsColumn.AllowFocus = false;
                gridView.Columns[i].OptionsColumn.AllowMove = false;
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
                var now = DateTime.Now;
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
                    }
                    service.Close();
                }
                else if (roles == "Admin") Interface.SelectedPage = InterfaceAdmin;
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
            catch (Exception err)
            {
                XtraMessageBox.Show(err.ToString());
            }
        }

        private void gridView6_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Clicks == 2)
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
            var data = new praktikan {NRP = username.Text};
            _dataPraktikan.Add(service.getProfilePraktikan(data));
            simpleLabelItem22.Text = _dataPraktikan[0].NRP;
            simpleLabelItem19.Text = _dataPraktikan[0].Nama;
            simpleLabelItem20.Text = _dataPraktikan[0].jurusan.NamaJurusan;
            simpleLabelItem21.Text = _dataPraktikan[0].angkatan.TahunAngkatan;
            var img = new MemoryStream(_dataPraktikan[0].Foto);
            pictureEdit3.Image = Image.FromStream(img);
            Interface.SelectedPage = InterfacePraktikan;
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
                    angkatan = new angkatan {KodeAngkatan = angkatan.KodeAngkatan},
                    jurusan = new jurusan {KodeJurusan = jurusan.KodeJurusan}
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new {x.Foto, x.NRP, x.Nama}).ToList();
                SettingGridView(gridView1);
                gridView1.Columns[0].OptionsColumn.AllowMerge = DefaultBoolean.False;
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;
                service.Close();
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(err.ToString());
            }
        }

        private void Admin_Logout_Button(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
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

        private void SaveProfilPraktikanClick(object sender, EventArgs e)
        {
            try
            {
                var service = new IadmClient();
                var data = new praktikan {NRP = NrpPraktikan.Text};
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
                    jurusan = new jurusan {KodeJurusan = jurusan.KodeJurusan},
                    angkatan = new angkatan {KodeAngkatan = angkatan.KodeAngkatan},
                    Users = new Users
                    {
                        password = PasswordPraktikan.Text,
                        status = "Praktikan"
                    }
                };
                if (profilepraktikan.NRP == null) //simpan
                    service.AddPraktikan(datapraktikan);
                else //edit
                    service.EditPraktikan(nrp, datapraktikan);
                a_.SelectedPage = a_praktikan;
                Cari_Praktikan_Button(sender, e);
            }
            catch (Exception error)
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
            var jadwal = new jadwal_umum {id_periode = PeriodeId(comboBoxEdit5)};
            gridControl3.DataSource = ToDataTable(service.ViewJadwalUmum(jadwal).Select(x => new
            {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    x.fk_jadwalUmum_Shift.mulai.ToString("HH:mm") + " - " +
                    x.fk_jadwalUmum_Shift.selesai.ToString("HH:mm"),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            }).ToList());
            SettingGridView(gridView5);
        }

        private void jadwal_praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_praktikan;
        }

        private void lihat_jadwal(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var jadwal = new jadwal_umum {id_periode = PeriodeId(comboBoxEdit5)};
            gridControl3.DataSource = ToDataTable(service.ViewJadwalUmum(jadwal).Select(x => new
            {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    x.fk_jadwalUmum_Shift.mulai.ToString("HH:mm") + " - " +
                    x.fk_jadwalUmum_Shift.selesai.ToString("HH:mm"),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            }).ToList());
            SettingGridView(gridView5);
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            var frm = new FrmTambahJadwalUmum();
            frm.ShowDialog();
        }

        private void hapus_jadwal(object sender, EventArgs e)
        {
            var service = new IadmClient();
            try
            {
                var jadwal = new jadwal_umum {id_periode = PeriodeId(comboBoxEdit5)};
                service.DeleteJadwal(jadwal);
            }
            catch (Exception)
            {
                XtraMessageBox.Show("Jadwal tidak dapat dihapus !" + Environment.NewLine + "Periode sudah berlangsung");
            }
            lihat_jadwal(sender, e);
        }

        private void Tambah_Periode_Click(object sender, EventArgs e)
        {
            var frm = new FrmPeriode();
            frm.ShowDialog();
        }

        private void StaffLofout_ButtonClick(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
            _dataStaff.Clear();
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
                    staff = new Staff {id_staff = _dataStaff[0].id_staff},
                    jadwal_umum = new jadwal_umum {id_periode = periode.id_periode}
                };
                gridControl6.DataSource =
                    ToDataTable(service.GetStaffJadwal(jadwal).Select(
                        x => new
                        {
                            x.jadwal_umum.hari,
                            shift = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                            waktu =
                                x.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " +
                                x.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
                            praktikum = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                            kelas = x.jadwal_umum.fk_jadwalUmum_kelas.Kelas
                        }).ToList());
                SettingGridView(gridView6);
                service.Close();
            }
            catch (Exception)
            {
                // ignored
            }
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

        private void JadwalStaffKPraktikumClick(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Praktikum;
            var service = new IadmClient();
            ComboBoxEditAdd("Periode", comboBoxEdit7);
            ComboBoxEditAdd("Semester", comboBoxEdit8);
            var awalSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(0, 4));
            var akhirSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(5, 4));
            try
            {
                var data = new periode
                {
                    semester = comboBoxEdit8.SelectedItem.ToString(),
                    awalSemester = new DateTime(awalSemester, 1, 1, 1, 1, 1),
                    akhirSemester = new DateTime(akhirSemester, 1, 1, 1, 1, 1)
                };
                gridControl8.DataSource = ToDataTable(service.jadwalUmumStaff(data).Select(r => new
                {
                    r.jadwal_umum.hari,
                    shift = r.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                    waktu =
                        r.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " +
                        r.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
                    praktikum = r.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                    r.staff.nama
                }).ToList());
                SettingGridView(gridView8);
                /*
                 *  seting column merge
                 */
                gridView8.OptionsView.AllowCellMerge = true;
                gridView8.Columns["nama"].OptionsColumn.AllowMerge = DefaultBoolean.False;
                /* add manual RepositoryItemComboBox 
                RepositoryItemComboBox nama = new RepositoryItemComboBox( )
                {
                    //config RepositoryItemComboBox nama
                    TextEditStyle = TextEditStyles.DisableTextEditor,
                    AllowDropDownWhenReadOnly = DevExpress.Utils.DefaultBoolean.True,
                };
                //add manual EventHandler SelectedIndexChanged
                nama.SelectedIndexChanged += new EventHandler( nama_SelectedIndexChanged );
                var a = service.getStaffID( ).Select( n => n.nama ).ToList( );
                //add listArray to RepositoryItemComboBox from a
                nama.Items.AddRange( a );
                gridControl8.RepositoryItems.Add( nama );
                //config column nama
                gridView8.Columns["nama"].ColumnEdit = nama;
                gridView8.Columns["nama"].OptionsColumn.AllowEdit = true;
                gridView8.Columns["nama"].OptionsColumn.ReadOnly = false;
                gridControl8.ForceInitialize( );
                */
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
            }
        }

        //private void nama_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ComboBoxEdit editor = sender as ComboBoxEdit;
        //    IadmClient service = new IadmClient();
        //    //get id_staff by name, to show s.id_staff
        //    var s = service.getStaffID().FirstOrDefault(id => id.nama == editor.SelectedItem.ToString());
        //    //XtraMessageBox.Show(s.id_staff);
        //    //periode.id_periode
        //    var periode = service.viewPeriode().FirstOrDefault(
        //        x => x.awalSemester < DateTime.Now &&
        //             x.akhirSemester > DateTime.Now
        //        );
        //    //XtraMessageBox.Show(periode.id_periode.ToString());
        //    // get value all column in selected rows
        //    DataRow row = gridView8.GetDataRow(gridView8.FocusedRowHandle);
        //    //get id_staff by name, to show s.id_staff
        //    var ss = service.getStaffID().FirstOrDefault(id => id.nama == row[4].ToString());
        //    //XtraMessageBox.Show(ss.id_staff);
        //    jadwalStaff data = new jadwalStaff()
        //    {
        //        staff = new Staff()
        //        {
        //            id_staff = ss.id_staff
        //        },
        //        jadwal_umum = new jadwal_umum()
        //        {
        //            fk_jadwalUmum_Shift = new Shift() { id_shift = row[1].ToString() },
        //            fk_jadwalUmum_periode = new periode() { id_periode = periode.id_periode },
        //            fk_jadwalUmum_matakuliah = new matkul() { mata_kuliah = row[3].ToString() }
        //        }
        //    };
        //    service.updateJadwalStaff(s.id_staff, data);
        //    service.Close();
        //}

        private void K_Logout_Click(object sender, EventArgs e)
        {
            Interface.SelectedPage = InterfaceLogin;
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var awalSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(0, 4));
            var akhirSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(5, 4));
            try
            {
                var data = new periode
                {
                    semester = comboBoxEdit8.SelectedItem.ToString(),
                    awalSemester = new DateTime(awalSemester, 1, 1, 1, 1, 1),
                    akhirSemester = new DateTime(akhirSemester, 1, 1, 1, 1, 1)
                };
                gridControl8.DataSource = ToDataTable(service.jadwalUmumStaff(data).Select(r => new
                {
                    r.jadwal_umum.hari,
                    shift = r.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                    waktu =
                        r.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " +
                        r.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
                    praktikum = r.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                    r.staff.nama
                }).ToList());
                SettingGridView(gridView8);
                /*
                 *  seting column merge
                 */
                gridView8.OptionsView.AllowCellMerge = true;
                gridView8.Columns["nama"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            }
            catch (Exception)
            {
            }
        }

        private void accordionControlElement26_Click(object sender, EventArgs e)
        {
        }

        private void AbsensiPraktikanClick(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffJadwal;
            AbsensiPraktikanSearch(sender, e);
        }

        private void AbsensiPraktikanSearch(object sender, EventArgs e)
        {
            var shift = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[1]);
            var mk = gridView6.GetRowCellDisplayText(gridView6.FocusedRowHandle, gridView6.Columns[3]);
            viewStaff.SelectedPage = viewStaffJadwal;
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
                foto = x.Foto,
                nrp = x.NRP,
                nama = x.Nama,
                nilai = x.absen.Nilai
            }).ToList());
            gridView5.Columns["foto"].OptionsColumn.AllowEdit = false;
            gridView5.Columns["nrp"].OptionsColumn.AllowEdit = false;
            gridView5.Columns["nama"].OptionsColumn.AllowEdit = false;
            gridView5.Columns["foto"].OptionsColumn.AllowFocus = false;
            gridView5.Columns["nrp"].OptionsColumn.AllowFocus = false;
            gridView5.Columns["nama"].OptionsColumn.AllowFocus = false;
            gridView5.Appearance.FocusedRow.BackColor = Color.Aqua;
            var nilai = new RepositoryItemTextEdit();
            var photo = new RepositoryItemPictureEdit();

            nilai.EditValueChanged += new EventHandler(nilai_EditValueChanged);
            gridControl5.RepositoryItems.Add(nilai);
            gridControl5.RepositoryItems.Add(photo);

            gridView5.Columns["foto"].ColumnEdit = photo;
            gridView5.Columns["nilai"].ColumnEdit = nilai;
            gridView5.Columns["nilai"].OptionsColumn.ReadOnly = false;
            gridView5.RowHeight = 60;
            gridControl5.ForceInitialize();
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

            if (nil.Text == "")
            {
                a = "0";
            }

            var data = new AbsensiPraktikan()
            {
                Nilai = int.Parse(a),
                staff = new Staff() {id_staff = _dataStaff[0].id_staff},
                Pertemuan = new pertemuan() {id_jenis_pertemuan = comboBoxEdit9.SelectedItem.ToString()},
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
            var jadwal = new periode {id_periode = PeriodeId(comboBoxEdit10)};
            gridControl4.DataSource = ToDataTable(service.jadwalUmumStaff(jadwal).Select(x => new
            {
                HARI = x.jadwal_umum.hari,
                SHIFT = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    x.jadwal_umum.fk_jadwalUmum_Shift.mulai.ToString("HH:mm") + " - " +
                    x.jadwal_umum.fk_jadwalUmum_Shift.selesai.ToString("HH:mm"),
                PRAKTIKUM = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                PENGAJAR = x.staff.nama
            }).ToList());
            SettingGridView(gridView4);
            gridView4.OptionsView.AllowCellMerge = true;
            gridView4.Columns["PENGAJAR"].OptionsColumn.AllowMerge = DefaultBoolean.False;
        }

        private void PrintJadwalStaffClick(object sender, EventArgs e)
        {
            gridView4.ShowPrintPreview();
        }

        private void EditStaffClick(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var idStaff = gridView2.GetRowCellDisplayText(gridView2.FocusedRowHandle, gridView2.Columns[0]);
            var data = new Staff {id_staff = idStaff};
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

        private void textEdit4_EditValueChanging(object sender, ChangingEventArgs e)
        {
            textEdit5.Text = textEdit4.Text;
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
            var data = new Staff {id_staff = textEdit4.Text};
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
                if (profileStaff.id_staff == null) //simpan
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
    }
}