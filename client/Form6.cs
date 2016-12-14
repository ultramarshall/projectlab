using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Text.RegularExpressions;
using static client.Library.Method;
namespace client
{
    public partial class Form6 : DevExpress.XtraEditors.XtraForm
    {
       
        DateTime waktu = new DateTime();
        public Form6()
        {
            InitializeComponent();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            waktu = DateTime.Now;
            

            if (waktu.ToString("hh:mm:ss") == "02:26:00")
            {
                this.Close();
            }
            else{
                this.Text = waktu.ToString("hh:mm:ss");
            }

        }

        bool check(string souceString, string stringLike)
        {
            bool a = false;

            try
            {
                bool result = false;
                int y = souceString.Length;
                int x = stringLike.Length;

                for (int i = 0; i < souceString.Length; i++)
                {
                    if (y < x)
                    {
                        x =x-1;
                    }

                    result = souceString.Substring(i, x).Like(stringLike);

                    if (result == true)
                    {
                        a = true;
                        continue;
                    };
                    Console.WriteLine(result);
                }
                
            }
            catch (Exception) { this.labelControl1.Text = "false"; this.labelControl1.ForeColor = Color.Red; }
            return a;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            DateTime start = new DateTime(2016, 12, 12, 11, 0, 0);
            DateTime finish = new DateTime(2016, 12, 12, 11, 2, 0);

            var now = new DateTime(2016, 12, 12, 11, 2, 0);

            if((now >= start) && (now <= finish))
            {
                XtraMessageBox.Show("true");
            }else
            {
                XtraMessageBox.Show("false");
            }
        }
    }

        }
  
