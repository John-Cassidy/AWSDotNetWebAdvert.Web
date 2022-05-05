using AdvertApi.Models;
using AutoMapper;
using AWSDotNetWebAdvert.Web.Models.AdvertManagement;
using AWSDotNetWebAdvert.Web.ServiceClients;
using AWSDotNetWebAdvert.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSDotNetWebAdvert.Web.Controllers {
    public class AdvertManagementController : Controller {

        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;
        private readonly IMapper _mapper;

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient, IMapper mapper) {
            _fileUploader = fileUploader ?? throw new ArgumentNullException(nameof(fileUploader));
            _advertApiClient = advertApiClient ?? throw new ArgumentNullException(nameof(advertApiClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IActionResult Create(CreateAdvertViewModel model) {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile) {
            if (ModelState.IsValid) {

                //var id = "11111";
                // you must make a call to Advert Api, create the advertisement in the database and return Id
                var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                createAdvertModel.UserName = User.Identity.Name;

                var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
                var id = apiCallResponse.Id;

                bool isOkToConfirmAd = true; // optimistic just in case user does not upload image with advertisement
                string filePath = string.Empty;
                if (imageFile != null) {

                    // upload image
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    filePath = $"{id}/{fileName}";

                    try {
                        using (var readStream = imageFile.OpenReadStream()) {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                .ConfigureAwait(false);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please see the logs for details.");
                        }
                    } catch (Exception e) {

                       //Call Advert Api and cancel the advertisement
                       isOkToConfirmAd = false;
                        var confirmModel = new ConfirmAdvertRequest() {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };
                        await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                        Console.WriteLine(e);
                    }
                }

                if (isOkToConfirmAd) {
                    //Call Advert Api and confirm the advertisement
                    var confirmModel = new ConfirmAdvertRequest() {
                        Id = id,
                        FilePath = filePath,
                        Status = AdvertStatus.Active
                    };
                    await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
