using FirstProject.Teachers;
using SQLite;
using System.Collections.Generic;

public class TeachersModel
{
    [PrimaryKey]
    public string TeacherId { get; set; }

    public string TeacherName { get; set; }
    public string TeacherDepartment { get; set; }
    public string Gender { get; set; }
    public string TeacherYear { get; set; }

    // Inside the TeachersModel class
    public string DisplayText => $"{TeacherName} - {TeacherId}";

    //display the name in the picker
    public string HodName { get; set; }
   
}
