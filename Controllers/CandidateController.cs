using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mmService.Entities;
using System.Net;
using System.Runtime.InteropServices;

using System.Linq;

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

            return Ok(new { token = "loggedin", email = candidate.emailId, candidateId = candidate.id, isPremium = candidate.isPremium });
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

        [HttpPost("saveCandidateInterest")]
        public IActionResult saveCandidateInterest(CandidateInterest candidateInterest)
        {
            _dbContext.Add(candidateInterest);
            var result = _dbContext.SaveChanges();
            return Ok("Candidate intrest send successfully!");
        }

        [HttpPost("updatePremium/{candidateId}")]
        public IActionResult updatePremium(int candidateId)
        {
            var candidate = _dbContext?.Candidate.Where(p => p.id == candidateId).SingleOrDefault();
            candidate.isPremium = 1;
            _dbContext.SaveChanges();
            return Ok("Candidate premium activated successfully!");
        }

        #endregion

        #region Candidate Profile

        [HttpGet("isProfileExists/{candidateId}")]
        public CandidateProfile isProfileExists(int candidateId)
        {
            return _dbContext?.CandidateProfile.Where(p => p.candidateId == candidateId).FirstOrDefault();
        }


        [HttpGet("getCandidateProfileByCandidateId/{candidateId}")]
        public CandidateProfile getCandidateProfileByCandidateId(int candidateId)
        {
            var candidate = _dbContext?.CandidateProfile.Where(p => p.candidateId == candidateId).SingleOrDefault();


            //new CandidateProfileWithPhotos
            //{
            //    candidateId = item.CandidateProfile.candidateId,
            //    firstName = item.CandidateProfile.firstName.Substring(0, 2) + "####",
            //    lastName = item.CandidateProfile.lastName.Substring(0, 2) + "####",
            //    photoPath = filePath + newFileName,
            //    dob = item.CandidateProfile.DOB.ToLongDateString(),
            //    villageOrCity = item.CandidateProfile.villageOrCity.Substring(0, 2) + "####",
            //    district = item.CandidateProfile.district.Substring(0, 2) + "####",
            //    maskedContactNumber = "#######" + (new Random()).Next(100, 1000).ToString(),
            //    age = (DateTime.Today.Year - item.CandidateProfile.DOB.Year).ToString()
            //});

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


        [HttpPost("saveMarriedCandidates")]
        public HttpStatusCode saveMarriedCandidates([FromForm] CandidateProfile candidate)
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
                lstCandidates = lstCandidates.Where(p => p.height >= searchCriteria.height);
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
            List<CandidateProfileWithPhotos> lstResult = new List<CandidateProfileWithPhotos>();

            gender = gender.ToLower() == "male" ? "female" : "male";

            try
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
            }
            catch (Exception ex)
            {

                throw;
            }

            return lstResult;
        }

        [HttpGet("getSentInterestMatchingProfilesByCandidateId/{candidateId}")]
        public IEnumerable<CandidateProfileWithPhotos> getSentInterestMatchingProfilesByCandidateId(int candidateId)
        {
            List<CandidateProfileWithPhotos> lstResult = new List<CandidateProfileWithPhotos>();

            try
            {
                var lstInterestSentCandidates = _dbContext.CandidateInterest.Where(p => p.candidateId == candidateId).Select(p => p.interestedCandidateId).ToList();
                //var lstInterestSentCandidates = _dbContext.CandidateInterest.Where(p => p.candidateId == candidateId).Select(p => p.candidateId).ToList();

                var matchingProfiles = _dbContext.CandidateProfile
               .Where(p => lstInterestSentCandidates.Contains(p.candidateId))
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

                var lstIntCandidatesByIntCandidateIds = _dbContext.CandidateInterest.Where(p => lstInterestSentCandidates.Contains(p.candidateId)).ToList();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var filePath = $"{baseUrl}/images/";

                foreach (var item in matchingProfiles)
                {
                    var newFileName = Path.GetFileName(item.CandidatePhoto.photoPath);
                    //var data = lstIntCandidatesByIntCandidateIds?.FirstOrDefault(p => p.interestedCandidateId == item.CandidateProfile.candidateId);
                    var data = _dbContext?.CandidateInterest.Where(p => p.interestedCandidateId == item.CandidateProfile.candidateId && p.candidateId == candidateId).FirstOrDefault();
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
                        age = (DateTime.Today.Year - item.CandidateProfile.DOB.Year).ToString(),
                        isAccepted = (byte)(data != null ? data.isAccepted : 0)
                        //isAccepted = (byte)(lstIntCandidatesByIntCandidateIds?.FirstOrDefault(p => p.interestedCandidateId == item.CandidateProfile.candidateId)?.isAccepted ?? 0)
                    });
                }
            }
            catch (Exception ex)
            {

                throw;
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
            existingCandidate.familyDeity = !string.IsNullOrWhiteSpace(updatedCandidate.familyDeity) ? updatedCandidate.familyDeity : existingCandidate.familyDeity;
            existingCandidate.deityRepresentation = !string.IsNullOrWhiteSpace(updatedCandidate.deityRepresentation) ? updatedCandidate.deityRepresentation : existingCandidate.deityRepresentation;
            existingCandidate.constellation = !string.IsNullOrWhiteSpace(updatedCandidate.constellation) ? updatedCandidate.constellation : existingCandidate.constellation;
            existingCandidate.zodiacSign = !string.IsNullOrWhiteSpace(updatedCandidate.zodiacSign) ? updatedCandidate.zodiacSign : existingCandidate.zodiacSign;
            existingCandidate.category = !string.IsNullOrWhiteSpace(updatedCandidate.category) ? updatedCandidate.category : existingCandidate.category;
            existingCandidate.pulse = !string.IsNullOrWhiteSpace(updatedCandidate.pulse) ? updatedCandidate.pulse : existingCandidate.pulse;
            existingCandidate.height = updatedCandidate.height;
            existingCandidate.weight = updatedCandidate.weight;
            existingCandidate.complexion = !string.IsNullOrWhiteSpace(updatedCandidate.complexion) ? updatedCandidate.complexion : existingCandidate.complexion;
            existingCandidate.bloodGRoup = !string.IsNullOrWhiteSpace(updatedCandidate.bloodGRoup) ? updatedCandidate.bloodGRoup : existingCandidate.bloodGRoup;
            existingCandidate.education = !string.IsNullOrWhiteSpace(updatedCandidate.education) ? updatedCandidate.education : existingCandidate.education;
            existingCandidate.profession = !string.IsNullOrWhiteSpace(updatedCandidate.profession) ? updatedCandidate.profession : existingCandidate.profession;
            existingCandidate.annualIncome = !string.IsNullOrWhiteSpace(updatedCandidate.annualIncome) ? updatedCandidate.annualIncome : existingCandidate.annualIncome;
            existingCandidate.property = !string.IsNullOrWhiteSpace(updatedCandidate.property) ? updatedCandidate.property : existingCandidate.property;
            existingCandidate.familyBackground = !string.IsNullOrWhiteSpace(updatedCandidate.familyBackground) ? updatedCandidate.familyBackground : existingCandidate.familyBackground;
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


        [HttpGet("getMarriedCouples")]
        public ActionResult getMarriedCouples()
        {

            var marriedCandidatesWithPhotos = _dbContext.MarriedCandidate
    .Join(_dbContext.CandidatePhotos,
          mc => mc.candidate1,
          cp1 => cp1.candidateId,
          (mc, cp1) => new { mc, cp1 })
    .Join(_dbContext.CandidateProfile,
          temp => temp.cp1.candidateId,
          c1 => c1.candidateId,
          (temp, c1) => new { temp.mc, temp.cp1, c1 })
    .Join(_dbContext.CandidatePhotos,
          temp => temp.mc.candidate2,
          cp2 => cp2.candidateId,
          (temp, cp2) => new { temp.mc, temp.cp1, temp.c1, cp2 })
    .Join(_dbContext.CandidateProfile,
          temp => temp.cp2.candidateId,
          c2 => c2.candidateId,
          (temp, c2) => new
          {
              MarriedDate = temp.mc.marriedDate,
              Couple = new
              {
                  Ids = $"{temp.cp1.candidateId}, {temp.cp2.candidateId}", // Merging candidate IDs
                  Names = $"{temp.c1.firstName} {temp.c1.middleName} {temp.c1.lastName} & {c2.firstName} {c2.middleName} {c2.lastName}",
                  Photos = new List<string> { temp.cp1.photoPath, temp.cp2.photoPath } // List of Photos
              }
          }).Distinct()
    .ToList();

            //        var marriedCandidatesWithPhotos = _dbContext.MarriedCandidate
            //.Join(_dbContext.CandidatePhotos,
            //      mc => mc.candidate1,
            //      cp1 => cp1.candidateId,
            //      (mc, cp1) => new { mc, cp1 })
            //.Join(_dbContext.CandidatePhotos,
            //      temp => temp.mc.candidate2,
            //      cp2 => cp2.candidateId,
            //      (temp, cp2) => new
            //      {
            //          MarriedDate = temp.mc.marriedDate,
            //          Candidate1 = new { Id = temp.cp1.candidateId, Name = "AAA" + " " + "bbbb", Photo = temp.cp1.photoPath },
            //          Candidate2 = new { Id = cp2.candidateId, Name = "ccc" + " " + "dd", Photo = cp2.photoPath }
            //      })
            //.ToList();



            //var marriedCandidatesWithPhotos =
            //(from mc in _dbContext.MarriedCandidate
            // join cp1 in _dbContext.CandidatePhotos on mc.candidate1 equals cp1.candidateId
            // join cp2 in _dbContext.CandidatePhotos on mc.candidate2 equals cp2.candidateId
            // join c1 in _dbContext.CandidateProfile on cp1.candidateId equals c1.candidateId
            // join c2 in _dbContext.CandidateProfile on cp2.candidateId equals c2.candidateId
            // select new
            // {
            //     MarriedDate = mc.marriedDate,
            //     Candidate1 = new
            //     {
            //         Id = cp1.candidateId,
            //         Name = c1.firstName + " " + (c1.middleName != null ? c1.middleName + " " : "") + c1.lastName,
            //         Photo = cp1.photoPath
            //     },
            //     Candidate2 = new
            //     {
            //         Id = cp2.candidateId,
            //         Name = c2.firstName + " " + (c2.middleName != null ? c2.middleName + " " : "") + c2.lastName,
            //         Photo = cp2.photoPath
            //     }
            // }).ToList();
            //return Ok(marriedCandidatesWithPhotos);
            var result = from mc in _dbContext.MarriedCandidate
                         join
                         cp in _dbContext.CandidatePhotos on mc.candidate2 equals cp.candidateId
                         join cpr in _dbContext.CandidateProfile on cp.candidateId equals cpr.candidateId
                         select new
                         {
                             c1Fname = cpr.firstName,
                             c1FirstName = cpr.lastName,
                             c1photoPath = cp.photoPath,

                         };
            //return result;
            return Ok(marriedCandidatesWithPhotos);
        }
        #endregion
    }
}