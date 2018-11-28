using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MySql.Data.MySqlClient;

namespace Yoeca.Sql.MySql
{
    internal sealed class MySqlImplementation : ISqlConnection
    {
        [NotNull]
        private readonly string m_ConnectionString;

        public MySqlImplementation([NotNull] string connectionString)
        {
            m_ConnectionString = connectionString;
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

        public T ExecuteSingle<T>(ISqlCommand<T> command)
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

        public async Task<T> ExecuteSingleAsync<T>(ISqlCommand<T> command)
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

        [NotNull]
        private MySqlConnection Open()
        {
            var connection = new MySqlConnection(m_ConnectionString);

            connection.Open();
            return connection;
        }

        private sealed class MySqlFields : ISqlFields
        {
            [NotNull]
            [ItemNotNull]
            private readonly DbDataReader m_Reader;

            public MySqlFields([NotNull] [ItemNotNull] DbDataReader reader)
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