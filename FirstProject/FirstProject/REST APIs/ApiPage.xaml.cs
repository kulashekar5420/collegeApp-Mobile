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

            try
            {
                var response = await httpClient.GetStringAsync("https://reqres.in/api/users?page=1&per_page=15");

                var userContainer = JsonConvert.DeserializeObject<UserContainer>(response);

                var student = userContainer?.Data;

                if (student != null)
                {
                    userListView.ItemsSource = student;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                refreshView.IsRefreshing = false;
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddData());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var selectedItem = ((StackLayout)sender).BindingContext as UserData;

            if (selectedItem != null)
            {
                bool result = await DisplayAlert("Delete Confirmation", $"Are you sure want to delete user {selectedItem.first_name}?", "Yes", "No");

                if (result)
                {
                    await DeleteUser(selectedItem.id);
                }
            }
        }

        private async Task DeleteUser(int userId)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.DeleteAsync($"https://reqres.in/api/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                await GetApiData();
                await DisplayAlert("Success", $"User {userId} has been deleted successfully.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete user.", "OK");
            }
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            await GetApiData();
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var selectedItem = ((Frame)sender).BindingContext as UserData;

            if (selectedItem != null)
            {
                await Navigation.PushAsync(new DisplayUserPage(selectedItem));
            }
        }
    }
}
