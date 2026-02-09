using AutoMapper;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId == null || src.DoctorId == Guid.Empty ? (Guid?)null : src.DoctorId))
                .ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Doctor, DoctorDto>().ReverseMap();
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<Vitals, VitalsDto>().ReverseMap();
            CreateMap<Medications, MedicationsDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<Diagnosis, DiagnosisDto>().ReverseMap();
        }
    }
}
