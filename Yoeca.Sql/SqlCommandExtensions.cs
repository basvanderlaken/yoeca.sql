using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yoeca.Sql
{
    public static class SqlCommandExtensions
    {
        public static void Execute( this ISqlCommand command,  ISqlConnection connection)
        {
            connection.Execute(command);
        }

        
        public static Task ExecuteAsync( this ISqlCommand command,  ISqlConnection connection)
        {
            return connection.ExecuteAsync(command);
        }

        public static IEnumerable<T> ExecuteRead<T>(
             this ISqlCommand<T> command,
             ISqlConnection connection)
        {
            return connection.ExecuteRead(command);
        }

        public static bool ExecuteCheck(
             this ISqlCommand<bool> command,
             ISqlConnection connection)
        {
            return connection.ExecuteSingle(command);
        }

        
        public static Task<bool> ExecuteCheckAsync(
             this ISqlCommand<bool> command,
             ISqlConnection connection)
        {
            return connection.ExecuteSingleAsync(command);
        }

        public static bool TryExecute( this ISqlCommand command,  ISqlConnection connection)
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