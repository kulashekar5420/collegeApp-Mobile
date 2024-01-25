using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System;
using FirstProject;
using System.Collections.Generic;
using FirstProject.ViewModel;
using System.Windows.Input;
using Xamarin.Forms;

public class HodsViewModel : INotifyBaseViewModel
{
    private readonly string databasePath;
    private bool isNoDataVisible;
    private ObservableCollection<HodsModel> hods;
    private ObservableCollection<TeachersModel> availableteachers;
    private bool isRefreshing;
    private string hodName;
    private string hodDepartment;
    private string hodGender;
    private string selectedTeacherTeacherid;
    private string hodId;
    public ICommand RefreshCommand { get; set; }

    public bool IsRefreshing
    {
        get { return isRefreshing; }
        set
        {
            if (isRefreshing != value)
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
    }
    public ObservableCollection<HodsModel> Hods
    {
        get { return hods; }
        set
        {
            hods = value;
            OnPropertyChanged(nameof(Hods));
        }
    }

    public bool IsNoDataVisible
    {
        get { return isNoDataVisible; }
        set
        {
            if (isNoDataVisible != value)
            {
                isNoDataVisible = value;
                OnPropertyChanged(nameof(IsNoDataVisible));
            }
        }
    }

    public ObservableCollection<TeachersModel> AvailableTeachers
    {
        get { return availableteachers; }
        set
        {
            availableteachers = value;
            OnPropertyChanged(nameof(AvailableTeachers));
        }
    }
    public string HodName
    {
        get { return hodName; }
        set
        {
            if (hodName != value)
            {
                hodName = value;
                OnPropertyChanged(nameof(HodName));
            }
        }
    }

    public string HodDepartment
    {
        get { return hodDepartment; }
        set
        {
            if (hodDepartment != value)
            {
                hodDepartment = value;
                OnPropertyChanged(nameof(HodDepartment));
            }
        }
    }

    public string HodGender
    {
        get { return hodGender; }
        set
        {
            if (hodGender != value)
            {
                hodGender = value;
                OnPropertyChanged(nameof(HodGender));
            }
        }
    }
     public string SelectedTeacherTeacherid
    {
        get { return selectedTeacherTeacherid; }
        set
        {
            if (selectedTeacherTeacherid != value)
            {
                selectedTeacherTeacherid = value;
                OnPropertyChanged(nameof(SelectedTeacherTeacherid));
            }
        }
    }

    public string HodId
    {
        get { return hodId; }
        set
        {
            if (hodId != value)
            {
                hodId = value;
                OnPropertyChanged(nameof(HodId));
            }
        }
    }

    public HodsViewModel()
    {
        databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "School.db");
        RefreshCommand = new Command(async () => await RefreshDataAsync());
        LoadHods();
    }

    private async Task RefreshDataAsync()
    {
        await LoadHods();
    }

    public async Task LoadHods()
    {
        IsRefreshing = true;
        var database = new SchoolDatabase(databasePath);
        var hodList = await database.GetAllHodsAsync();

        Hods = new ObservableCollection<HodsModel>(hodList);
        IsNoDataVisible = Hods.Count == 0;
        IsRefreshing = false;

    }
    public async Task LoadAvailableTeachersAsync()
    {
        var database = new SchoolDatabase(databasePath);

        var allTeachers = await database.GetAllTeachersAsync();
        var hodList = await database.GetAllHodsAsync();

        // get the hod teacher id it meanas teacherid == hod teacher id
        var hodTeacherIds = hodList.Select(hod => hod.HodTeacherId).ToList();

        // Filter out teachers who are already HODs based on the teacherid it menas if the teacher rollid availble in the hod table 
        var availableTeachersList = allTeachers
            .Where(teacher => !hodTeacherIds.Contains(teacher.TeacherId))
            .ToList();

        AvailableTeachers = new ObservableCollection<TeachersModel>(availableTeachersList);
      
    }



    public async Task AddHodAsync(HodsModel newHod)
    {
       
        string generatedHodid = await GenerateHodIsAsync(newHod.HodDepartment);

        var hod = new HodsModel
        {
            
            HodName = newHod.HodName,
            HodDepartment = newHod.HodDepartment,
            HodGender = newHod.HodGender,
            HodId = generatedHodid,
            HodTeacherId = newHod.HodTeacherId,
        };

        await LoadAvailableTeachersAsync();
        await App.DatabaseforSchool.SaveHodsAsync(hod);
        Hods.Add(hod);
       

    }

    private async Task<string> GenerateHodIsAsync(string teacher)
    {
        string hodId;
        int orderNumber = 1;

        while (Hods.Any(s => s.HodId == $"UG-HOD{teacher}{orderNumber}"))
        {
            orderNumber++;
        }

        hodId = $"UGHOD{teacher}{orderNumber}";

        // Mark the teacher as HOD
        var teacherModel = AvailableTeachers.FirstOrDefault(t => t.TeacherDepartment == teacher);
      

        return await Task.FromResult(hodId);
    }

    public async Task DeleteHodAsync(HodsModel selectedHod)
    {
        //unmap the hod from teacher
        await UnMapHod(selectedHod.HodName);
        await App.DatabaseforSchool.DeleteHodAsync(selectedHod);
        await LoadHods();
    }

    //Get the teachers data and findout the mapped hodname 
    public async Task<List<TeachersModel>> GetTeachersByHodName(string hodName)
    {
        var allTeachers = await App.DatabaseforSchool.GetAllTeachersAsync();
        return allTeachers.Where(t => t.HodName == hodName).ToList();
    }
    //Get the students data and find out the mapped hodname 
    public async Task<List<StudentsModel>> GetStudentsByHodName(string hodName)
    {
        var allStudents = await App.DatabaseforSchool.GetAllStudentsAsync();
        return allStudents.Where(s => s.HodName == hodName).ToList();
    }


    //UnMap the Hod from the teacher and Student 
    private async Task UnMapHod(string hodName)
    {
        var teachersWithHod = await GetTeachersByHodName(hodName);
        var studentswithHod = await GetStudentsByHodName(hodName);

        foreach (var teacher in teachersWithHod)
        {
            teacher.HodName = null; 
            await App.DatabaseforSchool.UpdateTeacherAsync(teacher);
        }

        foreach (var student in studentswithHod)
        {
            student.HodName = null;
            await App.DatabaseforSchool.UpdateStudentAsync(student);
        }
    }

    public Task<bool> isHodidAvailableAsync(string hodId)
    {
        return Task.FromResult(Hods.All(s => s.HodId != hodId));
    }
}






