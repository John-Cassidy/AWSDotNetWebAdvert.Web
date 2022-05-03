using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace AWSDotNetWebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<CreateAdvertResponse> Create(AdvertModel model);
        //Task<AdvertResponse> CreateAsync(CreateAdvertModel model);
        //Task<bool> ConfirmAsync(ConfirmAdvertRequest model);
        //Task<List<Advertisement>> GetAllAsync();
        //Task<Advertisement> GetAsync(string advertId);
    }
}
