using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.Dtos
{
	public class MaterialDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public int UploadedById { get; set; }

		//public string DoctorName { get; set; }
		//public string FilePath { get; set; }
	}
}
