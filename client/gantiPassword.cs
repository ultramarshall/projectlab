using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using client.lik;
namespace client
{
    public partial class gantiPassword : DevExpress.XtraEditors.XtraForm
    {
        readonly string id;

        public gantiPassword(string nrp)
        {
            InitializeComponent();
            id = nrp;
        }

        private void gantiPassword_Load(object sender, EventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            var error = false;
            try
            {
                var service = new IadmClient();
                var data = new Users() { username = id, password = textEdit1.Text };
                service.EditPassword(data);
                service.Close();
            }
            catch (Exception)
            {
                error = true;
                XtraMessageBox.Show("ada kesalahan");
            }

            if(error == false)
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