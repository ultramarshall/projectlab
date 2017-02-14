using System;
using client.lab;
using DevExpress.XtraEditors;
using System.Linq;
using System.Collections.Generic;

namespace client
{
    public partial class FormTambahJadwalAsisten : DevExpress.XtraEditors.XtraForm
    {
        public FormTambahJadwalAsisten()
        {
            InitializeComponent();
        }

        private void TambahJadwalAsisten_Load(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var asisten = service.getStaffID().ToList();
            var praktikum = service.GetMatKul().ToList();
            for (var i = 0; i < asisten.Count(); i++)
            {
                comboBoxEdit1.Properties.Items.Add(asisten[i].id_staff);
            }
            for (var i = 0; i < praktikum.Count(); i++)
            {
                comboBoxEdit4.Properties.Items.Add(praktikum[i].mata_kuliah);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var service = new IadmClient();
            var periode = service.viewPeriode().ToList()
                .FirstOrDefault(x=> x.awalSemester < DateTime.Now && 
                                    x.akhirSemester > DateTime.Now);
            var praktikum = comboBoxEdit4.SelectedItem.ToString();
            var shift = comboBoxEdit3.SelectedItem.ToString();
            var periodeID = new jadwal_umum() { id_periode = periode.id_periode };
            var jadwal = service.ViewJadwalUmum(periodeID).ToList()
                .FirstOrDefault(x => x.fk_jadwalUmum_Shift.id_shift == shift &&
                                     x.fk_jadwalUmum_matakuliah.mata_kuliah == praktikum &&
                                     x.hari == comboBoxEdit2.SelectedItem.ToString());

            bool errorMessage = false;
            try
            {
                List<jadwalStaff> j = new List<jadwalStaff>();
                var data = new jadwalStaff()
                {
                    staff = new Staff()
                    {
                        id_staff = comboBoxEdit1.SelectedItem.ToString()
                    },
                    jadwal_umum = new jadwal_umum()
                    {
                        id_jadwal_umum = jadwal.id_jadwal_umum
                    }
                };
                j.Add(data);
                service.AddJadwalStaffAsisten(j.ToArray());
                service.Close();
            }
            catch(Exception)
            {
                errorMessage = true;
                XtraMessageBox.Show("Tidak ada praktikum " + praktikum +" shift " + shift + " hari " + comboBoxEdit2.SelectedItem.ToString());
            }

            if(errorMessage == false)
            {
                Close();
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}