﻿using Firebase.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FireBaseTesting.Models;

namespace TestingProject.Controllers
{
    public class AccountController : Controller
    {
        private static string ApiKey = "Api key here";
        private static string Bucket = "Path here";
        // GET: Account

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.Name, true);
                ModelState.AddModelError(string.Empty, "Please Verify your email then login Plz.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.
                if (this.Request.IsAuthenticated)
                {

                    //  return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // Info.
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    if (token != "")
                    {

                        this.SignInUser(user.Email, token, false);
                        return this.RedirectToLocal(returnUrl);

                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        private void SignInUser(string email, string token, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        private void ClaimIdentities(string username, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();
            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            }
            catch (Exception ex)
            {
                // Info
                ModelState.AddModelError(string.Empty, ex.Message);

            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // Info.
            return this.RedirectToAction("LogOff", "Account");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

    }
}
