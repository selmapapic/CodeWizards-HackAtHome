using CodeWizards.Data;
using CodeWizards.Entities;
using CodeWizards.Models;
using CodeWizards.Models.Medicine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
