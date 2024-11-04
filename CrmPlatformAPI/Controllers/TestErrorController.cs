using CrmPlatformAPI.Data;
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
        public IActionResult GetSecret()
        {
            return Ok("Secret text");
        }

        [HttpGet("notfound")]
        public IActionResult GetNotFoundRequest()
        {
            var thing = _context.Users.Find(42);

            if (thing == null)
            {
                return NotFound();
            }

            return Ok(thing);
        }

        [HttpGet("servererror")]
        public IActionResult GetServerError()
        {
            var thing = _context.Users.Find(-1).ToString() ?? throw new Exception("error server");

            var thingToReturn = thing.ToString();

            return Ok(thingToReturn);
        }

        [HttpGet("badrequest")]
        public IActionResult GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }

    }
}
