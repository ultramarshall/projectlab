using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using static client.Library.Method;
using static client.Library.border;
using static client.Library.ConvertFromTo;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;

namespace client
{
    public partial class FrmMain : XtraForm
    {
        private readonly List<Staff> _dataStaff = new List<Staff>();
        private readonly List<praktikan> _dataPraktikan = new List<praktikan>();
        private jadwalPraktikan _jadwalPraktikan = new jadwalPraktikan();


        public FrmMain ()
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

                var culture = new CultureInfo("id-ID");// waktu format indonesia
                var day = culture.DateTimeFormat.GetDayName(DateTime.Today.DayOfWeek); //menampilkan hari ini/ ex: Senin, Selasa, Rabu, Kamis, Jumat, Sabtu
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
                                {
                                    XtraMessageBox.Show("tidak bisa login. praktikum belum di mulai");
                                }
                                else if (now.TimeOfDay >= timeEnd) // praktikum sudah lewat
                                {
                                    XtraMessageBox.Show("tidak bisa login. praktikum sudah selesai");
                                }
                                else
                                {
                                    layoutControlItem6.Text = @"Praktikum " +
                                                              jadwal[0].id_jadwal_umum.fk_jadwalUmum_matakuliah
                                                                  .mata_kuliah;
                                    var data = new jadwalPraktikan()
                                    {
                                        id_jadwal_praktikan = jadwal[0].id_jadwal_praktikan
                                    };
                                    var praktikumTersedia = service.GetPertemuan(data).Select(x => x.id_jenis_pertemuan);
                                    listBoxControl1.Items.AddRange(praktikumTersedia.ToArray());
                                    Jadwal.SelectedPage = Jadwal_Tersedia;
                                }
                            }
                            else
                            {
                                XtraMessageBox.Show("hari ini tidak ada jadwal praktikum");}
                        }
                    }
                    catch (Exception ){
                        XtraMessageBox.Show("tidak ada praktikum saat ini");
                    }
                    
                }
                else if (roles == "Asisten") //aslab
                {
                    Interface.SelectedPage = InterfaceStaff;
                    var data = new Staff() //kirim id_staff 
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

                        //periode.id_periode
                        var periode = service.viewPeriode().FirstOrDefault(
                            x => x.awalSemester < DateTime.Now &&
                                 x.akhirSemester > DateTime.Now
                            );


                        var jadwal = new jadwalStaff()
                        {
                            staff = new Staff() {id_staff = _dataStaff[0].id_staff },
                            jadwal_umum = new jadwal_umum() { id_periode = periode.id_periode }
                        };
                        gridControl6.DataSource = service.GetStaffJadwal(jadwal).Select(
                            x=> new
                            {
                                hari = x.jadwal_umum.hari,// hari
                                shift = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                                waktu = x.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " + x.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
                                praktikum = x.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah,
                                kelas = x.jadwal_umum.fk_jadwalUmum_kelas.Kelas
                            });
                    }
                    service.Close();
                }else if (roles == "Admin")
                {Interface.SelectedPage = InterfaceAdmin;
                }
                else if (roles == "Koordinator") //kordinator
                {
                    Interface.SelectedPage = InterfaceKoordinator;
                    var data = new Staff() //kirim id_staff 
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

        }

        private void MulaiButton_Click(object sender, EventArgs e)
        {
            Jadwal.SelectedPage = Jadwal_Blank;

            var service = new IadmClient();

            var p = service.ListPertemuan().FirstOrDefault(x=> x.id_jenis_pertemuan == listBoxControl1.SelectedItem.ToString());

            var key = new AbsensiPraktikan()
            {
                JadwalPraktikan = new jadwalPraktikan()
                {
                    id_jadwal_praktikan = _jadwalPraktikan.id_jadwal_praktikan
                },
                Pertemuan = new pertemuan()
                {
                    id_pertemuan = p.id_pertemuan
                },};

            service.PostAbsenPraktikan(key);

            var data = new praktikan() { NRP = username.Text };
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
                Debug.Assert(angkatan?.KodeAngkatan != null, "angkatan.KodeAngkatan != null");
                Debug.Assert(jurusan != null, "jurusan != null");
                var data = new praktikan()
                {
                    angkatan = new angkatan() { KodeAngkatan = angkatan.KodeAngkatan },
                    jurusan = new jurusan() { KodeJurusan = jurusan.KodeJurusan }
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama }).ToList();
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;
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

        private void Praktikan(object sender, EventArgs e)
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
            var frm = new FrmImportExcel();
            frm.ShowDialog();
        }

        private void jadwal_umum(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_umum;

            var service = new IadmClient( );
            var periode = service.viewPeriode().Select(x=> new {x.semester, x.awalSemester, x.akhirSemester}).ToList();
            foreach (var t in periode)
            {
                comboBoxEdit5.Properties.Items.Add(string.Format("{0} {1}/{2}", t.semester, t.awalSemester.ToString("yyyy"),t.akhirSemester.ToString("yyyy")));
            }

            var AwalTahun = string.Empty;
            var AkhirTahun = string.Empty;
            var cmbbxperiode = comboBoxEdit5.SelectedItem.ToString();
            if (cmbbxperiode[5] == ' ')
            {
                AwalTahun = cmbbxperiode.Substring(6, 4);
                AkhirTahun = cmbbxperiode.Substring(11, 4);
            }
            else
            {
                AwalTahun = cmbbxperiode.Substring( 7, 4 );
                AkhirTahun = cmbbxperiode.Substring( 12, 4 );
            }

            XtraMessageBox.Show(AwalTahun);
            XtraMessageBox.Show(AkhirTahun);

            //var jadwal = new jadwal_umum( )
            //{
            //    fk_jadwalUmum_periode = new periode( )
            //    {
            //        id_periode =,
            //    },
            //};
            //gridControl3.DataSource = service.ViewJadwalUmum( jadwal ).Select( x => new { x.hari, x.id_shift, x.waktu, x.mata_kuliah, x.kelas } ).ToList( );
        }

        private void jadwal_praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_praktikan;
        }

        private void lihat_jadwal(object sender, EventArgs e)
        {
            //try
            //{
            //    var service = new IadmClient();
                
            //    var start = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(0, 4));
            //    var finish = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(5, 4));

            //    jadwal_umum jadwal = new jadwal_umum()
            //    {
            //        fk_jadwalUmum_periode = new periode() {
            //            semester = comboBoxEdit5.Text.TrimEnd(),
            //            awalSemester = new DateTime(start, 1, 1),
            //            akhirSemester = new DateTime(finish, 1, 1)
            //        },
            //    };
            //    gridControl3.DataSource = service.ViewJadwalUmum(jadwal).Select(x => new {
            //        x.hari,
            //        shift = x.fk_jadwalUmum_Shift.id_shift,
            //        waktu = x.fk_jadwalUmum_Shift.mulai.ToString("HH:mm") + " - " + x.fk_jadwalUmum_Shift.selesai.ToString("HH:mm"),
            //        praktikum =x.fk_jadwalUmum_matakuliah.mata_kuliah,
            //        kelas = x.fk_jadwalUmum_kelas.Kelas
            //    }).ToList();

            //    gridView3.Columns["shift"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            //    gridView3.Columns["waktu"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            //    gridView3.Columns["praktikum"].OptionsColumn.AllowMerge = DefaultBoolean.False;
            //    gridView3.Columns["kelas"].OptionsColumn.AllowMerge = DefaultBoolean.False;

            //    //gridView3.Columns["id_shift"].Caption = "shift";
            //    //gridView3.Columns["mata_kuliah"].Caption = "praktikum";

            //    service.Close();
            //}
            //catch (Exception err)
            //{
            //    XtraMessageBox.Show(err.ToString());
            //}
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            var frm = new FrmTambahJadwalUmum();
            frm.ShowDialog();
        }

        private void hapus_jadwal(object sender, EventArgs e)
        {
            //var service = new IadmClient();
            //var start = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(0, 4));
            //var finish = int.Parse(comboBoxEdit6.SelectedItem.ToString().Substring(5, 4));

            //var jadwal = new jadwal_umum()
            //{
            //    fk_jadwalUmum_periode = new periode()
            //    {
            //        semester = comboBoxEdit5.Text.TrimEnd(),
            //        awalSemester = new DateTime(start, 1, 1),
            //        akhirSemester = new DateTime(finish, 1, 1)
            //    },
            //};
            //try
            //{
            //    service.DeleteJadwal(jadwal);
            //}
            //catch (Exception)
            //{
            //    XtraMessageBox.Show( "Jadwal tidak dapat dihapus !" + Environment.NewLine + "Periode sudah berlangsung" );
            //}
            //lihat_jadwal(sender, e);
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
                var jadwal = new jadwalStaff()
                {
                    staff = new Staff() { id_staff = _dataStaff[0].id_staff },
                    jadwal_umum = new jadwal_umum() { id_periode = periode.id_periode }
                };
                gridControl6.DataSource = ToDataTable(service.GetStaffJadwal(jadwal).Select(
                    x => new
                    {
                        x.jadwal_umum.hari,
                        shift = x.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                        waktu = x.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " + x.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
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

        private void K_Praktikum_Click(object sender, EventArgs e)
        {
            K_.SelectedPage = K_Praktikum;
            var service = new IadmClient();
            ComboBoxEditAdd("Periode",comboBoxEdit7);
            ComboBoxEditAdd("Semester",comboBoxEdit8);
            var awalSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(0, 4));
            var akhirSemester = int.Parse(comboBoxEdit7.SelectedItem.ToString().Substring(5, 4));
            try
            {

                var data = new periode()
                {
                    semester = comboBoxEdit8.SelectedItem.ToString(),
                    awalSemester = new DateTime(awalSemester, 1, 1, 1, 1, 1),
                    akhirSemester = new DateTime(akhirSemester, 1, 1, 1, 1, 1)
                };

                gridControl8.DataSource = ToDataTable(service.jadwalUmumStaff(data).Select(r => new
                {
                    r.jadwal_umum.hari,
                    shift = r.jadwal_umum.fk_jadwalUmum_Shift.id_shift,
                    waktu = r.jadwal_umum.fk_jadwalUmum_Shift.mulai.TimeOfDay + " - " + r.jadwal_umum.fk_jadwalUmum_Shift.selesai.TimeOfDay,
                    praktikum = r.jadwal_umum.fk_jadwalUmum_matakuliah.mata_kuliah, r.staff.nama,
                } ).ToList());
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

        }

        private void accordionControlElement26_Click(object sender, EventArgs e)
        {
        }
        private void AbsensiPraktikanClick(object sender, EventArgs e)
        {
            viewStaff.SelectedPage = viewStaffJadwal;
           
            var service = new IadmClient();
            var periode = service.viewPeriode( ).FirstOrDefault(
                    x => x.awalSemester < DateTime.Now &&
                         x.akhirSemester > DateTime.Now
                    );
            var data = new AbsensiPraktikan()
            {
                Pertemuan = new pertemuan( )
                {
                    id_jenis_pertemuan = comboBoxEdit9.SelectedItem.ToString()
                },
                JadwalPraktikan = new jadwalPraktikan( )
                {
                    id_jadwal_umum = new jadwal_umum( )
                    {
                        fk_jadwalUmum_periode = new periode( )
                        {
                            id_periode = periode.id_periode
                        },
                        fk_jadwalUmum_Shift = new Shift( )
                        {
                            id_shift = comboBoxEdit6.SelectedItem.ToString( )
                        }
                    }
                }
            };
            gridControl5.DataSource = service.GetAbsensiPraktikans(data).Select(x=>new
            {
                foto=x.Foto,
                nrp=x.NRP,
                nama=x.Nama
            });
            SettingGridView(gridView5);
            gridView5.RowHeight = 60;
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            if (gridView6 == null || gridView6.SelectedRowsCount == 0) return;

            var rows = new DataRow[gridView6.SelectedRowsCount];

            for (var i = 0; i < gridView6.SelectedRowsCount; i++)
            {
                rows[i] = gridView6.GetDataRow(gridView6.GetSelectedRows()[i]);
                XtraMessageBox.Show(rows[i].ToString());
            }
        }

        private void absensiPraktikanSearch (object sender, EventArgs e)
        {
            AbsensiPraktikanClick(sender,e);
        }
    }
}
