using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class TheaterService : BaseService
{
    public TheaterService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<Theater> GetAllTheaters() => Query(
        "SELECT TheatreID, TheatreName, TheatreCity, TheatreAddress, TheatreContactNumber FROM Theatre ORDER BY TheatreID",
        r => new Theater
        {
            TheatreID = r.GetInt32(0),
            TheatreName = r.GetString(1),
            TheatreCity = r.IsDBNull(2) ? string.Empty : r.GetString(2),
            TheatreAddress = r.IsDBNull(3) ? string.Empty : r.GetString(3),
            TheatreContactNumber = r.IsDBNull(4) ? string.Empty : r.GetString(4)
        });

    public void CreateTheater(Theater theater) => Execute(
        @"INSERT INTO Theatre (TheatreID, TheatreName, TheatreCity, TheatreAddress, TheatreContactNumber)
          VALUES ((SELECT NVL(MAX(TheatreID),0)+1 FROM Theatre), :name, :city, :address, :contact)",
        Param("name", theater.TheatreName),
        Param("city", theater.TheatreCity),
        Param("address", theater.TheatreAddress),
        Param("contact", theater.TheatreContactNumber));

    public void UpdateTheater(Theater theater) => Execute(
        @"UPDATE Theatre SET TheatreName=:name, TheatreCity=:city, TheatreAddress=:address,
          TheatreContactNumber=:contact WHERE TheatreID=:id",
        Param("name", theater.TheatreName),
        Param("city", theater.TheatreCity),
        Param("address", theater.TheatreAddress),
        Param("contact", theater.TheatreContactNumber),
        Param("id", theater.TheatreID));

    public void DeleteTheater(int theatreId) => Execute(
        "DELETE FROM Theatre WHERE TheatreID = :id",
        Param("id", theatreId));
}
