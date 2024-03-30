using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yoeca.Sql
{
    public interface ISqlConnection
    {
        void Execute(ISqlCommand command);

        
        Task ExecuteAsync(ISqlCommand command);


        IEnumerable<T> ExecuteRead<T>(ISqlCommand<T> command);

        T? ExecuteSingle<T>(ISqlCommand<T> command);

        Task<T?> ExecuteSingleAsync<T>(ISqlCommand<T> command);
    }
}