﻿namespace FootballLeague.API.Internal
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.OpenApi.Models;

    using FootballLeague.Data;
    using FootballLeague.Data.Repositories;
    using FootballLeague.Data.Common.Repositories;
    using FootballLeague.Services;
    using FootballLeague.Services.Interfaces;
    using FootballLeague.Services.Mapping;
    using FootballLeague.Data.Models;

    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ConfigureApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add controllers
            services.AddControllers();

            // Register Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Football League API",
                    Version = "v1"
                });
            });

            services.AddHealthChecks();

            AutoMapperConfig.RegisterMappings(typeof(Team).Assembly);

            services.AddSingleton(AutoMapperConfig.MapperInstance);

            services.AddDbContext<FootballLeagueDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IMatchService, MatchService>();

            return services;
        }

        internal static IApplicationBuilder ConfigureApplicationPipeline(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable Swagger UI in development environment
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Football League API V1");
                });
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            return app;
        }
    }
}
