
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Task47.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        [HttpPost]
        public IActionResult Update([FromForm] IFormFile image, [FromForm] string owner)
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

            string fileName = Path.GetFileName(image.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);
            string jsonPath = Path.ChangeExtension(filePath, ".json");

            try
            {
                if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(jsonPath))
                {
                    return BadRequest("File does not exist!");
                }

                string json = System.IO.File.ReadAllText(jsonPath);

                var metadata = JsonSerializer.Deserialize<JsonElement>(json);

                string savedOwner = metadata.GetProperty("Owner").GetString();

                if (savedOwner != owner)
                {
                    return StatusCode(403, "You are not allowed to update this file!");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                var updatedMetadata = new
                {
                    Owner = savedOwner,
                    CreationTime = metadata.GetProperty("CreationTime").GetDateTime(),
                    LastModificationTime = DateTime.Now
                };

                string updatedJson = JsonSerializer.Serialize(updatedMetadata);

                System.IO.File.WriteAllText(jsonPath, updatedJson);

                return Ok("Image updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
