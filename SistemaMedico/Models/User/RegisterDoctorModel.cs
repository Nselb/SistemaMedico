using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SistemaMedico.Models.Doctor;
using System.ComponentModel.DataAnnotations;

namespace SistemaMedico.Models.User
{
    public class RegisterDoctorModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public int SpecialityId { get; set; }
        [ValidateNever]
        public List<Speciality> Specialities { get; set; }
    }
}
