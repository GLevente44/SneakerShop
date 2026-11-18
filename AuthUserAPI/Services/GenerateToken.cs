using AuthUserAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthUserAPI.Services
{
    public class GenerateToken
    {
        public string Token(Guid id, string email, ICollection<Role> role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ide-jon-az-a-kulcs-amivel-azonositjuk-az-issuer-es-az-audience");

            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            foreach (var r in role)
            {
                claimList.Add(new Claim(ClaimTypes.Role, r.Name));
                
            }

            var tokenDesc = new SecurityTokenDescriptor
            {
                Audience = "auth-client",
                Issuer = "auth-api",
                Subject = new ClaimsIdentity(claimList),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            var token = tokenHandler.CreateToken(tokenDesc);

            return tokenHandler.WriteToken(token);

        }
    }
}
