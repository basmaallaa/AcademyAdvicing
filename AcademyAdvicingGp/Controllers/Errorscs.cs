using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AcademyAdvicingGp
{
    internal class ApiResponse : ModelStateDictionary
    {
        public ApiResponse(int maxAllowedErrors) : base(maxAllowedErrors)
        {
        }
    }
}