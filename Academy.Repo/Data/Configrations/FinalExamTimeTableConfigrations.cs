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
	public class FinalExamTimeTableConfigrations : IEntityTypeConfiguration<FinalExamTimeTable>
	{
		public void Configure(EntityTypeBuilder<FinalExamTimeTable> builder)
		{
			//3lshan el relation elly beno w ben el available courses
			//builder.HasOne(s => s.AvailableCourses)
			//		.WithMany(a => a.)
			//		.HasForeignKey(s => s.AvailableCourseId)
			//		.OnDelete(DeleteBehavior.Restrict);

			//3lshan el relation elly beno w ben el student
			builder.HasMany(s => s.Students)
					.WithOne(st => st.finalExamTimeTable)
					.HasForeignKey(st => st.ScheduleId);
		}
	}
}
