using Acr.UserDialogs;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Teachers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeacherStudentsPage : ContentPage
    {
        private TeachersModel selectedTeacher;

        public TeacherStudentsPage(TeachersModel teacher)
        {
            InitializeComponent();
            selectedTeacher = teacher;
            BindingContext = App.StudentViewModel;
            Title = $"{selectedTeacher.TeacherName}'s Students";

            LoadStudentsForSelectedTeacher();
        }
        private async void LoadStudentsForSelectedTeacher()
        {

            if (selectedTeacher != null)
            {
                await App.StudentViewModel.LoadStudentsByTeacher(selectedTeacher);
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if ((sender as StackLayout)?.BindingContext is StudentsModel selectedStudent)
            {
                bool confirmation = await DisplayAlert("Remove Confirmation", $"Do you want to remove {selectedStudent.StudentName}?", "Yes", "No");

                if (confirmation)
                {
                    
                    await App.StudentViewModel.RemoveStudentAsync(selectedStudent, selectedTeacher);
                    UserDialogs.Instance.Alert($"{selectedStudent.StudentName} has been removed from {selectedTeacher.TeacherName}.", "Student Removed", "OK");
                }
            }
        }
    }
}
