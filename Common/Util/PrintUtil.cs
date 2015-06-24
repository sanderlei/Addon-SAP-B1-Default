using System;
using Microsoft.Office.Interop.Word;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace Common.Util
{
    public class PrintUtil
    {
        public string PrinterName { get; set; }

        public string LayoutEtiqueta { get; set; }

        public int CopiesQuantity { get; set; }

        public SAPbobsCOM.Company DiCompany { get; set; }

        public PrintUtil()
        {
            CopiesQuantity = 1;
        }

        /// <summary>
        /// Imprime arquivo no formato Rich Text Format
        /// </summary>
        /// <param name="fileName">Caminho completo do arquivo</param>
                        
        public void PrintRTF(List<KeyValuePair<int, string>> EtiquetasRTF)
        {

            ApplicationClass ac = new ApplicationClass();
            _Application app = ac.Application;

            // I'm setting all of the alerts to be off            
            app.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            // Open the document to print...
            object filename = "";
            object missingValue = Type.Missing;

            _Document document = null;

            int nrEtiq;

            int i = 0;
            foreach (var etiqueta in EtiquetasRTF)
            {
                filename = etiqueta.Value;
                nrEtiq = etiqueta.Key;

                if (i == 0)
                {
                    // Using OpenOld so as to be compatible with other versions of Word
                    document = (_Document)app.Documents.OpenOld(ref filename,
                                                                ref missingValue, ref missingValue, ref missingValue, ref missingValue, ref missingValue,
                                                                ref missingValue, ref missingValue, ref missingValue, ref missingValue);
                }
                else
                {
                    app.Selection.EndKey(6);                             /* Posiciona cursor no final do arquivo */
                    app.Selection.InsertBreak(7);                        /* Quebra pagina antes de inserir arquivo */
                    app.Selection.InsertFile(filename.ToString());

                    File.Delete(filename.ToString());
                }                               

                i++;

            }

            // Set the active printer
            app.ActivePrinter = PrinterName;

            object myTrue = true;
            object myFalse = false;
            object nrCopias = CopiesQuantity;

            // Using PrintOutOld to be version independent
            app.ActiveDocument.PrintOutOld(ref myFalse,// Print in background
                                           ref myFalse, ref missingValue, ref missingValue, ref missingValue, ref missingValue,
                                           ref missingValue, ref nrCopias, ref missingValue, ref missingValue, ref myFalse,
                                           ref missingValue, ref missingValue, ref missingValue);

            object saveOption = Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges;

            // Make sure all of the documents are gone from the queue
            while (app.BackgroundPrintingStatus > 0)
            {
                System.Threading.Thread.Sleep(250);
            }

            app.NormalTemplate.Saved = true;
            document.Close(ref saveOption, ref missingValue, ref missingValue);

            app.Quit(ref saveOption, ref missingValue, ref missingValue);

        }

        /// <summary>
        /// Abre o dialog para selecionar impressora
        /// </summary>
        /// <returns>Nome da impressora selecionada</returns>
        [STAThread]
        public static string GetPrinter()
        {
            string printerSelected = "";
            PrintDialog printDialog1 = new PrintDialog();
            //printDialog1.Document = printDocument1;
            DialogResult result = printDialog1.ShowDialog(WindowWrapper.GetForegroundWindowWrapper());
            if (result == DialogResult.OK)
            {
                printerSelected = printDialog1.PrinterSettings.PrinterName;
            }

            return printerSelected;
        }
    }
}
