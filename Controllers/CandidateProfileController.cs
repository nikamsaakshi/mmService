using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mmService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateProfileController : ControllerBase
    {
        private DbContext _dbContext;
        public CandidateProfileController(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Paste methods here
    }
}
