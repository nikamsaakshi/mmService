
namespace mmService.Entities
{
    public class CandidatePhotos
    {
        public int photoId { get; set; }
        public int candidateId { get; set; }
        public byte[] photo { get; set; }
        public DateTime uploadedAt { get; set; } = DateTime.UtcNow;
    }
}
