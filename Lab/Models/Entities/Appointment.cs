using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab.Models.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public required string Status { get; set; }

        // Lidhja me Doktorin
        [ForeignKey("DoktoriId")]
        public Guid DoktoriId { get; set; }
        public Doktori Doktori { get; set; } = null!; // Sigurohemi që nuk është null

        // Lidhja me Pacientin
        [ForeignKey("PacientiId")]
        public Guid PacientiId { get; set; }
        public Pacienti Pacienti { get; set; } = null!; // Sigurohemi që nuk është null
    }
}
