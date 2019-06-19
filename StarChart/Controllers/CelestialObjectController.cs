using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject is null)
            {
                return NotFound();
            }

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            foreach (var co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.OrbitedObjectId == null);

            foreach (var co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject celestialObject)
        {
            var co = _context.CelestialObjects.Find(id);

            if (co is null)
            {
                return NotFound();
            }

            co.Name = celestialObject.Name;
            co.OrbitalPeriod = celestialObject.OrbitalPeriod;
            co.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(co);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var co = _context.CelestialObjects.Find(id);

            if (co is null)
            {
                return NotFound();
            }

            co.Name = name;

            _context.CelestialObjects.Update(co);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);

            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
