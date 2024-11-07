using FluentAssertions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AuthServer.IntegrationTests
{
    public class UsersIntegrationTests
    {
        private static string _token;
        private static int _userId;
        private static readonly RestClient _client = new RestClient("https://localhost:7011/api/users");

        static UsersIntegrationTests()
        {
            // Setup antes de todos os testes
            var loginRequest = new RestRequest("/login", Method.Post);
            loginRequest.AddJsonBody(new
            {
                email = "admin@authserver.com",
                password = "admin"
            });

            var loginResponse = _client.ExecuteAsync(loginRequest).Result;

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonResponse = JObject.Parse(loginResponse.Content);

            _token = jsonResponse["token"].ToString();
            _userId = (int)jsonResponse["user"]["id"];
        }

        private RestRequest CreateRequestWithAuth(string resource)
        {
            var request = new RestRequest(resource, Method.Get);
            request.AddHeader("Authorization", $"Bearer {_token}");
            return request;
        }

        [Fact]
        public async Task GetUsers_ShouldReturnValidResponse()
        {
            var request = new RestRequest("/", Method.Get);
            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonResponse = JArray.Parse(response.Content);
            jsonResponse.Should().HaveCountGreaterThan(0);

            // Validar o esquema de JSON (semelhante ao JSON Schema Validation do RestAssured)
            ValidateJsonSchema(jsonResponse.ToString(), "get-users.json");
        }

        [Fact]
        public async Task GetMe_ShouldReturnLoggedUser()
        {
            var request = CreateRequestWithAuth("/me");
            var response = await _client.ExecuteAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonResponse = JObject.Parse(response.Content); // Alterado para JObject

            jsonResponse["id"].ToString().Should().Be(_userId.ToString());

            // Validar o esquema de JSON
            ValidateJsonSchema(response.Content, "user.json");
        }
        private void ValidateJsonSchema(string json, string schemaFile)
        {
            var schemaDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Schemas");

            // Carregar o conteúdo do schema principal
            var mainSchemaPath = Path.Combine(schemaDirectory, schemaFile);
            if (!File.Exists(mainSchemaPath))
            {
                throw new FileNotFoundException($"O arquivo de esquema não foi encontrado: {mainSchemaPath}");
            }

            var mainSchemaContent = File.ReadAllText(mainSchemaPath);

            // Substituir o $ref pelo caminho absoluto para user.json
            var userSchemaPath = Path.Combine(schemaDirectory, "user.json");
            if (!File.Exists(userSchemaPath))
            {
                throw new FileNotFoundException($"O arquivo de esquema referenciado não foi encontrado: {userSchemaPath}");
            }

            var absoluteRefPath = new Uri(userSchemaPath).AbsoluteUri;
            mainSchemaContent = mainSchemaContent.Replace("\"$ref\": \"user.json\"", $"\"$ref\": \"{absoluteRefPath}\"");

            // Configurar o resolver para garantir que ele esteja ciente do schema referenciado
            var resolver = new JSchemaPreloadedResolver();
            resolver.Add(new Uri(absoluteRefPath), File.ReadAllText(userSchemaPath));

            // Configurações de leitura de schema
            var settings = new JSchemaReaderSettings
            {
                Resolver = resolver
            };

            // Carregar o schema principal com o resolver configurado
            var schema = JSchema.Parse(mainSchemaContent, settings);

            // Parse do JSON de entrada e validação
            JToken jsonObject = schemaFile == "get-users.json" ? JArray.Parse(json) : JObject.Parse(json);
            jsonObject.Validate(schema);
        }





    }
}
