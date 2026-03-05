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
        using var cmd = new OracleCommand("SELECT UserID, UserName, ContactNumber, Email, Address FROM UserTable ORDER BY UserID", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new User
            {
                UserID = reader.GetInt32(0),
                UserName = reader.GetString(1),
                ContactNumber = reader.GetString(2),
                Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Address = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return users;
    }

    public void CreateUser(User user)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "INSERT INTO UserTable (UserName, ContactNumber, Email, Address) VALUES (:name, :contact, :email, :address)", conn);
        cmd.Parameters.Add(new OracleParameter("name", user.UserName));
        cmd.Parameters.Add(new OracleParameter("contact", user.ContactNumber));
        cmd.Parameters.Add(new OracleParameter("email", user.Email));
        cmd.Parameters.Add(new OracleParameter("address", (object?)user.Address ?? DBNull.Value));
        cmd.ExecuteNonQuery();
    }

    public void UpdateUser(User user)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "UPDATE UserTable SET UserName = :name, ContactNumber = :contact, Email = :email, Address = :address WHERE UserID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("name", user.UserName));
        cmd.Parameters.Add(new OracleParameter("contact", user.ContactNumber));
        cmd.Parameters.Add(new OracleParameter("email", user.Email));
        cmd.Parameters.Add(new OracleParameter("address", (object?)user.Address ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("id", user.UserID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteUser(int userId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM UserTable WHERE UserID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", userId));
        cmd.ExecuteNonQuery();
    }
}
