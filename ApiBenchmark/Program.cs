using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

//executa teste carga ---->>> dotnet run -c Release
public class ApiBenchmark
{
    private static readonly HttpClient client = new HttpClient();

    // Lista de usuários com e-mails e senhas (dados diretamente no código)
    private static List<User> users = new List<User>
    {
        new User { Email = "admin@authserver.com", Password = "admin" },
        new User { Email = "gustavo@gmail.com", Password = "password" }
    };

    [Benchmark]
    public async Task GetUsersMe()
    {
        foreach (var user in users)
        {
            // Fazer o login e obter o token
            var loginResponse = await client.PostAsync("https://localhost:7011/api/users/login", new StringContent(
                JsonConvert.SerializeObject(new { email = user.Email, password = user.Password }),
                Encoding.UTF8,
                "application/json"
            ));

            loginResponse.EnsureSuccessStatusCode();
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
            var loginData = JsonConvert.DeserializeObject<LoginResponse>(loginResponseBody);
            var token = loginData?.Token;

            // Verificar os dados do usuário autenticado
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7011/api/users/me")
            {
                Headers =
                {
                    { "Authorization", $"Bearer {token}" }
                }
            };

            var userResponse = await client.SendAsync(requestMessage);

            userResponse.EnsureSuccessStatusCode();
            Console.WriteLine($"User data for {user.Email} retrieved successfully.");
        }
    }
}


public class User
{
    public string Email { get; set; }
    public string Password { get; set; }
}


public class LoginResponse
{
    public string Token { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ApiBenchmark>();
    }
}
