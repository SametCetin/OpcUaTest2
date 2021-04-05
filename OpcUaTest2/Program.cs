using Opc.Ua.Client.Controls;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpcUaTest2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
            ApplicationInstance application = new ApplicationInstance();
            application.ApplicationType = Opc.Ua.ApplicationType.Client;
            application.ConfigSectionName = "UA";

            try
            {
                application.LoadApplicationConfiguration(false).Wait();
                application.CheckApplicationInstanceCertificate(false, 0).Wait();
                Application.Run(new Form1(application.ApplicationConfiguration));
            }
            catch
            {
                return;
            }
        }
    }
}
