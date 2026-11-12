using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task47.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateController : ControllerBase
    {


        [HttpPost]
        public IActionResult Upload([FromForm] IFormFile image, [FromForm] string owner)
        {
            if (image == null || image.Length == 0 || string.IsNullOrEmpty(owner))
            {
                return BadRequest("Image and Owner are required!");
            }

            if (!image.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only JPG images are allowed!");
            }
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            Directory.CreateDirectory(uploadsFolder);

            string fileName = Path.GetFileName(image.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("File already exists!");
            }
            using (var stream = new FileStream(filePath, FileMode.CreateNew))
            {
                image.CopyTo(stream);
            }

            // Create image metadata
            var metadata = new
            {
                Owner = owner,
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now
            };

            string jsonPath = Path.ChangeExtension(filePath, ".json");

            string json = JsonSerializer.Serialize(metadata);

            System.IO.File.WriteAllText(jsonPath, json);

            return Ok("Image uploaded successfully!");
        }

    }
}