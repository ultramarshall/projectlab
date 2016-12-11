using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using static client.Library.Method;
using static client.Library.border;
using System.Windows.Forms;

namespace client
{
    public partial class Form1 : XtraForm
    {
        List<Staff> data_staff = new List<Staff>();


        public Form1()
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

        private void LoginButton_Click(object sender, EventArgs e)
        {
            try
            {
                string roles = PostLogin(username.Text, password.Text);

                if (roles == "Admin") //admin
                {
                    Interface.SelectedPage = InterfaceAdmin;
                    
                    
                }
                else if (roles == "Koordinator") //kordinator
                {
                    Interface.SelectedPage = InterfaceKoordinator;
                }
                else if (roles == "Asisten") //aslab
                {
                    Interface.SelectedPage = InterfaceStaff;


                    IadmClient service = new IadmClient();
                    
                    Staff data = new Staff() //kirim id_staff 
                    {
                        id_staff = username.Text
                    };


                    data_staff.Add(service.GetStaffID(data));
                    simpleLabelItem11.Text = data_staff[0].id_staff;
                    simpleLabelItem12.Text = data_staff[0].nama;
                    simpleLabelItem13.Text = data_staff[0].no_hp;simpleLabelItem14.Text = data_staff[0].alamat;
                    MemoryStream foto = new MemoryStream(data_staff[0].foto);
                    pictureEdit2.Image = Image.FromStream(foto);

                    gridControl6.DataSource = service.GetStaffJadwal(data_staff[0].id_staff);
                    service.Close();
                }else if (roles == "Praktikan") //mahasiswa
                {
                    Jadwal.SelectedPage = Jadwal_Tersedia; //melihat jadwal tersedia
                }
                else //  tidak terdaftar
                {
                    XtraMessageBox.Show("username atau password salah");
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
            Login_Button.Enabled = true;
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
                
                praktikan data = new praktikan()
                {
                    KodeAngkatan = angkatan.KodeAngkatan,
                    KodeJurusan = jurusan.KodeJurusan
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama, x.KodeAngkatan, x.KodeJurusan }).ToList();
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;

                gridView1.Columns["NRP"].Caption = "NO MAHASISWA";
                gridView1.Columns["Foto"].Caption = "FOTO";
                gridView1.Columns["Nama"].Caption = "NAMA";
                gridView1.Columns["KodeAngkatan"].Caption = "ANGKATAN";
                gridView1.Columns["KodeJurusan"].Caption = "JURUSAN";

                for (int i = 0; i < gridView1.Columns.Count; i++)
                {
                    gridView1.Columns[i].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                }

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
            Form2 frm = new Form2();
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
            Form3 frm = new Form3();
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

        private void listBoxControl2_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                var id_staff = listBoxControl2.SelectedValue.ToString();
                IadmClient service = new IadmClient();
                gridControl4.DataSource = service.GetStaffJadwal(id_staff);
            } catch (Exception)
            {
                return;
            }
        }

        private void accordionControlElement6_Click(object sender, EventArgs e)
        {
            Form4 frm = new Form4();
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
    }
}
