using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.API.Data;
using SocialNetwork.API.Dtos;
using SocialNetwork.API.Helpers;
using SocialNetwork.API.Models;

namespace SocialNetwork.API.Controllers
{
  [Authorize]
  [Route("api/users/{userId}/photos")]
  public class PhotosController : Controller
  {
    private readonly ISocialNetworkRepository _repo;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;

    public PhotosController(ISocialNetworkRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
    {
      _repo = repo;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;

      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
      var photoFromRepo = await _repo.GetPhoto(id);

      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

      return Ok(photo);
    }

    [HttpPost, DisableRequestSizeLimit]
    [AllowAnonymous]
    public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoDto)
    {
      var user = await _repo.GetUser(userId);

      if (user == null)
        return BadRequest("Could not find user");

      var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      if (currentUserId != user.Id)
        return Unauthorized();

      var file = photoDto.File;

      var uploadResult = new ImageUploadResult();

      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream)
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
      }
      else
      {
        return BadRequest("No image file.");
      }

      photoDto.Url = uploadResult.Uri.ToString();
      photoDto.PublicId = uploadResult.PublicId;
      //   photoDto.Width = uploadResult.Width;
      //   photoDto.Height = uploadResult.Height;

      var photo = _mapper.Map<Photo>(photoDto);
      photo.User = user;

      var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

      if (currentMainPhoto != null)
        currentMainPhoto.IsMain = false;

      photo.IsMain = true;

      user.Photos.Add(photo);

      var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);

      if (await _repo.SaveAll())
        return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);

      return BadRequest("Could not add the photo");
    }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int id)
    {
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var photoFromRepo = await _repo.GetPhoto(id);

      if (photoFromRepo == null)
        return NotFound();

      if (photoFromRepo.IsMain)
        return BadRequest("This is already the main photo");

      var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

      if (currentMainPhoto != null)
        currentMainPhoto.IsMain = false;

      photoFromRepo.IsMain = true;

      if (await _repo.SaveAll())
        return NoContent();

      return BadRequest("Could not set photo to main");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var photoFromRepo = await _repo.GetPhoto(id);

      if (photoFromRepo == null)
        return NotFound();

      var deleteParams = new DeletionParams(photoFromRepo.PublicId);

      var result = _cloudinary.Destroy(deleteParams);

      if (result.Result == "ok")
        _repo.Delete(photoFromRepo);

      if (await _repo.SaveAll())
        return Ok();

      return BadRequest("Failed to delete the photo");
    }
  }
}