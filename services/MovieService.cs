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
            "SELECT MovieID, Title, Duration, Genre, Language, ReleaseDate FROM Movie ORDER BY MovieID", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            movies.Add(new Movie
            {
                MovieID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Duration = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                Genre = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Language = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                ReleaseDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
            });
        }

        return movies;
    }

    public void CreateMovie(Movie movie)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            @"INSERT INTO Movie (MovieID, Title, Duration, Genre, Language, ReleaseDate) 
              VALUES ((SELECT NVL(MAX(MovieID),0)+1 FROM Movie), :title, :duration, :genre, :lang, :releaseDate)", conn);
        cmd.Parameters.Add(new OracleParameter("title", movie.Title));
        cmd.Parameters.Add(new OracleParameter("duration", movie.Duration));
        cmd.Parameters.Add(new OracleParameter("genre", movie.Genre));
        cmd.Parameters.Add(new OracleParameter("lang", movie.Language));
        cmd.Parameters.Add(new OracleParameter("releaseDate", (object?)movie.ReleaseDate ?? DBNull.Value));
        cmd.ExecuteNonQuery();
    }

    public void UpdateMovie(Movie movie)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand(
            @"UPDATE Movie SET Title = :title, Duration = :duration, Genre = :genre, 
              Language = :lang, ReleaseDate = :releaseDate WHERE MovieID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("title", movie.Title));
        cmd.Parameters.Add(new OracleParameter("duration", movie.Duration));
        cmd.Parameters.Add(new OracleParameter("genre", movie.Genre));
        cmd.Parameters.Add(new OracleParameter("lang", movie.Language));
        cmd.Parameters.Add(new OracleParameter("releaseDate", (object?)movie.ReleaseDate ?? DBNull.Value));
        cmd.Parameters.Add(new OracleParameter("id", movie.MovieID));
        cmd.ExecuteNonQuery();
    }

    public void DeleteMovie(int movieId)
    {
        using var conn = _dbHelper.GetConnection();
        conn.Open();
        using var cmd = new OracleCommand("DELETE FROM Movie WHERE MovieID = :id", conn);
        cmd.Parameters.Add(new OracleParameter("id", movieId));
        cmd.ExecuteNonQuery();
    }
}
