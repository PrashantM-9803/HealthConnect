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
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.Name : null))
                .ReverseMap();
            CreateMap<Vitals, VitalsDto>().ReverseMap();
            CreateMap<Medications, MedicationsDto>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<Diagnosis, DiagnosisDto>().ReverseMap();

            // Map Patient + User to PatientUpdateProfileDto (for GET)
            CreateMap<Patient, PatientUpdateProfileDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.User.Dob))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        }
    }
}
