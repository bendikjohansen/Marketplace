using EventStore.Client;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var client = new EventStoreClient(EventStoreClientSettings.Create(config.GetConnectionString("EventStore")));
var classifiedAdDetails = new List<ReadModels.ClassifiedAdDetails>();
var userDetails = new List<ReadModels.UserDetails>();
var documentStore = ConfigureRavenDb(config.GetSection("ravenDb"));
var getSession = () => documentStore.OpenAsyncSession();

builder.Services
    .AddHttpClient()
    .AddSingleton(client)
    .AddTransient(_ => getSession)
    .AddSingleton<Func<Guid, Task<string>>>(async userId => (await getSession.GetUserDetails(userId)).DisplayName)
    .AddSingleton<IProjection, ClassifiedAdDetailsProjection>()
    .AddSingleton<IProjection, UserDetailsProjection>()
    .AddSingleton<ProjectionManager>()
    .AddSingleton<ICheckpointStore>(context => new RavenDbCheckpointStore(getSession, "readmodels"))
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



IDocumentStore ConfigureRavenDb(IConfiguration configuration)
{
    var store = new DocumentStore
    {
        Urls = new[] {configuration["server"]},
        Database = configuration["database"]
    };
    store.Initialize();
    var record = store.Maintenance.Server.Send(
        new GetDatabaseRecordOperation(store.Database));
    if (record == null)
    {
        store.Maintenance.Server.Send(
            new CreateDatabaseOperation(new DatabaseRecord(store.Database)));
    }

    return store;
}
