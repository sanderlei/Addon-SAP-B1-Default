using System;

namespace Common.Forms
{
    public class BaseFormParent : Common.Interfaces.IForm
    {
        public SAPbouiCOM.MenuEvent menuEvent;

        public virtual Boolean MenuEvent()
        {
            throw new NotImplementedException();
        }


        public virtual Boolean AppEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Boolean FormDataEvent()
        {
            throw new NotImplementedException();
        }

        public virtual void Freeze(Boolean freeze)
        {
            throw new NotImplementedException();
        }

        public virtual Boolean ItemEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Boolean PrintEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Boolean ProgressBarEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Boolean ReportDataEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Boolean RightClickEvent()
        {
            throw new NotImplementedException();
        }

        public virtual Object Show()
        {
            throw new NotImplementedException();
        }

        public virtual Object Show(String[] args)
        {
            throw new NotImplementedException();
        }

        public virtual Boolean StatusBarEvent()
        {
            throw new NotImplementedException();
        }
    }
}
