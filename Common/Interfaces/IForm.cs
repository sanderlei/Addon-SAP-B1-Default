using System;

namespace Common.Interfaces
{
    public interface IForm
    {
        Boolean AppEvent();

        Boolean FormDataEvent();

        void Freeze(Boolean freeze);

        Boolean ItemEvent();

        Boolean MenuEvent();

        Boolean PrintEvent();

        Boolean ProgressBarEvent();

        Boolean ReportDataEvent();

        Boolean RightClickEvent();

        Object Show();

        Object Show(String[] args);

        Boolean StatusBarEvent();
    }
}