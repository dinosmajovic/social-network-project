using System;
using System.Collections.Generic;

namespace SocialNetwork.API.Dtos
{
  public class PostsDto
  {
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime DatePublished { get; set; }
    public int LikesNum { get; set; }
    public int UserId { get; set; }
  }
}