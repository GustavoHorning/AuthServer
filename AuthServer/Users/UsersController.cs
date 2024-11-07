using AuthServer.Users.requests;
using AuthServer.Users.responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Transactions; 
using Microsoft.IdentityModel.JsonWebTokens;



namespace AuthServer.Users
{
    [ApiController] 

   
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _service;
        public UsersController(UsersService service)
        {
            _service = service;
        }

        // GET: /api/users
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<List<UserResponse>> ListUsers([FromQuery] string? role = null)
        {
            var users = _service.FindAll(role)
                .Select(user => user.ToResponse())
                .ToList();

            return Ok(users);
        }


        // POST: /api/users
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<UserResponse> CreateUser([FromBody, BindRequired] UserRequest req)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdUser = _service.Save(req).ToResponse();
                scope.Complete();

                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
        }

        // GET: /api/users/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetUser(long id)
        {
            var user = _service.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user.ToResponse());
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest credentials)
        {
            var loginResponse = _service.Login(credentials);
            if (loginResponse != null)
            {
                return Ok(loginResponse); 
            }

            return Unauthorized(); 
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(long id)
        {
            var result = _service.Delete(id);
            if (!result)
            {
                return NotFound(); 
            }
            return Ok(); 
        }

        [HttpGet("me")]
        [Authorize] 
        public IActionResult GetSelf()
        {
          
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(); 
            }

            long userId = long.Parse(userIdClaim.Value); 
            var user = _service.GetById(userId); 

            if (user == null)
            {
                return NotFound(); 
            }

            return Ok(user.ToResponse()); 
        }

    }
}
