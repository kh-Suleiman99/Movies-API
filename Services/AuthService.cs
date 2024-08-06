using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.Entities;
using MoviesApi.Helpers;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesApi.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt) : IAuthService
    {
        private readonly JWT _jwt = jwt.Value;

        public async Task<AuthModel> GetokenAsync(TokenRequestModel registerModel)
        {
            var authmodel = new AuthModel();

            var user = await userManager.FindByEmailAsync(registerModel.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, registerModel.Password))
            {
                authmodel.Message = "Email or password is incorrect!";
                return authmodel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roleList = await userManager.GetRolesAsync(user);

            authmodel.IsAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authmodel.Email = user.Email;
            authmodel.Username = user.UserName;
            authmodel.ExpiresOn = jwtSecurityToken.ValidTo;
            authmodel.Roles = roleList.ToList();
            

            return authmodel;
        }

        public async Task<AuthModel> RegesterAsync(RegisterModel registerModel)
        {
            if (await userManager.FindByEmailAsync(registerModel.Email) is not null)
                return new AuthModel { Message = "Email is already registerd" };
            if (await userManager.FindByNameAsync(registerModel.Username) is not null)
                return new AuthModel { Message = "Username is already registerd" };

            ApplicationUser user = new()
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                UserName = registerModel.Username,
                Email = registerModel.Email,
            };
            var result = await userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                var errors = String.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }

            await userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName
            };
        }

        public async Task<string> AddRoleAsync(AddRoleModel addRoleModel)
        {
            var user = await userManager.FindByIdAsync(addRoleModel.UserId);
            //var rule = await roleManager.FindByNameAsync(addRoleModel.RoleName);

            if (user == null || !await roleManager.RoleExistsAsync(addRoleModel.RoleName))
                return "Invalid user id or role";

            if(await userManager.IsInRoleAsync(user, addRoleModel.RoleName))
                return "User is already assigned to this role";

            var result = await userManager.AddToRoleAsync(user, addRoleModel.RoleName);

            if (result.Succeeded)
                return string.Empty;
            else
                return "Something went Wrong";

        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.LifeTimeInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
