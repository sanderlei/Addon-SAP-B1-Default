using System;
using SAPbouiCOM;
using Common.Controllers;

namespace Common.Controllers
{
    public class EventController
    {
        #region Variables
        public static Int32 _iItemEventRow = -1;
        public static String _sItemEventCol = String.Empty;
        public static String _sFormDataEventObjectKey = String.Empty;
        #endregion

        #region Events
        public static void AppEvent(BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    System.Environment.Exit(0);
                    break;
            }
        }

        public static void FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
            // The event provides the unique ID (BusinessObjectInfo.ObjectKey) of the modified business object.
            _sFormDataEventObjectKey = BusinessObjectInfo.ObjectKey;

            // Executa o método FormDataEvent do formulário em que ocorreu o evento
            BubbleEvent = ExecuteEvent<BusinessObjectInfo>(BusinessObjectInfo.FormTypeEx, BusinessObjectInfo, "FormDataEvent", true);
        }

        public static void ItemEvent(String FormUID, ref ItemEvent itemEvent, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
            if (itemEvent.EventType == BoEventTypes.et_FORM_UNLOAD && !itemEvent.BeforeAction)
            {
                return;
            }

            // Obtém a linha e a coluna em que o evento está ocorrendo, caso se trate de um grid ou matrix
            _iItemEventRow = itemEvent.Row;
            _sItemEventCol = itemEvent.ColUID;

            // Executa o método ItemEvent do formulário em que ocorreu o evento
            BubbleEvent = ExecuteEvent<ItemEvent>(itemEvent.FormTypeEx, itemEvent, "ItemEvent", true);
        }

        public static void MenuEvent(ref MenuEvent menuEvent, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
            ExecuteEvent<MenuEvent>(menuEvent.MenuUID, menuEvent, "MenuEvent", false);
        }

        public static void RightClickEvent(ref ContextMenuInfo contextMenuInfo, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
            SBOApp oApplication = new SBOApp();
            // Executa o método RightClickEvent do formulário em que ocorreu o evento
            ExecuteEvent<ContextMenuInfo>(SBOApp.Application.Forms.Item(contextMenuInfo.FormUID.ToString()).TypeEx,
                                                        contextMenuInfo,
                                                        "RightClickEvent",
                                                        false);
        }
        #endregion

        #region ExecuteEvent
        /// <summary>
        /// Executa evento
        /// </summary>
        /// <typeparam name="T">Tipo do evento</typeparam>
        /// <param name="formID">ID do form</param>
        /// <param name="eventInfo">Objeto do evento</param>
        /// <param name="eventName">Nome do evento a ser executado</param>
        /// <param name="finishTransactionYN">Finaliza transação em caso de erro</param>
        /// <returns>Evento foi executado?</returns>
        public static Boolean ExecuteEvent<T>(String formID, T eventInfo, String eventName, Boolean finishTransactionYN)
        {
            Interfaces.IForm oForm = null;

            try
            {
                Type tType = FormController.GetFormType(formID);
                // Verifica se a variável tType contém algum valor, caso não, o método é interrompido
                if (tType == null) return true;

                // Instancia o objeto referente ao menu que foi selecionado
                oForm = FormController.CreateForm<T>(tType, ref eventInfo);
                // Verifica se a variável oForm contém algum valor, caso não, o método é interrompido
				//if (oForm == null) return false;
				if (oForm == null) return true;

                return (Boolean)oForm.GetType().GetMethod(eventName).Invoke(oForm, null);
            }
            catch (Exception ex)
            {
                ErrorController.SetErrorMessage(ex, finishTransactionYN);

                //if (oForm != null)
                //{
                //    oForm.Freeze(false);
                //}

                return false;
            }
        }
        #endregion

        #region NonImplementedEvents
        #region PrintEvent
        public static void PrintEvent(ref PrintEventInfo eventInfo, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
        }
        #endregion

        #region ProgressBarEvent
        public static void ProgressBarEvent(ref ProgressBarEvent progressBarEvent, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
        }
        #endregion

        #region ReportDataEvent
        public static void ReportDataEvent(ref ReportDataInfo reportDataInfo, out Boolean BubbleEvent)
        {
            BubbleEvent = true;
        }
        #endregion

        #region StatusBarEvent
        public static void StatusBarEvent(String Text, BoStatusBarMessageType MessageType)
        {
        }
        #endregion
        #endregion
    }
}