using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace Common.Controllers
{
    public class ListenerController
    {
        // Define o intervalo, em segundos, em que a thread ficará em modo sleep
        private static int _iIntervalInSeconds = 10;

        /// <summary>
        /// Inicia o gerenciamento da memória ocupada pelo add-on
        /// </summary>
        public void startListener()
        {
            // início-while :: Loop infinito em que é realizado o flush na memória ocupada pelo add-on 
            while (true)
            {
                // início-if :: Se o valor contido na variável lIntervalInSeconds seja superior a 
                //              zero, realiza o flush na memória ocupada pelo add-on;
                //           :: Senão, realiza uma coleta de sujeira na memória (garbage collection)
                if (_iIntervalInSeconds > 0)
                {
                    Thread.Sleep(_iIntervalInSeconds * 1000);
                    flushMemory();
                }
                else
                {
                    Thread.Sleep(1000);
                    GC.Collect();
                }
                // fim-if                
            }
            // fim-while
        }

        [DllImport("kernel32.dll")]
        // SetProcessWorkingSetSize function: Sets the minimum and maximum working set sizes for the specified process. 
        // Ref.: http://msdn.microsoft.com/en-us/library/windows/desktop/ms686234(v=vs.85).aspx
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

        /// <summary>
        /// Realiza um flush na memória ocupada pelo add-on
        /// </summary>
        public static void flushMemory()
        {
            GC.Collect();

            // Suspends the current thread until the thread that is processing the queue of finalizers has emptied that queue     
            GC.WaitForPendingFinalizers();

            // início-if :: Se a plataforma do sistema operacional for Win32NT, define o 'working set size' do processo atual
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // The working set of the specified process can be emptied by specifying the value 0xffffffff (-1 in decimal) for 
                // both the minimum and maximum working set sizes.
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
            // fim-if
        }
    }
}