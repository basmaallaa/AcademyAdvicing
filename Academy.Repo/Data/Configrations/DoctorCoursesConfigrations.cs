using Academy.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Data.Configrations
{
	public class DoctorCoursesConfigrations : IEntityTypeConfiguration<DoctorCourses>
	{
		public void Configure(EntityTypeBuilder<DoctorCourses> builder)
		{
			builder.HasKey(DS => new { DS.CourseId, DS.DoctorId });
			builder.HasOne(dc => dc.Doctor)
						.WithMany(d => d.Courses)
						.HasForeignKey(dc => dc.DoctorId)
						.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(dc => dc.Course)
					.WithMany(c => c.Doctor)
					.HasForeignKey(dc => dc.CourseId)
					.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
