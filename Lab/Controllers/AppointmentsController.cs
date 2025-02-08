using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab.Data;
using Lab.Models.Entities;
using Lab.Models;


namespace Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public AppointmentsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ✅ Merr të gjitha takimet me detaje të plota (Doktori + Pacienti)
        [HttpGet]
        public IActionResult GetAllAppointments()
        {
            var appointments = dbContext.Appointments
                .Include(a => a.Doktori)
                .ThenInclude(d => d.Appointments)  // Siguron që të gjitha lidhjet e doktorit të ngarkohen
                .Include(a => a.Pacienti)
                .ThenInclude(p => p.Appointments)  // Siguron që të gjitha lidhjet e pacientit të ngarkohen
                .ToList();

            if (!appointments.Any())
            {
                return NotFound("Nuk ka asnjë Appointment të regjistruar.");
            }

            return Ok(appointments);
        }

        // ✅ Merr të gjitha takimet e një doktori me detaje të plota
        [HttpGet("doktor/{doktorId:guid}")]
        public IActionResult GetAppointmentsByDoktori(Guid doktorId)
        {
            var appointments = dbContext.Appointments
                .Where(a => a.DoktoriId == doktorId)
                .Include(a => a.Pacienti)
                .ToList();

            if (!appointments.Any())
            {
                return NotFound($"Nuk ka asnjë takim për Doktorin me ID {doktorId}.");
            }

            return Ok(appointments);
        }

        // ✅ Merr të gjitha takimet e një pacienti me detaje të plota
        [HttpGet("pacient/{pacientiId:guid}")]
        public IActionResult GetAppointmentsByPacienti(Guid pacientiId)
        {
            var appointments = dbContext.Appointments
                .Where(a => a.PacientiId == pacientiId)
                .Include(a => a.Doktori)
                .ToList();

            if (!appointments.Any())
            {
                return NotFound($"Nuk ka asnjë takim për Pacientin me ID {pacientiId}.");
            }

            return Ok(appointments);
        }

        // ✅ Shton një takim të ri me verifikim të Doktorit dhe Pacientit
        [HttpPost]
        public IActionResult AddAppointment(AddAppointmentDto appointmentDto)
        {
            // Verifikojmë nëse Doktori ekziston
            var doktori = dbContext.Doktoret.FirstOrDefault(d => d.Id == appointmentDto.DoktoriId);
            if (doktori == null)
            {
                return BadRequest("Doktori nuk ekziston!");
            }

            // Verifikojmë nëse Pacienti ekziston
            var pacienti = dbContext.Pacientet.FirstOrDefault(p => p.Id == appointmentDto.PacientiId);
            if (pacienti == null)
            {
                return BadRequest("Pacienti nuk ekziston!");
            }

            // ✅ Krijojmë një objekt të ri të Appointment duke përdorur DTO
            var appointment = new Appointment
            {
                Date = appointmentDto.Date,
                Status = appointmentDto.Status,
                DoktoriId = appointmentDto.DoktoriId,
                PacientiId = appointmentDto.PacientiId,
                Doktori = doktori,
                Pacienti = pacienti
            };

            dbContext.Appointments.Add(appointment);
            dbContext.SaveChanges();

            return Ok(appointment);
        }

        // ✅ Fshin një takim
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteAppointment(Guid id)
        {
            var appointment = dbContext.Appointments.Find(id);
            if (appointment == null) return NotFound();

            dbContext.Appointments.Remove(appointment);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
