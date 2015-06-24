using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace Common.Controllers
{
    public class ErrorController
    {
        /// <summary>
        /// Seta mensagem de erro no SAP e finaliza transação se existir
        /// </summary>
        /// <param name="ex">Exceção ocorrida</param>
        /// <param name="commitTransactionYN">Commit na transação aberta</param>
        public static void SetErrorMessage(Exception ex, Boolean commitTransactionYN)
        {
            if (ex.InnerException != null)
                 SBOApp.Application.SetStatusBarMessage(ex.InnerException.Message, BoMessageTime.bmt_Medium, true);
            else
                SBOApp.Application.SetStatusBarMessage(ex.Message, BoMessageTime.bmt_Medium, true);

            if (SBOApp.Company.Connected)
            {
                if (SBOApp.Company.InTransaction)
                {
                    if (commitTransactionYN)
                    {
                        SBOApp.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    }
                    else
                    {
                        SBOApp.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                    }
                }
            }
        }

        /// <summary>
        /// Seta mensagem de erro no SAP de acordo com o último erro ocorrido 
        /// </summary>
        public static void SetLastErrorMessage()
        {
            int errCode;
            string errMsg;

            SBOApp.Company.GetLastError(out errCode, out errMsg);
            SBOApp.Application.SetStatusBarMessage(errCode + " - " + errMsg, BoMessageTime.bmt_Medium, true);
        }

        public static string GetLastErrorDescription()
        {
            return SBOApp.Company.GetLastErrorDescription();
        }

        public static int GetLastErrorCode()
        {
            return SBOApp.Company.GetLastErrorCode();
        }
    }
}
