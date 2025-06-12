using FyreWorksPM.Utilities.LaborTemplateSupportClasses;
using FyreWorksPM.ViewModels.Editing;

namespace FyreWorksPM.Pages.Editing;

public partial class EditLaborTemplatePage : ContentPage
{
	public EditLaborTemplatePage(EditLaborTemplateViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    private void OnTemplateSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is TemplateModel selectedTemplate)
        {
            if (BindingContext is EditLaborTemplateViewModel vm)
            {
                vm.LoadTemplateIntoForm(selectedTemplate);
            }

            // Deselect to allow re-selection of same item later
            TemplatesList.SelectedItem = null;
        }
    }

}