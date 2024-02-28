namespace SistemaMedico.Models.User
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<Pet> Pets { get; set; } = new();
    }
}
