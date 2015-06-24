using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Forms;

namespace View
{
    class fM1001 : BaseFormParent
    {
        public fM1001(SAPbouiCOM.MenuEvent menuEvent)
        {
            this.menuEvent = menuEvent;
        }

        public override Boolean MenuEvent()
        {
            if (!menuEvent.BeforeAction)
            {
                f2000001001 form = new f2000001001();
                form.FormID = 1001;
                form.SrfFolder = "srfFiles";                
                form.Show();
            }

            return true;
        }
    }
}


