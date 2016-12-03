using System;
using System.Collections.Generic;
using System.Linq;
using client.lab;
using DevExpress.XtraEditors;
using static client.Library.Method;
using DevExpress.XtraGrid.Views.Grid;
using static client.Library.convertFromTo;

namespace client
{
    public partial class Form1 : XtraForm
    {

        public Form1()
        {
            InitializeComponent();
            disablelayoutcontrolmenu();
        }
        void disablelayoutcontrolmenu()
        {
            layoutControl1.AllowCustomizationMenu = false;
            layoutControl2.AllowCustomizationMenu = false;
            layoutControl3.AllowCustomizationMenu = false;
            layoutControl4.AllowCustomizationMenu = false;
            layoutControl5.AllowCustomizationMenu = false;
            layoutControl6.AllowCustomizationMenu = false;
            layoutControl7.AllowCustomizationMenu = false;
            //layoutControl8.AllowCustomizationMenu = false;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string roles = PostLogin(username.Text, password.Text);

            if (roles == "Admin") //admin
            {
                Interface.SelectedPage = InterfaceAdmin;
                ComboBoxEditAdd("Angkatan", comboBoxEdit1);
                ComboBoxEditAdd("Jurusan", comboBoxEdit2);
            }
            else if (roles == "Koordinator") //kordinator
            {

            }
            else if (roles == "Asisten") //aslab
            {

            }
            else if (roles == "Mahasiswa") //mahasiswa
            {
                Jadwal.SelectedPage = Jadwal_Tersedia; //melihat jadwal tersedia
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
            CariPraktikan(comboBoxEdit1, comboBoxEdit2, gridControl1, gridView1);
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
        }

        private void import_excel(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        private void jadwal_umum(object sender, EventArgs e)
        {
            a_.SelectedPage = a_jadwal_umum;
        }
    }
}
