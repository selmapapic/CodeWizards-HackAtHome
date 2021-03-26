using CodeWizards.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeWizards.Data
{
    public class CWDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<PatientMedicineLink> PatientMedicineLinks { get; set; }

        public CWDbContext(DbContextOptions<CWDbContext> options) : base(options)
        {}


    }
}
