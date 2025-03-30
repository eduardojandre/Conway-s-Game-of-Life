using BoardAccess;
using BoardAccess.Models;
using BoardManager;
using Conway_s_Game_of_Life.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Conway_s_Game_of_Life
{
    internal class Startup
    {
        public readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = _configuration["ConnectionStrings:SqlLite"];

            services.AddDbContext<BoardDbContext>(options =>
                options.UseSqlite(connection)
            );
            services.AddScoped<IRepository<Board>, Repository<Board>>();
            services.AddScoped<IGenerationRepository,GenerationRepository>();
            services.AddScoped<IBoardManager, BoardManager.BoardManager>();
            services.AddAutoMapper(typeof(MapperProfile));

            var jwtConfiguration = _configuration.GetSection("JwtConfiguration").Get<Utility.JwtConfiguration>();

            if (jwtConfiguration == null)
                throw new Exception("JwtConfiguration must be configured in order to start the application.");
            var key = Encoding.ASCII.GetBytes(jwtConfiguration.Secret ?? "");
            services.AddSingleton(jwtConfiguration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfiguration.Issuer,
                        ValidAudience = jwtConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    };
                });

            services.AddRequiredScopeAuthorization();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddProblemDetails();

            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "Please enter credentials",
                    Flows = new OpenApiOAuthFlows()
                    {
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri($"{jwtConfiguration.Issuer}/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { JwtConfiguration.ReadScope, string.Empty },
                                { JwtConfiguration.WriteScope, string.Empty }
                            }
                        }
                    },

                    Scheme = "Bearer"

                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new List < string > ()
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Configure the HTTP request pipeline.
            if (environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.OAuthClientId(JwtConfiguration.AllowedClients[0].User);
                    options.OAuthClientSecret(JwtConfiguration.AllowedClients[0].Password);
                    options.OAuthScopes(new string[] { JwtConfiguration.ReadScope, JwtConfiguration.WriteScope });
                });
            }

            app.UseHttpsRedirection();


            app.UseExceptionHandler();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints((e) =>
            {
                e.MapControllers();

            });
        }
    }
}