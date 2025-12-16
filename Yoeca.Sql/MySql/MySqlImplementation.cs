using System;
using System.Collections.Immutable;
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
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        public async Task ExecuteAsync(ISqlCommand command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public IEnumerable<T> ExecuteRead<T>(ISqlCommand<T> command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
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

        public async Task<ImmutableArray<T>> ExecuteReadAsync<T>(ISqlCommand<T> command)
        {
            var result = ImmutableArray.CreateBuilder<T>();
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        var fields = new MySqlFields(reader);
                        while (await reader.ReadAsync())
                        {
                            var row = command.TranslateRow(fields);

                            if (row != null)
                            {
                                result.Add(row);
                            }
                        }
                    }
                }
            }

            return result.ToImmutable ();
        }

        public T? ExecuteSingle<T>(ISqlCommand<T> command)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var connection = Open())
            {
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
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
                using (var sqlCommand = new MySqlCommand(formatted.Command, connection))
                {
                    AddParameters(sqlCommand, formatted.Parameters);
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

        private static void AddParameters(MySqlCommand command, ImmutableArray<SqlParameterValue> parameters)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Name, parameter.Value ?? DBNull.Value);
            }
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