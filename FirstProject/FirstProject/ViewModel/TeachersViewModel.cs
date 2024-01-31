using FirstProject;
using FirstProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

public class TeachersViewModel : INotifyBaseViewModel
{
    
    private bool isNoDataVisible;
    private readonly string databasePath;
    private ObservableCollection<TeachersModel> teachers;
    private readonly Dictionary<string, int> departmentOrderNumbers = new Dictionary<string, int>();
    private ObservableCollection<TeachersModel> availableDepteachers;
    private ObservableCollection<HodsModel> availableHods;
    private bool isRefreshing;
   
    //Propertys Zone
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

    public ObservableCollection<TeachersModel> Teachers
    {
        get { return teachers; }
        set
        {
            teachers = value;
            OnPropertyChanged(nameof(Teachers));
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

    public ObservableCollection<HodsModel> AvailableHods
    {
        get { return availableHods; }
        set
        {
            availableHods = value;
            OnPropertyChanged(nameof(AvailableHods));
        }
    }
    public ObservableCollection<TeachersModel> AvailableDepTeachers
    {
        get { return availableDepteachers; }
        set
        {
            availableDepteachers = value;
            OnPropertyChanged(nameof(AvailableDepTeachers));
        }
    }


    public string teacherName { get; set; }
    public string teacherDepartment { get; set; }
    public string teacherId { get; set; }
    public string teacherYear { get; set; }
    public string Gender { get; set; }
    public Command RefreshCommand { get; set; }

    public TeachersViewModel()
    {
        databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "School.db");
        _ = LoadTeachers();
        RefreshCommand = new Command(async () => await RefreshDataAsync());


    }

    private async Task RefreshDataAsync()
    {
        await LoadTeachers();
    }

    public async Task LoadTeachers()
    {
        IsRefreshing = true;
        var database = new SchoolDatabase(databasePath);
        var teachersList = await database.GetAllTeachersAsync();

        //Binding Teachers property in the collectionview 
        Teachers = new ObservableCollection<TeachersModel>(teachersList);
        IsNoDataVisible = teachersList.Count == 0;
        IsRefreshing = false;

    }

    
    //Load available HOD's in the database
    public async Task LoadAvailableHods()
    {
        var database = new SchoolDatabase(databasePath);
        var hodList = await database.GetAllHodsAsync();
        AvailableHods = new ObservableCollection<HodsModel>(hodList);
    }


    public async Task LoadAvailableDepTeachers()
    {
        var database = new SchoolDatabase(databasePath);
        var teachersList = await database.GetAllTeachersAsync();

        AvailableDepTeachers = new ObservableCollection<TeachersModel>(teachersList);

    }

    public bool IsClassTeacherExists(string teacherYear, string teacherDepartment)
    {
        return Teachers.Any(t => t.TeacherYear == teacherYear && t.TeacherDepartment == teacherDepartment && !string.IsNullOrEmpty(t.TeacherYear));
    }

    public async Task AddTeachersAsync(TeachersModel newTeacher)
    {
        string generatedRollId = await GenerateRollIdAsync(newTeacher.TeacherDepartment);
        newTeacher.TeacherId = generatedRollId;

        var teacher = new TeachersModel
        {
            TeacherName = newTeacher.TeacherName,
            TeacherDepartment = newTeacher.TeacherDepartment,
            Gender = newTeacher.Gender,
            HodName = newTeacher.HodName,
            TeacherYear = newTeacher.TeacherYear,
            TeacherId = generatedRollId,
            
        };

       // If it's a normal teacher, add the teacher without checking for class teacher
       await App.DatabaseforSchool.SaveTeachersAsync(teacher);
       Teachers.Add(teacher);
       
    }


    public Task<bool> IsRollIdAvailableAsync(string tRollId)
    {
        return Task.FromResult(Teachers.All(s => s.TeacherId != tRollId));
    }

    private async Task<string> GenerateRollIdAsync(string department)
    {
        string rollId;

        if (!departmentOrderNumbers.ContainsKey(department))
        {
            departmentOrderNumbers[department] = 1;
        }

        int orderNumber = departmentOrderNumbers[department];

        while (Teachers.Any(s => s.TeacherId == $"101{department}{orderNumber}"))
        {
            orderNumber++;
        }

        departmentOrderNumbers[department] = orderNumber + 1;
        rollId = $"101{department}{orderNumber}";

        return await Task.FromResult(rollId);
    }

    public async Task UpdateTeacherAsync(TeachersModel updatedTeacher)
    {
        await App.DatabaseforSchool.UpdateTeacherAsync(updatedTeacher);
        await LoadTeachers();
        
    }


    public async Task DeleteTeacherAsync(TeachersModel selectedTeacher)
    {
        await UnMapTeacher(selectedTeacher.TeacherId);
   
        await App.DatabaseforSchool.DeleteTeacherAsync(selectedTeacher); 
        await LoadTeachers();
    }

    //
    public async Task<List<StudentsModel>> GetTeachersByTeacherId(string teacherId)
    {
        var allStudents = await App.DatabaseforSchool.GetAllStudentsAsync();
        return allStudents.Where(s => s.TeacherId == teacherId).ToList();
    }


    private async Task UnMapTeacher(string teacherRollId)
    {
        var studentswithTeacher = await GetTeachersByTeacherId(teacherRollId);
      
        foreach (var student in studentswithTeacher)
        {
            student.TeacherId = null;
            await App.DatabaseforSchool.UpdateStudentAsync(student);
        }
    }

 

}
