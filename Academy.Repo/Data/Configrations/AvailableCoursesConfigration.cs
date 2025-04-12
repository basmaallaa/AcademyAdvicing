using Academy.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Data.Configrations
{
    public class AvailableCoursesConfigration : IEntityTypeConfiguration<AvailableCourse>
    {
        public void Configure(EntityTypeBuilder<AvailableCourse> builder)
        {
            builder.HasOne(ac => ac.Doctor)
                    .WithMany(d => d.Courses)
                    .HasForeignKey(ac => ac.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dc => dc.Course)
                    .WithMany(c => c.Doctors)
                    .HasForeignKey(dc => dc.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
