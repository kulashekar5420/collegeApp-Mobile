using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FirstProject.REST_APIs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayUserPage : ContentPage
    {
        public DisplayUserPage(UserData user)
        {
            InitializeComponent();
            BindingContext = user;
        }
    }
}