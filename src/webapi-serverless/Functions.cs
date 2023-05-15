using System.Net;

namespace webapi_serverless;

public class Functions : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder
            .UseStartup<Program>();
    }

    protected void Init(IHostBuilder builder)
    {
    }
}