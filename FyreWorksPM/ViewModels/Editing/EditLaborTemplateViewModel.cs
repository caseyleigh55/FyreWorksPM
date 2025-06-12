using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FyreWorksPM.DataAccess.DTO;
using FyreWorksPM.Services.Labor;
using FyreWorksPM.Utilities.LaborTemplateSupportClasses;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FyreWorksPM.ViewModels.Editing
{
   

    public partial class EditLaborTemplateViewModel : ObservableObject
    {
        private readonly ILaborTemplateService _laborTemplateService;

        // === Template Info ===
        private Guid? _editingTemplateId;


        private string _templateName;
        public string TemplateName
        {
            get => _templateName;
            set => SetProperty(ref _templateName, value);
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get => _isDefault;
            set => SetProperty(ref _isDefault, value);
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
                
            
        }


        // === Labor Calculator Fields ===
        public Dictionary<string, LaborCategory> LaborCategories { get; set; } = new();

        // === Labor Rates ===
        public LaborRate JourneymanRate { get; set; } = new();
        // === Journeyman Rate Bindable Proxies ===
        public decimal JourneymanRegularDirectRate
        {
            get => JourneymanRate.RegularDirect;
            set
            {
                if (JourneymanRate.RegularDirect != value)
                {
                    JourneymanRate.RegularDirect = value;
                    OnPropertyChanged(nameof(JourneymanRegularDirectRate));

                    IsDirty = true;
                }
            }
        }

        public decimal JourneymanRegularBilledRate
        {
            get => JourneymanRate.RegularBilled;
            set
            {
                if (JourneymanRate.RegularBilled != value)
                {
                    JourneymanRate.RegularBilled = value;
                    OnPropertyChanged(nameof(JourneymanRegularBilledRate));

                    IsDirty = true;
                }
            }
        }

        public decimal JourneymanOvernightDirectRate
        {
            get => JourneymanRate.OvernightDirect;
            set
            {
                if (JourneymanRate.OvernightDirect != value)
                {
                    JourneymanRate.OvernightDirect = value;
                    OnPropertyChanged(nameof(JourneymanOvernightDirectRate));

                    IsDirty = true;
                }
            }
        }

        public decimal JourneymanOvernightBilledRate
        {
            get => JourneymanRate.OvernightBilled;
            set
            {
                if (JourneymanRate.OvernightBilled != value)
                {
                    JourneymanRate.OvernightBilled = value;
                    OnPropertyChanged(nameof(JourneymanOvernightBilledRate));

                    IsDirty = true;
                }
            }
        }

        public LaborRate ApprenticeRate { get; set; } = new();
        // === Journeyman Rate Bindable Proxies ===
        public decimal ApprenticeRegularDirectRate
        {
            get => ApprenticeRate.RegularDirect;
            set
            {
                if (ApprenticeRate.RegularDirect != value)
                {
                    ApprenticeRate.RegularDirect = value;
                    OnPropertyChanged(nameof(ApprenticeRegularDirectRate));

                    IsDirty = true;
                }
            }
        }

        public decimal ApprenticeRegularBilledRate
        {
            get => ApprenticeRate.RegularBilled;
            set
            {
                if (ApprenticeRate.RegularBilled != value)
                {
                    ApprenticeRate.RegularBilled = value;
                    OnPropertyChanged(nameof(ApprenticeRegularBilledRate));

                    IsDirty = true;
                }
            }
        }

        public decimal ApprenticeOvernightDirectRate
        {
            get => ApprenticeRate.OvernightDirect;
            set
            {
                if (ApprenticeRate.OvernightDirect != value)
                {
                    ApprenticeRate.OvernightDirect = value;
                    OnPropertyChanged(nameof(ApprenticeOvernightDirectRate));

                    IsDirty = true;
                }
            }
        }

        public decimal ApprenticeOvernightBilledRate
        {
            get => ApprenticeRate.OvernightBilled;
            set
            {
                if (ApprenticeRate.OvernightBilled != value)
                {
                    ApprenticeRate.OvernightBilled = value;
                    OnPropertyChanged(nameof(ApprenticeOvernightBilledRate));

                    IsDirty = true;
                }
            }
        }




        // === Template List ===
        public ObservableCollection<TemplateModel> Templates { get; set; } = new();
        public ObservableCollection<LaborHourRowModel> LaborHourMatrix { get; set; } = new();

        public ObservableCollection<TemplateModel> AvailableTemplates { get; set; } = new();

        public EditLaborTemplateViewModel(ILaborTemplateService laborTemplateService)
        {
            _laborTemplateService = laborTemplateService;

            // Leave this if you're still mocking for now, otherwise just:
            LoadTemplatesFromApiAsync();
        }

        public async void LoadTemplatesFromApiAsync()
        {
            var templates = await _laborTemplateService.GetAllTemplatesAsync();

            AvailableTemplates.Clear();

            foreach (var dto in templates)
            {
                AvailableTemplates.Add(new TemplateModel
                {
                    Id = dto.Id,
                    Name = dto.TemplateName,
                    IsDefault = dto.IsDefault,
                    Journeyman = MapRates(dto.LaborRates, "Journeyman"),
                    Apprentice = MapRates(dto.LaborRates, "Apprentice"),
                    Categories = dto.LocationHours.ToDictionary(
                        h => h.LocationName,
                        h => new LaborCategory
                        {
                            Normal = h.Normal,
                            Lift = h.Lift,
                            Panel = h.Panel,
                            Pipe = h.Pipe
                        })
                });
            }
        }

        private LaborRate MapRates(List<LaborRateDto> rates, string role)
        {
            var match = rates.FirstOrDefault(r => r.Role == role);
            return match != null
                ? new LaborRate
                {
                    RegularDirect = match.RegularDirectRate,
                    RegularBilled = match.RegularBilledRate,
                    OvernightDirect = match.OvernightDirectRate,
                    OvernightBilled = match.OvernightBilledRate
                }
                : new LaborRate();
        }

        [RelayCommand]
        public async Task SaveTemplateAsync()
        {
            var dto = new CreateLaborTemplateDto
            {
                TemplateName = TemplateName,
                IsDefault = IsDefault,
                LaborRates = new List<LaborRateDto>
        {
            new()
            {
                Role = "Journeyman",
                RegularDirectRate = JourneymanRate.RegularDirect,
                RegularBilledRate = JourneymanRate.RegularBilled,
                OvernightDirectRate = JourneymanRate.OvernightDirect,
                OvernightBilledRate = JourneymanRate.OvernightBilled
            },
            new()
            {
                Role = "Apprentice",
                RegularDirectRate = ApprenticeRate.RegularDirect,
                RegularBilledRate = ApprenticeRate.RegularBilled,
                OvernightDirectRate = ApprenticeRate.OvernightDirect,
                OvernightBilledRate = ApprenticeRate.OvernightBilled
            }
        },
                LocationHours = LaborHourMatrix.Select(row => new LocationHourDto
                {
                    LocationName = row.LocationName,
                    Normal = row.NormalHours,
                    Lift = row.LiftHours,
                    Panel = row.PanelHours,
                    Pipe = row.PipeHours
                }).ToList()
            };

            try
            {
                if (_editingTemplateId.HasValue)
                {
                    Debug.WriteLine($"Trying to update template ID: {_editingTemplateId.Value}");

                    var updated = await _laborTemplateService.UpdateTemplateAsync(_editingTemplateId.Value, dto);
                    if (!updated)
                    {
                        // Optional: show error message
                        return;
                    }
                }
                else
                {
                    var result = await _laborTemplateService.CreateTemplateAsync(dto);
                    _editingTemplateId = result.Id; // Optional: track it after creation
                }

                IsDirty = false;

                System.Diagnostics.Debug.WriteLine($"Returning to CreateBidPage with ID: {_editingTemplateId}");


                // Optional: navigate or refresh logic
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                {
                    { "SelectedTemplateId", _editingTemplateId.ToString() },
                    { "FromEdit", "true" }
                });

            }
            catch (Exception ex)
            {
                // Handle/log error if needed
                   Console.WriteLine($"Template save failed: {ex.Message}");
            }
        }


        public void LoadTemplateIntoForm(TemplateModel template)
        {
            _editingTemplateId = template.Id;
            TemplateName = template.Name;
            IsDefault = template.IsDefault;

            // Update existing instance properties so bindings remain valid
            JourneymanRate.RegularDirect = template.Journeyman.RegularDirect;
            JourneymanRate.RegularBilled = template.Journeyman.RegularBilled;
            JourneymanRate.OvernightDirect = template.Journeyman.OvernightDirect;
            JourneymanRate.OvernightBilled = template.Journeyman.OvernightBilled;

            ApprenticeRate.RegularDirect = template.Apprentice.RegularDirect;
            ApprenticeRate.RegularBilled = template.Apprentice.RegularBilled;
            ApprenticeRate.OvernightDirect = template.Apprentice.OvernightDirect;
            ApprenticeRate.OvernightBilled = template.Apprentice.OvernightBilled;

            // Trigger UI refresh manually
            OnPropertyChanged(nameof(JourneymanRegularDirectRate));
            OnPropertyChanged(nameof(JourneymanRegularBilledRate));
            OnPropertyChanged(nameof(JourneymanOvernightDirectRate));
            OnPropertyChanged(nameof(JourneymanOvernightBilledRate));

            OnPropertyChanged(nameof(ApprenticeRegularDirectRate));
            OnPropertyChanged(nameof(ApprenticeRegularBilledRate));
            OnPropertyChanged(nameof(ApprenticeOvernightDirectRate));
            OnPropertyChanged(nameof(ApprenticeOvernightBilledRate));




            LaborHourMatrix.Clear();
            foreach (var kvp in template.Categories)
            {
                LaborHourMatrix.Add(new LaborHourRowModel
                {
                    LocationName = kvp.Key,
                    NormalHours = kvp.Value.Normal,
                    LiftHours = kvp.Value.Lift,
                    PanelHours = kvp.Value.Panel,
                    PipeHours = kvp.Value.Pipe,
                    OnChanged = () => IsDirty = true
                });
            }
        }


    }

}
