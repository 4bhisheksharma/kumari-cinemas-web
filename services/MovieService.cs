using Oracle.ManagedDataAccess.Client;

namespace KumariCinemas.Web.Services;

public class MovieService : BaseService
{
    public MovieService(DatabaseConnHelper dbHelper) : base(dbHelper) { }

    public List<Movie> GetAllMovies() => Query(
        "SELECT MovieID, Title, Duration, Genre, Language, ReleaseDate FROM Movie ORDER BY MovieID",
        r => new Movie
        {
            MovieID = r.GetInt32(0),
            Title = r.GetString(1),
            Duration = r.IsDBNull(2) ? 0 : r.GetInt32(2),
            Genre = r.IsDBNull(3) ? string.Empty : r.GetString(3),
            Language = r.IsDBNull(4) ? string.Empty : r.GetString(4),
            ReleaseDate = r.IsDBNull(5) ? null : r.GetDateTime(5)
        });

    public void CreateMovie(Movie movie) => Execute(
        @"INSERT INTO Movie (MovieID, Title, Duration, Genre, Language, ReleaseDate)
          VALUES ((SELECT NVL(MAX(MovieID),0)+1 FROM Movie), :title, :duration, :genre, :lang, :releaseDate)",
        Param("title", movie.Title),
        Param("duration", movie.Duration),
        Param("genre", movie.Genre),
        Param("lang", movie.Language),
        Param("releaseDate", movie.ReleaseDate));

    public void UpdateMovie(Movie movie) => Execute(
        @"UPDATE Movie SET Title=:title, Duration=:duration, Genre=:genre,
          Language=:lang, ReleaseDate=:releaseDate WHERE MovieID=:id",
        Param("title", movie.Title),
        Param("duration", movie.Duration),
        Param("genre", movie.Genre),
        Param("lang", movie.Language),
        Param("releaseDate", movie.ReleaseDate),
        Param("id", movie.MovieID));

    public void DeleteMovie(int movieId) => Execute(
        "DELETE FROM Movie WHERE MovieID = :id",
        Param("id", movieId));
}
