using Academy.Core.Dtos;
using Academy.Core.ServicesInterfaces;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyAdvicingGp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int? id)
        {
            if (id is null) return BadRequest("Invalid id !!");

            var result = await _studentService.GetStudentByIdAsync(id.Value);

            if (result is null) return NotFound($"The Student with Id: {id} not found :(  ");
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddStudent([FromBody] StudentDto studentDto)
        //{
        //    if (studentDto == null)
        //        return BadRequest("Student data is required.");

        //    var addedStudent = await _studentService.AddStudentAsync(studentDto);
        //    return CreatedAtAction(nameof(GetAllStudents), addedStudent);
        //}

        [HttpPost("add")]
        [Authorize(Roles = "StudentAffair")] // السماح فقط لموظف شؤون الطلاب
        public async Task<IActionResult> AddStudent([FromBody] StudentDto model)
        {
            if (model == null)
                return BadRequest("Invalid student data.");

            try
            {
                var studentDto = await _studentService.AddStudentAsync(model);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDtoID studentDtoid)
        {
            if (studentDtoid == null || id != studentDtoid.Id)
                return BadRequest("Invalid student data or ID mismatch.");

            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
                return NotFound($"Student with ID {id} not found.");

            await _studentService.UpdateStudentAsync(studentDtoid);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
                return NotFound($"Student with ID {id} not found.");

            await _studentService.DeleteStudentAsync(id);
            return Ok();
        }



       
        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents([FromQuery] string? searchTerm)
        {
            var students = await _studentService.SearchStudentsAsync(searchTerm);

            if (students == null || !students.Any())
            {
                return NotFound("No students found matching the search criteria.");
            }

            return Ok(students);
        }
    }
}
