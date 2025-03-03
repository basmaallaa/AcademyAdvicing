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
	public class AssignedCoursesConfigrations : IEntityTypeConfiguration<AssignedCourse>
	{
		public void Configure(EntityTypeBuilder<AssignedCourse> builder)
		{
			builder.HasKey(AS => new { AS.StudentId, AS.CourseId });
		}
	}
}
