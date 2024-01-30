using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
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
        private LocalUserDB localUserDB;
        private bool isLongPressActive = false;
        public Command LongPressCommand { get; }

        public ApiPage()
        {
            InitializeComponent();
            localUserDB = new LocalUserDB();
            LongPressCommand = new Command<UserData>(async (selectedItem) => await HandleLongPress(selectedItem));
            BindingContext = this;
            CheckInternetAndGetData();
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
                await DisplayLocalData();
            }
        }

        public class UserContainer
        {
            [JsonProperty("data")]
            public List<UserData> Data { get; set; }
        }

        public async Task GetApiData()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetStringAsync("https://reqres.in/api/users?page=1&per_page=15");

            var userContainer = JsonConvert.DeserializeObject<UserContainer>(response);

            if (userContainer != null && userContainer.Data != null)
            {
                var users = userContainer.Data;
                foreach (var user in users)
                {
                    await localUserDB.SaveUserData(user);
                }

                userListView.ItemsSource = users;
            }

            refreshView.IsRefreshing = false;
        }

        private async Task DisplayLocalData()
        {          
            await localUserDB.InitializeDatabaseAsync();
            var localData = await localUserDB.GetUserData();

            if (localData != null && localData.Count > 0)
            {
                userListView.ItemsSource = localData;
            }
            else
            {
               await DisplayAlert("No Local Data", "No data available locally.", "OK");
            }
         
            refreshView.IsRefreshing = false;
           
        }
        //Add data image button like FloatingActionButton
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddData());
        }

        private async Task HandleLongPress(UserData selectedItem)
        {
            isLongPressActive = true;

            var result = await DisplayActionSheet("Options", "Cancel", null, "Delete", "Edit");

            switch (result)
            {
                case "Delete":
                    if (selectedItem != null)
                    {
                        bool answer = await DisplayAlert("Confirmation", $"Do you want to delete user {selectedItem.first_name}?", "Yes", "No");
                        if (answer)
                        {
                            await DeleteUser(selectedItem.id);
                        }
                    }
                    break;

                case "Edit":
                    if (selectedItem != null)
                    {
                        await Navigation.PushAsync(new DisplayUserPage(selectedItem));
                    }
                    break;
            }
            
            isLongPressActive = false;
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
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                await GetApiData();
            }
            else
            {
                await DisplayLocalData();
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!isLongPressActive)
            {
                var selectedItem = ((Frame)sender).BindingContext as UserData;

                if (selectedItem != null)
                {
                    await Navigation.PushAsync(new DisplayUserPage(selectedItem));
                }
            }

        }
    }
}
