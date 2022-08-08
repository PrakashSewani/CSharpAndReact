using Application.Activities;
using API.Extentions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opt=>{
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            }).AddFluentValidation(configuration=>{
                configuration.RegisterValidatorsFromAssemblyContaining<Create>();
            });
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            // });

            // services.AddDbContext<DataContext>(opt =>
            // {
            //     opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            // });
            // services.AddCors(opt=>{
            //     opt.AddPolicy("CorsPolicy",policy=>{
            //         policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
            //     });
            // });
            // services.AddMediatR(typeof(List.Handler).Assembly);
            // services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            services.AddAplicationServices(configuration);
            services.AddIdentityServices(configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
