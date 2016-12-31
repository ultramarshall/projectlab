using System;
using System.Drawing;
using DevExpress.XtraEditors;
using static client.Library.border;

namespace client
{
    public partial class FrmExitApp : XtraForm
    {
        public FrmExitApp()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var year = int.Parse("2014");
            var dateTime = new DateTime(year, 1, 1);
            XtraMessageBox.Show(dateTime.ToString("yyyy"));
        }
    }
}