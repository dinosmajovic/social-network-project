using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.API.Dtos;
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
    }

}