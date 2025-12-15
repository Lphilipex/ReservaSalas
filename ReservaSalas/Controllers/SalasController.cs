using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Data;
using ReservaSalas.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ReservaSalas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var salas = await _context.Salas.ToListAsync();
            return View(salas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sala sala)
        {
            if (ModelState.IsValid)
            {
                _context.Salas.Add(sala);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sala);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var sala = await _context.Salas.FindAsync(id);

            if (sala == null)
                return NotFound();

            return View(sala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sala sala)
        {
            if (id != sala.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(sala);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sala);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sala = await _context.Salas.FirstOrDefaultAsync(x => x.Id == id);

            if (sala == null)
                return NotFound();

            var temReservas = await _context.Reservas.AnyAsync(r => r.SalaId == id);

            if (temReservas)
            {
                ViewData["DeleteError"] = "Essa sala possui reservas.";
            }

            return View(sala);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sala = await _context.Salas.FindAsync(id);

            if (sala == null)
                return NotFound();

            var temReservas = await _context.Reservas.AnyAsync(r => r.SalaId == id);

            if (temReservas)
            {
                ViewData["DeleteError"] = "Não é possível excluir a sala.";
                return View("Delete", sala);
            }

            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
