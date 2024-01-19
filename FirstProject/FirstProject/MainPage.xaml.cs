using FirstProject.Students;
using FirstProject.Teachers;
using System;
using Xamarin.Forms;

namespace FirstProject
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
           await Navigation.PushAsync(new DashboardTabbed());
           
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TeacherPage());
            
        }


    }
}


