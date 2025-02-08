using System.Text.Json.Serialization;

namespace Lab.Models.Entities
{
    public class Doktori
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set;  }


        public string? Specialization { get; set; }

        public int? ExperienceYears { get; set; }


        // Lidhja me Appointment
        [JsonIgnore]
        public List<Appointment>? Appointments { get; set; }

    }
}
