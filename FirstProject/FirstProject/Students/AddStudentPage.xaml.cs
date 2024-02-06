using Acr.UserDialogs;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddStudentPage : ContentPage
    {       
        private static int orderNumber = 1;
        private string generatedRollId;
        private string MappingHod;
        private string MappingTeacher;
        private List<StateModel> localStates;
        private ObservableCollection<TeachersModel> availableDepTeachers;
        private ObservableCollection<HodsModel> availableHods;
        private ObservableCollection<TeachersModel> availableDepteachers;
        private SchoolDatabase schoolDatabase;
        public AddStudentPage()
        {
            InitializeComponent();
            BindingContext = new StudentsViewModel();

            departmentPicker.SelectedIndexChanged += DepartmentPicker_SelectedIndexChanged;
            studentYearPicker.SelectedIndexChanged += StudentYearPicker_SelectedIndexChanged;
            statePicker.SelectedIndexChanged += StatePicker_SelectedIndexChanged;

            availableHods = new ObservableCollection<HodsModel>();
            availableDepTeachers = new ObservableCollection<TeachersModel>();

            // Initialize SchoolDatabase
            schoolDatabase = App.DatabaseforSchool;
            LoadAvailableTeachers();
            LoadAvailableHods();
           
        }

        //Load the localstates from the database and load it on statepicker
        private async void LoadLocalStates()
        {
            localStates = await schoolDatabase.GetAllStatesAsync();

            if (localStates.Any())
            {               
                statePicker.ItemsSource = localStates.Select(state => state.Name).ToList();

                // Open the picker without selecting any item
                statePicker.Focus();
            }
            else
            {
                statePicker.IsVisible = false;
            }         
            
        }

        private async Task SaveLocalStates(List<StateModel> states)
        {
            await schoolDatabase.SaveStatesAsync(states);
        }

        private  void StatePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = statePicker.SelectedItem as string;
            ((StudentsViewModel)BindingContext).StudentState = selectedState;

        }

        //LoadStates from the API url and sava into the Local SQLite database
        //Tabel Name StateModels

        private async void LoadStatesDataAsync()
        {
            UserDialogs.Instance.ShowLoading("Loading States");
            using (HttpClient client = new HttpClient())
            {
                string url = "https://gist.githubusercontent.com/shubhamjain/35ed77154f577295707a/raw/7bc2a915cff003fb1f8ff49c6890576eee4f2f10/IndianStates.json";
                string json = await client.GetStringAsync(url);
                Dictionary<string, string> statesData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                List<StateModel> stateModels = statesData.Select(kv => new StateModel { Abbreviation = kv.Key, Name = kv.Value }).ToList();
                await SaveLocalStates(stateModels);
                statePicker.ItemsSource = stateModels.Select(state => state.Name).ToList();

                // Open the picker without selecting any item
                statePicker.Focus();
            }

            UserDialogs.Instance.HideLoading();
        }


        private async void LoadAvailableHods()
        {
            var studentsMapHod = (StudentsViewModel)BindingContext;
            await studentsMapHod.LoadAvailableHods();
            availableHods = studentsMapHod.AvailableHods;
        }
        private async void DepartmentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string stuDepartment = ((StudentsViewModel)BindingContext).Department;
            var hod = availableHods.FirstOrDefault(h => h.HodDepartment == stuDepartment);

            if (!string.IsNullOrWhiteSpace(stuDepartment))
            {
                orderNumber = 1;
                generatedRollId = await GenerateRollIdAsync(stuDepartment);
                MappingHod = await GenerateHodAsync(stuDepartment);

                rollIdLabel.Text = generatedRollId;
                HodLabel.Text = MappingHod;

                if (!string.IsNullOrEmpty(MappingHod))
                {
                    HodLabel.Text = $"{hod.HodName} - {hod.HodId}";
                }
                else
                {
                    HodLabel.Text = "No HOD Found";
                }

                LoadAvailableTeachers();
            }
        }
        private async void LoadAvailableTeachers()
        {
            var studentsViewModel = (StudentsViewModel)BindingContext;
            await studentsViewModel.LoadAvailableTeachersAsync();

            string selectedDepartment = studentsViewModel.Department;

            availableDepTeachers = new ObservableCollection<TeachersModel>(
                studentsViewModel.AvailableTeachers.Where(t => t.TeacherDepartment == selectedDepartment)
            );
        }
        private async void StudentYearPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string studentYear = ((StudentsViewModel)BindingContext).StudentYear;

            if (!string.IsNullOrWhiteSpace(studentYear))
            {
                TeachersModel teacher = await MapStudentTeacherAsync(studentYear);
                MappingTeacher = teacher?.TeacherId ?? "";
                TeacherLabel.Text = teacher?.TeacherName ?? "";
                LoadAvailableDepTeachers();
            }
        }

        private async void LoadAvailableDepTeachers()
        {
            var availableDepTeacher = (StudentsViewModel)BindingContext;
            await availableDepTeacher.LoadAvailableDepTeachers();

            string selectedStudentYear = availableDepTeacher.StudentYear;

            availableDepteachers = new ObservableCollection<TeachersModel>(
                await App.StudentViewModel.GetTeachersByYearAndDepartmentAsync(selectedStudentYear, availableDepTeacher.Department)
            );

            var generatedTeacher = await MapStudentTeacherAsync(selectedStudentYear);

            var selectedTeacher = availableDepteachers.FirstOrDefault(t => t.TeacherYear == selectedStudentYear);

            if (selectedTeacher != null)
            {
                TeacherLabel.Text = $"{selectedTeacher.TeacherName} - {selectedTeacher.TeacherId}";
            }            
            else
            {
                TeacherLabel.Text = "No Teacher Found";
            }
        }

        private async Task<TeachersModel> MapStudentTeacherAsync(string studentYear)
        {
            var teacher = availableDepTeachers.FirstOrDefault(t => t.TeacherYear == studentYear);
            return await Task.FromResult(teacher);
        }

        private async Task<string> GenerateHodAsync(string department)
        {
            var hod = availableHods.FirstOrDefault(h => h.HodDepartment == department);
            return await Task.FromResult(hod != null ? hod.HodName :"");
        }

        private async Task<string> GenerateRollIdAsync(string department)
        {
            string rollId;

            do
            {
                rollId = $"UG23{department}{orderNumber}";
                orderNumber++;
            } while (!await App.StudentViewModel.IsRollIdAvailableAsync(rollId));

            return rollId;
        }

        //Add the Student information into the SQLite database
        private async void Button_Clicked(object sender, EventArgs e)
        {
            string studentName = ((StudentsViewModel)BindingContext).Name;
            string studentDepartment = ((StudentsViewModel)BindingContext).Department;
            string studentGender = ((StudentsViewModel)BindingContext).Gender;
            string studentYear = ((StudentsViewModel)BindingContext).StudentYear;
            string studentState = ((StudentsViewModel)BindingContext).StudentState;
           
            if (string.IsNullOrWhiteSpace(studentName) || studentName.Length <= 4)
            {
                await DisplayAlert("Error Name Alert", "Name must be at least 5 characters.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(studentDepartment))
            {
                await DisplayAlert("Error Department", "Select student department", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(studentYear))
            {
                await DisplayAlert("Error Student Year", "Select student year", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(studentGender))
            {
                await DisplayAlert("Error Gender", "Select your gender", "OK");
                return;
            }

            StudentsModel newStudent = new StudentsModel
            {
                StudentName = studentName,
                StudentDepartment = studentDepartment,
                StudentId = generatedRollId,
                Gender = studentGender,
                StudentYear = studentYear,
                StudentState = studentState,
                TeacherId = MappingTeacher,
                HodName = MappingHod,
                TeacherName = TeacherLabel.Text
            };

            UserDialogs.Instance.ShowLoading("Loading");
            await App.StudentViewModel.AddStudentAsync(newStudent);
            await Task.Delay(1000);

            UserDialogs.Instance.HideLoading();
            await Navigation.PopAsync();
        }



        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {   
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                statePicker.IsVisible = true;
                LoadStatesDataAsync();
            }
            else 
            {
                await DisplayAlert("No Internet Connection", "Please connect your mobile to the internet and try again", "OK");

                statePicker.IsVisible = true;
                LoadLocalStates();
               
            }
        }
    }
}
