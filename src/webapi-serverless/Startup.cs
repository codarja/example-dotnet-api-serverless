using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace webapi_serverless;

public class Startup
{
    #region [ Fields ]

    public readonly string ApiName;
    public readonly IConfiguration Configuration;

    #endregion

    #region [ Constructor ]

    public Startup(IWebHostEnvironment env, string apiName = "")
    {
        ApiName = apiName;

        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        Configuration = builder.Build();
    }
    #endregion

    #region [ Public Methods ]
    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();

        ConfigureSwagger(app);

        app.UseHttpsRedirection();

        app.UseRouting();
        
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        ConfigureServicesSwagger(services);
    }
    #endregion

    #region [ Private Methods ]
    

    private void ConfigureSwagger(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("./swagger/v1/swagger.json", ApiName);
            c.RoutePrefix = string.Empty;
        });
    }

    private void ConfigureServicesSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(Configuration["SwaggerInfo:Version"], new OpenApiInfo
            {
                Version = Configuration["SwaggerInfo:Version"],
                Title = Configuration["SwaggerInfo:Title"],
                Description = Configuration["SwaggerInfo:Description"],
            });

            try
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }
            catch { }
        });
    }

    #endregion
}
