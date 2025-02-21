using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmService.Entities;

namespace mmService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private DBContext _dbContext;
        public ImagesController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file, int candidateId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                byte[] photoData = memoryStream.ToArray();

                var photo = new CandidatePhotos
                {
                    candidateId = candidateId,
                    photo = photoData
                };

                _dbContext.CandidatePhotos.Add(photo);
                await _dbContext.SaveChangesAsync();
            }

            return Ok(new { message = "Photo uploaded successfully" });
        }

        // ✅ API to Get Candidate Photo Path
        [HttpGet("get-photo/{candidateId}")]
        public IActionResult GetPhoto(int candidateId)
        {
            var photo = _dbContext.CandidatePhotos.FirstOrDefault(p => p.candidateId == candidateId);
            if (photo == null)
                return NotFound("Photo not found");

            return Ok(new { photo = photo });
        }

        //// ✅ API to Delete Candidate Photo
        [HttpDelete("delete-photo/{candidateId}")]
        public IActionResult DeletePhoto(int candidateId)
        {
            var photo = _dbContext.CandidatePhotos.FirstOrDefault(p => p.candidateId == candidateId);
            if (photo == null)
                return NotFound("Photo not found");

            _dbContext.CandidatePhotos.Remove(photo);
            _dbContext.SaveChanges();

            return Ok(new { message = "Photo deleted successfully" });
        }
    }
}
