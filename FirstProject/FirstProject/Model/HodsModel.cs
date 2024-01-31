using SQLite;
public class HodsModel
{
    [PrimaryKey]
    public string HodId { get; set; }
    public string HodName { get; set; }
    public string HodDepartment { get; set; }
    public string HodGender { get; set; }
    public string HodTeacherId { get; set; }


    public string HodInfo => $"{HodName} - {HodId}";




}

