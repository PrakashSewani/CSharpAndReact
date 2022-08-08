using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistance;

namespace API.Extentions
{
    public static class IdentityServiceExtentions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,IConfiguration configuration){
            services.AddIdentityCore<AppUser>(opt=>{
                opt.Password.RequireNonAlphanumeric=false;

            })
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>();

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt=>
            {
                opt.TokenValidationParameters= new TokenValidationParameters{
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey= key,
                    ValidateAudience=false,
                    ValidateIssuer=false,
                };
            });
            services.AddAuthorization(opt=>{
                opt.AddPolicy("isActivityHost",policy=>{
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler,IsHostRequirementHandler>();
            services.AddScoped<TokenService>();

            return services;
        }
    }
}