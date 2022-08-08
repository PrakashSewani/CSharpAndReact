using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly TokenService tokenService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            this.tokenService = tokenService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loogindto)
        {
            var user = await userManager.FindByEmailAsync(loogindto.Email);
            if (user == null) return Unauthorized();
            var result = await signInManager.CheckPasswordSignInAsync(user, loogindto.Password, false);
            if (result.Succeeded)
            {
                return new UserDTO
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = tokenService.CreateToken(user),
                    Username = user.UserName,
                };
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register (RegisterDTO registerdto){
            if (await userManager.Users.AnyAsync(x=>x.Email==registerdto.Email )){
                return BadRequest("Email Taken");
            }
            if (await userManager.Users.AnyAsync(x=>x.UserName==registerdto.Username )){
                return BadRequest("USerNAme Taken");
            }

            var user=new AppUser{
                DisplayName=registerdto.DisplayName,
                Email=registerdto.Email,
                UserName=registerdto.Username
            };

            var result= await userManager.CreateAsync(user,registerdto.Password);

            if(result.Succeeded){
                return new UserDTO{
                    DisplayName=user.DisplayName,
                    Image=null,
                    Token=tokenService.CreateToken(user),
                    Username=user.UserName
                };
            }

            return BadRequest("Problem Registering You");
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetCurrentUser(){
            var user = await userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return createUserObject(user);            
        }

        private UserDTO createUserObject(AppUser user){
            return new UserDTO{
                    DisplayName=user.DisplayName,
                    Image=null,
                    Token=tokenService.CreateToken(user),
                    Username=user.UserName
                };
        }
    }
}