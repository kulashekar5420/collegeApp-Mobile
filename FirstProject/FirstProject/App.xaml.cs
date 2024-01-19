using System;
using System.IO;
using Xamarin.Forms;
namespace FirstProject
{
    public partial class App : Application
    {
        
        public static SchoolDatabase DatabaseforSchool { get; private set; }
        public static StudentsViewModel StudentViewModel { get; private set; }
        public static TeachersViewModel TeacherViewModel { get; private set; }
        public static HodsViewModel HodsViewModel { get; private set; }



        public App()
        {
            InitializeComponent();


            DatabaseforSchool = new SchoolDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "School.db"));

          
            StudentViewModel = new StudentsViewModel();
            TeacherViewModel = new TeachersViewModel();
            HodsViewModel = new HodsViewModel();
 

            MainPage = new NavigationPage(new MainPage());
           
            

        }
       
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

