namespace mmService.Entities
{
    public class CandidateProfile
    {
        public int candidateId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string middleName { get; set; }

        public DateTime DOB { get; set; }

        public string gender { get; set; }

        //public string contactNo { get; set; }

        //public string emailId { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string taluka { get; set; }

        public string district { get; set; }

        public string pincode { get; set; }

        public string villageOrCity { get; set; }

        public string religion { get; set; }

        public string cast { get; set; }

        public string subCast { get; set; }

        public decimal height { get; set; }

        public decimal weight { get; set; }

        public string complexion { get; set; }

        public IFormFile? image { get; set; }

        public IFormFile? doc { get; set; }
    }


    public class CandidateProfileVM
    {
        public int candidateId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string middleName { get; set; }

        public DateTime DOB { get; set; }

        public string gender { get; set; }

        //public string contactNo { get; set; }

        //public string emailId { get; set; }

        public string addressLine1 { get; set; }

        public string addressLine2 { get; set; }

        public string taluka { get; set; }

        public string district { get; set; }

        public string pincode { get; set; }

        public string villageOrCity { get; set; }

        public string religion { get; set; }

        public string cast { get; set; }

        public string subCast { get; set; }

        public decimal height { get; set; }

        public decimal weight { get; set; }

        public string complexion { get; set; }

        public IFormFile image { get; set; }

        public IFormFile doc { get; set; }
    }

    public class CandidateProfileVM1
    {
        public IFormFile image { get; set; }

        public IFormFile doc { get; set; }
    }
}
