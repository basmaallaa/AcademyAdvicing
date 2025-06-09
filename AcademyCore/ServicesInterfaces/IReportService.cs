using Academy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Core.ServicesInterfaces
{
    public interface IReportService
    {
        Task<Report> GenerateGraduatesReportAsync(int reportId, List<Student> data);
    }
}
