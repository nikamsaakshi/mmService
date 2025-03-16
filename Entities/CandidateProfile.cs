using System.ComponentModel.DataAnnotations;

namespace mmService.Entities
{


    
    public class CandidateProfile
    {
        //public int id { get; set; }
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

        public string? familyDeity { get; set; }

        public string? deityRepresentation { get; set; }

        public string? constellation { get; set; }

        public string? zodiacSign { get; set; }

        public string? category { get; set; }

        public string? pulse { get; set; }

        public decimal height { get; set; }

        public decimal weight { get; set; }

        public string? complexion { get; set; }

        public string? bloodGroup { get; set; }

        public string? education { get; set; }

        public string? profession { get; set; }

        public string? annualIncome { get; set; }

        public string? property { get; set; }

        public string? familyBackground { get; set; }

        public IFormFile? image { get; set; }

        public IFormFile? doc { get; set; }
        public string? photoPath { get; set; }
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

        public string familyDeity { get; set; }

        public string deityRepresentation { get; set; }

        public string constellation{ get; set; }

        public string zodiacSign { get; set; }

        public string category { get; set; }

        public string pulse{ get; set; }

        public decimal height { get; set; }

        public decimal weight { get; set; }

        public string complexion { get; set; }

        public string bloodGRoup { get; set; }

        public string education{ get; set; }

        public string profession{ get; set; }

        public string annualIncome{ get; set; }

        public string property { get; set; }

        public string famaliybackground { get; set; }

        public IFormFile image { get; set; }

        public IFormFile doc { get; set; }
    }

    public class CandidateProfileVM1
    {
        public IFormFile image { get; set; }

        public IFormFile doc { get; set; }
    }
}
