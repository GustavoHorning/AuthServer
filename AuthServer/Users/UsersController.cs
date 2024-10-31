using AuthServer.Users.requests;
using AuthServer.Users.responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Transactions; // Não se esqueça de adicionar o namespace


namespace AuthServer.Users
{
    [ApiController] // Define que esse controlador é uma API REST

    // Isso é equivalente a @RequestMapping("/users")
    // Define a rota base como /api/users
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _service;
        public UsersController(UsersService service)
        {
            _service = service;
        }

        // GET: /api/users
        [HttpGet]
        public ActionResult<List<UserResponse>> ListUsers([FromQuery] string? role = null)
        {
            var users = _service.FindAll(role)
                .Select(user => user.ToResponse())
                .ToList();

            return Ok(users);
        }


        // POST: /api/users
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
        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetUser(long id)
        {
            var user = _service.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user.ToResponse());
        }

    }
}
