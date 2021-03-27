using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeWizards.Data;
using CodeWizards.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CodeWizards.Controllers
{
    public class VolunteerController : Controller
    {

        private readonly CWDbContext _context;
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public VolunteerController(CWDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> VolunteerAsync()
        {
            List<Patient> pacijenti = await _context.Patients.ToListAsync();
            
            ViewBag.Patients = pacijenti.OrderByDescending(pacijent => pacijent.Emergency).ToList();
            ViewBag.Medicine = await _context.Medicines.ToListAsync();
            ViewBag.LinkList = await _context.PatientMedicineLinks.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusAsync(int patientId)
        {
            List<Patient> pacijenti = await _context.Patients.ToListAsync();
            List<Volunteer> volonteri = await _context.Volunteers.ToListAsync();

            foreach (Patient pacijent in pacijenti){
                if(pacijent.Id == patientId)
                {
                    pacijent.Served = true;
                    pacijent.Emergency = false;
                    _context.Patients.Update(pacijent);
                    await _context.SaveChangesAsync();

                    MailMessage mail = new MailMessage();
                    mail.IsBodyHtml = false;
                    mail.From = new MailAddress("code.wizards2021@gmail.com");
                    mail.To.Add(pacijent.Email);

                    mail.Subject = "Status isporuke";
                    mail.Body = "Poštovani, <br /> " + Environment.NewLine +
                        "     <br />   želimo da Vas obavijestimo da je Vašu narudžbu primio naš volonter. <br />" +
                        Environment.NewLine +
                        "       Za sva pitanja možete nam se javiti putem emaila: code.wizards2021@gmail.com ili na broj telefona: +387 62 531 942. Narudžba će biti " +
                        "isporučena do kraja dana. <br />" + Environment.NewLine +
                        "        " + Environment.NewLine +
                        "      <br />  Srdačan pozdrav, <br /> " + Environment.NewLine
                              + " <br /> CodeWizards tim";

                    mail.IsBodyHtml = true;

                    ViewBag.From = mail.From;
                    ViewBag.To = mail.To;
                    ViewBag.Subject = mail.Subject;
                    ViewBag.Body = mail.Body;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential("code.wizards2021@gmail.com", "hakaton2021!");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                }
            }

            TempData["ShowAlert"] = "show";
            return RedirectToAction("Volunteer");
        }

        public IActionResult LoginVolunteer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterVolunteerAsync(string imePrezime, string email)
        {
            Volunteer noviVolonter = new Volunteer();
            noviVolonter.Name = imePrezime;
            noviVolonter.Email = email;
            noviVolonter.AccessCode = RandomString(8);

            _context.Volunteers.Add(noviVolonter);
            await _context.SaveChangesAsync();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("code.wizards2021@gmail.com");
            mail.To.Add(noviVolonter.Email);

            mail.Subject = "Pristupni kod";
            mail.Body = "Poštovani, <br />" + Environment.NewLine +
                "        u nastavku maila Vam šaljemo pristupni kod za Volonter hub. <br />" +
                Environment.NewLine +
                "       Za sva eventualna pitanja nam se možete obratiti na mail: code.wizards2021@gmail.com ili na broj telefona: +387 62 531 942. <br />" + Environment.NewLine +
                "        " + Environment.NewLine + "PRISTUPNI KOD: " + noviVolonter.AccessCode +
                "       <br /> Dobrodošli u naš tim! <br />" + Environment.NewLine
                      + "CodeWizards tim";
            mail.IsBodyHtml = true;

            ViewBag.From = mail.From;
            ViewBag.To = mail.To;
            ViewBag.Subject = mail.Subject;
            ViewBag.Body = mail.Body;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new System.Net.NetworkCredential("code.wizards2021@gmail.com", "hakaton2021!");
            smtp.EnableSsl = true;
            smtp.Send(mail);

            TempData["ShowAlert"] = "show";

            return RedirectToAction("LoginVolunteer");
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string kod)
        {
            List<Volunteer> volonteri = await _context.Volunteers.ToListAsync();
            Boolean postoji = false;
            
            foreach(Volunteer volonter in volonteri)
            {
                if (volonter.AccessCode.Equals(kod)){
                    postoji = true;
                    break;
                }
            }
            if (postoji)
            {
                return RedirectToAction("Volunteer");
            }
            else
            {
                return RedirectToAction("LoginVolunteer");
            }
        }
    }
}
