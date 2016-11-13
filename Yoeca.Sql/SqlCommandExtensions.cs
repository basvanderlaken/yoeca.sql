using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public static class SqlCommandExtensions
    {
        public static void Execute([NotNull] this ISqlCommand command, [NotNull] ISqlConnection connection)
        {
            connection.Execute(command);
        }

        [NotNull]
        public static Task ExecuteAsync([NotNull] this ISqlCommand command, [NotNull] ISqlConnection connection)
        {
            return connection.ExecuteAsync(command);
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<T> ExecuteRead<T>(
            [NotNull] this ISqlCommand<T> command,
            [NotNull] ISqlConnection connection)
        {
            return connection.ExecuteRead(command);
        }

        public static bool ExecuteCheck(
            [NotNull] this ISqlCommand<bool> command,
            [NotNull] ISqlConnection connection)
        {
            return connection.ExecuteCheck(command);
        }

        [NotNull]
        public static Task<bool> ExecuteCheckAsync(
            [NotNull] this ISqlCommand<bool> command,
            [NotNull] ISqlConnection connection)
        {
            return connection.ExecuteCheckAsync(command);
        }

        public static bool TryExecute([NotNull] this ISqlCommand command, [NotNull] ISqlConnection connection)
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
    }
}