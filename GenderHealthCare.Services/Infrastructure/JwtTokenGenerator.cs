﻿using GenderHealthCare.Core.Config;
using GenderHealthCare.Entity;
using GenderHealthCare.ModelViews.AuthenticationModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GenderHealthCare.Services.Infrastructure
{
    public class JwtTokenGenerator
    {
        public async Task<AuthenticationModel> CreateToken(User user, JwtSettings jwtSettings)
        {
            jwtSettings.IsValid();
            DateTime now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("role", user.Role),
                new Claim("token_type", "access")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var expires = now.AddMinutes(jwtSettings.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthenticationModel
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(token),
                UserResponse = new UserResponseModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    DateOfBirth = user.DateOfBirth,
                    Role = user.Role,
                    IsCycleTrackingOn = user.IsCycleTrackingOn,
                    CreatedTime = user.CreatedTime
                }
            };
        }
    }
}
