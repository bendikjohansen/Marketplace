using Marketplace.ClassifiedAd;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var store = new DocumentStore
{
    Urls = new[] {"http://localhost:8080"},
    Database = "Marketplace",
    Conventions = new DocumentConventions
    {
        FindIdentityProperty = m => m.Name == "_databaseId"
    }
};
store.Initialize();

builder.Services
    .AddHttpClient()
    .AddScoped(_ => store.OpenAsyncSession())
    .AddScoped<IUnitOfWork, RavenDbUnitOfWork>()
    .AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>()
    .AddScoped<IUserProfileRepository, UserProfileRepository>()
    .AddScoped<UserProfileApplicationService>()
    .AddSingleton<ICurrencyLookup, CurrencyLookup>()
    .AddScoped<ClassifiedAdsApplicationService>()
    .AddScoped<PurgomalumClient>()
    .AddScoped<CheckTextForProfanity>(s => s.GetRequiredService<PurgomalumClient>().CheckForProfanity);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
