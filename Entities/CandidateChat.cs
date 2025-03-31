namespace mmService.Entities
{
    public class CandidateChat
    {
        public int chatId { get; set; }
        public int senderId { get; set; }

        public int reciverId { get; set; }

        public string message { get; set; }

        public enum MessageType;

       
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool  isRead { get; set; }


    }



}
public enum MessageType
{
    Text,   // 'text'
    Image,  // 'image'
    Video,  // 'video'
    File    // 'file'
}
