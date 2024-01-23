
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.REST_APIs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ApiPage : ContentPage
    {
        public ApiPage()
        {
            InitializeComponent();
            CheckInternetAndGetData();
        }

        public class UserContainer
        {
            [JsonProperty("data")]
            public List<UserData> Data { get; set; }
        }

        private async void CheckInternetAndGetData()
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetApiData();
            }
            else
            {
                await DisplayAlert("No Internet Connection", "Please check your internet connection and try again later - Kindly reopen the app.", "OK");
            }
        }

        public async Task GetApiData()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://reqres.in/api/users?page=1&per_page=40");
            var userContainer = JsonConvert.DeserializeObject<UserContainer>(response);

            var student = userContainer?.Data;

            if (student != null)
            {
                userListView.ItemsSource = student;
            }
        }

        private async  void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddData());
        }

        //Delete Button 
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}
