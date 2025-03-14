namespace mmService.Entities
{
    public class CandidateInterest
    {
        public int id { get; set; }
        public int candidateId { get; set; }

        public int interestedCandidateId{ get; set; }

        public byte isAccepted{ get; set; }

        public byte isRejected{ get; set; }
    }
}
