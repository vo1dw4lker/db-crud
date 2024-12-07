using System.Data;
using System.Data.SqlClient;

namespace DB
{
    public interface IDatabaseExecutor
    {
        T Execute<T>(string query, Func<IDbCommand, T> operation, Action<IDbCommand>? parameterSetup = null);
    }

    public class DatabaseExecutor : IDatabaseExecutor
    {
        private readonly string _connectionString;

        public DatabaseExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public T Execute<T>(string query, Func<IDbCommand, T> operation, Action<IDbCommand>? parameterSetup = null)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    parameterSetup?.Invoke(command); 

                    return operation(command);
                }
            }
        }
    }
}