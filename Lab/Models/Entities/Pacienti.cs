using System.Text.Json.Serialization;

namespace Lab.Models.Entities
{
    public class Pacienti
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }

        // Lidhja me Appointments dhe Records
        [JsonIgnore]
        public List<Appointment>? Appointments { get; set; }
        
    }
}
