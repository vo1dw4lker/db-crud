namespace DB;

public class SongManager
{
    private readonly IDatabase _db;

    public SongManager(IDatabase database)
    {
        _db = database;
    }
    
    public void PrintMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("\t1. Add song");
        Console.WriteLine("\t2. Edit song");
        Console.WriteLine("\t3. Delete song");
        Console.WriteLine("\t4. Search songs");
        Console.WriteLine("\t5. Get song info");
        Console.WriteLine("\t6. Get all songs");
        Console.WriteLine("Enter your choice:");
    }

    public Song NoIdSongPrompt()
    {
        Console.WriteLine("Enter: song name, album, year, artist");
        string[] info = new string[4];
        var i = 0;
        while (i < 4)
        {
            var input = Console.ReadLine();
            if (input is null || input.Trim() == "")
            {
                Console.WriteLine("Try again");
                continue;
            }

            // name check
            if (i == 0 
                && input.Length < 3) {
                Console.WriteLine("Name must be greater than 3 symbols");
                continue;
            }

            // year check
            if (i == 2)
            {
                if (!IsInt(input)) {
                    Console.WriteLine("Try again");
                    continue;
                }

                var val = Int32.Parse(input);
                if (val < 1600 || val > DateTime.Now.Year)
                {
                    Console.WriteLine("Year must be between 1600 and current");
                    continue;
                }
            }
            info[i] = input;
            i++;
        }

        var year = Int32.Parse(info[2]);

        var song = new Song(
            0,
            info[0],
            info[1],
            year,
            info[3]
        );

        return song;
    }

    public bool IsInt(object obj)
    {
        return int.TryParse(obj.ToString(), out var _);
    }

    public void CreateSong() {
        var song = NoIdSongPrompt();
        var new_id = _db.CreateSong(song);
        Console.WriteLine("Song created with id " + new_id.ToString());
    }

    public void EditSong() {
        Console.WriteLine("Enter song ID to update:");

        var toUpdate = Console.ReadLine();
        if (!IsInt(toUpdate))
        {
            Console.WriteLine("Try again");
            return;
        }
        var idToUpdate = Int32.Parse(toUpdate);

        if (!_db.CheckExists(idToUpdate))
        {
            Console.WriteLine("Song with this id not exists");
            return;
        }

        var songToEdit = NoIdSongPrompt();

        songToEdit.Id = idToUpdate;
        _db.UpdateSong(songToEdit);
        Console.WriteLine("Song updated with id " + idToUpdate);
    }

    public void DeleteSong()
    {
        Console.WriteLine("Enter song ID to delete:");
        var toDelete = Console.ReadLine();
        if (!IsInt(toDelete))
        {
            Console.WriteLine("Try again");
            return;
        }
        var idToDelete = Int32.Parse(toDelete);

        if (!_db.CheckExists(idToDelete))
        {
            Console.WriteLine("Song with this id not exists");
            return;
        }

        _db.DeleteSong(idToDelete);
        Console.WriteLine("Deleted song id: " + idToDelete);
    }

    public void SearchSong() {
        Console.WriteLine("Enter search query:");
        var query = Console.ReadLine();
        var searchResults = _db.SearchSong(query);

        if (searchResults.Count == 0)
        {
            Console.WriteLine("No songs found matching your query.");
            return;
        }

        Console.WriteLine("Search Results:");
        foreach (var found in searchResults)
        {
            Console.WriteLine(found);
        }

    }

    public void GetSong() {
        Console.WriteLine("Enter song ID to get details:");
        var toFetch = Console.ReadLine();
        if (!IsInt(toFetch))
        {
            Console.WriteLine("Try again");
            return;
        }
        var idToFetch = Int32.Parse(toFetch);
        var songDetails = _db.ReadSong(idToFetch);
        if (songDetails.Id == 0)
        {
            Console.WriteLine("Song not found");
        }
        else
        {
            Console.WriteLine(songDetails);
        }
    }

    public void AllSongs()
    {
        var allSongs = _db.SearchSong("%");
        if (allSongs.Count == 0)
        {
            Console.WriteLine("No songs found.");
        }
        else
        {
            Console.WriteLine("All Songs:");
            foreach (var found in allSongs)
            {
                Console.WriteLine(found);
            }
        }
    }
}