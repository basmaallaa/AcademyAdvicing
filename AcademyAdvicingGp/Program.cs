using Academy.Core;
using Academy.Core.Mapping;
using Academy.Core.ServicesInterfaces;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Core.Models.Identity;

using Academy.Repo;
using Academy.Repo.Data;
using Academy.Repo.Identity;

using Academy.Services.Services;
using Academy.Services.Services.CourseService;

using AcademyAdvicingGp.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;
using Academy.Core.Models.Email;
using QuestPDF.Infrastructure;

namespace AcademyAdvicingGp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers().AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();

            #region Email Setting
            // تحميل إعدادات البريد الإلكتروني
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            #endregion

            #region Swagger Configuration
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PaymentApi",
                    Version = "v1"
                });

                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization using JWT bearer security scheme"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearerAuth"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            #endregion

            #region Database Configuration
            builder.Services.AddDbContext<AcademyContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
            #endregion

            #region Dependency Injection
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new StudentProfile()));

            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new DoctorProfile()));

            builder.Services.AddScoped<IDoctorCourseService, DoctorCourseService>();

            builder.Services.AddScoped<IAvailableCourse, AvailableCourseService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new AvailableCourseProfile()));

            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new MaterialProfile()));


            builder.Services.AddScoped<ICourseService, CreateCourseService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new CourseProfile()));

			builder.Services.AddScoped<IScheduleTimeTableService, ScheduleTimeTableService>();
			builder.Services.AddAutoMapper(M => M.AddProfile(new ScheduleTimeTableProfile()));


            builder.Services.AddTransient<IFileService, FileService>();

            builder.Services.AddScoped<IEmailService, EmailService>();
            #endregion

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            #endregion

            #region Identity
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.Configure<DataProtectionTokenProviderOptions>(opts =>
            {
                opts.TokenLifespan = TimeSpan.FromHours(10);
            });
            #endregion
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SupportNonNullableReferenceTypes(); // يدعم الـ file types
                c.MapType<IFormFile>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });
            });

            builder.Services.AddScoped<TranscriptService>();
            
            var app = builder.Build();

            #region Database Migration & Seeding
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var dbContext = services.GetRequiredService<AcademyContext>();
                await dbContext.Database.MigrateAsync();

                var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
                await identityDbContext.Database.MigrateAsync();

                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await AppIdentityDbContextSeed.SeedUserAsync(identityDbContext);
                
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during applying the migration");
            }
            #endregion

            #region Middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
                RequestPath = "/Resources"
            });
            
           // app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
