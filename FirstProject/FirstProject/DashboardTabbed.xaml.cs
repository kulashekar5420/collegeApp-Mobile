
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace FirstProject
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardTabbed : Xamarin.Forms.TabbedPage
    {
        public DashboardTabbed()
        {
            InitializeComponent();
          
        }
    }
}