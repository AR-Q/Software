using Media.Application.MediaService.Delete.Intefrace;
using Media.Application.MediaService.Get.Interface;
using Media.Application.MediaService.Save.Interface;
using Media.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Media.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly ISave _save;
        private readonly IGet _get;
        private readonly IDelete _delete;
        private string checkUrl = "http://host.docker.internal:5000/auth/CheckToken";

        public MediaController(ISave save, IGet get, IDelete delete)
        {
            _save = save;
            _get = get;
            _delete = delete;
        }


        [HttpPost("save")]
        public async Task<ActionResult> Save(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var mediaFile = new FileVM()
            {
                FileName = file.FileName,
                Name = file.Name,
                ContentType = file.ContentType,
                Content = file.OpenReadStream()
            };

            string res = await _save.handle(mediaFile, 0 /* change to mediaId later*/);

            if (res.Equals("error"))
            {
                return BadRequest();
            }
            // TODO
            return Ok(res);
        }

        [HttpGet("GetMedia")]
        //[Authorize]
        public async Task<ActionResult> GetMedia(int mediaId,string filename)
        {
            /* string token = HttpContext.Request.Headers.Authorization;
             token = token.Split(" ")[1];
             try
             {
                 string jsonToken = JsonConvert.SerializeObject(token);
                 // create http content to send
                 HttpContent content = new StringContent(jsonToken, Encoding.UTF8, "application/json");
                 // send request using post
                 HttpResponseMessage response = await _httpClient.PostAsync(checkUrl, content);

                 if (!response.IsSuccessStatusCode)
                 {
                     return BadRequest("invalid token provided");
                 }

                 token = await response.Content.ReadAsStringAsync();
                 token = token.Remove(0, 1).Remove(token.Length - 2);
             }
             catch (Exception e)
             {
                 Console.WriteLine(e);
                 return BadRequest("wrong token");
             }*/

            FileVM? media = await _get.handle(mediaId, filename);
            if (media is null)
            {
                return NotFound("correct token or file not found");
            }
            media.Content.Position = 0;
            return File(media.Content, media.ContentType, media.FileName);
        }

        //public ActionResult Get()
        //{
        //    // TODO
        //    return null;
        //}

        //public ActionResult Delete()
        //{
        //    // TODO
        //    return null;
        //}

        //public ActionResult DeleteAll()
        //{
        //    // TODO
        //    return null;
        //}
    }
}
