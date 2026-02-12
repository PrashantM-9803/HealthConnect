using AutoMapper;
using HealthConnect.Models;
using HealthConnect.Models.Dto;

namespace HealthConnect.Mapping
{
    public class SlotProfile : Profile
    {
        public SlotProfile()
        {
            CreateMap<DoctorSlot, DoctorSlotDto>();
            CreateMap<DoctorSlot, AvailableSlotDto>()
                .ForMember(dest => dest.SlotId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TimeDisplay, 
                    opt => opt.MapFrom(src => $"{src.StartTime.ToString(@"hh\:mm")} - {src.EndTime.ToString(@"hh\:mm")}"));
        }
    }
}
