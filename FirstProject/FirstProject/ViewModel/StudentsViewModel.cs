using FirstProject;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using FirstProject.ViewModel;

public class StudentsViewModel : INotifyBaseViewModel
{
    private readonly string databasePath;
    private bool isNoDataVisible;
    private bool isRefreshing;
    private ObservableCollection<StudentsModel> students;
    private ObservableCollection<TeachersModel> availableTeachers;
    private ObservableCollection<TeachersModel> availableDepteachers;
    private readonly Dictionary<string, int> departmentOrderNumbers = new Dictionary<string, int>();
    private ObservableCollection<HodsModel> availableHods;
    private ObservableCollection<TeachersModel> tId;
    private ObservableCollection<TeachersModel> classTeachers;
    //Get and set the value of isNoDataVisible
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

    public ObservableCollection<StudentsModel> Students
    {
        get { return students; }
        set
        {
            students = value;
            OnPropertyChanged(nameof(Students));
        }
    }
    public ObservableCollection<TeachersModel> AvailableTeachers
    {
        get { return availableTeachers; }
        set
        {
            availableTeachers = value;
            OnPropertyChanged(nameof(AvailableTeachers));
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

    public ObservableCollection<TeachersModel> TId 
    { 
        get { return tId; } 
        set
        {
            tId = value;
            OnPropertyChanged(nameof(TId));
        }
    }
   

    public ObservableCollection<TeachersModel> ClassTeachers
    {
        get { return classTeachers; }
        set
        {
            classTeachers = value;
            OnPropertyChanged(nameof(ClassTeachers));
        }
    }
    private string studentDepartment;
    public string StudentDepartment
    {
        get { return studentDepartment; }
        set
        {
            if (studentDepartment != value)
            {
                studentDepartment = value;
                OnPropertyChanged(nameof(StudentDepartment));
            }
        }
    }
    private string _teacherDisplayName;
    public string TeacherDisplayName
    {
        get { return _teacherDisplayName; }
        set
        {
            if (_teacherDisplayName != value)
            {
                _teacherDisplayName = value;
                OnPropertyChanged(nameof(TeacherDisplayName));
            }
        }
    }
    public string Name { get; set; }
    public string Department { get; set; }
    public string Gender { get; set; }
    public string StudentYear { get; set; }
    public ICommand RefreshCommand { get;  set; }
    public StudentsViewModel()
    {

        databasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "School.db");
        RefreshCommand = new Command(async () => await RefreshDataAsync());
        // Load students 
        LoadStudents();
    }
    private async Task RefreshDataAsync()
    {
        await LoadStudents();
        if (StudentDepartment != null)
        {
            StudentDepartment = null;
        }

    }
    public async Task LoadStudents()
    {
        IsRefreshing = true;
        var database = new SchoolDatabase(databasePath);
        var studentsList = await database.GetAllStudentsAsync();
        Students = new ObservableCollection<StudentsModel>(studentsList);
        IsNoDataVisible = Students.Count == 0;
       
       
        IsRefreshing = false;
    }

    public async Task LoadStudentsByTeacher(TeachersModel teacher)
    {
        var studentsList = await GetStudentsByTeacherAsync(teacher);
        Students = new ObservableCollection<StudentsModel>(studentsList);
        IsNoDataVisible = Students.Count == 0;
    }

    public async Task<List<StudentsModel>> GetStudentsByTeacherAsync(TeachersModel teacher)
    {
        var allStudents = await App.DatabaseforSchool.GetAllStudentsAsync();
        return allStudents.Where(s => s.TeacherId == teacher.TeacherId && s.StudentYear == teacher.TeacherYear).ToList();
    }

    //Load students based on the department
    public async Task LoadStudentsByDepartment(string department)
    {
        var studentsList = await GetStudentsByDepartmentAsync(department);

        Students = new ObservableCollection<StudentsModel>(studentsList);
        IsNoDataVisible = Students.Count == 0;
    }
    public async Task<List<StudentsModel>> GetStudentsByDepartmentAsync(string selectedDepartment)
    {
        var depStudentList = await App.DatabaseforSchool.GetAllStudentsAsync();
        return depStudentList.Where(s => s.StudentDepartment == selectedDepartment).ToList();
    }
  
    //Load available teachers from the department and year
    public async Task LoadAvailableDepTeachers()
    {
        var database = new SchoolDatabase(databasePath);
        var teachersList = await database.GetAllTeachersAsync();
        AvailableDepTeachers = new ObservableCollection<TeachersModel>(teachersList);
    }
    //Load available HOD's in the database
    public async Task LoadAvailableHods()
    {
        var database = new SchoolDatabase(databasePath);
        var hodList = await database.GetAllHodsAsync();
        AvailableHods = new ObservableCollection<HodsModel>(hodList);
    }
    public async Task LoadAvailableTeachersAsync()
    {
        var database = new SchoolDatabase(databasePath);
        var teachersList = await database.GetAllTeachersAsync();
        AvailableTeachers = new ObservableCollection<TeachersModel>(teachersList);    
    }
    public Task<bool> IsRollIdAvailableAsync(string rollId)
    {
        return Task.FromResult(Students.All(s => s.StudentId != rollId));
    }
    public async Task<List<TeachersModel>> GetTeachersByYearAndDepartmentAsync(string selectedStudentYear, string department)
    {
        var database = new SchoolDatabase(databasePath);
        var allTeachers = await database.GetAllTeachersAsync();

        return allTeachers.Where(t => t.TeacherYear == selectedStudentYear && t.TeacherDepartment == department).ToList();
    }

    public Task LoadAllDepartmentsStudents()
    {
        var alldepartmentstudents = GetStudentsByDepartmentAsync(StudentDepartment);
        return alldepartmentstudents;
    }

    public async Task AddStudentAsync(StudentsModel newStudent)
    {
        string generatedRollId = await GenerateRollIdAsync(newStudent.StudentDepartment);

        var student = new StudentsModel
        {
            StudentName = newStudent.StudentName,
            StudentDepartment = newStudent.StudentDepartment,
            StudentId = generatedRollId,
            Gender = newStudent.Gender,
            StudentYear = newStudent.StudentYear,
            TeacherId = newStudent.TeacherId,
            HodName = newStudent.HodName
        };

        await App.DatabaseforSchool.SaveStudentAsync(student);
        Students.Add(student);
    }
    private async Task<string> GenerateRollIdAsync(string department)
    {
        string rollId;

        if (!departmentOrderNumbers.ContainsKey(department))
        {
            departmentOrderNumbers[department] = 1;
        }

        int orderNumber = departmentOrderNumbers[department];

        while (Students.Any(s => s.StudentId == $"UG23{department}{orderNumber}"))
        {
            orderNumber++;
        }

        departmentOrderNumbers[department] = orderNumber + 1;
        rollId = $"UG23{department}{orderNumber}";

        return rollId;
    }


    public async Task UpdateStudentAsync(StudentsModel updatedStudent)
    {
        var database = new SchoolDatabase(databasePath);
        await database.UpdateStudentAsync(updatedStudent);
    }
    public async Task DeleteStudentAsync(StudentsModel deletedStudent)
    {
        var database = new SchoolDatabase(databasePath);
        await database.DeleteStudentAsync(deletedStudent);

        Students.Remove(deletedStudent);
        await LoadStudents();

        string deletedStudentDepartment = deletedStudent.StudentDepartment;
        if (departmentOrderNumbers.ContainsKey(deletedStudentDepartment))
        {
            departmentOrderNumbers[deletedStudentDepartment] = 1;
        }
    }
    public async Task RemoveStudentAsync(StudentsModel removeStudent, TeachersModel teacher)
    {
        var database = new SchoolDatabase(databasePath);

        removeStudent.TeacherId = null;
       
        //unmap the students in database
        await database.UpdateStudentAsync(removeStudent);

        //get the updated students list in the databse
        var studentsList = await GetStudentsByTeacherAsync(teacher);
        Students = new ObservableCollection<StudentsModel>(studentsList);

        IsNoDataVisible = Students.Count == 0;
    }
}
