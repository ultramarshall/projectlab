using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace client
{
    public partial class Form5 : DevExpress.XtraEditors.XtraForm
    {
        public Form5()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var year = int.Parse("2014");
            DateTime dateTime = new DateTime(year, 1, 1);
            XtraMessageBox.Show(dateTime.ToString("yyyy"));
        }
    }
}