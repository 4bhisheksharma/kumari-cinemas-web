using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class MovieService
{
    private readonly DatabaseConnHelper _dbHelper;

    public MovieService(DatabaseConnHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<Movie> GetAllMovies()
    {
        var movies = new List<Movie>();

        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "SELECT MovieID, Title, Genre, Duration, Certificate, Language, Status FROM MovieTable ORDER BY MovieID", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            movies.Add(new Movie
            {
                MovieID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Genre = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                Duration = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Certificate = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Language = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Status = reader.IsDBNull(6) ? "Active" : reader.GetString(6)
            });
        }

        return movies;
    }

    public void CreateMovie(Movie movie)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "INSERT INTO MovieTable (Title, Genre, Duration, Certificate, Language, Status) VALUES (:title, :genre, :duration, :cert, :lang, :status)", conn);
        cmd.Parameters.Add(new OracleParameter("title", movie.Title));
        cmd.Parameters.Add(new OracleParameter("genre", movie.Genre));
        cmd.Parameters.Add(new OracleParameter("duration", movie.Duration));
        cmd.Parameters.Add(new OracleParameter("cert", movie.Certificate));
        cmd.Parameters.Add(new OracleParameter("lang", movie.Language));
        cmd.Parameters.Add(new OracleParameter("status", movie.Status));
        cmd.ExecuteNonQuery();
    }

    public void UpdateMovie(Movie movie)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            "UPDATE MovieTable SET Title = :title, Genre = :genre, Duration = :duration, Certificate = :cert, Language = :lang, Status = :status WHERE MovieID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("title", movie.Title));
        cmd.Parameters.Add(new OracleParameter("genre", movie.Genre));
        cmd.Parameters.Add(new OracleParameter("duration", movie.Duration));
        cmd.Parameters.Add(new OracleParameter("cert", movie.Certificate));
        cmd.Parameters.Add(new OracleParameter("lang", movie.Language));
        cmd.Parameters.Add(new OracleParameter("status", movie.Status));
        cmd.Parameters.Add(new OracleParameter("id", movie.MovieID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteMovie(int movieId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM MovieTable WHERE MovieID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", movieId));
        cmd.ExecuteNonQuery();
    }
}
