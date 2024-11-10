using CrmPlatformAPI.Data;
using CrmPlatformAPI.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrmPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestErrorController : Controller
    {
        private readonly ApplicationDbContext? _context;

        public TestErrorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "Secret text";
        }

        [HttpGet("notfound")]
        public ActionResult<User> GetNotFoundRequest()
        {
            var thing = _context.Users.Find(42);

            if (thing == null)
            {
                return NotFound();
            }

            return thing;
        }

        [HttpGet("servererror")]
        public ActionResult<User> GetServerError()
        {
            var thing = _context.Users.Find(-1) ?? throw new Exception("error server");

            return thing;
        }

        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }

    }
}
