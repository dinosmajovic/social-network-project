using SocialNetwork.API.Models;
using Microsoft.EntityFrameworkCore;

namespace SocialNetwork.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> PostComments { get; set; }
        public DbSet<Like> PostLikes {get; set;}

    }
}