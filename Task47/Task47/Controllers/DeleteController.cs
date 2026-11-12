using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

namespace Task47.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeleteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Delete(string fileName, string fileOwner)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileOwner))
            {
                return BadRequest("FileName and FileOwner are required!");
            }
            if (Path.GetFileName(fileName) != fileName)
            {
                return BadRequest("Invalid file name!");
            }
            try { 
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            string filePath = Path.Combine(uploadsFolder, fileName);

            string jsonPath = Path.ChangeExtension(filePath, ".json");

            if (!System.IO.File.Exists(filePath) || !System.IO.File.Exists(jsonPath))
            {
                return BadRequest("File not found!");
            }
            string json = System.IO.File.ReadAllText(jsonPath);

            var metadata = JsonSerializer.Deserialize<JsonElement>(json);

            string savedOwner = metadata.GetProperty("Owner").GetString();

            if (savedOwner != fileOwner)
            {
                return StatusCode(403, "You are not allowed to delete this file!");
            }

            System.IO.File.Delete(filePath);

            System.IO.File.Delete(jsonPath);

            return Ok("File deleted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}