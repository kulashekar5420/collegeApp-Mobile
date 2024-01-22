using Acr.UserDialogs;
using FirstProject;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPage : PopupPage
    {

        private string selectedDepartment;

        public FilterPage()
        {
            InitializeComponent();     
        }

        private void departmentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDepartment = departmentPicker.SelectedItem.ToString();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading($"Loading {selectedDepartment} Students");
            await Task.Delay(1000);

            if (selectedDepartment == "All Department")
            {
                App.StudentViewModel.StudentDepartment = "All Department";
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

