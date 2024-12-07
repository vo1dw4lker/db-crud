using Moq;
using DB;

namespace TestDb
{
    public class SongManagerTests
    {
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly SongManager _songManager;

        public SongManagerTests()
        {
            _mockDatabase = new Mock<IDatabase>();
            _songManager = new SongManager(_mockDatabase.Object);
        }

        [Fact]
        public void PrintMenu_ShouldPrintMenuOptions()
        {
            // Arrange
            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.PrintMenu();

            // Assert
            var consoleOutput = output.ToString();
            Assert.Contains("1. Add song", consoleOutput);
            Assert.Contains("2. Edit song", consoleOutput);
            Assert.Contains("6. Get all songs", consoleOutput);
        }

        [Fact]
        public void NoIdSongPrompt_ShouldReturnValidSong()
        {
            // Arrange
            var inputSequence = new Queue<string>(["Test Song", "Test Album", "2023", "Test Artist"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            // Act
            var song = _songManager.NoIdSongPrompt();

            // Assert
            Assert.Equal("Test Song", song.Title);
            Assert.Equal("Test Album", song.Album);
            Assert.Equal(2023, song.Year);
            Assert.Equal("Test Artist", song.Artist);
        }

        [Fact]
        public void IsInt_ShouldReturnTrueForValidInteger()
        {
            // Act
            bool result = _songManager.IsInt("123");
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInt_ShouldReturnFalseForInvalidInteger()
        {
            // Act
            bool result = _songManager.IsInt("abc");
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateSong_ShouldCallDatabaseAndPrintMessage()
        {
            // Arrange
            _mockDatabase.Setup(db => db.CreateSong(It.IsAny<Song>())).Returns(1);

            var inputSequence = new Queue<string>(["Test Song", "Test Album", "2023", "Test Artist"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.CreateSong();

            // Assert
            _mockDatabase.Verify(db => db.CreateSong(It.IsAny<Song>()), Times.Once);
            Assert.Contains("Song created with id 1", output.ToString());
        }

        [Fact]
        public void EditSong_ShouldUpdateSongIfExists()
        {
            // Arrange
            _mockDatabase.Setup(db => db.CheckExists(It.IsAny<int>())).Returns(true);

            var inputSequence = new Queue<string>(["1", "New Title", "New Album", "2023", "New Artist"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.EditSong();

            // Assert
            _mockDatabase.Verify(db => db.UpdateSong(It.Is<Song>(s => s.Title == "New Title")), Times.Once);
            Assert.Contains("Song updated with id 1", output.ToString());
        }

        [Fact]
        public void EditSong_ShouldNotUpdateIfSongDoesNotExist()
        {
            // Arrange
            _mockDatabase.Setup(db => db.CheckExists(It.IsAny<int>())).Returns(false);

            var inputSequence = new Queue<string>(["1"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.EditSong();

            // Assert
            _mockDatabase.Verify(db => db.UpdateSong(It.IsAny<Song>()), Times.Never);
            Assert.Contains("Song with this id not exists", output.ToString());
        }

        [Fact]
        public void DeleteSong_ShouldDeleteIfSongExists()
        {
            // Arrange
            _mockDatabase.Setup(db => db.CheckExists(It.IsAny<int>())).Returns(true);

            var inputSequence = new Queue<string>(["1"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.DeleteSong();

            // Assert
            _mockDatabase.Verify(db => db.DeleteSong(1), Times.Once);
            Assert.Contains("Deleted song id: 1", output.ToString());
        }

        [Fact]
        public void DeleteSong_ShouldNotDeleteIfSongDoesNotExist()
        {
            // Arrange
            _mockDatabase.Setup(db => db.CheckExists(It.IsAny<int>())).Returns(false);

            var inputSequence = new Queue<string>(["1"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.DeleteSong();

            // Assert
            _mockDatabase.Verify(db => db.DeleteSong(It.IsAny<int>()), Times.Never);
            Assert.Contains("Song with this id not exists", output.ToString());
        }

        [Fact]
        public void SearchSong_ShouldPrintResults()
        {
            // Arrange
            var songs = new List<Song>
            {
                new Song(1, "Song 1", "Album 1", 2020, "Artist 1"),
                new Song(2, "Song 2", "Album 2", 2021, "Artist 2")
            };
            _mockDatabase.Setup(db => db.SearchSong("test")).Returns(songs);

            var inputSequence = new Queue<string>(["test"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.SearchSong();

            // Assert
            _mockDatabase.Verify(db => db.SearchSong("test"), Times.Once);
            Assert.Contains("Song 1", output.ToString());
            Assert.Contains("Song 2", output.ToString());
        }

        [Fact]
        public void GetSong_ShouldPrintSongDetailsIfFound()
        {
            // Arrange
            var song = new Song(1, "Song 1", "Album 1", 2020, "Artist 1");
            _mockDatabase.Setup(db => db.ReadSong(1)).Returns(song);

            var inputSequence = new Queue<string>(["1"]);
            Console.SetIn(new StringReader(string.Join(Environment.NewLine, inputSequence)));

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.GetSong();

            // Assert
            _mockDatabase.Verify(db => db.ReadSong(1), Times.Once);
            Assert.Contains("Song 1", output.ToString());
        }

        [Fact]
        public void AllSongs_ShouldPrintAllSongs()
        {
            // Arrange
            var songs = new List<Song>
            {
                new Song(1, "Song 1", "Album 1", 2020, "Artist 1"),
                new Song(2, "Song 2", "Album 2", 2021, "Artist 2")
            };
            _mockDatabase.Setup(db => db.SearchSong("%")).Returns(songs);

            var output = new StringWriter();
            Console.SetOut(output);

            // Act
            _songManager.AllSongs();

            // Assert
            _mockDatabase.Verify(db => db.SearchSong("%"), Times.Once);
            Assert.Contains("Song 1", output.ToString());
            Assert.Contains("Song 2", output.ToString());
        }
    }
}