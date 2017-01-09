using System;
using System.Drawing;
using System.Windows.Forms;
using static client.Library.ConvertFromTo;
using static client.Library.border;
using System.Linq;
using DevExpress.XtraEditors;
using client.lab;
using static System.String;

namespace client
{
    public partial class FrmImportExcel : XtraForm
    {
        public FrmImportExcel()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        readonly OpenFileDialog _dialog = new OpenFileDialog();

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            _dialog.Filter = @"Excel Files (*.xlsx)|*.xlsx";
            if (_dialog.ShowDialog() != DialogResult.OK) return;
            listBoxControl1.Items.Clear();
            var list = ExcelSheetList(_dialog.FileName);
            foreach (var t in list)
            {
                listBoxControl1.Items.Add(t.Replace("'", Empty));
            }
        }

        private void listBoxControl1_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                var sheet = listBoxControl1.SelectedItem.ToString().Replace("'", Empty);
                var data = ExcelToDataTable(_dialog.FileName, sheet);
                listBoxControl2.Items.Clear();
                for (var i = 0; i < data.Rows.Count; i++)
                {
                    listBoxControl2.Items.Add(
                        $"{i + 1:000}           {data.Rows[i][0],-20}         {data.Rows[i][1],-20}");
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
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            var sheet = listBoxControl1.SelectedItem.ToString().Replace("'", Empty);
            var data = ExcelToDataTable(_dialog.FileName, sheet);
            var praktikan = new praktikan[data.Rows.Count];
            var jur = Empty;
            var angk = Empty;
            for (var i = 0; i < data.Rows.Count; i++)
            {
                var nrpmhs = data.Rows[i][0].ToString();
                var biodata = new praktikan()
                {
                    NRP = data.Rows[i][0].ToString(),
                    Nama = data.Rows[i][1].ToString(),
                    jurusan = new jurusan() {KodeJurusan = Concat(nrpmhs[0], nrpmhs[1], nrpmhs[2])},
                    angkatan = new angkatan() {KodeAngkatan = Concat(nrpmhs[3], nrpmhs[4])},
                    Foto = ImageToByteArray(pictureEdit1.Image)
                };
                praktikan[i] = biodata;
                jur = Concat(nrpmhs[0], nrpmhs[1], nrpmhs[2]);
                angk = Concat(nrpmhs[3], nrpmhs[4]);
            }
            try
            {
                var service = new IadmClient();
                var jurusan = service.GetJurusan().FirstOrDefault(q => q.KodeJurusan == jur);
                var angkatan = service.GetAngkatan().FirstOrDefault(q => q.KodeAngkatan == angk);
                var j = jurusan?.KodeJurusan;
                var a = angkatan?.KodeAngkatan;
                if (j == jur && a == angk)
                {
                    service.InsertMultiplePraktikan(praktikan);
                }
                service.Close();
            }
            catch (Exception error)
            {
                XtraMessageBox.Show(error.ToString());
                XtraMessageBox.Show("Gagal import data mahasiswa.");
            }
        }
    }
}