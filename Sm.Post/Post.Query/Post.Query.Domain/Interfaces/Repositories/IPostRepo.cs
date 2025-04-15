using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Interfaces.Repositories;
public interface IPostRepo
{
    Task<PostEntity> GetByIdAsync(Guid postId);
    Task<List<PostEntity>> ListAllAsync();
    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes);
    Task<List<PostEntity>> ListWithCommentsAsync();
    Task CreateAsync(PostEntity post);
    Task UpdateAsync(PostEntity post);
    Task DeleteAsync(Guid postId);
}
