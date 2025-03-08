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

            return Ok(new { token = "loggedin", email = candidate.emailId, candidateId = candidate.id });
        }

        #endregion

        #region Candidate

        [HttpPost("saveCandidate")]
        public IActionResult saveCandidate(Candidate candidate)
        {
            _dbContext.Add(candidate);
            var result = _dbContext.SaveChanges();
            return Ok(new { token = "registered", email = candidate.emailId, candidateId = candidate.id });
        }


        #endregion

        #region Candidate Profile

        [HttpGet("getCandidateProfileByCandidateId/{candidateId}")]
        public CandidateProfile getCandidateProfileByCandidateId(int candidateId)
        {
            var candidate = _dbContext?.CandidateProfile.Where(p => p.candidateId == candidateId).SingleOrDefault();
            if (candidate != null)
                return candidate;
            return new CandidateProfile();
        }

        [HttpPost("saveCandidateProfile")]
        public HttpStatusCode saveCandidateProfile([FromForm] CandidateProfile candidate)
        {
            //add logic for update also - check if already exists

            _dbContext.Add(candidate);
            var result = _dbContext.SaveChanges();

            if (result <= 0)
            {
                return HttpStatusCode.BadRequest;
            }

            if (candidate.image == null && candidate.doc == null && result > 0)
            {
                return HttpStatusCode.Created;
            }

            if (candidate.image != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var image = candidate.image;
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyToAsync(stream);
                }

                CandidatePhotos candidatePhoto = new CandidatePhotos();
                candidatePhoto.photoPath = filePath;
                candidatePhoto.candidateId = candidate.candidateId;
                candidatePhoto.uploadedAt = DateTime.Now;

                _dbContext.CandidatePhotos.Add(candidatePhoto);
                _dbContext.SaveChanges();
            }

            if (candidate.doc != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "docs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var doc = candidate.doc;
                var uniqueFileNameDoc = Guid.NewGuid().ToString() + Path.GetExtension(doc.FileName);
                var filePathDoc = Path.Combine(uploadsFolder, uniqueFileNameDoc);

                using (var stream = new FileStream(filePathDoc, FileMode.Create))
                {
                    doc.CopyToAsync(stream);
                }

                CandidatePhotos candidatePhotoDoc = new CandidatePhotos();
                candidatePhotoDoc.photoPath = filePathDoc;
                candidatePhotoDoc.candidateId = candidate.candidateId;
                candidatePhotoDoc.uploadedAt = DateTime.Now;

                _dbContext.CandidatePhotos.Add(candidatePhotoDoc);
                _dbContext.SaveChanges();
            }
            return HttpStatusCode.Created;
        }

        [HttpPost("saveCandidateProfile1")]
        public ActionResult saveCandidateProfile1([FromForm] CandidateProfileVM1 candidate)
        {

            string uniqueFileName = null;
            string uniqueFileName1 = null;
            if (candidate.doc != null && candidate.doc.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(candidate.image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    candidate.doc.CopyToAsync(stream);
                }
            }
            if (candidate.image != null && candidate.image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(candidate.doc.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    candidate.doc.CopyToAsync(stream);
                }
            }
            // Save the candidate data along with image path (here we simply return them for demonstration)
            var response = new
            {
                ImagePath = uniqueFileName != null ? $"/uploads/{uniqueFileName}" : null,
                ImagePath1 = uniqueFileName != null ? $"/uploads/{uniqueFileName1}" : null,
                Message = "Candidate registered successfully."
            };

            // In real scenario, save the data in a database

            return Ok(response);
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

        [HttpGet("getMatchingProfilesByGender/{gender}")]
        public IEnumerable<CandidateProfileWithPhotos> getMatchingProfilesByGender(string gender)
        {
            var matchingProfile = (from p in _dbContext.CandidateProfile
                                   join cp in _dbContext.CandidatePhotos
                                   on p.candidateId equals cp.candidateId
                                   where p.gender.ToLower() == gender.ToLower()
                                   select new CandidateProfileWithPhotos
                                   {
                                       firstName = p.firstName,
                                       lastName = p.lastName,
                                       photoPath = cp.photoPath,
                                       candidateId = cp.candidateId
                                   }).ToList();

            //return matchingProfile;

            var matchingProfile1 = _dbContext.CandidateProfile
                                    .Where(p => p.gender.ToLower() == gender.ToLower())
                                    .Join(_dbContext.CandidatePhotos,
                                        p => p.candidateId,
                                        cp => cp.candidateId,
                                        (p, cp) => new
                                        {
                                            CandidateProfile = p,
                                            CandidatePhoto = cp
                                        })
                                    .Distinct()
                                    .ToList();

            List<CandidateProfileWithPhotos> lstResult = new List<CandidateProfileWithPhotos>();
            foreach (var item in matchingProfile1)
            {

                lstResult.Add(
                    new CandidateProfileWithPhotos
                    {
                        candidateId = item.CandidateProfile.candidateId,
                        firstName = item.CandidateProfile.firstName,
                        lastName = item.CandidateProfile.lastName,
                        photoPath = item.CandidatePhoto.photoPath
                    });
            }

            return lstResult;
        }

        // PUT api/<CandidateController>/5
        [HttpPut("{candidateId}")]
        public ActionResult Put(int candidateId, [FromBody] CandidateProfile updatedCandidate)
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
            //existingCandidate.contactNo = !string.IsNullOrWhiteSpace(updatedCandidate.contactNo) ? updatedCandidate.contactNo : existingCandidate.contactNo;
            //existingCandidate.emailId = !string.IsNullOrWhiteSpace(updatedCandidate.emailId) ? updatedCandidate.emailId : existingCandidate.emailId;
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

        // PUT api/<CandidateController>/5
        [HttpPost("{candidateId}")]
        public ActionResult saveCandidateProfile(int candidateId, [FromBody] CandidateProfile updatedCandidate)
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

            try
            {
                _dbContext.CandidateProfile.Add(updatedCandidate);
                _dbContext.SaveChangesAsync();
                return Ok("Candidate Added successfully.");
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