using Academy.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Repo.Data.Configrations
{
	public class ScheduleTimeTableConfigrations :IEntityTypeConfiguration<ScheduleTimeTable> 
	{
		public void Configure(EntityTypeBuilder<ScheduleTimeTable> builder)
		{

			builder.HasOne(s => s.AvailableCourse)
					.WithMany(a => a.ScheduleTimeTables)
					.HasForeignKey(s => s.AvailableCourseId)
					.OnDelete(DeleteBehavior.Restrict);

		}
	}
}
