using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class UserService
{
    private readonly DatabaseConnHelper _dbHelper;

    public UserService(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();

        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("SELECT * FROM UserTable", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                UserID = reader.GetInt32(0),
                UserName = reader.GetString(1),
                ContactNumber = reader.GetString(2),
                Email = reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return users;
    }

    

    
}
