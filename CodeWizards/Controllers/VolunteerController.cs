using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeWizards.Data;
using CodeWizards.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
