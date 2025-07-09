using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApiProducts.Models; // Para usar tu clase Producto
using System.Collections.Generic; // Para List
using System.Linq; // Para LINQ (ej. .FirstOrDefault)

namespace SimpleApiProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {

        private readonly ApplicationDbContext _context; // Declara una variable para tu contexto de DB

        // Constructor: Aquí se 'inyecta' el ApplicationDbContext
        public ProductosController(ApplicationDbContext context)
        {
            _context = context; // Asigna el contexto inyectado a tu variable privada
        }

        // --- GET: api/Productos (Obtener todos los productos de la DB) ---
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            // ToListAsync() obtiene todos los productos de la tabla Productos de forma asíncrona
            // Ahora lee de la DB, no de la lista en memoria.
            return Ok(await _context.Productos.ToListAsync());
        }

        // --- GET: api/Productos/5 (Obtener un producto por ID de la DB) ---
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            // FindAsync() busca un producto por su clave primaria (Id) en la DB
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado.");
            }

            return Ok(producto);
        }
    }
}
