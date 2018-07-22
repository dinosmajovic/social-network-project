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
        public DbSet<Follow> Followers {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Follow>()
                .HasKey(k => new {k.FollowerId, k.FollowedId});

            builder.Entity<Follow>()
                .HasOne(u => u.Followed)
                .WithMany(u => u.Follower)
                .HasForeignKey(u => u.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(u => u.Follower)
                .WithMany(u => u.Followed)
                .HasForeignKey(u => u.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}