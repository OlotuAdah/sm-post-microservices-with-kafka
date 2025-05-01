

using Post.Query.Domain.Entities;
using Post.Query.Domain.Interfaces.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler(IPostRepo postRepo) : IQueryHandler
{
    private readonly IPostRepo _postRepo = postRepo;

    public async Task<List<PostEntity>> HandleAsync(FindAllPostQuery query)
    {
        return await _postRepo.ListAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        PostEntity? post = await _postRepo.GetByIdAsync(query.Id);
        return post == null ? new List<PostEntity>() : new List<PostEntity> { post };
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query)
    {
        return await _postRepo.ListByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostWithCommentsQuery query)
    {
        return await _postRepo.ListWithCommentsAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostWithLikesQuery query)
    {
        return await _postRepo.ListWithLikesAsync(query.NumberOfLikes);
    }
}