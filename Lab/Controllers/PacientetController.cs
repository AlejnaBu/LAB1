using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Lab.Data;
using Lab.Models;
using Lab.Models.Entities;

namespace Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacientetController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;

        public PacientetController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        // ✅ Merr të gjithë pacientët (Vetëm admini)
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllPacientet()
        {
            var pacientet = dbContext.Pacientet.ToList();
            return Ok(pacientet);
        }

        // ✅ Merr profilin e pacientit të autentikuar
        [HttpGet("profile")]
        [Authorize(Roles = "patient")]
        public IActionResult GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Merr ID e përdoruesit nga JWT

            var pacienti = dbContext.Pacientet.FirstOrDefault(p => p.UserId.ToString() == userId);
            if (pacienti == null) return NotFound("Pacienti nuk u gjet.");

            return Ok(pacienti);
        }

        // ✅ Shton një pacient të ri (Vetëm admini)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddPacienti([FromBody] Pacienti pacienti)
        {
            // 1️⃣ Krijo një përdorues për pacientin në AspNetUsers
            var user = new User
            {
                UserName = pacienti.Email,
                Email = pacienti.Email,
                UserRole = "patient"
            };

            var result = await userManager.CreateAsync(user, "DefaultPass@123");
            if (!result.Succeeded) return BadRequest(result.Errors);

            // 2️⃣ Lidh pacientin me UserId e përdoruesit të ri
            pacienti.UserId = user.Id; // 📌 Lidhja me përdoruesin në AspNetUsers

            dbContext.Pacientet.Add(pacienti);
            await dbContext.SaveChangesAsync();

            return Ok(pacienti);
        }

        // ✅ Përditëson të dhënat e një pacienti (Vetëm admini)
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdatePacienti(Guid id, [FromBody] Pacienti updatedPacienti)
        {
            var pacienti = dbContext.Pacientet.Find(id);
            if (pacienti == null) return NotFound();

            pacienti.Name = updatedPacienti.Name;
            pacienti.Email = updatedPacienti.Email;
            pacienti.Phone = updatedPacienti.Phone;

            dbContext.SaveChanges();
            return Ok(pacienti);
        }

        // ✅ Fshin një pacient (Vetëm admini)
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
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
