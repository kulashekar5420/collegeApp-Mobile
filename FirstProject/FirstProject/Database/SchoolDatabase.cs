using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SchoolDatabase 
{
    private readonly SQLiteAsyncConnection _database;

    public SchoolDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        InitializeTablesAsync().Wait();
    }

    private async Task InitializeTablesAsync()
    {
        await _database.CreateTableAsync<StudentsModel>().ConfigureAwait(false);
        await _database.CreateTableAsync<TeachersModel>().ConfigureAwait(false);
        await _database.CreateTableAsync<HodsModel>().ConfigureAwait(false);
    }

    // Student Module CRUD

    public async Task SaveStudentAsync(StudentsModel student)
    {
        await _database.InsertAsync(student);
    }

    public async Task<List<StudentsModel>> GetAllStudentsAsync()
    {
        return await _database.Table<StudentsModel>().ToListAsync();
    }

    public async Task<int> UpdateStudentAsync(StudentsModel student)
    {
        return await _database.UpdateAsync(student);
    }

    public async Task<int> DeleteStudentAsync(StudentsModel student)
    {
        return await _database.DeleteAsync(student);
    }

    // Teacher Module CRUD
    public async Task SaveTeachersAsync(TeachersModel teacher)
    {
        await _database.InsertAsync(teacher);
    }

    public async Task<List<TeachersModel>> GetAllTeachersAsync()
    {
        return await _database.Table<TeachersModel>().ToListAsync();
    }

    public async Task<int> UpdateTeacherAsync(TeachersModel teacher)
    {
        return await _database.UpdateAsync(teacher);
    }

    public async Task<int> DeleteTeacherAsync(TeachersModel teacher)
    {
        return await _database.DeleteAsync(teacher);
    }

    // Hod Module CRUD methods

    public async Task SaveHodsAsync(HodsModel hod)
    {
        await _database.InsertAsync(hod);
    }
     
    public async Task<List<HodsModel>> GetAllHodsAsync()
    {
        return await _database.Table<HodsModel>().ToListAsync();
    }
   
    public async Task<int> DeleteHodAsync(HodsModel hods)
    {
        return await _database.DeleteAsync(hods);
    }
    
    //No Need Now
    public async Task<int> UpdateHodAsync(HodsModel hods)
    {
        return await _database.UpdateAsync(hods);
    }
   
}
