using System;

namespace SocialNetwork.API.Dtos
{
    public class PostLikesDto
    {
        public int Id { get; set; }
        public DateTime DateLiked { get; set; }
        public int LikerId { get; set; }
        public int PostId { get; set; }
    }
}