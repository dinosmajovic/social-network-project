using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SocialNetwork.API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime DateCommented { get; set; }
        //public User User { get; set; }
        public int CommenterId { get; set; }
        //public Post Post { get; set; }
        public int PostId { get; set; }
    }
}