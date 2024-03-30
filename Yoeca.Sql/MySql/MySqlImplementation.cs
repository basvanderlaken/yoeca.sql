using System.Data.Common;
using MySql.Data.MySqlClient;

namespace Yoeca.Sql.MySql
{
    internal sealed class MySqlImplementation : ISqlConnection
    {
        private readonly string mConnectionString;

        public MySqlImplementation(string connectionString)
        {
            mConnectionString = connectionString;
        }

        public void Execute(ISqlCommand command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted, connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public async Task ExecuteAsync(ISqlCommand command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted, connection))
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public IEnumerable<T> ExecuteRead<T>(ISqlCommand<T> command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted, connection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var fields = new MySqlFields(reader);
                        while (reader.Read())
                        {
                            var row = command.TranslateRow(fields);

                            if (row != null)
                            {
                                yield return row;
                            }
                        }
                    }
                }
            }
        }

        public T? ExecuteSingle<T>(ISqlCommand<T> command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted, connection))
                {
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        var fields = new MySqlFields(reader);
                        while (reader.Read())
                        {
                            return command.TranslateRow(fields);
                        }
                    }
                }
            }

            return default(T);
        }

        public async Task<T?> ExecuteSingleAsync<T>(ISqlCommand<T> command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted, connection))
                {
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        var fields = new MySqlFields(reader);
                        while (reader.Read())
                        {
                            return command.TranslateRow(fields);
                        }
                    }
                }
            }

            return default(T);
        }

        private MySqlConnection Open()
        {
            var connection = new MySqlConnection(mConnectionString);

            connection.Open();
            return connection;
        }

        private sealed class MySqlFields : ISqlFields
        {
            private readonly DbDataReader m_Reader;

            public MySqlFields(DbDataReader reader)
            {
                m_Reader = reader;
            }

            public object Get(int fieldIndex)
            {
                return m_Reader[fieldIndex];
            }
        }
    }
}