using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Yoeca.Sql
{
    public interface ISqlConnection
    {
        void Execute([NotNull] ISqlCommand command);

        [NotNull]
        Task ExecuteAsync([NotNull] ISqlCommand command);


        [NotNull, ItemNotNull]
        IEnumerable<T> ExecuteRead<T>([NotNull] ISqlCommand<T> command);

        [CanBeNull]
        T ExecuteSingle<T>([NotNull] ISqlCommand<T> command);

        [NotNull, ItemCanBeNull]
        Task<T> ExecuteSingleAsync<T>([NotNull] ISqlCommand<T> command);
    }
}