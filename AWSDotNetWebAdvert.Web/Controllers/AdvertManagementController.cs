using AWSDotNetWebAdvert.Web.Models.AdvertManagement;
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

        public AdvertManagementController(IFileUploader fileUploader) {
            _fileUploader = fileUploader;
        }

        public IActionResult Create(CreateAdvertViewModel model) {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile) {
            if (ModelState.IsValid) {

                var id = "11111";
                // you must make a call to Advert Api, create the advertisement in the database and return Id

                //var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                //createAdvertModel.UserName = User.Identity.Name;

                //var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
                //var id = apiCallResponse.Id;

                bool isOkToConfirmAd = true;
                string filePath = string.Empty;
                if (imageFile != null) {
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

                        // Call Advert Api and confirm the advertisement

                        return RedirectToAction("Index", "Home");

                    } catch (Exception e) {

                        // Call Advert Api and cancel the advertisement

                        //isOkToConfirmAd = false;
                        //var confirmModel = new ConfirmAdvertRequest() {
                        //    Id = id,
                        //    FilePath = filePath,
                        //    Status = AdvertStatus.Pending
                        //};
                        //await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);

                        Console.WriteLine(e);
                    }


                }

                //if (isOkToConfirmAd) {
                //    var confirmModel = new ConfirmAdvertRequest() {
                //        Id = id,
                //        FilePath = filePath,
                //        Status = AdvertStatus.Active
                //    };
                //    await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                //}

                //return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
