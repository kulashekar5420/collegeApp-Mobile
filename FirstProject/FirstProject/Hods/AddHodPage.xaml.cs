using Acr.UserDialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Hods
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddHodPage : ContentPage
    {
        private string generatedHodid;
        private ObservableCollection<TeachersModel> availableTeachers;
       
        public AddHodPage()
        {
            InitializeComponent();
            BindingContext = new HodsViewModel();

   
            teacherPicker.SelectedIndexChanged += teacherPicker_SelectedIndexChanged;

            availableTeachers = new ObservableCollection<TeachersModel>();

            //load the available teachers
            LoadAvailableTeachers();
        }

        private async void LoadAvailableTeachers()
        {
            var hodsViewModel = (HodsViewModel)BindingContext;
            await hodsViewModel.LoadAvailableTeachersAsync();


            availableTeachers = hodsViewModel.AvailableTeachers;
            teacherPicker.ItemsSource = availableTeachers;
        }

        private async void teacherPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTeacher = (TeachersModel)teacherPicker.SelectedItem;

            if(selectedTeacher == null)
            {
                return;
            }

            if (selectedTeacher != null)
            {
                var hodsViewModel = (HodsViewModel)BindingContext;
                hodsViewModel.HodName = selectedTeacher.TeacherName;
                hodsViewModel.HodDepartment = selectedTeacher.TeacherDepartment;
                hodsViewModel.HodGender = selectedTeacher.Gender;
                hodsViewModel.SelectedTeacherTeacherid = selectedTeacher.TeacherId;

                generatedHodid = await GenerateHodidAsync(selectedTeacher.TeacherDepartment);
                rollIdLabel.Text = $"{generatedHodid}";

            }
           
        }
        
        private async Task<string> GenerateHodidAsync(string teacher)
        {
            string hodId;
            
            do
            {
                hodId = $"UGHOD{teacher}";

            } while (!await App.HodsViewModel.isHodidAvailableAsync(hodId));

            return hodId ;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var selectedTeacher = (TeachersModel)teacherPicker.SelectedItem;

            if (selectedTeacher != null)
            {
                var hodsViewModel = (HodsViewModel)BindingContext;

                var generatedHodid = await GenerateHodidAsync(selectedTeacher.TeacherDepartment);

                var newHod = new HodsModel
                {
                    HodName = selectedTeacher.TeacherName,
                    HodDepartment = selectedTeacher.TeacherDepartment,
                    HodGender = selectedTeacher.Gender,
                    HodId = generatedHodid,
                    HodTeacherId = selectedTeacher.TeacherId
                };

                if (App.HodsViewModel.Hods.Any(existingHod => existingHod.HodDepartment == newHod.HodDepartment))
                {
                    UserDialogs.Instance.Alert($"HOD for department {newHod.HodDepartment} already exists.", "Hod Already Exists", "OK");
                    return;
                }


                // Show the loading
                UserDialogs.Instance.ShowLoading("Loading");

                await hodsViewModel.AddHodAsync(newHod);
               
                // Hide the loading
                UserDialogs.Instance.HideLoading();

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Please select a teacher.", "OK");
            }
        }

    }
}


