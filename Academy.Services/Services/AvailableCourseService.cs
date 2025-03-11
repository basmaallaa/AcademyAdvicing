using Academy.Core.Dtos;
using Academy.Core.Models;
using Academy.Core;
using Academy.Core.ServicesInterfaces;
using Academy.Core.ServicesInterfaces.ICoursesInterface;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Services.Services
{
    public class AvailableCourseService : IAvailableCourse
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AvailableCourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AvailableCourseDto> CreateAvailableCourseAsync(AvailableCourseDto availableCourseDto)
        {
            var availableCourse = _mapper.Map<AvailableCourse>(availableCourseDto);
            await _unitOfWork.Repository<AvailableCourse>().AddAsync(availableCourse);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<AvailableCourseDto>(availableCourse);
        }

        public async Task<AvailableCourseDto> UpdateAvailableCourseAsync(int id, AvailableCourseDto updateAvailableCourseDto)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            if (availableCourse == null) return null;

            _mapper.Map(updateAvailableCourseDto, availableCourse);
            _unitOfWork.Repository<AvailableCourse>().Update(availableCourse);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AvailableCourseDto>(availableCourse);
        }

        public async Task<AvailableCourseDto> GetAvailableCourseByIdAsync(int id)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            return availableCourse == null ? null : _mapper.Map<AvailableCourseDto>(availableCourse);
        }

        public async Task<IEnumerable<AvailableCourseDto>> GetAllAvailableCoursesAsync()
        {
            var availableCourses = await _unitOfWork.Repository<AvailableCourse>().GetAllAsync();
            return _mapper.Map<IEnumerable<AvailableCourseDto>>(availableCourses);
        }

        public async Task<bool> DeleteAvailableCourseAsync(int id)
        {
            var availableCourse = await _unitOfWork.Repository<AvailableCourse>().GetAsync(id);
            if (availableCourse == null) return false;

            _unitOfWork.Repository<AvailableCourse>().Delete(availableCourse);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    
}
}
