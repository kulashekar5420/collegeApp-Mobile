using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace FirstProject.REST_APIs
{
    public partial class AddData : ContentPage
    {
        private ApiService apiService;

        public AddData()
        {
            InitializeComponent();
            apiService = new ApiService();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailEntry.Text) || string.IsNullOrWhiteSpace(fnameEntry.Text))
            {
                await DisplayAlert("Error", "Email and First Name are required fields.", "OK");
                return;
            }

            UserData user = new UserData
            {
                email = emailEntry.Text,
                first_name = fnameEntry.Text,
                last_name = lnameEntry.Text,
                avatar = avatarEntry.Text
            };


            bool isSuccess = false;

            isSuccess = await apiService.CreateUserAsync(user);

            if (isSuccess)
            {
                await DisplayAlert("Success", "User created successfully", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to create user. Please try again later.", "OK");
            }

        }
    }
}
