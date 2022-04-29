using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Movies.API.Data;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;

namespace Movies.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var displaySwaggerApi = Convert.ToBoolean(Configuration["displaySwaggerApi"]);
            if (displaySwaggerApi)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Movies Service Microservice API", Version = "v1.0" });

                    // Configure Swagger to use the xml documentation file
                    var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                    options.IncludeXmlComments(xmlFile);

                    //options.SchemaFilter<SwaggerSkipPropertyFilter>();

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme (\"Bearer O2xzhbG09BhTj98...\")."
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                         {
                           new OpenApiSecurityScheme
                             {
                                 Reference = new OpenApiReference
                                 {
                                     Type = ReferenceType.SecurityScheme,
                                     Id = "Bearer"
                                 }
                             },
                             new string[] {}
                         }
                    });
                });
            }



            services.AddDbContext<MoviesContext>(options =>
                    options.UseInMemoryDatabase("Movies"));

            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                       // options.Authority = "https://localhost:5005";
                        options.Authority = Configuration["Authority"];
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false
                        };
                    });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient", "movies_mvc_client"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.Equals("Working") || env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options => { options.SwaggerEndpoint("./v1.0/swagger.json", "Movies.API Service Microservice API."); });

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
