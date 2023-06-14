﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RestaurantAPI.Exceptions;
using System.IO;

namespace RestaurantAPI.Controllers
{
    [Route("file")]
    [Authorize]
    public class FileController : ControllerBase
    {
        [HttpGet]
        public ActionResult GetFile([FromQuery] string fileName)
        {
            var rootPath = Directory.GetCurrentDirectory();
            var filePath = $"{rootPath}/PrivateFiles/{fileName}";
            var fileExists = System.IO.File.Exists(filePath);
            if (!fileExists)
            {
                return NotFound();
            }
            var contentProvider = new FileExtensionContentTypeProvider();
            contentProvider.TryGetContentType(fileName, out string contentType);
            var fileContents = System.IO.File.ReadAllBytes(filePath);
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult UploadFile([FromForm] IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var rootPath = Directory.GetCurrentDirectory();
                var path = $"{rootPath}/PrivateFiles/{file.FileName}";

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok();
            }
            return BadRequest();
        }
    }
}