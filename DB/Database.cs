using System.Data.SqlClient;

namespace DB;

public interface IDatabase
{
    int CreateSong(Song song);
    bool UpdateSong(Song song);
    bool DeleteSong(int id);
    Song ReadSong(int id);
    List<Song> SearchSong(string name);
    bool CheckExists(int id);
}

public class Database: IDatabase
{
    private readonly IDatabaseExecutor _executor;

    public Database(IDatabaseExecutor executor)
    {
        _executor = executor;
    }

    public int CreateSong(Song song)
    {
        string query = "INSERT INTO Song OUTPUT INSERTED.id VALUES (@title, @album, @year, @artist)";
        return _executor.Execute(
            query,
            cmd =>
            {
                var result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Failed to insert song and retrieve ID.");
                return Convert.ToInt32(result);
            },
            cmd =>
            {
                var parameters = cmd.Parameters;
                parameters.Add(new SqlParameter("title", song.Title));
                parameters.Add(new SqlParameter("album", song.Album));
                parameters.Add(new SqlParameter("year", song.Year));
                parameters.Add(new SqlParameter("artist", song.Artist));
            });
    }

    public Song ReadSong(int id)
    {
        string query = "SELECT * FROM Song WHERE id = @id";
        return _executor.Execute(
            query,
            cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Song(
                            id: (int)reader["id"],
                            title: (string)reader["title"],
                            album: (string)reader["album"],
                            year: (int)reader["year"],
                            artist: (string)reader["artist"]);
                    }
                }
                return new Song(0, "", "", 0, "");
            },
            cmd => cmd.Parameters.Add(new SqlParameter("id", id)));
    }

    public List<Song> SearchSong(string name)
    {
        string query = "SELECT * FROM Song WHERE title LIKE @name";
        return _executor.Execute(
            query,
            cmd =>
            {
                var songs = new List<Song>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        songs.Add(new Song(
                            id: (int)reader["id"],
                            title: (string)reader["title"],
                            album: (string)reader["album"],
                            year: (int)reader["year"],
                            artist: (string)reader["artist"]));
                    }
                }
                return songs;
            },
            cmd => cmd.Parameters.Add(new SqlParameter("name", "%" + name + "%")));
    }

    public bool UpdateSong(Song song)
    {
        string query = "UPDATE Song SET title = @title, album = @album, year = @year, artist = @artist WHERE id = @id";
        return _executor.Execute(
            query,
            cmd => cmd.ExecuteNonQuery() > 0, // cmd.ExecuteNonQuery returns 1 for true
            cmd =>
            {
                var parameters = cmd.Parameters;
                parameters.Add(new SqlParameter("id", song.Id));
                parameters.Add(new SqlParameter("title", song.Title));
                parameters.Add(new SqlParameter("album", song.Album));
                parameters.Add(new SqlParameter("year", song.Year));
                parameters.Add(new SqlParameter("artist", song.Artist));
            });
    }

    public bool DeleteSong(int id)
    {
        string query = "DELETE FROM Song WHERE id = @id";
        return _executor.Execute(
            query,
            cmd => cmd.ExecuteNonQuery() > 0,
            cmd => cmd.Parameters.Add(new SqlParameter("id", id)));
    }

    public bool CheckExists(int id)
    {
        string query = "SELECT id FROM Song WHERE id = @id";
        return _executor.Execute(
            query,
            cmd =>
            {
                using (var reader = cmd.ExecuteReader())
                {
                    return reader.Read() && (int)reader["id"] != 0;
                }
            },
            cmd => cmd.Parameters.Add(new SqlParameter("id", id)));
    }
}
