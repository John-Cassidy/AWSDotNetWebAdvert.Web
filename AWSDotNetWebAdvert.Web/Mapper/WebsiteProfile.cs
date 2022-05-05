using AutoMapper;
using AWSDotNetWebAdvert.Web.Models.AdvertManagement;
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

        }
    }
}
