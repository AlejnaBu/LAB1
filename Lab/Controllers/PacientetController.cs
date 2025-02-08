using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab.Data;
using Lab.Models.Entities;

namespace Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public PacientetController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Merr të gjithë pacientët
        [HttpGet]
        public IActionResult GetAllPacientet()
        {
            var pacientet = dbContext.Pacientet.ToList();
            return Ok(pacientet);
        }

        // Merr një pacient me ID
        [HttpGet("{id:guid}")]
        public IActionResult GetPacientiById(Guid id)
        {
            var pacienti = dbContext.Pacientet.Find(id);
            if (pacienti == null) return NotFound();
            return Ok(pacienti);
        }

        // Shton një pacient të ri
        [HttpPost]
        public IActionResult AddPacienti(Pacienti pacienti)
        {
            dbContext.Pacientet.Add(pacienti);
            dbContext.SaveChanges();
            return Ok(pacienti);
        }

        // Përditëson një pacient
        [HttpPut("{id:guid}")]
        public IActionResult UpdatePacienti(Guid id, Pacienti updatedPacient)
        {
            var pacienti = dbContext.Pacientet.Find(id);
            if (pacienti == null) return NotFound();

            pacienti.Name = updatedPacient.Name;
            pacienti.Email = updatedPacient.Email;
            pacienti.Phone = updatedPacient.Phone;

            dbContext.SaveChanges();
            return Ok(pacienti);
        }

        // Fshin një pacient
        [HttpDelete("{id:guid}")]
        public IActionResult DeletePacienti(Guid id)
        {
            var pacienti = dbContext.Pacientet.Find(id);
            if (pacienti == null) return NotFound();

            dbContext.Pacientet.Remove(pacienti);
            dbContext.SaveChanges();
            return Ok();
        }
    }
}
