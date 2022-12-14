using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;
namespace API.Services
{
    public class TokenService
    {
        public string CreateToken(AppUser user){
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
            };

            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key"));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor=new SecurityTokenDescriptor{
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(7),
                SigningCredentials=creds
            };

            var TokenHandler=new JwtSecurityTokenHandler();

            var token=TokenHandler.CreateToken(tokenDescriptor);

            return TokenHandler.WriteToken(token);
        }
    }
}