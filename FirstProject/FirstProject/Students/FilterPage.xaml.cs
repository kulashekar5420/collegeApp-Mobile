using System;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPage : PopupPage
    {
        private static string selectedDepartment;
        public FilterPage()
        {
            InitializeComponent();
            departmentPicker.SelectedItem = selectedDepartment;

        }

        private async void departmentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {         
            selectedDepartment = departmentPicker.SelectedItem?.ToString();    
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading($"Loading {selectedDepartment} Students");
            await Task.Delay(1000);

            if (selectedDepartment == "All")
            {
                App.StudentViewModel.StudentDepartment = "All";
                await App.StudentViewModel.LoadAllDepartmentsStudents();
            }
            else
            {
                App.StudentViewModel.StudentDepartment = selectedDepartment;
                await App.StudentViewModel.LoadStudentsByDepartment(selectedDepartment);
            }

            await PopupNavigation.Instance.PopAsync();

            UserDialogs.Instance.HideLoading();
        }
    }  
}
