using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthConnect.Data;
using HealthConnect.Models;
using HealthConnect.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace HealthConnect.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly HealthConnectDbContext _context;

        public AppointmentRepository(HealthConnectDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            // Verify the slot exists and is available
            var slot = await _context.DoctorSlots.FindAsync(dto.SlotId);
            if (slot == null)
                throw new Exception("Slot not found.");

            if (slot.IsBooked)
                throw new Exception("Slot is already booked.");

            if (slot.DoctorId != dto.DoctorId)
                throw new Exception("Slot does not belong to the specified doctor.");

            // Get the patient
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            // Always update patient's doctor to the most recent doctor they book with
            patient.DoctorId = dto.DoctorId;

            // Create the appointment
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                SlotId = dto.SlotId,
                AppointmentDate = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = AppointmentStatus.Pending,
                Reason = dto.Reason
            };

            // Mark the slot as booked
            slot.IsBooked = true;
            slot.UpdatedAt = DateTime.UtcNow;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            return appointment;
        }

        public async Task<Appointment> GetAppointmentByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Vitals)
                .Include(a => a.Medications)
                .Include(a => a.Invoice)
                .Include(a => a.Diagnosis)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Vitals)
                .Include(a => a.Medications)
                .Include(a => a.Invoice)
                .Include(a => a.Diagnosis)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Slot)
                .Include(a => a.Vitals)
                .Include(a => a.Medications)
                .Include(a => a.Invoice)
                .Include(a => a.Diagnosis)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> CancelAppointmentAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return false;

            // Update appointment status
            appointment.Status = AppointmentStatus.Cancelled;

            // Free up the slot
            if (appointment.Slot != null)
            {
                appointment.Slot.IsBooked = false;
                appointment.Slot.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(Guid appointmentId, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Diagnosis> AddDiagnosisAsync(AddDiagnosisDto dto)
        {
            // Load with navs so we can attach/update without severing required 1:1 relationship
            var appointment = await _context.Appointments
                .Include(a => a.Diagnosis)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            // If a diagnosis already exists for this appointment, update it (1:1)
            if (appointment.Diagnosis != null)
            {
                appointment.Diagnosis.DiagnosisDetails = dto.DiagnosisDetails;
                appointment.Diagnosis.PatientId = dto.PatientId;
                await _context.SaveChangesAsync();
                return appointment.Diagnosis;
            }

            // Otherwise create new
            var diagnosis = new Diagnosis
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                DiagnosisDetails = dto.DiagnosisDetails
            };

            _context.Add(diagnosis);

            // Attach to appointment (1:1)
            appointment.Diagnosis = diagnosis;

            await _context.SaveChangesAsync();
            return diagnosis;
        }

        public async Task<Vitals> AddVitalsAsync(AddVitalsDto dto)
        {
            // Load with nav so we can keep appointment in sync
            var appointment = await _context.Appointments
                .Include(a => a.Vitals)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            var existingVitals = await _context.Vitals.FirstOrDefaultAsync(v => v.AppointmentId == dto.AppointmentId);
            if (existingVitals != null)
            {
                // Update existing
                existingVitals.BloodPressure = dto.BloodPressure;
                existingVitals.HeartRate = dto.HeartRate;
                existingVitals.Temperature = dto.Temperature;
                existingVitals.SpO2 = dto.SpO2;

                // Keep navigation updated
                appointment.Vitals = existingVitals;

                await _context.SaveChangesAsync();
                return existingVitals;
            }
            else
            {
                // Create new
                var vitals = new Vitals
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = dto.AppointmentId,
                    PatientId = dto.PatientId,
                    BloodPressure = dto.BloodPressure,
                    HeartRate = dto.HeartRate,
                    Temperature = dto.Temperature,
                    SpO2 = dto.SpO2
                };

                _context.Vitals.Add(vitals);

                // Attach to appointment (1:1)
                appointment.Vitals = vitals;

                await _context.SaveChangesAsync();
                return vitals;
            }
        }

        public async Task<Medications> AddMedicationsAsync(AddMedicationsDto dto)
        {
            // Load with navs so we can attach the new medication to the appointment (1:many)
            var appointment = await _context.Appointments
                .Include(a => a.Medications)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            // Always create new medication entry (since Appointment can have multiple medications)
            var medications = new Medications
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                Drug = dto.Drug,
                Dose = dto.Dose,
                Route = dto.Route,
                Frequency = dto.Frequency,
                Activity = (HealthConnect.Models.Activity)dto.Activity
            };

            _context.Medications.Add(medications);

            // Attach to appointment
            appointment.Medications ??= new List<Medications>();
            appointment.Medications.Add(medications);

            await _context.SaveChangesAsync();
            return medications;
        }

        public async Task<Invoice> AddInvoiceAsync(AddInvoiceDto dto)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Invoice)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            var existingInvoice = await _context.Invoices.FirstOrDefaultAsync(i => i.AppointmentId == dto.AppointmentId);
            if (existingInvoice != null)
            {
                // Update existing invoice
                existingInvoice.ConsultationType = dto.ConsultationType;
                existingInvoice.ConsulationFee = dto.ConsulationFee;
                existingInvoice.LabFee = dto.LabFee;
                existingInvoice.MedicineFee = dto.MedicineFee;
                existingInvoice.Total = dto.Total;

                // Keep navigation updated
                appointment.Invoice = existingInvoice;

                await _context.SaveChangesAsync();
                return existingInvoice;
            }
            else
            {
                // Create new invoice
                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    AppointmentId = dto.AppointmentId,
                    PatientId = dto.PatientId,
                    ConsultationType = dto.ConsultationType,
                    ConsulationFee = dto.ConsulationFee,
                    LabFee = dto.LabFee,
                    MedicineFee = dto.MedicineFee,
                    Total = dto.Total,
                    Status = InvoiceStatus.Pending, // Set default status
                    IssuedDate = DateTime.UtcNow // Set issued date
                };
                _context.Invoices.Add(invoice);

                // Attach to appointment (1:1)
                appointment.Invoice = invoice;

                await _context.SaveChangesAsync();
                return invoice;
            }
        }
    }
}

