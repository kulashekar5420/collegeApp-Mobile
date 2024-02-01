using FirstProject.REST_APIs;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Hods
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HodPage : ContentPage
    {
        private bool isProcessingButtonClick = false;
        List<SwipeView> swipeViews { set; get; }

        public HodPage()
        {
            InitializeComponent();
            BindingContext = App.HodsViewModel;

            swipeViews = new List<SwipeView>();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = App.HodsViewModel.LoadHods();

        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            if (!isProcessingButtonClick)
            {
                isProcessingButtonClick = true;
                (sender as ImageButton).IsEnabled = false;
                await Navigation.PushAsync(new AddHodPage());
                (sender as ImageButton).IsEnabled = true;
                isProcessingButtonClick = false;
            }     
        }

        //Delete hod tapGesture
        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            if ((sender as StackLayout)?.BindingContext is HodsModel selectedHod)
            {
               bool confirmation = await DisplayAlert("Delete Confirmation", $"Do you want to delete {selectedHod.HodName}?", "Yes", "No");
                if (confirmation)
                {
                    await App.HodsViewModel.DeleteHodAsync(selectedHod);
                }
            }
        }

        private void SwipeView_SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            if (swipeViews.Count != 0)
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

