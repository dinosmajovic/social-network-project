using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using SocialNetwork.API.Helpers;
using SocialNetwork.API.Models;

namespace SocialNetwork.API.Data
{
  public interface ISocialNetworkRepository
  {
    void Add<T>(T entity) where T : class;

    void Delete<T>(T entity) where T : class;
    Task<bool> SaveAll();

    Task<IEnumerable<User>> GetUsers();
    Task<IEnumerable<Post>> GetPosts(int id);
    Task<IEnumerable<Comment>> GetComments(int id);

    Task<Post> GetPost(int id);
    Task<Like> GetLike(int postId, int likerId);

    Task<Comment> GetComment(int id);

    Task<User> GetUser(int id);
    Task<Post> GetPostFromComment(int id);
    Task<Follow> GetFollow(int userId, int recipientId);
    Task<Photo> GetPhoto(int id);
    Task<Event> GetEvent(int id);
    Task<Message> GetMessage(int id);
    Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    Task<Photo> GetMainPhotoForUser(int userId);
    Task<IEnumerable<Post>> GetFeed(int userId);
    Task<IEnumerable<string>> GetFollowers(int userId);
    Task<IEnumerable<int>> GetFollowed(int userId);

  }
}