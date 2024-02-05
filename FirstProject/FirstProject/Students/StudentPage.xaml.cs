using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using FirstProject.Teachers;
using System.Collections.Generic;


namespace FirstProject.Students
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StudentPage : ContentPage
    {
        private string studentDepartment;
        private static string selectedDepartment;
        private bool isProcessingTap = false;
        private bool isProcessingButtonClick = false;
        List<SwipeView> swipeViews { set; get; }

        public StudentPage()
        {
            InitializeComponent();
            BindingContext = App.StudentViewModel;

            swipeViews = new List<SwipeView>();
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        // Add Student Btn 
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (!isProcessingButtonClick)
            {
                isProcessingButtonClick = true;
                (sender as ImageButton).IsEnabled = false;
                await Navigation.PushAsync(new AddStudentPage());
                (sender as ImageButton).IsEnabled = true;
                isProcessingButtonClick = false;
            }
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
            if (!isProcessingTap)
            {
                isProcessingTap = true;

                if ((sender as Frame)?.BindingContext is StudentsModel selectedStudent)
                {
                    await Navigation.PushAsync(new UpdateStudentPage(selectedStudent));
                }

                isProcessingTap = false;
            }
        }

        private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            if(swipeViews.Count != 0)
            {
                swipeViews[0].Close();
                swipeViews.Remove(swipeViews[0]);
            }
        }

        private void SwipeView_SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            swipeViews.Add(sender as SwipeView);
        }

       
    }
}

