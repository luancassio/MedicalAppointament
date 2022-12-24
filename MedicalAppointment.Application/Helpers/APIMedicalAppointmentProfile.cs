using AutoMapper;
using MedicalAppointment.Application.Dto;
using MedicalAppointment.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace Hvex.Domain.Helpers {
    public class APIMedicalAppointmentProfile : Profile{
        public APIMedicalAppointmentProfile() {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, Login>().ReverseMap();
            //CreateMap<User, AuthDto>().ReverseMap();


        }
    }
}
