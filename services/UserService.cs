using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class UserService : BaseService
{
    public UserService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<User> GetAllUsers() => Query(
        "SELECT UserID, UserName, ContactNumber, Email, Address FROM UserTable ORDER BY UserID",
        r => new User
        {
            UserID = r.GetInt32(0),
            UserName = r.GetString(1),
            ContactNumber = r.GetString(2),
            Email = r.IsDBNull(3) ? string.Empty : r.GetString(3),
            Address = r.IsDBNull(4) ? null : r.GetString(4)
        });

    public void CreateUser(User user) => Execute(
        @"INSERT INTO UserTable (UserID, UserName, ContactNumber, Email, Address)
          VALUES ((SELECT NVL(MAX(UserID),0)+1 FROM UserTable), :name, :contact, :email, :address)",
        Param("name", user.UserName),
        Param("contact", user.ContactNumber),
        Param("email", user.Email),
        Param("address", user.Address));

    public void UpdateUser(User user) => Execute(
        "UPDATE UserTable SET UserName=:name, ContactNumber=:contact, Email=:email, Address=:address WHERE UserID=:id",
        Param("name", user.UserName),
        Param("contact", user.ContactNumber),
        Param("email", user.Email),
        Param("address", user.Address),
        Param("id", user.UserID));

    public void DeleteUser(int userId) => Execute(
        "DELETE FROM UserTable WHERE UserID = :id",
        Param("id", userId));
}
