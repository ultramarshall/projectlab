using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using client.lab;

namespace client
{
    public partial class FrmTambahJadwalPraktikan : DevExpress.XtraEditors.XtraForm
    {
        public FrmTambahJadwalPraktikan (string nrp, string nama)
        {
            InitializeComponent( );
            simpleLabelItem1.Text = nrp;
            simpleLabelItem2.Text = nama;
        }
        
        private void FrmTambahJadwalPraktikan_Load (object sender, EventArgs e)
        {
            var service = new IadmClient( );

            var periode = service.viewPeriode().FirstOrDefault(
                x => 
                DateTime.Now >= x.awalSemester && 
                DateTime.Now <= x.akhirSemester);

            var data = new jadwal_umum( ) { id_periode = periode.id_periode };
            gridControl1.DataSource = service.ViewJadwalUmum( data ).Select(x=> new {
                HARI = x.hari,
                SHIFT = x.fk_jadwalUmum_Shift.id_shift,
                WAKTU =
                    string.Format( "{0:HH:mm} - {1:HH:mm}",
                    x.fk_jadwalUmum_Shift.mulai,
                    x.fk_jadwalUmum_Shift.selesai ),
                PRAKTIKUM = x.fk_jadwalUmum_matakuliah.mata_kuliah,
                KELAS = x.fk_jadwalUmum_kelas.Kelas
            } );

            // add checkbox and enable multi select rows
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect; ;
            // disable focused row and move column
            for ( var i = 0; i < gridView1.Columns.Count; i++ )
            {
                gridView1.Columns[i].OptionsColumn.AllowFocus = false;
                gridView1.Columns[i].OptionsColumn.AllowMove = false;
            }
        }

        private void TambahJadwalPraktikan (object sender, EventArgs e)
        {
            var service = new IadmClient( );
            var jadwal = new List<jadwalPraktikan>( );
            var values = gridView1.GetSelectedRows( );
            var periode = service.viewPeriode( ).FirstOrDefault(
                x =>
                DateTime.Now >= x.awalSemester &&
                DateTime.Now <= x.akhirSemester );
            var data = new jadwal_umum( ) { id_periode = periode.id_periode };

            var listjadwal = new List<jadwalPraktikan>( );
            for ( var i = 0; i < values.Count( ); i++ )
            {
                var shift = gridView1.GetRowCellValue( values[i], gridView1.Columns[1] ).ToString( );
                var hari = gridView1.GetRowCellValue( values[i], gridView1.Columns[0] ).ToString( );
                var matkul = gridView1.GetRowCellValue( values[i], gridView1.Columns[3] ).ToString( );
                var id = service.ViewJadwalUmum( data ).FirstOrDefault( x =>
                (x.fk_jadwalUmum_Shift.id_shift == shift && x.hari == hari) &&
                 x.fk_jadwalUmum_matakuliah.mata_kuliah == matkul);


                var jadwalpraktikan = new jadwalPraktikan( )
                {
                    nrp = simpleLabelItem1.Text,
                    id_jadwal_umum = new jadwal_umum( ) { id_jadwal_umum = id.id_jadwal_umum }
                };
                listjadwal.Add( jadwalpraktikan );
            }
            try { service.AddJadwalPraktikan( listjadwal.ToArray( ) ); } catch ( Exception ) { XtraMessageBox.Show( "Tidak ada jadwal" ); }
            Close( );
        }

        private void CloseTambahJadwal (object sender, EventArgs e)
        {
            Close( );
        }
    }
}