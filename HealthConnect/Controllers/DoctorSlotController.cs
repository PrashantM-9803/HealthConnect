using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HealthConnect.Models.Dto;
using HealthConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorSlotController : ControllerBase
    {
        private readonly IDoctorSlotRepository _slotRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorSlotController(
            IDoctorSlotRepository slotRepository, 
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _slotRepository = slotRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Generate time slots for a doctor for a date range
        /// </summary>
        [HttpPost("generate")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> GenerateSlots([FromBody] GenerateSlotsDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "Start date must be before or equal to end date." });

            var doctor = await _doctorRepository.GetDoctorByIdAsync(dto.DoctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var slots = await _slotRepository.GenerateSlotsForDoctorAsync(dto.DoctorId, dto.StartDate, dto.EndDate);
            
            if (slots == null)
                return NotFound(new { message = "Doctor not found." });

            var slotDtos = _mapper.Map<List<DoctorSlotDto>>(slots);
            
            return Ok(new 
            { 
                message = $"Successfully generated {slots.Count} slots for doctor.",
                slots = slotDtos,
                totalGenerated = slots.Count
            });
        }

        /// <summary>
        /// Get available slots for a doctor (optionally filter by date)
        /// </summary>
        [HttpGet("doctor/{doctorId}/available")]
        public async Task<IActionResult> GetAvailableSlots(Guid doctorId, [FromQuery] DateTime? date = null)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var slots = await _slotRepository.GetAvailableSlotsByDoctorAsync(doctorId, date);
            
            var availableSlots = slots.Select(s => new AvailableSlotDto
            {
                SlotId = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                TimeDisplay = $"{s.StartTime.ToString(@"hh\:mm")} - {s.EndTime.ToString(@"hh\:mm")}"
            }).ToList();

            return Ok(new
            {
                doctorId = doctorId,
                doctorName = doctor.User?.Name,
                totalAvailableSlots = availableSlots.Count,
                slots = availableSlots
            });
        }

        /// <summary>
        /// Get all slots for a doctor (including booked ones)
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> GetAllSlots(
            Guid doctorId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var slots = await _slotRepository.GetAllSlotsByDoctorAsync(doctorId, startDate, endDate);
            var slotDtos = _mapper.Map<List<DoctorSlotDto>>(slots);

            return Ok(new
            {
                doctorId = doctorId,
                doctorName = doctor.User?.Name,
                totalSlots = slotDtos.Count,
                bookedSlots = slotDtos.Count(s => s.IsBooked),
                availableSlots = slotDtos.Count(s => !s.IsBooked),
                slots = slotDtos
            });
        }

        /// <summary>
        /// Get a specific slot by ID
        /// </summary>
        [HttpGet("{slotId}")]
        public async Task<IActionResult> GetSlotById(Guid slotId)
        {
            var slot = await _slotRepository.GetSlotByIdAsync(slotId);
            if (slot == null)
                return NotFound(new { message = "Slot not found." });

            var slotDto = _mapper.Map<DoctorSlotDto>(slot);
            return Ok(slotDto);
        }

        /// <summary>
        /// Delete a slot (only if not booked)
        /// </summary>
        [HttpDelete("{slotId}")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> DeleteSlot(Guid slotId)
        {
            var result = await _slotRepository.DeleteSlotAsync(slotId);
            if (!result)
                return BadRequest(new { message = "Slot not found or is already booked and cannot be deleted." });

            return Ok(new { message = "Slot deleted successfully." });
        }

        /// <summary>
        /// Delete slots for a date range (only unbooked slots)
        /// </summary>
        [HttpDelete("doctor/{doctorId}/date-range")]
        [Authorize(Roles = "DOCTOR,ADMIN")]
        public async Task<IActionResult> DeleteSlotsForDateRange(
            Guid doctorId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest(new { message = "Start date must be before or equal to end date." });

            var result = await _slotRepository.DeleteSlotsForDateRangeAsync(doctorId, startDate, endDate);
            if (!result)
                return NotFound(new { message = "No unbooked slots found for the specified date range." });

            return Ok(new { message = "Unbooked slots deleted successfully for the specified date range." });
        }

        /// <summary>
        /// Get available dates for a doctor (dates that have at least one available slot)
        /// </summary>
        [HttpGet("doctor/{doctorId}/available-dates")]
        public async Task<IActionResult> GetAvailableDates(
            Guid doctorId, 
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found." });

            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddMonths(1);

            var slots = await _slotRepository.GetAvailableSlotsByDoctorAsync(doctorId);
            
            var availableDates = slots
                .Where(s => s.Date >= start && s.Date <= end)
                .Select(s => s.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return Ok(new
            {
                doctorId = doctorId,
                doctorName = doctor.User?.Name,
                totalAvailableDates = availableDates.Count,
                dates = availableDates
            });
        }
    }
}
