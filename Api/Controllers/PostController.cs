using Api.Models;
using Api.Services;
using DAL.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly AttachService _attachService;

        public PostController(PostService postService, AttachService attachService)
        {
            _postService = postService;
            _attachService = attachService;
        }

        [HttpPost]
        [Authorize]
        public async Task AddPost(CreatePostModel post)
        {
            List<MetadataModel>? pictures = post.Pictures;
            //List<string> paths = new List<string>();
            Dictionary<MetadataModel, string> paths = new Dictionary<MetadataModel, string>();
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                if (pictures != null)
                {
                    foreach (var picture in pictures)
                    {
                        var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), picture.TempId.ToString()));
                        if (!tempFi.Exists)
                            throw new Exception("file not found");
                        else
                        {
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", picture.TempId.ToString());
                            paths.Add(picture, path);
                            var destFi = new FileInfo(path);
                            if (destFi.Directory != null && !destFi.Directory.Exists)
                                destFi.Directory.Create();
                            System.IO.File.Copy(tempFi.FullName, path, true);

                        }
                    }
                }
                await _postService.AddPost(userId, post, paths);
            }
            else throw new Exception("you are not authorized");
            
        }
        /*public async Task<PostModel> AddPost([FromForm] CreatePostModel post)
        {
            var res = new List<MetadataModel>();
            res = await _attachService.UploadFiles(post.Files);
            var description = post.Description;
            var newPost = new PostModel(res, description, DateTime.UtcNow);
            return newPost;
        }*/
    }
}
