namespace mmService.Entities
{
    public class CandidateProfileWithPhotos
    {
        public int candidateId { get; set; }
        public int interestedCandidateId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string photoPath { get; set; }
        public string dob { get; set; }
        public string villageOrCity { get; set; }
        public string district { get; set; }
        public string maskedContactNumber { get; set; }
        public string age { get; set; }
        public byte isAccepted { get; set; }
        public byte isRejected { get; set; }
    }

}
