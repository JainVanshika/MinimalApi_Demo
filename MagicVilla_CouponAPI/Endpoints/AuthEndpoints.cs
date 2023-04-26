using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Data.SqlTypes;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {

            app.MapPost("/api/login", Login)
                .WithName("Login").Accepts<LoginRequestDTO>("application/json").Produces<APIResponse>(200).Produces(400);
            app.MapPost("/api/register", Register)
               .WithName("Register").Accepts<RegistrationRequestDTO>("application/json").Produces<APIResponse>(200).Produces(400);

        }

        private async static Task<IResult> Login(IAuthRepository _authRepo, [FromBody] LoginRequestDTO loginRequestDTO)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            var loginResponse=await _authRepo.Login(loginRequestDTO);
            if (loginResponse == null)
            {
                response.ErrorMessages.Add("Username or password is incorrect");
                return Results.BadRequest(response);
            }
           
            response.Result = loginResponse;
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return Results.Ok(response);
        }
        private async static Task<IResult> Register(IAuthRepository _authRepo, [FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            bool IfUserNameIsUnique = _authRepo.IsUniqueUser(registrationRequestDTO.UserName);
            if(!IfUserNameIsUnique)
            {
                response.ErrorMessages.Add("User already exists");
                return Results.BadRequest(response);
            }

            var registerResponse=await _authRepo.Register(registrationRequestDTO);
            if (registerResponse == null || string.IsNullOrEmpty(registerResponse.UserName))
            {
                return Results.BadRequest(response);
            }
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return Results.Ok(response);
        }


    }
}
