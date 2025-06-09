using Academy.Core.Dtos.ScheduleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.FinalTimeTableDtos
{
	public class CreateFullFinalTimeTableDto
	{
		public List<CreateFinalExamTimeTableDto> Courses { get; set; }
	}
}
