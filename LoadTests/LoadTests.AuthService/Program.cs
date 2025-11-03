using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using System.Net.Http;
using System.Text;
using System.Text.Json;

Console.WriteLine("Starting load test...");

var httpClient = Http.CreateDefaultClient();

var scenario = Scenario.Create("register_user", async context =>
{
    var user = new
    {
        Username = $"user_{Guid.NewGuid():N}",
        Password = "pass123!",
        Rolename = "Users"
    };

    var json = JsonSerializer.Serialize(user);

    var request =
        Http.CreateRequest("POST", "http://localhost:5000/auth/register")
             //.WithHeader("Accept", "text/html");
             .WithHeader("Accept", "application/json")
             .WithBody(new StringContent(json, Encoding.UTF8, "application/json"));
            // .WithBody(new ByteArrayContent(new [] {1,2,3}))                        

    var response = await Http.Send(httpClient, request);// Условие: 201 Created или 409 Conflict — оба "успешны" для нас
    if (response.StatusCode == "OK" || response.StatusCode == "Created" || response.StatusCode == "Conflict")
    {
        // Возвращаем успешный результат с тем же статусом и временем
        return Response.Ok(statusCode: response.StatusCode, message: "OK");
    }
    else
    {
        // Любые другие статусы — ошибка
        return Response.Fail(statusCode: response.StatusCode, message: "Unexpected status");
    }
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.Inject(rate: 100,
                      interval: TimeSpan.FromSeconds(1),
                      during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(scenario)
    .WithReportFileName("load_test_auth_report")
    .WithReportFolder("reports")
    .Run();