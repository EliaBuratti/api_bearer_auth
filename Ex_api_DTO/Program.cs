
using Ex_api_DTO.Autentication;
using Ex_api_DTO.Database;
using Ex_api_DTO.Profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ex_api_DTO
{
    public class Program
    {
        private static readonly string Key = "4F876H32JK987Y5B4F876H32JK987Y5B";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddDbContext<FakeDatabase>(opt => opt.UseInMemoryDatabase("Ecommerce"));
            builder.Services.AddSingleton<JwtAutenticationManager>(new JwtAutenticationManager(Key));
            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            
            //builder.Services.AddSwaggerGen(); standard without autentication

            //with autentication, follow configuration 👇🏼 
            builder.Services.AddSwaggerGen(c =>
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Write: Bearer [Your token], the space betweeen bearer and token is necessary!",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                        {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                        }
                    });
                }
            );

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //this method create autentication
                .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false, //not useful for test app, on production it's useful
                            ValidateAudience = false, // disable validation public key, in this case it's disabled for test
                            ValidateIssuerSigningKey = true, //specific the key of client was validated
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key))
                        };
                    }
                );


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
