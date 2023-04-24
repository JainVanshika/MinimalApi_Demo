using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Repository;
using MagicVilla_CouponAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICouponRepository,CouponRepository>();
builder.Services.AddDbContext<ApplicationDBContext>(option=>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/*//adding endpoint: app.mapget have all get, post, put, delete methods for endpoints
//HTTP Get Action method
//"/helloworld" is the route name and ()=> this is the returning statement
app.MapGet("/helloworld/{id:int}", (int id) =>
{
    return Results.Ok("ID!!!"+id);
});

app.MapPost("/helloworld2", () => Results.Ok("hellow world 2"));*/

//to retrive all the list
app.MapGet("/api/coupon", async(ICouponRepository _couponRepo,ILogger<Program> _logger) => {
    APIResponse response = new();
    _logger.Log(LogLevel.Information, "Getting all coupons");
    response.Result = await _couponRepo.GetAllAsync();
    response.IsSuccess = true;
    response.StatusCode=HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupons").Produces<APIResponse>(200);


//to retrive data based on id
app.MapGet("/api/coupon/{id:int}", async(ICouponRepository _couponRepo, ILogger < Program > _logger,int id) =>
{
    APIResponse response = new();
    _logger.Log(LogLevel.Information, "Getting all coupons");
    response.Result=await _couponRepo.GetAsync(id);
    response.IsSuccess = true;
    response.StatusCode=HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);


app.MapPost("/api/coupon", async (ICouponRepository _couponRepo, IMapper _mapper,IValidator<CouponCreateDTO> _validation,[FromBody] CouponCreateDTO couponCreateDTO) =>
{
    APIResponse response = new()
    {
        IsSuccess = false,
        StatusCode=HttpStatusCode.BadRequest
    };
    var validationResult= await _validation.ValidateAsync(couponCreateDTO);
    if(!validationResult.IsValid)
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
    response.IsSuccess= true;
    return Results.Ok(response);
    //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
    //return Results.Created($"/api/coupon/{coupon.Id}",coupon);
}).WithName("CreateCoupons").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);


app.MapPut("/api/coupon", async (ICouponRepository _couponRepo, IMapper _mapper,IValidator<CouponUpdateDTO> _validation,[FromBody] CouponUpdateDTO couponUpdateDTO) =>
{
    APIResponse response = new()
    {
        IsSuccess=false,
        StatusCode= HttpStatusCode.BadRequest
    };
    var validationResult =await _validation.ValidateAsync(couponUpdateDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    await _couponRepo.UpdateAsync(_mapper.Map<Coupon>(couponUpdateDTO));
    await _couponRepo.SaveAsync();

    //CouponDTO couponDTO=_mapper.Map<CouponDTO>(couponFromStore);
    response.Result= _mapper.Map<CouponDTO>(await _couponRepo.GetAsync(couponUpdateDTO.Id));
    response.StatusCode=HttpStatusCode.OK;
    response.IsSuccess=true;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);


app.MapDelete("/api/coupon/{id:int}", async (ICouponRepository _couponRepo, int id) =>
{
    APIResponse response = new()
    {
        IsSuccess=false,
        StatusCode=HttpStatusCode.BadRequest
    };
    Coupon couponFromStore=await _couponRepo.GetAsync(id);
    if(couponFromStore!=null)
    {
        await _couponRepo.DeleteAsync(couponFromStore);
        await _couponRepo.SaveAsync();
        response.IsSuccess= true;
        response.StatusCode=HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Invalid Id");
        return Results.BadRequest(response);
    }
}).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400);


app.UseHttpsRedirection();
app.Run();
