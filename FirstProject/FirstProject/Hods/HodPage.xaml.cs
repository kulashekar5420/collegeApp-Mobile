using FirstProject;
using FirstProject.Hods;
using FirstProject.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.Hods
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HodPage : ContentPage
    {
       

        public HodPage()
        {
            InitializeComponent();
            BindingContext = App.HodsViewModel;
           
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = App.HodsViewModel.LoadHods();

        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddHodPage());


        }

        //Delete ho

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
    }
}

