using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace SistemaMedico.Models.User
{
    public class AppointmentModel
    {
        public int Id { get; set; }
        [ValidateNever]
        public int UserId { get; set; }
        public int DoctorId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public TimeOnly Time { get; set; }
        public string Receta { get; set; } = "";
    }
}
