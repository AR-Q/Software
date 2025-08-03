using IAM.Application.Common.Tokens;
using IAM.Contracts.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using IAM.Domain;
using System.IdentityModel.Tokens.Jwt;
using IAM.Application.AuthenticationService;
using IAM.Application.AuthenticationService.ViewModels;

namespace IAM.Infrastructure.Token
{
    public class JwtService : IJwtService
    {
        public string Generate(TokenVM user)
        {
            var claims = new Claim[3]
            {
                new Claim("Id",user.ID.ToString()),
                new Claim("Email",user.Email.ToString()),
                new Claim("Role",user.Role.ToString()),
            };

            var signing = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SkyDockProjectforSoftwareengin"))
                , SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                null,
                DateTime.Now.AddMinutes(60 * 24 * 15),
                signingCredentials: signing);

            String tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }

        public TokenCheckVM GetInfo(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var tokens = handler.ReadJwtToken(token);
                if (tokens is null)
                {
                    return null;
                }

                return new TokenCheckVM()
                {
                    ID = Convert.ToInt32(tokens.Claims.First(c => c.Type.Equals("Id")).Value),
                    Role = tokens.Claims.First(c => c.Type.Equals("Role")).Value,
                    Email = tokens.Claims.First(c => c.Type.Equals("Email")).Value,
                    User = null
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
