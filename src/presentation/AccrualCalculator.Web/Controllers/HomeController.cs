using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AppName.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AppName.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IConfiguration _configuration;

        public HomeController(
            IOptions<AppSettings> appSettings,
            IConfiguration configuration)
        {
            _appSettings = appSettings;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _appSettings.Value.Application.AppName;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel contact)
        {
            if (!ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(contact.Recaptcha))
                {
                    ViewData["Flash.Error"] = new[] {$"Recaptcha is required."};
                }
                
                return View(contact);
            }

            string recaptchaprivate = _configuration.GetSection("RECAPTCHA_PRIVATE").Value;

            using (HttpClient httpClient = new HttpClient())
            {
                var httpResponse = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaprivate}&response={contact.Recaptcha}").Result;
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    ViewData["Flash.Error"] = new[] {$"Recaptcha failed. Try again."};
                    return View(contact);
                }
            }

            var apiKey = _configuration.GetSection("SENDGRID_APIKEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(contact.Email, contact.Name);
            var to = new EmailAddress("r.rodemoyer+accrualchart@gmail.com", "Ryan Rodemoyer");

            var subject = "Accrual Calculator - Contact Mailer";
//            var plainTextContent = contact.Message;
//            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, contact.Message, contact.Message);

            Response response = await client.SendEmailAsync(msg);
            if ((int) response.StatusCode == 202)
            {
                ViewData["Flash.Success"] = new[] {"Message successfully sent!"};
            }
            else
            {
                ViewData["Flash.Error"] = new[] {$"Error sending message. Try again to send."};
                return View(contact);
            }

            ModelState.Clear();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Random()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}