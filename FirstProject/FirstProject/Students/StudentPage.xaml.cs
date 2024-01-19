using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;


namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentPage : ContentPage
    {
          
        public StudentPage()
        {
            InitializeComponent();
            BindingContext = App.StudentViewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await App.StudentViewModel.LoadStudents();
        } 
        // Add Student Btn 
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddStudentPage());

        }
        //FilterPage popup
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new FilterPage());
        }

        private async  void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            if ((sender as StackLayout)?.BindingContext is StudentsModel selectedStudent)
            {
                bool confirmation = await DisplayAlert("Delete Confirmation", $"Do you want to delete {selectedStudent.StudentName}?", "Yes", "No");

                if (confirmation)
                {
                    await App.StudentViewModel.DeleteStudentAsync(selectedStudent);

                }
            }
        }
        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            if ((sender as Frame)?.BindingContext is StudentsModel selectedStudent)
            {
                await Navigation.PushAsync(new UpdateStudentPage(selectedStudent));
            }
        }

    }
}

