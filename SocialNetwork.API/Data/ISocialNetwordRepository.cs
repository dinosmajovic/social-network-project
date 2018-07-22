using System.Collections.Generic;
using System.Threading.Tasks;
using SocialNetwork.API.Models;

namespace SocialNetwork.API.Data
{
    public interface ISocialNetworkRepository
    {
        void Add<T>(T entity) where T: class;

        void Delete<T>(T entity) where T: class;
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
    }
}