using System;
using System.Collections.Generic;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using static client.Library.Method;

namespace client
{
    public partial class Form1 : XtraForm
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            int roles = login(username.Text, password.Text);

            if (roles == 1 ) //admin
            {
                ComboBoxEditAdd("Angkatan", comboBoxEdit1);
                ComboBoxEditAdd("Jurusan", comboBoxEdit2);
                Interface.SelectedPage = InterfaceAdmin;
            }
            else if (roles == 2) //kordinator
            {

            }
            else if (roles == 3) //aslab
            {

            }
            else if (roles == 4) //mahasiswa
            {
                Jadwal.SelectedPage = Jadwal_Tersedia; //melihat jadwal tersedia
                Login_Button.Enabled = false;
            }
            else //  tidak terdaftar
            {
                XtraMessageBox.Show("username atau password salah");
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

                praktikan data = new praktikan()
                {
                    AngkatanID = comboBoxEdit1.SelectedIndex + 1,
                    JurusanID = comboBoxEdit2.SelectedIndex + 1
                };
                gridControl1.DataSource = service.GetPraktikan(data).Select(x => new { x.Foto, x.NRP, x.Nama }).ToList();
                gridView1.RowHeight = 60;
                gridView1.Columns["Foto"].Width = 70;
                gridView1.Columns["NRP"].Width = 150;
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

        private void praktikan(object sender, EventArgs e)
        {
            a_.SelectedPage = a_praktikan;
        }

        private void AddPraktikan(object sender, EventArgs e)
        {
            ComboBoxEditAdd("Angkatan", comboBoxEdit3);
            ComboBoxEditAdd("Jurusan", comboBoxEdit4);
            a_.SelectedPage = a_add_user;
            layoutControl6.AllowCustomizationMenu = false;
        }
    }
}
