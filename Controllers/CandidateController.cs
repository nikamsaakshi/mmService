using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmService.Entities;
using System.Net;
using System.Runtime.InteropServices;

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
            CandidateProfile candidateProfile = new CandidateProfile();
            if (candidate != null && candidate.candidateId > 0)
            {
                candidateProfile = _dbContext.CandidateProfile.Where(p => p.candidateId == candidate.candidateId).FirstOrDefault();
            }

            if (candidateProfile != null && candidate != null && candidate.candidateId > 0)
            {
                var result = Put(candidate.candidateId, candidate);
                return HttpStatusCode.OK;
            }
            else
            {
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
            var matchingProfiles = _dbContext.CandidateProfile
           .Where(p => p.gender.ToLower() == gender.ToLower())
           .GroupJoin(
               _dbContext.CandidatePhotos,
               p => p.candidateId,
               cp => cp.candidateId,
               (p, photos) => new
               {
                   CandidateProfile = p,
                   CandidatePhoto = photos.OrderBy(x => x.uploadedAt).FirstOrDefault()
               }
           ).ToList();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var filePath = $"{baseUrl}/images/";

            List<CandidateProfileWithPhotos> lstResult = new List<CandidateProfileWithPhotos>();
            foreach (var item in matchingProfiles)
            {
                var newFileName = Path.GetFileName(item.CandidatePhoto.photoPath);

                lstResult.Add(
                    new CandidateProfileWithPhotos
                    {
                        candidateId = item.CandidateProfile.candidateId,
                        firstName = item.CandidateProfile.firstName.Substring(0, 2) + "####",
                        lastName = item.CandidateProfile.lastName.Substring(0, 2) + "####",
                        photoPath = filePath + newFileName,
                        dob = item.CandidateProfile.DOB.ToLongDateString(),
                        villageOrCity = item.CandidateProfile.villageOrCity.Substring(0, 2) + "####",
                        district = item.CandidateProfile.district.Substring(0, 2) + "####",
                        maskedContactNumber = "#######" + (new Random()).Next(100, 1000).ToString(),
                        age = (DateTime.Today.Year - item.CandidateProfile.DOB.Year).ToString()
                    });
            }
            return lstResult;
        }


        [HttpGet("getImages")]
        public IActionResult GetImages()
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadsFolder))
            {
                return Ok(Enumerable.Empty<object>());
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var files = Directory.GetFiles(uploadsFolder)
                .Select(Path.GetFileName)
                .Select(fileName => new
                {
                    fileName,
                    filePath = $"{baseUrl}/images/{fileName}"
                })
                .ToList();

            return Ok(files);
        }


        // PUT api/<CandidateController>/5
        [HttpPut("{candidateId}")]
        public ActionResult Put(int candidateId, CandidateProfile updatedCandidate)
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