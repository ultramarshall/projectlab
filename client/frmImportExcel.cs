using System;
using System.Drawing;
using System.Windows.Forms;
using static client.Library.convertFromTo;
using static client.Library.border;
using System.Linq;
using System.Data;
using DevExpress.XtraEditors;
using client.lab;

namespace client
{
    public partial class frmImportExcel : XtraForm
    {

        public frmImportExcel()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        OpenFileDialog dialog = new OpenFileDialog();
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            dialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                listBoxControl1.Items.Clear();
                var list = ExcelSheetList(dialog.FileName);
                for (int i = 0; i < list.Count(); i++)
                {
                    listBoxControl1.Items.Add(list[i].Replace("'", string.Empty));
                }
            }
        }
        private void listBoxControl1_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string sheet = listBoxControl1.SelectedItem.ToString().Replace("'", string.Empty);
                DataTable data = ExcelToDataTable(dialog.FileName, sheet);
                listBoxControl2.Items.Clear();
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    listBoxControl2.Items.Add(string.Format("{0:000}           {1,-20}         {2,-20}", i + 1, data.Rows[i][0], data.Rows[i][1]));
                }
                if (data.Rows.Count <= 0)
                {
                    listBoxControl2.Enabled = false;
                    simpleButton3.Enabled = false;
                }
                else
                {
                    listBoxControl2.Enabled = true;
                    simpleButton3.Enabled = true;
                }
            }
            catch (Exception)
            {
                //XtraMessageBox.Show(error.ToString());
                return;
            }
        }
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            string sheet = listBoxControl1.SelectedItem.ToString().Replace("'", string.Empty);
            DataTable data = ExcelToDataTable(dialog.FileName, sheet);
            praktikan[] praktikan = new praktikan[data.Rows.Count];
            string jur = String.Empty;
            string angk = String.Empty;
            for (int i = 0; i < data.Rows.Count; i++)
            {
                
                string nrpmhs = data.Rows[i][0].ToString();
                praktikan biodata = new praktikan()
                {
                    NRP = data.Rows[i][0].ToString(),
                    Nama = data.Rows[i][1].ToString(),
                    jurusan = new jurusan() { KodeJurusan = String.Concat(nrpmhs[0], nrpmhs[1], nrpmhs[2]) },
                    angkatan = new angkatan() { KodeAngkatan = String.Concat(nrpmhs[3], nrpmhs[4]) },
                    Foto = imageToByteArray(pictureEdit1.Image)
                };
                praktikan[i] = biodata;
                jur = String.Concat(nrpmhs[0], nrpmhs[1], nrpmhs[2]);
                angk = String.Concat(nrpmhs[3], nrpmhs[4]);

            }
            try
            {
                IadmClient service = new IadmClient();
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.KodeJurusan == jur);
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.KodeAngkatan == angk);
                string j = jurusan.KodeJurusan;
                string a = angkatan.KodeAngkatan;
                if (j == jur && a == angk)
                {
                    service.InsertMultiplePraktikan(praktikan);
                }
                service.Close();

            } catch (Exception)
            {
                XtraMessageBox.Show("Gagal import data mahasiswa.");
            }
           
        }
    }
}