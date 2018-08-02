using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EntityFrameworkCore.Triggers;

namespace SocialNetwork.API.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
        public int LikesNum { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<Comment> PostComments { get; set; }
        public ICollection<Like> PostLikes {get; set;}
        public Post()
        {
            PostLikes = new Collection<Like>();
            PostComments = new Collection<Comment>();
        }

        // Trigger

        /*static Post()
        {
            Triggers<Post>.Inserted += e =>
            {
                int a = 1;
            };
        }*/

    }
}