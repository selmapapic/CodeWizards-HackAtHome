using CodeWizards.Data;
using CodeWizards.Entities;
using CodeWizards.Models;
using CodeWizards.Models.Medicine;
using CodeWizards.Models.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;

namespace CodeWizards.Controllers
{
    public class MedicineOrderController : Controller
    {
        private CWDbContext _dbContext;


        public MedicineOrderController(CWDbContext dbContext)
        {
            this._dbContext = dbContext;

        }


        public async Task<IActionResult> OrderAsync()
        {
            AllMedicine allMedicine = new AllMedicine
            {
                Medicines = await GetAllMedicine(),
            };

            return View(allMedicine);
        }

        public IActionResult OrderDone(MedOrder medOrder)
        {
            MakeOrder(medOrder);
            return View();
        }

        private async void MakeOrder(MedOrder medOrder)
        {
            List<Volunteer> volonteri = await _dbContext.Volunteers.ToListAsync();

            Patient patient = new Patient();
            patient.Name = medOrder.Name;
            patient.Telephone = medOrder.Telephone;
            patient.Location = medOrder.Address;
            patient.Email = medOrder.Email;
            patient.Emergency = medOrder.Emergency;

            _dbContext.Patients.Add(patient);
            _dbContext.SaveChanges();


            int Id = GetPatientId(patient);

            foreach (var medicineId in medOrder.Medicine)
            {
                PatientMedicineLink patientMedicineLink = new PatientMedicineLink
                {
                    PatientId = Id,
                    MedicineId = medOrder.Medicine[0]
                };

                _dbContext.PatientMedicineLinks.Add(patientMedicineLink);
                _dbContext.SaveChanges();
            }

           
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("code.wizards2021@gmail.com");

            foreach(Volunteer volonter in volonteri){
                mail.To.Add(volonter.Email);
            }
           
            mail.Subject = "Obavijest";
            mail.Body = "Poštovani, <br />" + Environment.NewLine +
                "        upravo je registrovana nova narudžba. Detalje možete provjeriti na Volonter hub-u. <br />" +
                Environment.NewLine + "CodeWizards tim";
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

        private int GetPatientId(Patient p)
        {
            Patient patient = _dbContext.Patients
                .Where(pat => pat.Name == p.Name &&
                               pat.Telephone == p.Telephone &&
                               pat.Location == p.Location &&
                               pat.Email == p.Email)
                .Select(pat => new Patient 
                { 
                    Id = pat.Id,
                    Name= pat.Name,
                    Location = pat.Location,
                    Telephone = pat.Telephone,
                    Email = pat.Email,
                    Served = pat.Served,
                    Emergency = pat.Emergency
                }).FirstOrDefault();

                return patient.Id;
        }

        private async Task<List<OneMedicine>> GetAllMedicine()
        {
            List<OneMedicine> medicines = await _dbContext.Medicines
                .Select(m => new OneMedicine
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Image = m.Image
                }).ToListAsync();

            return medicines;
        }



    }
}
