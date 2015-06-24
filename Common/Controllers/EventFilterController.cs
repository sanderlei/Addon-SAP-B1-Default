using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;

namespace Common.Controllers
{
    public class EventFilterController
    {
        private static EventFilters events;

        public static void SetDefaultEvents()
        {
            events = new EventFilters();

            // Sempre adicionar MENU CLICK, se não os menus não abrem
            EventFilter filter = events.Add(BoEventTypes.et_MENU_CLICK);

            // Adiciona Click e ChooseFromList por default
            filter = events.Add(BoEventTypes.et_CLICK);
            filter = events.Add(BoEventTypes.et_CHOOSE_FROM_LIST);

            // Adiciona FormLoad para poder setar o filtro nos system forms
            filter = events.Add(BoEventTypes.et_FORM_LOAD);

            SBOApp.Application.SetFilter(events);
        }

        /// <summary>
        /// Aciona evento no Form
        /// </summary>
        /// <param name="formId">Id do Form - Exemplos: 150 / 2000002001</param>
        /// <param name="eventType">Tipo do evento</param>
        public static void SetFormEvent(string formId, BoEventTypes eventType)
        {
            EventFilter filter;
            // Busca o evento na lista de eventos
            for (int i = 0; i < events.Count; i++)
            {
                filter = events.Item(i);
                if (filter.EventType == eventType)
                {
                    try
                    {
                        filter.AddEx(formId);
                    }
                    catch { }
                    SBOApp.Application.SetFilter(events);
                    return;
                }
            }

            // Se não encontrar o evento, adiciona
            filter = events.Add(eventType);
            filter.AddEx(formId);
            SBOApp.Application.SetFilter(events);
        }
    }
}
