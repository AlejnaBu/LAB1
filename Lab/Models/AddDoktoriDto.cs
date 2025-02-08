namespace Lab.Models
{
    public class AddDoktoriDto
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }

        public string? Specialization { get; set; }


        public int? ExperienceYears { get; set; }

    }
}
