using System.ComponentModel.DataAnnotations;

namespace mmService.Entities
{
    public class CandidateChatMedia
    {

        public  int madiaId{ get; set; }

        public int chatId { get; set; }

        public string? filePath { get; set; }

        public FileType fileType { get; set; }

        public DateTime uploadedAt { get; set; } = DateTime.UtcNow;
    }

    public enum FileType
    {
        Text,   // 'text'
        Image,  // 'image'
        Video,  // 'video'
        File    // 'file'
    }
}
