using System;
using System.Windows.Forms;

namespace EmailSenderApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm()); // MainForm sınıfını başlatır
        }
    }
}
