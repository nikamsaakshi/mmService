namespace mmService.Entities
{
    public class CandidateChatconversation
    {


        public int conversationId { get; set; }

        public int candidate1Id { get; set; }

        public int candidate2Id { get; set; }

        public int lastMessageId { get; set; }

        public DateTime lastUpdated { get; set; }
    }
}
