using AuthServer.Users.requests;
using AuthServer.Users.responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using System.Transactions; // Não se esqueça de adicionar o namespace
using Microsoft.IdentityModel.JsonWebTokens;



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
                return Ok(loginResponse); // Retorna o LoginResponse diretamente
            }

            return Unauthorized(); // Retorna 401 se as credenciais estiverem incorretas
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(long id)
        {
            var result = _service.Delete(id);
            if (!result)
            {
                return NotFound(); // Retorna 404 se a exclusão não for permitida
            }
            return Ok(); // Retorna 200 se a exclusão for realizada com sucesso
        }

        [HttpGet("me")]
        [Authorize] // Certifique-se de que apenas usuários autenticados possam acessar
        public IActionResult GetSelf()
        {
            // Obtém o ID do usuário do token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(); // Retorna 401 se o usuário não estiver autenticado
            }

            long userId = long.Parse(userIdClaim.Value); // Converte o ID para long
            var user = _service.GetById(userId); // Chama o método GetById para obter o usuário

            if (user == null)
            {
                return NotFound(); // Retorna 404 se o usuário não for encontrado
            }

            return Ok(user.ToResponse()); // Retorna o usuário autenticado
        }

    }
}
