using System;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace client
{
    internal static class Program
    {
        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool SetForegroundWindow (IntPtr hWnd);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            

            bool createdNew = true;
            using ( Mutex mutex = new Mutex( true, "MyApplicationName", out createdNew ) )
            {
                if ( createdNew )
                {
                    Application.EnableVisualStyles( );
                    Application.SetCompatibleTextRenderingDefault( false );

                    BonusSkins.Register( );
                    SkinManager.EnableFormSkins( );
                    UserLookAndFeel.Default.SetSkinStyle( "DevExpress Style" );
                    Application.Run( new FrmMain( ) );
                }
                else
                {
                    Process current = Process.GetCurrentProcess( );
                    foreach ( Process process in Process.GetProcessesByName( current.ProcessName ) )
                    {
                        if ( process.Id != current.Id )
                        {
                            SetForegroundWindow( process.MainWindowHandle );
                            break;
                        }
                    }
                }
            }
        }
    }
}