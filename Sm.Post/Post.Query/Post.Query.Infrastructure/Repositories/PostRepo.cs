using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Interfaces.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;
public class PostRepo(ReadDatabaseContextFactory readDatabaseContextFactory) : IPostRepo
{
    private readonly ReadDatabaseContextFactory _readDatabaseContextFactory = readDatabaseContextFactory;
    public Task CreateAsync(PostEntity post)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        context.Posts.Add(post);
        return context.SaveChangesAsync();
    }

    public Task DeleteAsync(Guid postId)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        var post = context.Posts.Find(postId);
        if (post == null) return Task.CompletedTask;
        context.Posts.Remove(post);
        return context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.PostId == postId).ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.
        AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(p => p.Author.Equals(author, StringComparison.CurrentCultureIgnoreCase))
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.
        AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(p => p.Comments != null && p.Comments.Any())
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes = 1)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.
        AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(p => p.Likes >= numberOfLikes)
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using var context = _readDatabaseContextFactory.CreateDbContext();

        context.Posts.Update(post);
        await context.SaveChangesAsync().ConfigureAwait(false);
        // The above line is equivalent to the following lines:
        // var existingPost = await context.Posts.FindAsync(post.PostId).ConfigureAwait(false);
        // if (existingPost == null) return;
        // context.Entry(existingPost).CurrentValues.SetValues(post);
        // context.Entry(existingPost).State = EntityState.Modified;
        // await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
