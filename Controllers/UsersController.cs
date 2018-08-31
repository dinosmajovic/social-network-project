using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.API.Data;
using System.Collections.Generic;
using SocialNetwork.API.Dtos;
using System.Security.Claims;
using System;
using Newtonsoft.Json;
using SocialNetwork.API.Models;
using SocialNetwork.API.Helpers;

namespace SocialNetwork.API.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [Route("api/[controller]")]
  public class UsersController : Controller
  {
    private readonly ISocialNetworkRepository _repo;
    private readonly IMapper _mapper;

    public UsersController(ISocialNetworkRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed()
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      var feed = await _repo.GetFeed(currentUserId);

      return Ok(feed);
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers()
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      var followers = await _repo.GetFollowers(currentUserId);

      return Ok(followers);
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowed()
    {
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      var followers = await _repo.GetFollowed(currentUserId);

      return Ok(followers);
    }

    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var eventToCreate = new Event
      {
        EventName = createEventDto.EventName,
        EventDescription = createEventDto.EventDescription,
        EventOwnerId = currentUserId,
      };

      _repo.Add(eventToCreate);

      if (await _repo.SaveAll())
        return StatusCode(201);

      throw new Exception("Failed on save.");
    }

    [HttpDelete("events/{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var e = await _repo.GetEvent(id);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var user = await _repo.GetUser(e.EventOwnerId);

      if (user.Id != currentUserId)
        return Unauthorized();

      _repo.Delete(e);

      if (await _repo.SaveAll())
        return StatusCode(200);

      throw new Exception("Failed on save.");
    }

    // GET request to get the id from the current user's token
    [HttpGet("current_user")]
    public ActionResult CurrentUser()
    {
      var currentUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      return Ok(Json(currentUser));
    }


    // GET request to get a list of users from the DB
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _repo.GetUsers();

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      return Ok(usersToReturn);
    }

    // GET request to get a detailed view of a single user by passing the user's id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);

      var userToReturn = _mapper.Map<UserForDetailedDto>(user);

      return Ok(userToReturn);
    }

    // GET request to get a list of posts from a user by passing the user's id
    [HttpGet]
    [Route("/api/[controller]/{id}/posts")]
    public async Task<IActionResult> GetPosts(int id)
    {
      var posts = await _repo.GetPosts(id);

      var postsToReturn = _mapper.Map<IEnumerable<PostsDto>>(posts);

      return Ok(postsToReturn);
    }

    [HttpPost]
    [Route("/api/[controller]/posts")]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var postToCreate = new Post
      {
        Content = createPostDto.Content,
        UserId = currentUserId,
        DatePublished = DateTime.UtcNow,
        LikesNum = createPostDto.LikesNum
      };

      _repo.Add(postToCreate);

      if (await _repo.SaveAll())
        return StatusCode(201);

      throw new Exception("Failed on save.");
    }

    // GET request to get a list of comments for a specific post by passing the post's id
    [HttpGet]
    [Route("/api/posts/{id}/comments")]
    public async Task<IActionResult> GetComments(int id)
    {
      var comments = await _repo.GetComments(id);

      var commentsToReturn = _mapper.Map<IEnumerable<PostCommentsDto>>(comments);

      return Ok(commentsToReturn);
    }

    [HttpPost]
    [Route("/api/posts/{id}/comments")]
    public async Task<IActionResult> CreateComment(int id, [FromBody] CreateCommentDto createCommentDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var commentToCreate = new Comment
      {
        CommentText = createCommentDto.CommentText,
        DateCommented = DateTime.Now,
        CommenterId = currentUserId,
        PostId = id
      };

      _repo.Add(commentToCreate);

      if (await _repo.SaveAll())
        return StatusCode(201);

      throw new Exception("Failed on save.");
    }

    [HttpDelete]
    [Route("/api/posts/comments/{id}")]

    public async Task<IActionResult> DeleteComment(int id)
    {
      var comment = await _repo.GetComment(id);
      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      if (comment == null)
        return NoContent();

      var usersPost = await _repo.GetPostFromComment(id);

      if (comment.CommenterId != currentUserId && currentUserId != usersPost.UserId)
        return Unauthorized();

      _repo.Delete(comment);

      if (await _repo.SaveAll())
        return Ok();

      throw new Exception("Failed on save.");
    }

    // GET request to get a sigle post by passing the post's id
    [HttpGet]
    [Route("/api/posts/{id}")]
    public async Task<IActionResult> GetPost(int id)
    {
      var post = await _repo.GetPost(id);

      var postToReturn = _mapper.Map<PostsDto>(post);

      return Ok(postToReturn);
    }

    [HttpDelete]
    [Route("/api/posts/{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
      var post = await _repo.GetPost(id);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      if (post == null)
        return StatusCode(204);

      if (currentUserId != post.UserId)
        return Unauthorized();

      _repo.Delete(post);

      if (await _repo.SaveAll())
        return StatusCode(200);

      throw new Exception("Failed on save.");
    }

    [HttpPut]
    [Route("/api/posts/{id}")]
    public async Task<IActionResult> EditPost(int id, [FromBody] EditPostDto editPostDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var postFromRepo = await _repo.GetPost(id);

      if (currentUserId != postFromRepo.UserId)
        return Unauthorized();

      _mapper.Map(editPostDto, postFromRepo);

      if (await _repo.SaveAll())
        return NoContent();

      throw new Exception($"Updating post {id} failed on save");
    }

    [HttpGet]
    [Route("/api/posts/{id}/likes")]
    public async Task<IActionResult> AddLike(int id)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var likeExists = await _repo.GetLike(id, currentUserId);

      if (likeExists != null)
        return StatusCode(304, "Like exists");
      else
      {
        var likeToCreate = new Like
        {
          LikerId = currentUserId,
          DateLiked = DateTime.Now,
          PostId = id
        };

        _repo.Add(likeToCreate);

        if (await _repo.SaveAll())
          return StatusCode(201);
      }

      throw new Exception("Failed on save.");
    }

    [HttpDelete]
    [Route("/api/posts/{id}/likes")]
    public async Task<IActionResult> RemoveLike(int id)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      var likeExists = await _repo.GetLike(id, currentUserId);

      if (likeExists == null)
        return StatusCode(304, "Like doesn't exist");
      else
      {
        _repo.Delete(likeExists);

        if (await _repo.SaveAll())
          return Ok();
      }

      throw new Exception("Failed on save.");
    }

    // PUT request to update an existing user by passing the user's id and a JSON object
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserForUpdateDto userForUpdateDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      var userFromRepo = await _repo.GetUser(id);

      if (userFromRepo == null)
        return NotFound($"Could not find user with an ID of {id}");

      if (currentUserId != userFromRepo.Id)
        return Unauthorized();

      _mapper.Map(userForUpdateDto, userFromRepo);

      if (await _repo.SaveAll())
        return NoContent();

      throw new Exception($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/follow/{recipientId}")]
    public async Task<IActionResult> FollowUser(int id, int recipientId)
    {
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var follow = await _repo.GetFollow(id, recipientId);

      if (follow != null)
        return BadRequest("You already followed this user");

      if (await _repo.GetUser(recipientId) == null)
        return NotFound();

      follow = new Follow
      {
        FollowerId = id,
        FollowedId = recipientId
      };

      _repo.Add<Follow>(follow);

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Failed to add user");
    }

    [HttpPost("{id}/unfollow/{recipientId}")]
    public async Task<IActionResult> UnfollowUser(int id, int recipientId)
    {
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var follow = await _repo.GetFollow(id, recipientId);

      if (follow == null)
        return BadRequest("You don't follow this user");

      if (await _repo.GetUser(recipientId) == null)
        return NotFound();

      _repo.Delete<Follow>(follow);

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Failed to unfollow user");
    }

    [HttpPost("{id}/isFollowing/{recipientId}")]
    public async Task<IActionResult> isFollowing(int id, int recipientId)
    {
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var follow = await _repo.GetFollow(id, recipientId);

      if (await _repo.GetUser(recipientId) == null)
        return NotFound();

      if (follow == null)
        return Ok(false);

      return Ok(true);
    }

  }
}