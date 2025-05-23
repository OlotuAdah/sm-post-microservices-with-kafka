using Microsoft.EntityFrameworkCore;
using Post.Common.Entities;
using Post.Query.Domain.Interfaces.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;
public class PostRepo(ReadDatabaseContextFactory readDatabaseContextFactory) : IPostRepo
{
    private readonly ReadDatabaseContextFactory _readDatabaseContextFactory = readDatabaseContextFactory;
    public async Task CreateAsync(PostEntity post)
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();

    }

    public async Task DeleteAsync(Guid postId)
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
        PostEntity? post = await context.Posts.FindAsync(postId);
        if (post == null) return;
        context.Posts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
        PostEntity? post = await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.PostId == postId).ConfigureAwait(false);
        return post;
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
        return await context.Posts.
        AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(p => p.Author.ToLower() == author.ToLower())
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
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
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();
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
        using ReadDatabaseContext? context = _readDatabaseContextFactory.CreateDbContext();

        context.Posts.Update(post);
        await context.SaveChangesAsync().ConfigureAwait(false);
        // The above line is equivalent to the following lines:
        // ReadDatabaseContext? existingPost = await context.Posts.FindAsync(post.PostId).ConfigureAwait(false);
        // if (existingPost == null) return;
        // context.Entry(existingPost).CurrentValues.SetValues(post);
        // context.Entry(existingPost).State = EntityState.Modified;
        // await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
