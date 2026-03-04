using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Oracle.ManagedDataAccess.Client;

public class UserList : PageModel
{
    private readonly DatabaseConnHelper _dbHelper;
    public List<User> Users { get; set; } = new();

    public UserList(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public void OnGet()
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("SELECT * FROM UserTable", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Users.Add(new User
            {
                UserID = reader.GetInt32(0),
                UserName = reader.GetString(1),
                ContactNumber = reader.GetString(2),
                Email = reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }
    }
}