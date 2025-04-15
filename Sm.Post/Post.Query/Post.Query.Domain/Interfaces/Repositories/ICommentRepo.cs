using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Interfaces.Repositories;

public interface ICommentRepo
{
    Task<CommentEntity> GetByIdAsync(Guid commentId);
    Task DeleteAsync(Guid commentId);

    Task CreateAsync(CommentEntity comment);
    Task UpdateAsync(CommentEntity comment);
}