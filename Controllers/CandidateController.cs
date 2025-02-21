using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmService.Entities;
using System.Net;

namespace mmService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private DBContext _dbContext;
        public CandidateController(DBContext dBContext)
        {
            _dbContext = dBContext;
        }

        #region Login

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.email) || string.IsNullOrWhiteSpace(request.password))
            {
                return BadRequest(new { message = "Email and Password are required" });
            }

            var candidate = _dbContext.Candidate
                .FirstOrDefault(c => c.emailId == request.email);

            if (candidate == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (candidate.password != request.password)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { message = "Login successful", email = candidate.emailId });
        }

        #endregion

        #region Candidate

        [HttpPost("saveCandidate")]
        public HttpStatusCode saveCandidate(Candidate candidate)
        {
            _dbContext.Add(candidate);
            var result = _dbContext.SaveChanges();
            return HttpStatusCode.Created;
        }

        #endregion

        #region Candidate Profile

        [HttpGet("getCandidates")]
        public IEnumerable<CandidateProfile> getCandidates()
        {
            var lstCandidates = _dbContext.CandidateProfile.ToList();
            return lstCandidates;
        }

        [HttpPost("saveCandidateProfile")]
        public HttpStatusCode saveCandidateProfile(CandidateProfile candidate)
        {
            _dbContext.Add(candidate);
            var result = _dbContext.SaveChanges();
            return HttpStatusCode.Created;

        }

        [HttpGet("getCandidatesBySearchCriteria")]
        public IEnumerable<CandidateProfile> getCandidatesBySearchCriteria(SearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                return _dbContext.CandidateProfile.ToList();
            }

            var lstCandidates = _dbContext.CandidateProfile.AsQueryable();

            if (!string.IsNullOrEmpty(searchCriteria.cast))
            {
                lstCandidates = lstCandidates.Where(p => p.cast == searchCriteria.cast);
            }

            if (searchCriteria.DOB != null)  // Assuming DOB is nullable DateTime? (DateTime?)
            {
                lstCandidates = lstCandidates.Where(p => p.DOB <= searchCriteria.DOB);
            }

            if (!string.IsNullOrEmpty(searchCriteria.district))
            {
                lstCandidates = lstCandidates.Where(p => p.district == searchCriteria.district);
            }

            if (searchCriteria.weight != 0)
            {
                lstCandidates = lstCandidates.Where(p => p.weight <= searchCriteria.weight);
            }

            if (searchCriteria.height != 0)
            {
                lstCandidates = lstCandidates.Where(p => p.weight >= searchCriteria.height);
            }

            if (String.IsNullOrEmpty(searchCriteria.complexion))
            {
                lstCandidates = lstCandidates.Where(p => p.complexion == searchCriteria.complexion);
            }

            return lstCandidates.ToList();
        }

        // PUT api/<CandidateController>/5
        [HttpPut("{candidateId}")]
        public ActionResult Put(int candidateId, [FromBody]CandidateProfile updatedCandidate)
        {
            if (updatedCandidate == null)
            {
                return BadRequest("Invalid candidate data.");
            }

            var existingCandidate = _dbContext.CandidateProfile.Where(p => p.candidateId == candidateId).FirstOrDefault();
            if (existingCandidate == null)
            {
                return NotFound($"Candidate with ID {candidateId} not found.");
            }

            // Update fields only if the new values are not null or empty
            existingCandidate.firstName = !string.IsNullOrWhiteSpace(updatedCandidate.firstName) ? updatedCandidate.firstName : existingCandidate.firstName;
            existingCandidate.middleName = !string.IsNullOrWhiteSpace(updatedCandidate.middleName) ? updatedCandidate.middleName : existingCandidate.middleName;
            existingCandidate.lastName = !string.IsNullOrWhiteSpace(updatedCandidate.lastName) ? updatedCandidate.lastName : existingCandidate.lastName;
            //existingCandidate.DOB = updatedCandidate.DOB ?? existingCandidate.DOB;
            existingCandidate.gender = !string.IsNullOrWhiteSpace(updatedCandidate.gender) ? updatedCandidate.gender : existingCandidate.gender;
            existingCandidate.contactNo = !string.IsNullOrWhiteSpace(updatedCandidate.contactNo) ? updatedCandidate.contactNo : existingCandidate.contactNo;
            existingCandidate.emailId = !string.IsNullOrWhiteSpace(updatedCandidate.emailId) ? updatedCandidate.emailId : existingCandidate.emailId;
            existingCandidate.addressLine1 = !string.IsNullOrWhiteSpace(updatedCandidate.addressLine1) ? updatedCandidate.addressLine1 : existingCandidate.addressLine1;
            existingCandidate.addressLine2 = !string.IsNullOrWhiteSpace(updatedCandidate.addressLine2) ? updatedCandidate.addressLine2 : existingCandidate.addressLine2;
            existingCandidate.taluka = !string.IsNullOrWhiteSpace(updatedCandidate.taluka) ? updatedCandidate.taluka : existingCandidate.taluka;
            existingCandidate.district = !string.IsNullOrWhiteSpace(updatedCandidate.district) ? updatedCandidate.district : existingCandidate.district;
            existingCandidate.pincode = updatedCandidate.pincode ?? existingCandidate.pincode;
            existingCandidate.villageOrCity = !string.IsNullOrWhiteSpace(updatedCandidate.villageOrCity) ? updatedCandidate.villageOrCity : existingCandidate.villageOrCity;
            existingCandidate.religion = !string.IsNullOrWhiteSpace(updatedCandidate.religion) ? updatedCandidate.religion : existingCandidate.religion;
            existingCandidate.cast = !string.IsNullOrWhiteSpace(updatedCandidate.cast) ? updatedCandidate.cast : existingCandidate.cast;
            existingCandidate.subCast = !string.IsNullOrWhiteSpace(updatedCandidate.subCast) ? updatedCandidate.subCast : existingCandidate.subCast;

            try
            {
                _dbContext.SaveChangesAsync();
                return Ok("Candidate updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/<CandidateController>/5
        [HttpDelete("{candidateId}")]
        public ActionResult Delete(int candidateId)
        {
            var candidate = _dbContext.CandidateProfile.Where(p => p.candidateId == candidateId).FirstOrDefault();

            if (candidate == null)
            {
                return NotFound($"Candidate with ID {candidateId} not found.");
            }

            _dbContext.CandidateProfile.Remove(candidate);

            try
            {
                _dbContext.SaveChangesAsync();
                return Ok($"Candidate with ID {candidateId} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}
