using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiService
{
    private const string ApiBaseUrl = "https://reqres.in/api/users";

    public async Task<bool> CreateUserAsync(UserData user)
    {
        using (HttpClient client = new HttpClient())
        { 
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string jsonUserData = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("", content);

            return response.IsSuccessStatusCode;
            
        }
    }
}
