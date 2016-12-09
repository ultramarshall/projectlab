using System;
using System.Drawing;
using DevExpress.XtraEditors;
using client.lab;
using static client.Library.border;
using System.Globalization;
using System.Linq;

namespace client
{
    public partial class Form4 : DevExpress.XtraEditors.XtraForm
    {
        public Form4()
        {
            InitializeComponent();
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // Format tgl indonesia
            DateFormatCultureInfo(dateEdit1, dateEdit2);

            //
            IadmClient service = new IadmClient();

            //
            gridControl1.DataSource = service.viewPeriode().Select(x => new
                                                            {
                                                                semester = string.Format("{0} {1}/{2}", 
                                                                               x.semester, 
                                                                               x.awalSemester.ToString("yyyy"), 
                                                                               x.akhirSemester.ToString("yyyy")),
                                                                awalSemester = x.awalSemester.ToString("dd MMMM yyyy", new CultureInfo("id-ID")),
                                                                akhirSemester = x.akhirSemester.ToString("dd MMMM yyyy", new CultureInfo("id-ID"))
                                                            }).ToList();

            service.Close();
        }
      
        private void DateFormatCultureInfo(DateEdit Control1, DateEdit Control2)
        {
            CultureInfo(Control1);
            CultureInfo(Control2);
        }

        private void CultureInfo(DateEdit Control)
        {
            Control.Properties.Mask.Culture = new CultureInfo("id-ID");
            Control.Properties.Mask.EditMask = "dd MMMM yyyy";
            Control.Properties.Mask.UseMaskAsDisplayFormat = true;
            //dateEdit1.Properties.CharacterCasing = CharacterCasing.Upper;
        }

        private void AddPeriode(object sender, EventArgs e)
        {
            DateTime mulai = dateEdit1.DateTime;
            DateTime selesai = dateEdit2.DateTime;
            string a = comboBoxEdit1.SelectedItem.ToString();
            IadmClient service = new IadmClient();
            periode data = new periode()
            {
                semester = a,
                awalSemester = mulai,
                akhirSemester = selesai
            };
            service.addPeriode(data);
            gridControl1.DataSource = service.viewPeriode().Select(x => new
            {
                semester = string.Format("{0} {1}/{2}",
                                                                               x.semester,
                                                                               x.awalSemester.ToString("yyyy"),
                                                                               x.akhirSemester.ToString("yyyy")),
                awalSemester = x.awalSemester.ToString("dd MMMM yyyy", new CultureInfo("id-ID")),
                akhirSemester = x.akhirSemester.ToString("dd MMMM yyyy", new CultureInfo("id-ID"))
            }).ToList();
            service.Close();
        }
    }
}