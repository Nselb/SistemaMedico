using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaMedico.Models.Doctor;
using SistemaMedico.Models.User;

namespace SistemaMedico.Data
{
    public class SistemaMedicoContext : DbContext
    {
        public SistemaMedicoContext (DbContextOptions<SistemaMedicoContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<Speciality> Speciality { get; set; } = default!;
        public DbSet<UserSpeciality> UserSpeciality { get; set; } = default!;
        public DbSet<Models.Appointment> Appointment { get; set; }
        public DbSet<Pet> Pet { get; set; }
    }
}
