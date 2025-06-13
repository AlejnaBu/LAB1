namespace Lab.Models
{
    public class AddPacientiDto
    {
        public required string Name { get; set; }

        public required string Email { get; set; }

        public string? Phone { get; set; }
    }
}
