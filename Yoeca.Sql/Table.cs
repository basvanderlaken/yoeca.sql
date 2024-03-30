using System.Threading.Tasks;

namespace Yoeca.Sql
{
    public static class Table
    {
        public static Task Drop<TDefinition>(ISqlConnection connection)
        {
            return DropTable.For<TDefinition>().ExecuteAsync(connection);
        }

        public static Task<bool> Has<TDefinition>(ISqlConnection connection)
        {
            return HasTable.For<TDefinition>().ExecuteCheckAsync(connection);
        }

        public static Task Create<TDefinition>(ISqlConnection connection)
        {
            return CreateTable.For<TDefinition>().ExecuteAsync(connection);
        }

        public static async Task Ensure<TDefinition>(ISqlConnection connection)
        {
            bool hasTable = await Has<TDefinition>(connection);
            if (!hasTable)
            {
                await Create<TDefinition>(connection);
            }
        }
    }
}