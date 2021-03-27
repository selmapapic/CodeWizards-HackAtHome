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

        public VolunteerController(CWDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> VolunteerAsync()
        {
            ViewBag.Patients = await _context.Patients.ToListAsync();
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
                    //pacijent.Served = true;
                    pacijent.Emergency = false;
                    _context.Patients.Update(pacijent);
                    await _context.SaveChangesAsync();

                    MailMessage mail = new MailMessage();
                    mail.IsBodyHtml = false;
                    mail.From = new MailAddress("code.wizards2021@gmail.com");
                    mail.To.Add(pacijent.Email);

                    mail.Subject = "Status isporuke";
                    mail.Body = "Poštovani, <br /> " + Environment.NewLine +
                        "     <br />   želimo da Vas obavijestimo da je Vašu narudžbu primio naš volonter Milica. <br />" +
                        Environment.NewLine +
                        "       Možete stupiti u kontakt sa njim putem emaila: mdokic1@etf.unsa.ba. Narudžba će biti " +
                        "isporučena do kraja dana. <br />" + Environment.NewLine +
                        "        " + Environment.NewLine +
                        "      <br />  Srdačan pozdrav, <br /> " + Environment.NewLine
                              + "<br /> CodeWizards tim";
                    //mail.Body = mail.Body.Replace("@", System.Environment.NewLine);
                    mail.IsBodyHtml = true;

                    ViewBag.From = mail.From;
                    ViewBag.To = mail.To;
                    ViewBag.Subject = mail.Subject;
                    ViewBag.Body = mail.Body;

                    //TempData["mail"] = Newtonsoft.Json.JsonConvert.SerializeObject(mail);
                    //MailMessage email = Newtonsoft.Json.JsonConvert.DeserializeObject<MailMessage>((string)TempData["mail"]);
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

            return RedirectToAction("Volunteer");
        }
    }
}
