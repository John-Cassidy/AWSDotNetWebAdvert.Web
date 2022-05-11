using AutoMapper;
using AWSDotNetWebAdvert.Web.Models;
using AWSDotNetWebAdvert.Web.Models.Home;
using AWSDotNetWebAdvert.Web.ServiceClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AWSDotNetWebAdvert.Web.Controllers {
    public class HomeController : Controller {
        public ISearchApiClient SearchApiClient { get; }
        public IMapper Mapper { get; }
        public IAdvertApiClient ApiClient { get; }

        public HomeController(ISearchApiClient searchApiClient, IMapper mapper, IAdvertApiClient apiClient) {
            SearchApiClient = searchApiClient;
            Mapper = mapper;
            ApiClient = apiClient;
        }
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index() {
            var allAds = await ApiClient.GetAllAsync().ConfigureAwait(false);
            var allViewModels = allAds.Select(x => Mapper.Map<IndexViewModel>(x));

            return View(allViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword) {
            var viewModel = new List<SearchViewModel>();

            var searchResult = await SearchApiClient.Search(keyword).ConfigureAwait(false);
            searchResult.ForEach(advertDoc => {
                var viewModelItem = Mapper.Map<SearchViewModel>(advertDoc);
                viewModel.Add(viewModelItem);
            });

            return View("Search", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
