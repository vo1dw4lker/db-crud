using Moq;
using System.Data;
using DB;

namespace TestDb
{
    public class DatabaseTests
    {
        private readonly Mock<IDatabaseExecutor> _mockExecutor;
        private readonly Database _database;

        public DatabaseTests()
        {
            _mockExecutor = new Mock<IDatabaseExecutor>();
            _database = new Database(_mockExecutor.Object);
        }
        
        [Fact]
        public void CreateSong_ShouldReturnNewSongId()
        {
            // Arrange
            var song = new Song(0, "Test Title", "Test Album", 2024, "Test Artist");
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, int>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(1);

            // Act
            int result = _database.CreateSong(song);

            // Assert
            Assert.Equal(1, result);
            _mockExecutor.Verify(e => e.Execute(
                "INSERT INTO Song OUTPUT INSERTED.id VALUES (@title, @album, @year, @artist)",
                It.IsAny<Func<IDbCommand, int>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }
        
        [Fact]
        public void ReadSong_ShouldReturnSongIfFound()
        {
            // Arrange
            int songId = 1;
            var expectedSong = new Song(songId, "Test Title", "Test Album", 2024, "Test Artist");
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, Song>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(expectedSong);

            // Act
            var result = _database.ReadSong(songId);

            // Assert
            Assert.Equal(expectedSong, result);
            _mockExecutor.Verify(e => e.Execute(
                "SELECT * FROM Song WHERE id = @id",
                It.IsAny<Func<IDbCommand, Song>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }
        
        [Fact]
        public void SearchSong_ShouldReturnMatchingSongs()
        {
            // Arrange
            string searchTerm = "Test";
            var expectedSongs = new List<Song>
            {
                new Song(1, "Test Title 1", "Test Album 1", 2024, "Test Artist 1"),
                new Song(2, "Test Title 2", "Test Album 2", 2024, "Test Artist 2")
            };

            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, List<Song>>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(expectedSongs);

            // Act
            var result = _database.SearchSong(searchTerm);

            // Assert
            Assert.Equal(expectedSongs, result);
            _mockExecutor.Verify(e => e.Execute(
                "SELECT * FROM Song WHERE title LIKE @name",
                It.IsAny<Func<IDbCommand, List<Song>>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }
        
        [Fact]
        public void UpdateSong_ShouldReturnTrueIfSuccessful()
        {
            // Arrange
            var song = new Song(1, "Updated Title", "Updated Album", 2024, "Updated Artist");
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, bool>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(true);

            // Act
            var result = _database.UpdateSong(song);

            // Assert
            Assert.True(result);
            _mockExecutor.Verify(e => e.Execute(
                "UPDATE Song SET title = @title, album = @album, year = @year, artist = @artist WHERE id = @id",
                It.IsAny<Func<IDbCommand, bool>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }
        
        [Fact]
        public void DeleteSong_ShouldReturnTrueIfSuccessful()
        {
            // Arrange
            int songId = 1;
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, bool>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(true);

            // Act
            var result = _database.DeleteSong(songId);

            // Assert
            Assert.True(result);
            _mockExecutor.Verify(e => e.Execute(
                "DELETE FROM Song WHERE id = @id",
                It.IsAny<Func<IDbCommand, bool>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }
        
        [Fact]
        public void CheckExists_ShouldReturnTrueIfRecordExists()
        {
            // Arrange
            int songId = 1;
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, bool>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(true);

            // Act
            var result = _database.CheckExists(songId);

            // Assert
            Assert.True(result);
            _mockExecutor.Verify(e => e.Execute(
                "SELECT id FROM Song WHERE id = @id",
                It.IsAny<Func<IDbCommand, bool>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once);
        }

        [Fact]
        public void CheckExists_ShouldReturnFalseIfRecordNotExists()
        {
            // Arrange
            int songId = 1;
            _mockExecutor
                .Setup(e => e.Execute(
                    It.IsAny<string>(),
                    It.IsAny<Func<IDbCommand, bool>>(),
                    It.IsAny<Action<IDbCommand>>()))
                .Returns(false);

            // Act
            var result = _database.CheckExists(songId);

            // Assert
            Assert.False(result);
            _mockExecutor.Verify(e => e.Execute(
                "SELECT id FROM Song WHERE id = @id",
                It.IsAny<Func<IDbCommand, bool>>(),
                It.IsAny<Action<IDbCommand>>()), Times.Once); 
        }
    }
}