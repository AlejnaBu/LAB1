using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab.Data;
using Lab.Models.Entities;
using Lab.Models;
using System.Security.Claims;

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

        // ✅ Merr të gjitha takimet me detaje të plota (Vetëm admini)
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllAppointments()
        {
            var appointments = dbContext.Appointments
                .Include(a => a.Doktori)
                .Include(a => a.Pacienti)
                .ToList();

            if (!appointments.Any())
            {
                return NotFound("Nuk ka asnjë Appointment të regjistruar.");
            }

            return Ok(appointments);
        }

        // ✅ Doktori merr vetëm takimet e tij
        [HttpGet("doktor/appointments")]
        [Authorize(Roles = "doktor")]
        public IActionResult GetAppointmentsByDoktor()
        {
            var userEmail = User.Identity.Name;
            var doktori = dbContext.Doktoret.FirstOrDefault(d => d.Email == userEmail);
            if (doktori == null) return Unauthorized("Doktori nuk u gjet.");

            var appointments = dbContext.Appointments
                .Where(a => a.DoktoriId == doktori.Id)
                .Include(a => a.Pacienti)
                .ToList();

            return Ok(appointments);
        }


        // ✅ Pacienti merr vetëm takimet e tij
        [HttpGet("pacient/appointments")]
        [Authorize(Roles = "patient")]
        public IActionResult GetAppointmentsByPacient()
        {
            var userEmail = User.Identity.Name;
            var pacienti = dbContext.Pacientet.FirstOrDefault(p => p.Email == userEmail);
            if (pacienti == null) return Unauthorized("Pacienti nuk u gjet.");

            var appointments = dbContext.Appointments
                .Where(a => a.PacientiId == pacienti.Id)
                .Include(a => a.Doktori)
                .ToList();

            return Ok(appointments);
        }

        // ✅ Shton një takim të ri (Vetëm admini)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult AddAppointment(AddAppointmentDto appointmentDto)
        {
            var doktori = dbContext.Doktoret.FirstOrDefault(d => d.Id == appointmentDto.DoktoriId);
            if (doktori == null)
            {
                return BadRequest("Doktori nuk ekziston!");
            }

            var pacienti = dbContext.Pacientet.FirstOrDefault(p => p.Id == appointmentDto.PacientiId);
            if (pacienti == null)
            {
                return BadRequest("Pacienti nuk ekziston!");
            }

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

        // ✅ Fshin një takim (Vetëm admini)
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
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
