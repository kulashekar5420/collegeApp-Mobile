using Acr.UserDialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
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

        private ObservableCollection<TeachersModel> availableDepTeachers;
        private ObservableCollection<HodsModel> availableHods;
        private ObservableCollection<TeachersModel> availableDepteachers;

        public AddStudentPage()
        {
            InitializeComponent();
            BindingContext = new StudentsViewModel();

            departmentPicker.SelectedIndexChanged += DepartmentPicker_SelectedIndexChanged;
            studentYearPicker.SelectedIndexChanged += StudentYearPicker_SelectedIndexChanged;

            availableHods = new ObservableCollection<HodsModel>();
            availableDepTeachers = new ObservableCollection<TeachersModel>();

            LoadAvailableTeachers();
            LoadAvailableHods();
          
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

        //AddStudents data 
        private async void Button_Clicked(object sender, EventArgs e)
        {
            string studentName = ((StudentsViewModel)BindingContext).Name;
            string studentDepartment = ((StudentsViewModel)BindingContext).Department;
            string studentGender = ((StudentsViewModel)BindingContext).Gender;
            string studentYear = ((StudentsViewModel)BindingContext).StudentYear;
           
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

    }
}
