using Acr.UserDialogs;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace FirstProject.REST_APIs
{
    public partial class AddData : ContentPage
    {
        private readonly ApiService apiService;
        public AddData()
        {
            InitializeComponent();
            apiService = new ApiService();

            fnameEntry.TextChanged += Entry_TextChanged;
            lnameEntry.TextChanged += Entry_TextChanged;
            emailEntry.TextChanged += Entry_TextChanged;
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (entry == fnameEntry)
                {
                    ErrorFname.Text = "";
                }
                else if (entry == lnameEntry)
                {
                    ErrorLname.Text = "";
                }
                else if (entry == emailEntry)
                {
                    ErrorEmail.Text = "";
                }
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            ErrorFname.Text = ErrorLname.Text = ErrorEmail.Text = "";

            if (string.IsNullOrWhiteSpace(fnameEntry.Text) || string.IsNullOrWhiteSpace(lnameEntry.Text) || string.IsNullOrWhiteSpace(emailEntry.Text))
            {
                ErrorFname.Text = string.IsNullOrWhiteSpace(fnameEntry.Text) ? "First Name is required" : "";
                ErrorLname.Text = string.IsNullOrWhiteSpace(lnameEntry.Text) ? "Last Name is required" : "";
               
                if (string.IsNullOrWhiteSpace(emailEntry.Text))
                {
                    ErrorEmail.Text = "Email is required";
                    return;
                }

                if (!IsValidEmail(emailEntry.Text))
                {
                    ErrorEmail.Text = "Invalid email format";
                    return;
                }

                return;
            }

            // Email validation
            if (!IsValidEmail(emailEntry.Text))
            {
                ErrorEmail.Text = "Invalid email format";
                return;
            }

            UserData user = new UserData
            {
                email = emailEntry.Text,
                first_name = fnameEntry.Text,
                last_name = lnameEntry.Text,
                avatar = avatarEntry.Text
            };

            using (UserDialogs.Instance.Loading("Adding user.."))
            {
                bool isSuccess = await apiService.PostDataAsync(user);

                if (isSuccess)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create user. Please try again later.", "OK");
                }
            }
        }

        //Email Validation Method
        private bool IsValidEmail(string email)
        {
            string emailPattern = "^[\\w\\.\\-]+@([\\w\\-]+\\.com|[\\w\\-]+\\.in)$";
            Regex regex = new Regex(emailPattern);

            return regex.IsMatch(email);
        }
    }
}
