using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos.MaterialsDtos
{
	public class CreateMaterialDto
	{
		public string Title { get; set; }
		public int CourseId { get; set; }
	}
}
