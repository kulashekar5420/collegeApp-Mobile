using Acr.UserDialogs;
using FirstProject;
using FirstProject.Teachers; 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Teachers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateTeacherPage : ContentPage
    {
        private readonly TeachersModel originalTeacher;
        public UpdateTeacherPage(TeachersModel teachers)
        {
            InitializeComponent();
            BindingContext = teachers;

            displayName.Text = teachers.TeacherName;

            displayName.TextChanged += DisplayName_TextChanged;

            originalTeacher = new TeachersModel
            {
                TeacherName = teachers.TeacherName,
                TeacherYear = teachers.TeacherYear,
                TeacherDepartment = teachers.TeacherDepartment,
                TeacherId = teachers.TeacherId,
                HodName = teachers.HodName,
                Gender = teachers.Gender


            };

            displayName.Text = teachers.TeacherName;
            displayTeacherId.Text = teachers.TeacherId;


            LoadDepHodAsync();

        }

        private async void LoadDepHodAsync()
        {
            var hod = BindingContext as TeachersModel;

            if (hod != null)
            {
                await App.TeacherViewModel.LoadAvailableHods();
                var availableHods = new ObservableCollection<HodsModel>(
                    App.TeacherViewModel.AvailableHods
                    .Where(h => h.HodDepartment == hod.TeacherDepartment)
                    );

                displayHodPicker.ItemsSource = null;
                displayHodPicker.ItemsSource = availableHods;
                displayHodPicker.ItemDisplayBinding = new Binding("HodInfo");

                displayHodPicker.SelectedItem = availableHods.FirstOrDefault(h => h.HodName == hod.HodName);
            }
        }

        //if teachers change the hod
        private void displayHodPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTeacher = displayHodPicker.SelectedItem as HodsModel;

            if (selectedTeacher != null)
            {
                var updatedTeacher = BindingContext as TeachersModel;

                updatedTeacher.HodName = selectedTeacher.HodName;


            }
        }


        private void DisplayName_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                displayName.Text = e.NewTextValue.ToUpper();
                displayName.Keyboard = Keyboard.Text;
                displayName.TextTransform = TextTransform.Uppercase;
            }
            else
            {

                displayName.Keyboard = Keyboard.Default;
                displayName.TextTransform = TextTransform.None;
            }
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            var updatedTeacher = BindingContext as TeachersModel;

            if (string.IsNullOrWhiteSpace(updatedTeacher.TeacherName))
            {
                await DisplayAlert("Teacher Name", "Enter your Name", "OK");
                return;
            }
            // Check if there is already a teacher with the same department and year
            bool teacherExists = App.TeacherViewModel.Teachers.Any(t =>
                t.TeacherDepartment == updatedTeacher.TeacherDepartment &&
                t.TeacherYear == updatedTeacher.TeacherYear &&
                t.TeacherId != updatedTeacher.TeacherId); 

            if (teacherExists)
            {
                await DisplayAlert("Teacher Already Exists", $"A teacher for {updatedTeacher.TeacherYear} year in {updatedTeacher.TeacherDepartment} department already exists. Please choose a different year in the department.", "OK");
                return;
            }
            if (updatedTeacher.TeacherName == originalTeacher.TeacherName &&
                updatedTeacher.TeacherDepartment == originalTeacher.TeacherDepartment &&
                updatedTeacher.HodName == originalTeacher.HodName &&
                updatedTeacher.TeacherYear == originalTeacher.TeacherYear &&
                updatedTeacher.Gender == originalTeacher.Gender)
            {
                bool confirm = await DisplayAlert("No Changes", "There are no changes. Are you sure you want to continue?", "Yes", "No");
                if (!confirm)
                {
                    return;
                }
            }
            await App.TeacherViewModel.UpdateTeacherAsync(updatedTeacher);
            await DisplayAlert("Teacher Update Success", $"{updatedTeacher.TeacherName}'s information has been updated successfully.", "OK");
            await Navigation.PopAsync();

        }

      
    }
}

