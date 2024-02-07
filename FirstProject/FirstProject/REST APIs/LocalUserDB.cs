using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

public class LocalUserDB
{
    private readonly SQLiteAsyncConnection _localUserDB;
    public LocalUserDB()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "local_user_db.sqlite");
        _localUserDB = new SQLiteAsyncConnection(dbPath);
        InitializeDatabaseAsync().Wait();
    }
    public async Task InitializeDatabaseAsync()
    {
        if (!_localUserDB.TableMappings.Any(m => m.MappedType.Name == typeof(UserData).Name))
        {
            await _localUserDB.CreateTableAsync<UserData>().ConfigureAwait(false);
        }
    }
    public async Task SaveUserData(UserData userData)
    {
        await _localUserDB.InsertAsync(userData);
    }

    public async Task UpdateUserData(UserData userData)
    {
        await _localUserDB.UpdateAsync(userData);
    }
    public async Task<List<UserData>> GetUserData()
    {
        return await _localUserDB.Table<UserData>().ToListAsync();
    }
}
