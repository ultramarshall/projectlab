using System;
using System.Drawing;
using DevExpress.XtraEditors;
using static client.Library.Method;
namespace client
{
    public partial class Form6 : DevExpress.XtraEditors.XtraForm
    {

        DateTime waktu = new DateTime();

        public Form6()
        {
            InitializeComponent();
            //timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            waktu = DateTime.Now;


            if (waktu.ToString("hh:mm:ss") == "02:26:00")
            {
                this.Close();
            }
            else
            {
                this.Text = waktu.ToString("hh:mm:ss");
            }

        }

        private bool Check(string souceString, string stringLike)
        {
            var a = false;

            try
            {
                int y = souceString.Length;
                int x = stringLike.Length;

                for (int i = 0; i < souceString.Length; i++)
                {
                    if (y < x)
                    {
                        x = x - 1;
                    }

                    var result = souceString.Substring(i, x).Like(stringLike);

                    if (result)
                    {
                        a = true;
                        continue;
                    }
                }

            }
            catch (Exception)
            {
                labelControl1.Text = @"false";
                labelControl1.ForeColor = Color.Red;
            }
            return a;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            //DateTime start = new DateTime(2016, 12, 12, 11, 0, 0);
            //DateTime finish = new DateTime(2016, 12, 12, 11, 2, 0);
            //var now = new DateTime(2016, 12, 12, 11, 2, 0);
            //if ((now >= start) && (now <= finish))
            //{
            //    XtraMessageBox.Show("true");
            //}
            //else
            //{
            //    XtraMessageBox.Show("false");
            //}


        }

        private void ratingControl1_EditValueChanged(object sender, EventArgs e)
        {
            XtraMessageBox.Show(ratingControl1.EditValue.ToString());}
    }

}
  
