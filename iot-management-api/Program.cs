using iot_management_api.Configuration;
using iot_management_api.Context;
using iot_management_api.Helper;
using iot_management_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;

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
builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug()
    .AddAzureWebAppDiagnostics();


builder.Services.Configure<AzureFileLoggerOptions>(options =>
{
    options.FileName = "my-azure-diagnostics-";
    options.FileSizeLimit = 50 * 1024;
    options.RetainedFileCountLimit = 5;
});

//controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

//services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IDeviceInfoService, DeviceInfoService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();

builder.Services.AddTransient<JwtHandler>();
builder.Services.AddTransient<JwtGenerator>();
builder.Services.AddSingleton<JwtValidator>();
builder.Services.AddSingleton<StudyWeekService>();

builder.Services.AddSingleton<Encrypter>();

//builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//DbContext
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connString));

//jwt auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(x => x.Cookie.Name="token")
    .AddScheme<JwtBearerOptions, JwtHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("TeacherAccess", policy =>
    {
        policy.RequireClaim("role", "Teacher");
    });
    opts.AddPolicy("StudentAccess", policy =>
    {
        policy.RequireClaim("role", "Student");
    });
});

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv6", Version = "v1" });
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
//if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();

//serilog
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
