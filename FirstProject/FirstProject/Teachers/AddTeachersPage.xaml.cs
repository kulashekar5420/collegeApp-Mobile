using Acr.UserDialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Teachers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTeachersPage : ContentPage
    {
        private static int orderNumber = 1;
        private string generatedtRollId;
        private string generatedHod;
        private ObservableCollection<HodsModel> availableDephods;

        public AddTeachersPage()
        {
            InitializeComponent();
            BindingContext = new TeachersViewModel();        
            tDepartmentPicker.SelectedIndexChanged += TDepartmentPicker_SelectedIndexChanged;         
            availableDephods = new ObservableCollection<HodsModel>();                  
            //load availablehods
            LoadAvailableHods();
        }

       
        private async void LoadAvailableHods()
        {
            var teachersViewModel = (TeachersViewModel)BindingContext;
            //get the available hod from the database in viewmodel
            await teachersViewModel.LoadAvailableHods();
            availableDephods = teachersViewModel.AvailableHods;

        }

        private async void TDepartmentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string department = ((TeachersViewModel)BindingContext).teacherDepartment;
            var hod = availableDephods.FirstOrDefault(h => h.HodDepartment == department);
            if (!string.IsNullOrWhiteSpace(department))
            {
                orderNumber = 1;
                generatedtRollId = await GeneratetRollIdAsync(department);
                // Generate HOD based on the selected department
                generatedHod = await GenerateHodAsync(department);
                trollIdLabel.Text = $"{generatedtRollId}";
                HodLabel.Text = $"{generatedHod}";

                if (!string.IsNullOrEmpty(generatedHod))
                {
                    HodLabel.Text = $"{hod.HodName} - {hod.HodId}";
                }
                else
                {
                    HodLabel.Text = "No HOD Found";
                }
            }
        }

        //Get the avaialable hod 
        private async Task<string> GenerateHodAsync(string department)
        {
            var hod = availableDephods.FirstOrDefault(h => h.HodDepartment == department);
            return await Task.FromResult(hod != null ? hod.HodName : "");
        }

        //generate the teacherid based on the department and ordernumber - 101CSE1 the 101 wil be the keynumber for teacher and
        private async Task<string> GeneratetRollIdAsync(string department)
        {
            string tRollId;
            do
            {
                tRollId = $"101{department}{orderNumber}";
                orderNumber++;
            } while (!await App.TeacherViewModel.IsRollIdAvailableAsync(tRollId));
            return tRollId;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            string teacherName = ((TeachersViewModel)BindingContext).teacherName;
            string teacerDepartment = ((TeachersViewModel)BindingContext).teacherDepartment;
            string teacherGender = ((TeachersViewModel)BindingContext).Gender;
            string teacherYear = ((TeachersViewModel)BindingContext).teacherYear;
          
            if (string.IsNullOrWhiteSpace(teacherName) || teacherName.Length <= 4)
            {
                await DisplayAlert("Error Name Alert", "Name must be at least 5 characters.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(teacerDepartment))
            {
                await DisplayAlert("Error Department", "Select teacher department", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(teacherGender))
            {
                await DisplayAlert("Error Gender", "Select your gender", "OK");
                return;
            }
  
            TeachersModel newTeacher = new TeachersModel
            {
                TeacherName = teacherName,
                TeacherDepartment = teacerDepartment,
                Gender = teacherGender,
                TeacherYear = teacherYear, 
                TeacherId = generatedtRollId,
                HodName = generatedHod
            };
            if (!string.IsNullOrWhiteSpace(teacherYear))
            {
                // Check if already class teacher exists or not
                bool isClassTeacherExists = App.TeacherViewModel.IsClassTeacherExists(teacherYear, teacerDepartment);
                if (isClassTeacherExists)
                {
                    await DisplayAlert("Class Teacher Exists", $"{newTeacher.TeacherName}'s A class teacher already exists for the selected department and year.", "OK");
                    return;
                }
            }
            UserDialogs.Instance.ShowLoading("Loading");
            // Add teacher to the ViewModel
            await App.TeacherViewModel.AddTeachersAsync(newTeacher);
            await Task.Delay(1000);
            UserDialogs.Instance.HideLoading();
            await Navigation.PopAsync();
        }
    }
}

