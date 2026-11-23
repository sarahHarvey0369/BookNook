using BookNook.CatalogService.Services;
using System.Net;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<GoogleBooksService>()
    .AddStandardResilienceHandler(options =>
    {
        // Retry: prevents repeated requests when Google returns 429.
        options.Retry.ShouldHandle = args =>
        {
            if (args.Outcome.Result?.StatusCode ==
                HttpStatusCode.TooManyRequests)
            {
                return ValueTask.FromResult(false);
            }

            return ValueTask.FromResult(
                args.Outcome.Exception is HttpRequestException
                    or TimeoutRejectedException ||
                args.Outcome.Result is { } response &&
                (response.StatusCode == HttpStatusCode.RequestTimeout ||
                 (int)response.StatusCode >= 500));
        };

        // Circuit breaker: prevents 429 responses from counting toward the threshold that could open the circuit
        options.CircuitBreaker.ShouldHandle = args =>
        {
            if (args.Outcome.Result?.StatusCode ==
                HttpStatusCode.TooManyRequests)
            {
                return ValueTask.FromResult(false);
            }

            return ValueTask.FromResult(
                args.Outcome.Exception is HttpRequestException
                    or TimeoutRejectedException ||
                args.Outcome.Result is { } response &&
                (response.StatusCode == HttpStatusCode.RequestTimeout ||
                 (int)response.StatusCode >= 500));
        };
    });
builder.Services.AddHttpClient<OpenLibraryService>()
    .AddStandardResilienceHandler(options =>
    {
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(20);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(40);
    });
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
