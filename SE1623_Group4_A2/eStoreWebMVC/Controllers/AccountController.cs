﻿using Client_eStore.Helper;
using eStoreAPI.DTOs;
using eStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

namespace eStoreWebMVC.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly HttpClient client;
        private string baseUrl;

        public AccountController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            baseUrl = "http://localhost:5273/api/account/";
        }

        [HttpGet("login", Name = "login")]
        public IActionResult Login() => View();

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginInfo info)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = await client.PostOrPutApi(info, baseUrl + "login", method: "POST");
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    var userInfo = JsonConvert.DeserializeObject<Member>(data);

                    HttpContext.Session.SetString("userEmail", userInfo.Email);
                    HttpContext.Session.SetString("userId", Convert.ToString(userInfo.MemberId));
                    return Redirect("/");
                }
                else
                {
                    ViewData["error"] = "Login failed";
                }
            }
            return View();
        }

        [HttpGet("register", Name = "register")]
        public IActionResult Register() => View();

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterInfo info)
        {
            if (ModelState.IsValid)
            {
                if (info.Password != info.ConfirmPassword)
                {
                    ViewData["error"] = "Confirm password invalid";
                    return View();
                }

                Member mbInfo = new Member();
                mbInfo.Email = info.Email;
                mbInfo.Password = info.Password;
                mbInfo.City = info.City;
                mbInfo.Country = info.Country;
                mbInfo.CompanyName = info.CompanyName;

                HttpResponseMessage res = await client.PostOrPutApi(mbInfo, baseUrl + "register", method: "POST");
                if (res.IsSuccessStatusCode)
                {
                    return Redirect("/account/login");
                }
                else
                {
                    var data = await res.Content.ReadAsStringAsync();
                    var errorMessage = JsonConvert.DeserializeObject<string>(data);
                    ViewData["error"] = errorMessage;
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userEmail");
            HttpContext.Session.Remove("userId");
            return Redirect("/");
        }
    }
}
