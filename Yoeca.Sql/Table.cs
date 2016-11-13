using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public static class Table
    {
        [NotNull]
        public static Task Drop<TDefinition>([NotNull] ISqlConnection connection)
        {
            return DropTable.For<TDefinition>().ExecuteAsync(connection);
        }

        [NotNull]
        public static Task<bool> Has<TDefinition>([NotNull] ISqlConnection connection)
        {
            return HasTable.For<TDefinition>().ExecuteCheckAsync(connection);
        }

        [NotNull]
        public static Task Create<TDefinition>([NotNull] ISqlConnection connection)
        {
            return CreateTable.For<TDefinition>().ExecuteAsync(connection);
        }

        [NotNull]
        public static async Task Ensure<TDefinition>([NotNull] ISqlConnection connection)
        {
            bool hasTable = await Has<TDefinition>(connection);
            if (!hasTable)
            {
                await Create<TDefinition>(connection);
            }
        }
    }
}