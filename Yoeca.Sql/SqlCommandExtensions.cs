using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MySql.Data.MySqlClient;

namespace Yoeca.Sql
{
    public static class SqlCommandExtensions
    {
        public static void Execute([NotNull] this ISqlCommand command, [NotNull] MySqlConnection connection)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var sqlCommand = new MySqlCommand(formatted, connection))
            {
                sqlCommand.ExecuteNonQuery();
            }
        }

        [NotNull]
        public static IEnumerable<T> ExecuteRead<T>(
            [NotNull] this ISqlCommand<T> command,
            [NotNull] MySqlConnection connection)
        {
            var formatted = command.Format(SqlFormat.MySql);
            using (var sqlCommand = new MySqlCommand(formatted, connection))
            {
                using (var reader = sqlCommand.ExecuteReader())
                {
                    var fields = new MySqlFields(reader);
                    while (reader.Read())
                    {
                        yield return command.TranslateRow(fields);
                    }
                }
            }
        }

        public static bool ExecuteCheck(
            [NotNull] this ISqlCommand<bool> command,
            [NotNull] MySqlConnection connection)
        {
            var formatted = command.Format(SqlFormat.MySql);
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

            return false;
        }

        public static bool TryExecute([NotNull] this ISqlCommand command, [NotNull] MySqlConnection connection)
        {
            try
            {
                command.Execute(connection);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private sealed class MySqlFields : ISqlFields
        {
            private readonly MySqlDataReader m_Reader;

            public MySqlFields([NotNull] MySqlDataReader reader)
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