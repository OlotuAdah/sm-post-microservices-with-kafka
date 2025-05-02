
using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Common.Entities;

namespace Post.Query.Infrastructure.Dispatchers;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _queryHandlers = new();
    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
        if (_queryHandlers.ContainsKey(typeof(TQuery)))
        {
            throw new ArgumentException($"Handler for query type {typeof(TQuery).Name} is already registered.");
        }
        //Add the handler to the dictionary with the query type as the key and the handler as the value.
        //The handler is a function that takes a BaseQuery and returns a Task<List<PostEntity>>.
        //The query is cast to the concrete type TQuery when the handler is invoked.
        _queryHandlers.Add(typeof(TQuery), query => handler((TQuery)query)); // Where query is BaseQuery and TQuery is the concrete Query Object Type e.g FindPostWithCommentsQuery
    }


    public async Task<List<PostEntity>> SendQueryAsync(BaseQuery query)
    {
        if (!_queryHandlers.ContainsKey(query.GetType()))
        {
            throw new InvalidOperationException($"No query handler was registered for this query type: {query.GetType().Name}");
        }
        // Now that you're sure the handler exists, you can retrieve it and invoke it.
        Func<BaseQuery, Task<List<PostEntity>>> handler = _queryHandlers[query.GetType()];
        // Execute the handler with the query.
        return await handler(query);
    }
}