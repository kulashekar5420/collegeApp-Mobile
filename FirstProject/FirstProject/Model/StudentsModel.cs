using SQLite;

public class StudentsModel
{
    [PrimaryKey]
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public string StudentDepartment { get; set; }
    public string StudentYear { get; set; }
    public string Gender { get; set; }
    public string StudentState {  get; set; }

    //get hod for mapping 
    public string HodName { get; set; }

    //Teachername for student mapping 
    public string TeacherName { get; set; }
    public string TeacherId {  get; set; }
  
}

public class StateModel
{
    public string Abbreviation { get; set; }
    public string Name { get; set; }
}


