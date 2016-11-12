using System.Threading.Tasks;
using JetBrains.Annotations;
using MySql.Data.MySqlClient;

namespace Yoeca.Sql
{
    public static class Table
    {
        [NotNull]
        public static Task Drop<TDefinition>([NotNull] MySqlConnection connection)
        {
            return DropTable.For<TDefinition>().ExecuteAsync(connection);
        }

        [NotNull]
        public static Task<bool> Has<TDefinition>([NotNull] MySqlConnection connection)
        {
            return HasTable.For<TDefinition>().ExecuteCheckAsync(connection);
        }

        [NotNull]
        public static Task Create<TDefinition>([NotNull] MySqlConnection connection)
        {
            return CreateTable.For<TDefinition>().ExecuteAsync(connection);
        }

        [NotNull]
        public static async Task Ensure<TDefinition>([NotNull] MySqlConnection connection)
        {
            bool hasTable = await Has<TDefinition>(connection);
            if (!hasTable)
            {
                await Create<TDefinition>(connection);
            }
        }
    }
}