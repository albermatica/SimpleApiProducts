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
            if (_context.Productos == null)
            {
                return NotFound("No hay productos disponibles.");
            }
            return Ok(await _context.Productos.ToListAsync());
        }

        // --- GET: api/Productos/5 (Obtener un producto por ID de la DB) ---
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            // FindAsync() busca un producto por su clave primaria (Id) en la DB
            if (_context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado.");
            }

            return Ok(producto);
        }
        // --- POST: api/Productos (Crear un nuevo producto en la DB) ---
        // Este método ya lo tenías, pero asegúrate de que esté como sigue:
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto([FromBody] Producto producto)
        {
            if (_context.Productos == null)
            {
                return Problem("Entidad 'Productos' es nula.");
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, [FromBody] Producto producto)
        {
            // Verifica que el ID de la URL coincida con el ID del producto en el cuerpo de la solicitud
            if (id != producto.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del producto en el cuerpo."); // 400 Bad Request
            }

            // Indica a EF Core que el estado de esta entidad es 'Modificado'.
            // EF Core rastreará los cambios y actualizará la fila correspondiente en la DB.
            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // ¡Guarda los cambios en la base de datos!
            }
            catch (DbUpdateConcurrencyException)
            {
                // Manejo de concurrencia: si el producto no existe o ya fue modificado por otro proceso
                // Verifica si el producto realmente existe en la DB
                if (!await _context.Productos.AnyAsync(e => e.Id == id))
                {
                    return NotFound($"Producto con ID {id} no encontrado para actualizar."); // 404 Not Found
                }
                else
                {
                    throw; // Si el producto existe pero hay otro error de concurrencia, relanza la excepción.
                }
            }

            return NoContent(); // 204 No Content (Indica éxito sin devolver un cuerpo de respuesta)
        }

        // --- DELETE: api/Productos/5 (Eliminar un producto de la DB) ---
        // Añade o revisa este método
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (_context.Productos == null)
            {
                return NotFound("Entidad 'Productos' es nula.");
            }

            // Busca el producto por ID para asegurar que existe antes de intentar eliminarlo
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado para eliminar."); // 404 si no existe
            }

            _context.Productos.Remove(producto); // Marca el producto para ser eliminado
            await _context.SaveChangesAsync(); // ¡Guarda los cambios en la base de datos!

            return NoContent(); // 204 No Content
        }
    }
}
