using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Teachers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeacherPage : ContentPage
    {
        private bool isProcessingTap = false;
        private bool isProcessingButtonClick = false;
        public TeacherPage()
        {
            InitializeComponent();
            BindingContext = App.TeacherViewModel;
    
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await App.TeacherViewModel.LoadTeachers();
        }


        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (!isProcessingButtonClick)
            {
                isProcessingButtonClick = true;
                (sender as ImageButton).IsEnabled = false;
                await Navigation.PushAsync(new AddTeachersPage());
                (sender as ImageButton).IsEnabled = true;
                isProcessingButtonClick = false;
            }           
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if ((sender as StackLayout)?.BindingContext is TeachersModel selectedTeacher)
            {
                bool confirmation = await DisplayAlert("Delete Confirmation", $"Do you want to delete {selectedTeacher.TeacherName}?", "Yes", "No");

                if (confirmation)
                {
                    await App.TeacherViewModel.DeleteTeacherAsync(selectedTeacher);

                }
            }
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            if (!isProcessingTap)
            {
                isProcessingTap = true;

                if ((sender as Frame)?.BindingContext is TeachersModel selectedTeacher)
                {
                    await Navigation.PushAsync(new UpdateTeacherPage(selectedTeacher));
                }
                isProcessingTap = false;
            }        
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            if (!isProcessingTap)
            {
                isProcessingTap = true;

                if ((sender as Label)?.BindingContext is TeachersModel selectedTeacher)
                {
                    await Navigation.PushAsync(new TeacherStudentsPage(selectedTeacher));
                }

                isProcessingTap = false;
            }
           
        }

    }
}