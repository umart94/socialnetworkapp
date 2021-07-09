using System.Linq;
using AutoMapper;
using SocialApp.API.DTOS;
using SocialApp.API.Models;
namespace SocialApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        //automapper uses profiles to understand source and destination
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDTO>()
            .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Age, opt =>
            {
                opt.ResolveUsing(d => d.DateOfBirth.calculateAge());
            });

            CreateMap<User, UserForDetailedDTO>().ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            }).ForMember(dest => dest.Age, opt =>
            {
                opt.ResolveUsing(d => d.DateOfBirth.calculateAge());
            });
            CreateMap<Photo, PhotosForDetailedDTO>();
            CreateMap<UserForUpdateDTO, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDTO,User>();
            CreateMap<MessageForCreationDTO,Message>().ReverseMap();
            CreateMap<Message,MessageToReturnDTO>()
            .ForMember(m=> m.SenderPhotoUrl, opt => opt
            .MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(m=> m.RecipientPhotoUrl, opt => opt
            .MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}