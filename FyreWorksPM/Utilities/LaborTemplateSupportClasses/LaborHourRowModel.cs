using CommunityToolkit.Mvvm.ComponentModel;

namespace FyreWorksPM.Utilities.LaborTemplateSupportClasses
{
    public class LaborHourRowModel : ObservableObject
    {
        public string LocationName { get; set; }

        public Action? OnChanged { get; set; }

        private decimal _normalHours;
        public decimal NormalHours
        {
            get => _normalHours;
            set
            {
                if (SetProperty(ref _normalHours, value))
                    OnChanged?.Invoke();
            }
        }


        private decimal _liftHours;
        public decimal LiftHours
        {
            get => _liftHours;
            set => SetProperty(ref _liftHours, value);
        }

        private decimal _panelHours;
        public decimal PanelHours
        {
            get => _panelHours;
            set => SetProperty(ref _panelHours, value);
        }

        private decimal _pipeHours;
        public decimal PipeHours
        {
            get => _pipeHours;
            set => SetProperty(ref _pipeHours, value);
        }
    }


}
