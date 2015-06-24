using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Forms;
using SAPbouiCOM;
using Common;

namespace View
{
    class f2000001001 : BaseForm
    {

        #region Constructors
        private static Form form;

        public f2000001001()
        {
            FormCount++;
        }

        public f2000001001(ItemEvent itemEvent)
        {
            this.ItemEventInfo = itemEvent;            
            form = SBOApp.Application.Forms.GetFormByTypeAndCount(itemEvent.FormType, itemEvent.FormTypeCount);
        }

        public f2000001001(BusinessObjectInfo businessObjectInfo)
        {
            this.BusinessObjectInfo = businessObjectInfo;
        }

        public f2000001001(ContextMenuInfo contextMenuInfo)
        {
            this.ContextMenuInfo = contextMenuInfo;
        }
        #endregion
    }
    
}



