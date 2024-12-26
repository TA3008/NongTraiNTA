using AspNetCore.Identity.Mongo;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using RauSach.Core.FrameworkModels;

namespace RauSach.Web.Startup
{
    public static class DatabaseSetup
    {
        public static IServiceCollection AddMongoDatabase(this WebApplicationBuilder builder)
        {
            var mongoUrl = builder.Configuration.GetConnectionString("DefaultConnection");
            MongoUrl url = new MongoUrl(mongoUrl);
            IMongoDatabase database = new MongoClient(url).GetDatabase(url.DatabaseName);
            builder.Services.AddSingleton(database);

            builder.Services.AddIdentityMongoDbProvider<User>(identity =>
            {
                identity.Password.RequireDigit = false;
                identity.Password.RequireLowercase = false;
                identity.Password.RequireNonAlphanumeric = false;
                identity.Password.RequireUppercase = false;
                identity.Password.RequiredLength = 1;
                identity.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                identity.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(24);
                identity.Lockout.MaxFailedAccessAttempts = 5000;
            },
                mongo =>
                {
                    mongo.ConnectionString = mongoUrl;
                }
            );

            builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                googleOptions.CallbackPath = "/google-callback";
            });
            var convention = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", convention, x => true);
            return builder.Services;
        }
    }
}
