﻿using Amazon.Extensions.CognitoAuthentication;
using Amazon.AspNetCore.Identity.Cognito;
using AWSDotNetWebAdvert.Web.Models.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSDotNetWebAdvert.Web.Controllers {
    public class AccountsController : Controller {

        private readonly CognitoUserPool _pool;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly SignInManager<CognitoUser> _signInManager;

        public AccountsController(CognitoUserPool pool, UserManager<CognitoUser> userManager, SignInManager<CognitoUser> signInManager) {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public IActionResult Signup() {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model) {
            if (ModelState.IsValid) {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null) {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

                if (createdUser.Succeeded)
                    RedirectToAction("Confirm");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Confirm(ConfirmModel model) {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user == null) {
                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }

                var result = await ((CognitoUserManager<CognitoUser>)_userManager)
                    .ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");

                foreach (var item in result.Errors)
                    ModelState.AddModelError(item.Code, item.Description);

                return View(model);
            }

            return View(model);
        }
    }
}