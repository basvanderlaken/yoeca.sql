using JetBrains.Annotations;
using Yoeca.Sql.MySql;

namespace Yoeca.Sql
{
    /// <summary>
    /// Static class that allows creating connections.
    /// </summary>
    public static class ConnectionFactory
    {
        /// <summary>
        /// Creates a MySql based SQL connection.
        /// </summary>
        /// <param name="connectionString">The SQL connection.</param>
        /// <returns>A new SQL connection instance.</returns>
        [NotNull]
        public static ISqlConnection MySql([NotNull] string connectionString)
        {
            return new MySqlImplementation(connectionString);
        }
    }
}