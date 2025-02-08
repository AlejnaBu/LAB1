using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lab.Data;
using Lab.Models;
using Lab.Models.Entities;

namespace Lab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoktoretController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public DoktoretController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public IActionResult GetAllDoktoret() {
            var allDoktoret = dbContext.Doktoret.ToList();
            return Ok(allDoktoret);
        }


        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetDoktoriById(Guid id) {
            var doktori = dbContext.Doktoret.Find(id);

            if (doktori is null) {
                return NotFound();
            }
            return Ok(doktori);
        }

        [HttpPost]
        public IActionResult AddDoktori(AddDoktoriDto addDoktoriDto) {


            var doktoriEntity = new Doktori() {
                Name = addDoktoriDto.Name,
                Email = addDoktoriDto.Email,
                Phone = addDoktoriDto.Phone,
                Specialization = addDoktoriDto.Specialization,  
                ExperienceYears = addDoktoriDto.ExperienceYears
            };



            dbContext.Doktoret.Add(doktoriEntity);
            dbContext.SaveChanges();

            return Ok(doktoriEntity);

        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateDoktori(Guid id, UpdateDoktoriDto updateDoktoriDto) {
            var doktori = dbContext.Doktoret.Find(id);
            if (doktori is null) {
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

        [HttpDelete]
        [Route("{id:guid}")]

        public IActionResult DeleteDoktori(Guid id) {
            var doktori = dbContext.Doktoret.Find(id);

            if (doktori is null) {
                return NotFound();
           }

            dbContext.Doktoret.Remove(doktori);
            dbContext.SaveChanges();

            return Ok();

        }

    }
}
