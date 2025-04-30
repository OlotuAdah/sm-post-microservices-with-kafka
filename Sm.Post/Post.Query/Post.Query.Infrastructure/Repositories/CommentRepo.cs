using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Interfaces.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;
public class CommentRepo(ReadDatabaseContextFactory readDatabaseContextFactory) : ICommentRepo
{
    private readonly ReadDatabaseContextFactory _readDatabaseContextFactory = readDatabaseContextFactory;
    public async Task CreateAsync(CommentEntity comment)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid commentId)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        var comment = context.Comments.Find(commentId);
        if (comment == null) return Task.CompletedTask;
        context.Comments.Remove(comment);
        return context.SaveChangesAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CommentId == commentId)
            .ConfigureAwait(false);




    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
    }
}
