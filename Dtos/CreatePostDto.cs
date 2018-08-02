using System;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.API.Dtos
{
    public class CreatePostDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 4, ErrorMessage = "A post has to be between 1 and 1000 characters.")]
        public string Content { get; set; }
        public DateTime DatePublished { get; set; }
        public int LikesNum { get; set; }
        public int UserId { get; set; }
    }
}