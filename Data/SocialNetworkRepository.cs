using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Dtos;
using SocialNetwork.API.Helpers;
using SocialNetwork.API.Models;

namespace SocialNetwork.API.Data
{
  public class SocialNetworkRepository : ISocialNetworkRepository
  {
    private readonly DataContext _context;

    public SocialNetworkRepository(DataContext context)
    {
      _context = context;
    }

    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public async Task<User> GetUser(int id)
    {
      var user = await _context.Users
          .Include(p => p.Photos)
          .FirstOrDefaultAsync(u => u.Id == id);

      return user;
    }


    public async Task<IEnumerable<User>> GetUsers()
    {
      var users = await _context.Users.Include(p => p.Photos).ToListAsync();

      return users;
    }

    public async Task<IEnumerable<Post>> GetPosts(int id)
    {
      var posts = await _context.Posts
          .Where(p => p.UserId == id)
          .ToListAsync();

      return posts;
    }

    public async Task<Post> GetPost(int id)
    {
      var post = await _context.Posts
          .FirstOrDefaultAsync(p => p.Id == id);

      return post;
    }

    public async Task<IEnumerable<Comment>> GetComments(int id)
    {
      var comments = await _context.PostComments.Where(c => c.PostId == id).ToListAsync();

      return comments;
    }

    public async Task<Comment> GetComment(int id)
    {
      var comment = await _context.PostComments.FirstOrDefaultAsync(c => c.Id == id);

      return comment;
    }

    public async Task<Post> GetPostFromComment(int id)
    {
      var comment = await _context.PostComments.FirstOrDefaultAsync(c => c.Id == id);

      var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == comment.PostId);

      return post;
    }

    public async Task<Like> GetLike(int postId, int likerId)
    {
      var like = await _context.PostLikes
          .Where(l => l.LikerId == likerId)
          .FirstOrDefaultAsync(l => l.PostId == postId);

      return like;
    }
    public async Task<bool> SaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Follow> GetFollow(int userId, int recipientId)
    {
      return await _context.Followers.FirstOrDefaultAsync(u => u.FollowerId == userId && u.FollowedId == recipientId);
    }

    public async Task<Event> GetEvent(int id)
    {
      return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Photo> GetPhoto(int id)
    {
      var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

      return photo;
    }

    public async Task<Message> GetMessage(int id)
    {
      return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
    {
      var messages = _context.Messages
          .Include(u => u.Sender).ThenInclude(p => p.Photos)
          .Include(u => u.Recipient).ThenInclude(p => p.Photos)
          .AsQueryable();

      switch (messageParams.MessageContainer)
      {
        case "Inbox":
          messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
          break;
        case "Outbox":
          messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
          break;
        default:
          messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.IsRead == false && u.RecipientDeleted == false);
          break;
      }

      messages = messages.OrderByDescending(d => d.MessageSent);

      return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
    {
      var messages = await _context.Messages
          .Include(u => u.Sender).ThenInclude(p => p.Photos)
          .Include(u => u.Recipient).ThenInclude(p => p.Photos)
          .Where(m => (m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId)
              || (m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false))
          .OrderByDescending(m => m.MessageSent)
          .ToListAsync();

      return messages;
    }

    public Task<Photo> GetMainPhotoForUser(int userId)
    {
        return _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
    }

    /*public async Task<IEnumerable<Follow>> GetFeed(int userId)
    {
        var users = await _context.Followers
          .Where(u => u.FollowedId == userId).Select(u => new {id = u.FollowerId}).ToListAsync();

        var posts = await _context.Posts.Where(t => users.Contains(t.UserId)).ToListAsync();

          // .ForEachAsync(f => {
          //   _context.Posts.Where(u => u.UserId == f.FollowerId)
          //   .ToListAsync();
          // }).FirstOrDefaultAsync();

        
        return users;
    }*/

    }

}