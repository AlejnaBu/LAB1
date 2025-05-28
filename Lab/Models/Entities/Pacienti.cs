using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Lab.Models;

namespace Lab.Models.Entities
{
    public class Pacienti
    {
        public Guid Id { get; set; }  // Id e pacientit

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        // Lidhja me User
        public Guid UserId { get; set; }  // Lidhja me User
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        // Lidhja me Appointment
        [JsonIgnore]
        public List<Appointment>? Appointments { get; set; }
    }
}
