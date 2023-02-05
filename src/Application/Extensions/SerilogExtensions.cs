using Elastic.CommonSchema.Serilog;
using Elasticsearch.Net;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Application.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(CreateElasticsearchSinkOptions(configuration))
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);

        return builder;
    }

    private static ElasticsearchSinkOptions CreateElasticsearchSinkOptions(IConfiguration configuration)
    {
        return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
        {
            TypeName = null,
            AutoRegisterTemplate = true,
            IndexFormat = GetIndexFormat(configuration),
            BatchAction = ElasticOpType.Create,
            CustomFormatter = new EcsTextFormatter(),
            ModifyConnectionSettings = x => x.BasicAuthentication(configuration)
        };
    }

    private static ConnectionConfiguration BasicAuthentication(this ConnectionConfiguration connection,
        IConfiguration configuration)
    {
        return connection.BasicAuthentication(
            configuration["ElasticConfiguration:Username"],
            configuration["ElasticConfiguration:Password"]);
    }

    private static string GetIndexFormat(IConfiguration configuration)
    {
        var applicationName = configuration["ApplicationName"];
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        environmentName = environmentName?.ToLower().Replace(".", "-");
        var dateNow = $"{DateTime.UtcNow:yyyy-MM-dd}";

        return $"{applicationName}-logs-{environmentName}-{dateNow}";
    }

    public static WebApplication UseSerilog(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest;
        });

        return app;
    }
}