namespace DB;

internal class Program
{
    static void Main()
    {
        var dbExecutor = new DatabaseExecutor(
            "Server=localhost,1433;Database=csharp;User Id=SA;Password=MyPass@word"
            );
        var db = new Database(dbExecutor);
        var manager = new SongManager(db);
        
        while (true)
        {
            manager.PrintMenu();
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    manager.CreateSong();
                    break;
                case "2":
                    manager.EditSong();
                    break;
                case "3":
                    manager.DeleteSong();
                    break;
                case "4":
                    manager.SearchSong();
                    break;
                case "5":
                    manager.GetSong();
                    break;
                case "6":
                    manager.AllSongs();
                    break;
                default:
                    Console.WriteLine("Invalid input, please choose again.");
                    break;
            }
        }
    }
}
