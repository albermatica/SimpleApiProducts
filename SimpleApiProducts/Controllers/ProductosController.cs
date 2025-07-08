using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleApiProducts.Models; // Para usar tu clase Producto
using System.Collections.Generic; // Para List
using System.Linq; // Para LINQ (ej. .FirstOrDefault)

namespace SimpleApiProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private static List<Producto> _productos = new List<Producto>
        {
            new Producto(1, "Laptop XYZ", 1200.00, 50),
            new Producto(2, "Mouse ergonómico", 25.50, 200),
            new Producto(3, "Teclado mecánico", 75.00, 150)
        };
        [HttpGet]
        public ActionResult<IEnumerable<Producto>> GetProductos()
        {
            return Ok(_productos); // Devuelve todos los productos de la lista
        }

        // --- GET: api/Productos/5 (Obtener un producto por ID) ---
        [HttpGet("{id}")]
        public ActionResult<Producto> GetProducto(int id)
        {
            // Busca el producto en la lista por su Id
            var producto = _productos.FirstOrDefault(p => p.Id == id);

            if (producto == null)
            {
                return NotFound($"Producto con ID {id} no encontrado."); // Devuelve 404 si no se encuentra
            }

            return Ok(producto); // Devuelve 200 con el producto encontrado
        }

        // --- POST: api/Productos (Crear un nuevo producto) ---
        [HttpPost]
        public IActionResult PostProducto([FromBody] Producto nuevoProducto)
        {
            // Para este ejemplo simple, asignamos un Id básico.
            // En un escenario real, la DB se encargaría de esto.
            nuevoProducto.Id = _productos.Any() ? _productos.Max(p => p.Id) + 1 : 1;
            _productos.Add(nuevoProducto);

            return CreatedAtAction(nameof(GetProducto), new { id = nuevoProducto.Id }, nuevoProducto);
        }
    }
}
