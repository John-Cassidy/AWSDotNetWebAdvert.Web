using AdvertApi.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AWSDotNetWebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient {

        private readonly IMapper _mapper;
        private readonly string _baseAddress;
        private readonly HttpClient _client;

        public AdvertApiClient(IMapper mapper, IConfiguration configuration, HttpClient client) {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); 
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _baseAddress =  configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
        }

        public async Task<AdvertResponse> CreateAsync(CreateAdvertModel model) {

            var advertApiModel = _mapper.Map<AdvertModel>(model);

            var jsonModel = JsonSerializer.Serialize(advertApiModel);
            var response = await _client
                .PostAsync(new Uri($"{_baseAddress}/create"),
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            var createAdvertResponseAsString = await response.Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);
            var createAdvertResponse = JsonSerializer
                .Deserialize<CreateAdvertResponse>(
                    createAdvertResponseAsString, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var advertResponse = _mapper.Map<AdvertResponse>(createAdvertResponse);

            return advertResponse;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertRequest model) {
            var advertModel = _mapper.Map<ConfirmAdvertRequest>(model);

            var jsonModel = JsonSerializer.Serialize(advertModel);
            var response = await _client
                .PutAsync(new Uri($"{_baseAddress}/confirm"),
                    new StringContent(jsonModel, Encoding.UTF8, "application/json"))
                .ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<Advertisement>> GetAllAsync() {
            var response = await _client.GetAsync(new Uri($"{_baseAddress}/all")).ConfigureAwait(false);
            var allAdvertModelsAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var allAdvertModels = JsonSerializer
                .Deserialize<List<AdvertModel>>(
                    allAdvertModelsAsString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return allAdvertModels.Select(x => _mapper.Map<Advertisement>(x)).ToList();
        }

        public async Task<Advertisement> GetAsync(string advertId) {
            var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/{advertId}")).ConfigureAwait(false);

            var fullAdvertAsString = await apiCallResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            var fullAdvert = JsonSerializer
                .Deserialize<AdvertModel>(
                    fullAdvertAsString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return _mapper.Map<Advertisement>(fullAdvert);
        }
    }
}
