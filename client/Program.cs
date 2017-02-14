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
                    Application.AddMessageFilter(new AltF4Filter());
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
    public class AltF4Filter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_SYSKEYDOWN = 0x0104;
            if (m.Msg == WM_SYSKEYDOWN)
            {
                bool alt = ((int)m.LParam & 0x20000000) != 0;
                if (alt && (m.WParam == new IntPtr((int)Keys.F4)))
                    return true; // eat it!                
            }
            return false;
        }
    }
}