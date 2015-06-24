using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Common.Util
{
    public class DialogUtil
    {
        private string resultString;

        /// <summary>
        /// Dialog para selecionar pasta
        /// </summary>
        /// <returns>Pasta Selecionada</returns>
        public string FolderBrowserDialog()
        {
            Thread thread = new Thread(ShowFolderBrowserDialog);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return resultString;
        }

        /// <summary>
        /// Dialog para selecionar arquivo
        /// </summary>
        /// <returns>Arquivo Selecionado</returns>
        public string OpenFileDialog()
        {
            Thread thread = new Thread(ShowOpenFileDialog);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return resultString;
        }

        private void ShowFolderBrowserDialog()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(WindowWrapper.GetForegroundWindowWrapper()) == DialogResult.OK)
            {
                resultString = fbd.SelectedPath;
            }
            System.Threading.Thread.CurrentThread.Abort();
        }

        private void ShowOpenFileDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog(WindowWrapper.GetForegroundWindowWrapper()) == DialogResult.OK)
            {
                resultString = ofd.FileName;
            }
            System.Threading.Thread.CurrentThread.Abort();
        }
    }
}
