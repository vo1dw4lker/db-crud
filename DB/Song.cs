namespace DB;


public class Song
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Album { get; set; }
    public int Year { get; set; }
    public string Artist { get; set; }

    public Song(int id, string title, string album, int year, string artist)
    {
        Id = id;
        Title = title;
        Album = album;
        Year = year;
        Artist = artist;
    }

    public override string ToString()
    {
        return $"ID: {Id} | {Title} by {Artist} from the album {Album} ({Year})";
    }
}