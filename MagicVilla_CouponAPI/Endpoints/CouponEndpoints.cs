using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Data.SqlTypes;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {

            app.MapGet("/api/coupon", GetAllCoupon)
                .WithName("GetCoupons").Produces<APIResponse>(200).RequireAuthorization("AdminOnly");


            app.MapGet("/api/coupon/{id:int}", GetCoupon)
                .WithName("GetCoupon").Produces<APIResponse>(200);


            app.MapPost("/api/coupon", CreateCoupon)
                .WithName("CreateCoupons").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);


            app.MapPut("/api/coupon", UpdateCoupon)
                .WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);


            app.MapDelete("/api/coupon/{id:int}", DeleteCoupon)
                .WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400);


        }
        private async static Task<IResult> GetAllCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger)
        {
            APIResponse response = new();
            _logger.Log(LogLevel.Information, "Getting all coupons");
            response.Result = await _couponRepo.GetAllAsync();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
        [Authorize]
        private async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepo, int id)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            Coupon couponFromStore = await _couponRepo.GetAsync(id);
            if (couponFromStore != null)
            {
                await _couponRepo.DeleteAsync(couponFromStore);
                await _couponRepo.SaveAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);
            }
            else
            {
                response.ErrorMessages.Add("Invalid Id");
                return Results.BadRequest(response);
            }
        }
        [Authorize]
        private async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepo, IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO couponUpdateDTO)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            var validationResult = await _validation.ValidateAsync(couponUpdateDTO);
            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }

            await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(couponUpdateDTO));
            await _couponRepo.SaveAsync();

            //CouponDTO couponDTO=_mapper.Map<CouponDTO>(couponFromStore);
            response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetAsync(couponUpdateDTO.Id));
            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            return Results.Ok(response);
        }
        [Authorize]
        private async static Task<IResult> CreateCoupon(ICouponRepository _couponRepo, IMapper _mapper, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO couponCreateDTO)
        {
            APIResponse response = new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.BadRequest
            };
            var validationResult = await _validation.ValidateAsync(couponCreateDTO);
            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                return Results.BadRequest(response);
            }
            if (await _couponRepo.GetAsync(couponCreateDTO.Name) != null)
            {
                response.ErrorMessages.Add("Coupon name already exists");
                return Results.BadRequest(response);
            }
            Coupon coupon = _mapper.Map<Coupon>(couponCreateDTO);
            //coupon.Id = CouponStore.couponList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1; //id+1
            await _couponRepo.CreateAsync(coupon);
            await _couponRepo.SaveAsync();
            CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
            response.Result = couponDTO;
            response.StatusCode = HttpStatusCode.Created;
            response.IsSuccess = true;
            return Results.Ok(response);
            //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
            //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
        }
        
        private async static Task<IResult> GetCoupon(ICouponRepository _couponRepo, ILogger<Program> _logger, int id)
        {
            APIResponse response = new();
            _logger.Log(LogLevel.Information, "Getting all coupons");
            response.Result = await _couponRepo.GetAsync(id);
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }
    }
}
