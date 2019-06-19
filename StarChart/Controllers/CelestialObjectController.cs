﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;

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
    }
}
