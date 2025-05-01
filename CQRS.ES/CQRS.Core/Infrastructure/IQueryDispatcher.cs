
using CQRS.Core.Queries;

namespace CQRS.Core.Infrastructure;

public interface IQueryDispatcher<TEntity>
{
    //This method will register the handler for the query
    //It will be used to register the handler for the query in the constructor of the QueryDispatcher class
    //Using normal generic method, but we have also used the Liskov Substitution Principle (LSP) to make it work with the base class
    //This method will be used to register the handler for the query in the constructor of the QueryDispatcher class

    // Note: Liskov substitution principle(LSP) version of the below method (RegisterHandler) would be to use the base class as the parameter type instead of the generic type
    // void RegisterHandler(BaseQuery query, Func<BaseQuery, Task<List<TEntity>>> handler); This would require casting the query to the appropriate type in the handler method
    void RegisterHandler<TQuery>(Func<TQuery, Task<List<TEntity>>> handler) where TQuery : BaseQuery;

    //
    //This is the method that will be called from the controller to send the query to the appropriate handler
    //It wroks based on the Liskov Substitution Principle (LSP)
    Task<List<TEntity>> SendQueryAsync(BaseQuery query);

}