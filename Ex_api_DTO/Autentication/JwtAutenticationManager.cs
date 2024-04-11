using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ex_api_DTO.Autentication
{
    public class JwtAutenticationManager(string jwt)
    {
        private readonly string _jwt = jwt;

        public string Autenticate(string UserName, string Password)
        {
            var user = Users.GetUser(UserName);
            
            if (user == null || user.Password != Password)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenKey = Encoding.ASCII.GetBytes(_jwt);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.Role),
                    ]
                ),

                Expires = DateTime.UtcNow.AddHours(1),
                
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature //algoritmo di firma di tipo hashing
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // necesario per restituire il token in stringa

        }
    }
}
