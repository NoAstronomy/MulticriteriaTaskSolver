using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MulticriteriaTask.Solver.Db;
using MulticriteriaTasks.Solver.Services.Interfaces;
using MulticriteriaTasks.Solver.Services.Services;
using System.Reflection;

namespace MulticriteriaTask.Solver.Api
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
            var assemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetEntryAssembly(),
                Assembly.GetAssembly(typeof(MulticriteriaTaskSolverDbContext))
            };
            services.AddMediatR(assemblies);
            services.AddAutoMapper(assemblies);
            services.AddMvc();
            services.AddControllers();

            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(assemblies));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MulticriteriaTask.Solver.Api", Version = "v1" });

                c.CustomSchemaIds(x => x.FullName);
            });
            services.AddDbContext<MulticriteriaTaskSolverDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Database"));
            });

            services.AddTransient<IMulticriteriaTaskService, MulticriteriaTaskService>();
            services.AddTransient<ICalculationService, CalculationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MulticriteriaTaskSolverDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MulticriteriaTask.Solver.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbContext.Database.Migrate();
        }
    }
}