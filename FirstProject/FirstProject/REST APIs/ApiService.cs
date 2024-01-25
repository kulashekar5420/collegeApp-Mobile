using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiService
{
    private const string ApiBaseUrl = "https://reqres.in/api/users";


    public async Task<bool> PostDataAsync(UserData user)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string jsonUserData = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"API request failed with status code: {response.StatusCode}");
            }

            return false;
        }
    }

    public async Task<bool> PutDataAsync(UserData user)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri(ApiBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string jsonUserData = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(jsonUserData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PutAsync($"users/{user.id}", content).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"API request failed with status code: {response.StatusCode}");
            }

            return false;
        }
    }


}
