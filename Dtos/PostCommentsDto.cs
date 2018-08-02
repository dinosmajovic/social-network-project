using System;

namespace SocialNetwork.API.Dtos
{
    public class PostCommentsDto
    {
        //public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime DateCommented { get; set; }
        public int CommenterId { get; set; }
        //public int PostId { get; set; }
    }
}