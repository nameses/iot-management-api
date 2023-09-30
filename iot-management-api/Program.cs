using iot_management_api.Configuration;
using iot_management_api.Context;
using iot_management_api.Helper;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//configuration(appsettings.json)
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
builder.Services.Configure<PasswordEncryption>(configuration.GetSection("PasswordEncryption"));
var connString = configuration.GetSection("ConnectionStrings")["SqlConnection"];

//log services
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

//controllers
builder.Services.AddControllers();

//services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<JwtHandler>();
builder.Services.AddTransient<JwtGenerator>();
builder.Services.AddSingleton<JwtValidator>();
builder.Services.AddSingleton<Encrypter>();

//builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//DbContext
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connString));

//jwt auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, JwtHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });
//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv6", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});
//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .AllowAnyOrigin()
            //.AllowCredentials()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();

//swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();
