
using Academy.Core;
using Academy.Core.Mapping;

using Academy.Core.ServicesInterfaces;
using Academy.Repo;
using Academy.Repo.Data;
using Academy.Services.Services;

using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Academy.Services.Services.CourseService;

using Microsoft.EntityFrameworkCore;

namespace AcademyAdvicingGp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AcademyContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            // dependency enjection 
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new StudentProfile()));

            builder.Services.AddScoped<IAvailableCourse, AvailableCourseService>();
            builder.Services.AddAutoMapper(M => M.AddProfile(new AvailableCourseProfile()));


            builder.Services.AddScoped<ICourseService, CreateCourseService>();
         
            builder.Services.AddAutoMapper(M=>M.AddProfile(new CourseProfile()));

            var app = builder.Build();

            #region update-Database
            using var scope = app.Services.CreateScope();
            // group of services lifetime scooped
            var Services = scope.ServiceProvider; // مسكيت 
            // servics its self

            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<AcademyContext>();
                //ask clr for creating object from dbcontext explicity
                await dbContext.Database.MigrateAsync(); // update-database
                // scope.Dispose(); استخدمت using
                //await StoreContextSeed.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An Error Occured During Appling The Migration");

            }
            #endregion

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
