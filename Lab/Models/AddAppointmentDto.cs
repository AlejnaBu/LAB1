namespace Lab.Models
{
    public class AddAppointmentDto
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public required string Status { get; set; }
        public Guid DoktoriId { get; set; }
        public Guid PacientiId { get; set; }
    }
}
