using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using Common.Controllers;

namespace Main
{
    static class Program
    {
        [STAThread]
        static void Main(string[] Args)
        {
            // Se não foi passado nenhum argumento à aplicação, esta é finalizada
            if (Args.Length.Equals(0))
            {
                MessageBox.Show("O aplicativo deve ser iniciado dentro do SAP Business One Client.", "Add-On Default",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Application.Exit();   

                return;
            }
            //

            // Nova instância do controlador
            SBOApp sboApp = new SBOApp(Args[0], Application.StartupPath + "\\View.dll");
            sboApp.InitializeApplication();

            CreateMenu();

            EventFilterController.SetDefaultEvents();

            // Gera nova instância do AppListener para realizar o gerenciamento de memória do aplicativo 
            // O gerenciamento é feito em background através de uma nova thread                          
            ListenerController oListener = new ListenerController();
            System.Threading.Thread oThread = new System.Threading.Thread(new System.Threading.ThreadStart(oListener.startListener));
            oThread.IsBackground = true;
            oThread.Start();

            Application.Run();
        }

        public static void CreateMenu()
        {
            MenuController.LoadFromXML(Application.StartupPath + "\\Menu.xml");
        }
    }
}
