using Acr.UserDialogs;
using System;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.REST_APIs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayUserPage : ContentPage
    {
        private bool isEditing = false;
        private readonly UserData originalData;

        public DisplayUserPage(UserData user)
        {
            InitializeComponent();
            BindingContext = user;

            originalData = new UserData
            {
                id = user.id,
                first_name = user.first_name,
                last_name = user.last_name,
                email = user.email,
            };

            fnameEntryDisplay.TextChanged += Entry_TextChanged;
            lnameEntryDisplay.TextChanged += Entry_TextChanged;
            emailEntryDisplay.TextChanged += Entry_TextChanged;

        }
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (entry == fnameEntryDisplay)
                {
                    ErrorFname.Text = "";

                }
                else if (entry == lnameEntryDisplay)
                {
                    ErrorLname.Text = "";

                }
                else if (entry == emailEntryDisplay)
                {
                    ErrorEmail.Text = "";
                }

            }
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {

            ErrorFname.Text = ErrorLname.Text = ErrorEmail.Text = "";

            if (!isEditing)
            {
                fnameEntryDisplay.IsEnabled = true;
                lnameEntryDisplay.IsEnabled = true;
                emailEntryDisplay.IsEnabled = true;

                editButton.Text = "Save Data";

                isEditing = true;
            }
            else
            {
                UserData updatedUserData = new UserData
                {
                    id = (BindingContext as UserData).id,
                    first_name = fnameEntryDisplay.Text,
                    last_name = lnameEntryDisplay.Text,
                    email = emailEntryDisplay.Text,
                };


                if (updatedUserData.id == originalData.id &&
                    updatedUserData.first_name == originalData.first_name &&
                    updatedUserData.last_name == originalData.last_name &&
                    updatedUserData.email == originalData.email) 
                {
                    bool confirm = await DisplayAlert("No Changes Error", "There are no changes. Are you sure want to continue?", "Yes", "No");

                    if (confirm)
                    {
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        return;
                    }
                    
                }

                //Validations for Update Data
                if (string.IsNullOrWhiteSpace(fnameEntryDisplay.Text) || string.IsNullOrWhiteSpace(lnameEntryDisplay.Text) || string.IsNullOrWhiteSpace(emailEntryDisplay.Text))
                {
                    ErrorFname.Text = string.IsNullOrWhiteSpace(fnameEntryDisplay.Text) ? "First Name is required" : "";
                    ErrorLname.Text = string.IsNullOrWhiteSpace(lnameEntryDisplay.Text) ? "Last Name is required" : "";
                    
                    if (string.IsNullOrWhiteSpace(emailEntryDisplay.Text))
                    {
                        ErrorEmail.Text = "Email is required";
                        return;
                    }

                    if (!IsValidEmail(emailEntryDisplay.Text))
                    {
                        ErrorEmail.Text = "Invalid email format";
                        return;
                    }

                    return;

                }

                if (!IsValidEmail(emailEntryDisplay.Text))
                {
                    ErrorEmail.Text = "Invalid email format";
                    return;
                }

                bool IsValidEmail(string email)
                {
                    string emailPattern = "^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$";
                    Regex regex = new Regex(emailPattern);

                    return regex.IsMatch(email);
                }

             
                using (UserDialogs.Instance.Loading("Update Data"))
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        bool isUpdateSuccessful = new ApiService().PutDataAsync(updatedUserData).Result;

                        if (isUpdateSuccessful)
                        {
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Error UPDATE! data", "data will be not update in the api", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await DisplayAlert("No Internet Connection", "Please check your internet connection and try again", "OK");
                        return;
                    }
                    
                }
  
            }

        }

    }
}