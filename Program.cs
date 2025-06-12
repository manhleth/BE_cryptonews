using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsPaper.src.Application.Features;
using NewsPaper.src.Application.Mapping;
using NewsPaper.src.Application.Services;
using NewsPaper.src.Domain.Interfaces;
using NewsPaper.src.Domain.ValueObjects;
using NewsPaper.src.Infrastructure.Persistence;
using NewsPaper.src.Presentation.Controllers; // Thêm namespace cho controller

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("V1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "NewsPaper API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Id = "Bearer",
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                }
            },
            new string[] { }
        }
    });

});
//var apiOption = builder.Configuration.GetSection("ApiOptions").Get<ApiOptions>();
//builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("ApiOptions"));
var apiOption = builder.Configuration.GetSection("ApiOptions").Get<ApiOptions>();
builder.Services.AddSingleton(apiOption);


builder.Services.AddAuthentication(c =>
{
    c.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    c.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    c.DefaultScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(c =>
{
    c.SaveToken = true;
    c.RequireHttpsMetadata = false;
    c.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = apiOption.ValidAudience,
        ValidIssuer = apiOption.ValidIssuer,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(apiOption.Secret))
    };
});


builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(apiOption.StringConnection);
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingConfig());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<OTPConfiguration>(builder.Configuration.GetSection("Otp"));
builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<OTPService>();


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IBaseService, NewService>();  // Chú ý tên class là NewService
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<MediaFileService>();
builder.Services.AddScoped<ChildrenCategoryService>();
builder.Services.AddScoped<SavedService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WatchlistService>();

// Thêm TransactionService vào DI container
builder.Services.AddScoped<TransactionService>();

// allow cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials() // Quan trọng!
              .SetIsOriginAllowed(_ => true); // Có thể cần thiết trong môi trường dev
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");


app.UseStaticFiles();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DefaultModelExpandDepth(-1);
    c.SwaggerEndpoint("/swagger/V1/swagger.json", "NewsPaper API V1");
    c.DocumentTitle = "NewsPaper API";
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

app.MapControllers();


app.Run();