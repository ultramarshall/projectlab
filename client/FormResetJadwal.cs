using System;
using DevExpress.XtraEditors;
using client.lab;
using System.Linq;

namespace client
{
    public partial class FormResetJadwal : XtraForm
    {
        private string nrppraktikan;
        public FormResetJadwal (string nrp)
        {
            InitializeComponent( );
            nrppraktikan = nrp;
        }

        private void simpleButton1_Click (object sender, EventArgs e)
        {
            Close( );
        }

        private void simpleButton2_Click (object sender, EventArgs e)
        {
            var service = new IadmClient( );
            var periode = service.viewPeriode( ).FirstOrDefault( x => DateTime.Now >= x.awalSemester &&
                                                                          DateTime.Now <= x.akhirSemester );
            var data = new jadwalPraktikan( )
            {
                nrp = nrppraktikan,
                id_jadwal_umum = new jadwal_umum( )
                {
                    id_periode = periode.id_periode
                }
            };
            service.DeleteJadwalPraktikan( data );
            Close( );
        }
    }
}