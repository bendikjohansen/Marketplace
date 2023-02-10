using EventStore.Client;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var client = new EventStoreClient(EventStoreClientSettings.Create(config.GetConnectionString("EventStore")));
var classifiedAdDetails = new List<ReadModels.ClassifiedAdDetails>();
var userDetails = new List<ReadModels.UserDetails>();
builder.Services
    .AddHttpClient()
    .AddSingleton(client)
    .AddSingleton<Func<Guid, string?>>(_ => id => userDetails.FirstOrDefault(user => user.UserId == id)?.DisplayName)
    .AddSingleton<IProjection, ClassifiedAdDetailsProjection>()
    .AddSingleton<IProjection, UserDetailsProjection>()
    .AddSingleton<ProjectionManager>()
    .AddSingleton<IList<ReadModels.ClassifiedAdDetails>>(classifiedAdDetails)
    .AddSingleton<IList<ReadModels.UserDetails>>(userDetails)
    .AddSingleton<IAggregateStore, EsAggregateStore>()
    .AddScoped<UserProfileApplicationService>()
    .AddSingleton<ICurrencyLookup, CurrencyLookup>()
    .AddScoped<ClassifiedAdsApplicationService>()
    .AddScoped<PurgomalumClient>()
    .AddScoped<CheckTextForProfanity>(s => s.GetRequiredService<PurgomalumClient>().CheckForProfanity)
    .AddHostedService<EventStoreService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
