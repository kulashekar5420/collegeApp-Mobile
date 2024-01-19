using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateStudentPage : ContentPage
    {
        private readonly StudentsModel originalStudent;
        private StudentsModel studentsModel; 

        public UpdateStudentPage(StudentsModel student)
        {
            InitializeComponent();
            BindingContext = student;
            studentYearPicker.SelectedIndexChanged += studentYearPicker_SelectedIndexChanged;
            originalStudent = new StudentsModel
            {
                StudentName = student.StudentName,
                StudentDepartment = student.StudentDepartment,
                StudentYear = student.StudentYear,
                TeacherId = student.TeacherId,
                TeacherName = student.TeacherName,
                HodName = student.HodName,
                Gender = student.Gender
            };

            if (string.IsNullOrEmpty(TeacherLabel.Text))
            {
                TeacherLabel.Text = "No Teacher Found";
            }

            LoadDepHodAsync();
            LoadAvailableDepAndYearTeachers();
          
        }

        private async void LoadDepHodAsync()
        {
            var hod = BindingContext as StudentsModel;

            if (hod != null)
            {
                await App.TeacherViewModel.LoadAvailableHods();
                var availableHods = new ObservableCollection<HodsModel>(
                    App.TeacherViewModel.AvailableHods
                    .Where(h => h.HodDepartment == hod.StudentDepartment)
                    );

                displayHodPicker.ItemsSource = null;
                displayHodPicker.ItemsSource = availableHods;
                displayHodPicker.ItemDisplayBinding = new Binding("HodInfo");

                displayHodPicker.SelectedItem = availableHods.FirstOrDefault(h => h.HodName == hod.HodName);
            }
        }

        private void displayHodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedStudent = displayHodPicker.SelectedItem as HodsModel;

            if (selectedStudent != null)
            {
                var updatedStudent = BindingContext as StudentsModel;
                updatedStudent.HodName = selectedStudent.HodName;
            }
        }

        private async void studentYearPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BindingContext is StudentsModel studentsModel)
            {
                string selectedStudentYear = studentsModel.StudentYear;

                if (!string.IsNullOrWhiteSpace(selectedStudentYear))
                {
                    this.studentsModel = studentsModel;

                    TeachersModel teacher = await GetMappedTeacherAsync(selectedStudentYear);

                    if (teacher != null)
                    {
                        if (this.studentsModel != null)
                        {
                            this.studentsModel.TeacherId = teacher.TeacherId;
                            TeacherLabel.Text = $"{teacher.TeacherName} - {teacher.TeacherId}";
                        }
                     
                    }

                    LoadAvailableDepAndYearTeachers();
                }
            }
        }

        private async Task<TeachersModel> GetMappedTeacherAsync(string selectedStudentYear)
        {
            var availableTeachers = App.StudentViewModel.AvailableDepTeachers;
            var selectedTeacher = availableTeachers.FirstOrDefault(t =>
                t.TeacherDepartment == studentsModel.StudentDepartment &&
                t.TeacherYear == selectedStudentYear);

            return await Task.FromResult(selectedTeacher);
        }

        private async void LoadAvailableDepAndYearTeachers()
        {
            if (BindingContext is StudentsModel studentsModel)
            {
                await App.StudentViewModel.LoadAvailableDepTeachers();

                string selectedStudentYear = studentsModel.StudentYear;

                if (App.StudentViewModel.AvailableDepTeachers != null && App.StudentViewModel.AvailableDepTeachers.Any())
                {
                    var selectedTeacher = App.StudentViewModel.AvailableDepTeachers.FirstOrDefault(t => 
                    t.TeacherDepartment == studentsModel.StudentDepartment &&
                    t.TeacherYear == selectedStudentYear);

                    if (selectedTeacher != null)
                    {
                        var updatedStudent = BindingContext as StudentsModel;
                        updatedStudent.TeacherId = selectedTeacher.TeacherId;
                        TeacherLabel.Text = $"{selectedTeacher.TeacherName}-{selectedTeacher.TeacherId}";
                    }
                    else
                    {
                        TeacherLabel.Text = "No Class Teacher";
                    }

                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is StudentsModel updatedStudent)
            {
                // Check if there are changes in the student information
                if (updatedStudent.StudentName == originalStudent.StudentName &&
                    updatedStudent.StudentDepartment == originalStudent.StudentDepartment &&
                    updatedStudent.Gender == originalStudent.Gender &&
                    updatedStudent.TeacherId == originalStudent.TeacherId &&
                    updatedStudent.HodName == originalStudent.HodName &&
                    updatedStudent.StudentYear == originalStudent.StudentYear)
                {
                    bool confirm = await DisplayAlert("No Changes", "There are no changes. Are you sure you want to continue?", "Yes", "No");
                    if (!confirm)
                        return;
                }

                // Validate the student's name and year
                if (string.IsNullOrWhiteSpace(updatedStudent.StudentName))
                {
                    await DisplayAlert("Name Error", "Enter your name", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(updatedStudent.StudentYear))
                {
                    await DisplayAlert("Student Year Error", "Pick your student year", "OK");
                    return;
                }

                await App.StudentViewModel.UpdateStudentAsync(updatedStudent);
                await DisplayAlert("Student Update Success", $"{updatedStudent.StudentName}'s information has been updated successfully.", "OK");

                await Navigation.PopAsync();
            }
        }
    }
}
