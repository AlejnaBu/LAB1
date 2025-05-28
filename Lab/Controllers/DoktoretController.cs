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
    [Authorize(Roles = "admin,doktor")]
    public class DoktoretController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<User> userManager;

        public DoktoretController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        // ✅ Merr të gjithë doktorët (Vetëm admini)
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllDoktoret()
        {
            var allDoktoret = dbContext.Doktoret.ToList();
            return Ok(allDoktoret.Select(d => new
            {
                id = d.Id,
                name = d.Name,
                email = d.Email,
                phone = d.Phone,
                specialization = d.Specialization,
                experienceYears = d.ExperienceYears
            }));
        }

        // ✅ Merr profilin e doktorit të autentikuar
        [HttpGet("profile")]
        [Authorize(Roles = "doktor")]
        public IActionResult GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Merr ID e përdoruesit nga JWT

            var doktori = dbContext.Doktoret.FirstOrDefault(d => d.UserId.ToString() == userId);
            if (doktori == null) return NotFound("Doktori nuk u gjet.");

            return Ok(doktori);
        }

        [Authorize(Roles = "admin")]

        // ✅ Shton një doktor të ri (Vetëm admini)
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> AddDoktori(AddDoktoriDto addDoktoriDto)
        {
            // 1️⃣ Krijo një përdorues të ri për doktorin në AspNetUsers
            var user = new User
            {
                UserName = addDoktoriDto.Email,
                Email = addDoktoriDto.Email,
                UserRole = "doktor"
            };

            var result = await userManager.CreateAsync(user, "DefaultPass@123"); // 📌 Sigurohu që passwordi është valid
            if (!result.Succeeded) return BadRequest(result.Errors);

            // 2️⃣ Krijo doktorin dhe lidhe me UserId e përdoruesit të ri
            var doktoriEntity = new Doktori
            {
                Name = addDoktoriDto.Name,
                Email = addDoktoriDto.Email,
                Phone = addDoktoriDto.Phone,
                Specialization = addDoktoriDto.Specialization,
                ExperienceYears = addDoktoriDto.ExperienceYears,
                UserId = user.Id  // 📌 Lidhja me përdoruesin në AspNetUsers
            };

            dbContext.Doktoret.Add(doktoriEntity);
            await dbContext.SaveChangesAsync();

            return Ok(doktoriEntity);
        }

        // ✅ Përditëson të dhënat e një doktori
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateDoktori(Guid id, UpdateDoktoriDto updateDoktoriDto)
        {
            var doktori = dbContext.Doktoret.Find(id);
            if (doktori is null)
            {
                return NotFound();
            }

            doktori.Name = updateDoktoriDto.Name;
            doktori.Email = updateDoktoriDto.Email;
            doktori.Phone = updateDoktoriDto.Phone;
            doktori.Specialization = updateDoktoriDto.Specialization;
            doktori.ExperienceYears = updateDoktoriDto.ExperienceYears;

            dbContext.SaveChanges();

            return Ok(doktori);
        }
        [Authorize(Roles = "admin")]
        // ✅ Fshin një doktor (Vetëm admini)
        [HttpDelete("{id:guid}")]
       // [Authorize(Roles = "admin")]
        public IActionResult DeleteDoktori(Guid id)
        {
            var doktori = dbContext.Doktoret.Find(id);

            if (doktori is null)
            {
                return NotFound();
            }

            dbContext.Doktoret.Remove(doktori);
            dbContext.SaveChanges();

            return Ok();
        }
    }
}
