using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaMedico.Data;
using SistemaMedico.Models;
using SistemaMedico.Models.Doctor;
using SistemaMedico.Models.User;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SistemaMedico.Controllers
{
    public class UserController : Controller
    {
        private readonly SistemaMedicoContext _context;

        public UserController(SistemaMedicoContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return _context.User != null ?
                        View(await _context.User.ToListAsync()) :
                        Problem("Entity set 'SistemaMedicoContext.User'  is null.");
        }

        // GET: Users/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = _context.User
                .FirstOrDefault(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Rol")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Rol")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = _context.User
                .FirstOrDefault(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'SistemaMedicoContext.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            var user = _context.User.FirstOrDefault(user => user.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(
                new ProfileModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    Appointments = _context.Appointment.Where(a => a.UserId == user.Id).ToList(),
                    Pets = _context.Pet.Where(p => p.Owner == userId).ToList(),
                });
        }

        public IActionResult DoctorProfile()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioId");
            var user = _context.User.FirstOrDefault(user => user.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(
                new ProfileModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    Appointments = _context.Appointment.Where(a => a.DoctorId == user.Id).ToList()
                });
        }

        public IActionResult RegisterReceta(int appointmentId)
        {
            
            return View();
        }

        public IActionResult RegisterDoctor()
        {
            var model = new RegisterDoctorModel
            {
                Specialities = GetSpecialities(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult RegisterDoctor(RegisterDoctorModel model)
        {
            model.Specialities = GetSpecialities();
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
                return View(model);
            }

            if (_context.User.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este email");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var especialidadId = _context.Speciality
                    .Where(e => e.Id == model.SpecialityId)
                    .Select(e => e.Id)
                    .FirstOrDefault();
                var user = new User { Name = model.Name, Email = model.Email, Password = model.Password, Rol = 1 };
                _context.User.Add(user);
                _context.SaveChanges();
                var doctor = new UserSpeciality { Id = user.Id, SpecialityId = especialidadId };
                _context.UserSpeciality.Add(doctor);
                _context.SaveChanges();

                return RedirectToAction("DoctorProfile", "User");
            }

            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");
                return View(model);
            }

            if (_context.User.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con este email");
                return View(model);
            }

            var nuevoUsuario = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = model.Password,
                Rol = 0
            };

            _context.User.Add(nuevoUsuario);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var usuarioAutenticado = AutenticarUsuario(model.Email, model.Password);

            if (usuarioAutenticado != null)
            {
                HttpContext.Session.SetInt32("UsuarioId", usuarioAutenticado.Id);
                HttpContext.Session.SetString("UsuarioNombre", usuarioAutenticado.Name);
                HttpContext.Session.SetInt32("UsuarioRol", usuarioAutenticado.Rol);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("Password", "Credenciales inválidas");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UsuarioId");
            HttpContext.Session.Remove("UsuarioNombre");
            HttpContext.Session.Remove("UsuarioRol");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Appointment()
        {
            ViewBag.DoctorList = new SelectList(_context.User.Where(user => user.Rol == 1), "Id", "Name");
            ViewBag.Specialities = new SelectList(_context.Speciality.ToList(), "Id", "Name");
            return View(new Appointment { Date = DateTime.Now });
        }

        [HttpPost]
        public IActionResult Appointment(Appointment model)
        {
            if (FechaHoraDisponible(model.Date))
            {
                var nuevaCita = new Models.Appointment
                {
                    UserId = HttpContext.Session.GetInt32("UsuarioId").GetValueOrDefault(),
                    DoctorId = model.DoctorId,
                    Date = model.Date,
                    Time = model.Time,
                    Receta = ""
                };

                _context.Appointment.Add(nuevaCita);
                _context.SaveChanges();

                TempData["StatusMessage"] = "Cita agendada con éxito.";
                return RedirectToAction("Perfil");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "La fecha y hora seleccionadas no están disponibles. Por favor, elige otra.");
            }
            ViewBag.DoctorList = new SelectList(_context.User.Where(user => user.Rol == 1), "Id", "Name", model.DoctorId);
            return View(model);
        }

        private bool FechaHoraDisponible(DateTime fechaCita)
        {
            var fecha = new DateTime(fechaCita.Year, fechaCita.Month, fechaCita.Day);
            var now = DateTime.Now;
            if (fecha < new DateTime(now.Year, now.Month, now.Day))
            {
                return false;
            }
            return true;
        }

        private User? AutenticarUsuario(string email, string contraseña)
        {
            return _context.User.FirstOrDefault(u => u.Email == email && u.Password == contraseña);
        }



        private List<Speciality> GetSpecialities()
        {
            return _context.Speciality.ToList();
        }

        public IActionResult CreatePet()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePet(Pet pet)
        {
            pet.Owner = HttpContext.Session.GetInt32("UsuarioId").GetValueOrDefault();
            _context.Pet.Add(pet);
            _context.SaveChanges();

            return RedirectToAction("Profile");
        }
    }
}
