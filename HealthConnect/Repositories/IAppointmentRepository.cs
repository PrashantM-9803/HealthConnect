using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
        Task<Appointment> GetAppointmentByIdAsync(Guid id);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<bool> CancelAppointmentAsync(Guid appointmentId);
        Task<bool> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus status);
        Task<Diagnosis> AddDiagnosisAsync(AddDiagnosisDto dto);
    }
}

