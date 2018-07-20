using System;

namespace SocialNetwork.API.Models
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime DateLiked { get; set; }
        public int LikerId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
    }
}