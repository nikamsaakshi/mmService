
namespace mmService.Entities
{
    public class CandidatePhotos
    {
        public int id { get; set; }
        public int candidateId { get; set; }
        public string photoPath { get; set; }
        public DateTime uploadedAt { get; set; } = DateTime.UtcNow;
    }

    public class Docs
    {
        public IFormFile photo;
        public IFormFile biodata;
    }
    
   
}

