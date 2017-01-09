﻿using System;
using client.lab;
using DevExpress.XtraEditors;
using System.Linq;

namespace client
{
    public partial class FormAngkatan : XtraForm
    {
        public FormAngkatan ()
        {
            InitializeComponent( );
        }

        private void FormAngkatan_Load (object sender, EventArgs e)
        {
            var service = new IadmClient( );
            var angkatan = service.GetAngkatan().Select( x => x.TahunAngkatan ).ToList();
            listBoxControl1.Items.AddRange( angkatan.ToArray() );
            service.Close();
        }

        private void simpleButton1_Click (object sender, EventArgs e)
        {
            try
            {
                if(textEdit1.Text.Length != 4 )
                {
                    XtraMessageBox.Show( "Format Tahun Salah !" );
                }
                else
                {
                    var service = new IadmClient( );
                    var data = new angkatan( )
                    {
                        KodeAngkatan = textEdit1.Text.Substring( 2, 2 ),
                        TahunAngkatan = textEdit1.Text.Substring( 0, 4 )
                    };
                    service.TambahAngaktan( data );
                    listBoxControl1.Items.Add( textEdit1.Text );
                }
            }
            catch (Exception)
            {
                XtraMessageBox.Show( "Format Tahun Salah !" );
            }
        }
    }
}