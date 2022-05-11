using AdvertApi.Models;
using AutoMapper;
using AWSDotNetWebAdvert.Web.Models;
using AWSDotNetWebAdvert.Web.Models.AdvertManagement;
using AWSDotNetWebAdvert.Web.Models.Home;
using AWSDotNetWebAdvert.Web.ServiceClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSDotNetWebAdvert.Web.Mapper
{
    public class WebsiteProfile : Profile {
        public WebsiteProfile() {
            CreateMap<CreateAdvertModel, CreateAdvertViewModel>().ReverseMap();

            CreateMap<AdvertModel, Advertisement>().ReverseMap();

            CreateMap<Advertisement, IndexViewModel>()
                .ForMember(
                    dest => dest.Title, src => src.MapFrom(field => field.Title))
                .ForMember(dest => dest.Image, src => src.MapFrom(field => field.FilePath));

            CreateMap<AdvertType, SearchViewModel>()
                .ForMember(
                    dest => dest.Id, src => src.MapFrom(field => field.Id))
                .ForMember(
                    dest => dest.Title, src => src.MapFrom(field => field.Title));
        }
    }
}
