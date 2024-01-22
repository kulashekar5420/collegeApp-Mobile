using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

            Getstudents();

        }


        public async void Getstudents()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync("https://reqres.in/api/users");
            var userContainer = JsonConvert.DeserializeObject<UserContainer>(response);

            var student = userContainer?.Data;

            if (student != null)
            {
                userListView.ItemsSource = student;
            }
        }

        public class UserContainer
        {
            [JsonProperty("data")]
            public List<UserData> Data { get; set; }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddData());
        }
    }
}
