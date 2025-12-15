using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservaSalas.Data;
using ReservaSalas.Data.ReservaSalas.Models;
using ReservaSalas.Models;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class ReservasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public ReservasController(ApplicationDbContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var reservas = _context.Reservas
            .Include(r => r.Sala)
            .Include(r => r.Usuario)
            .AsQueryable();

        if (!User.IsInRole("Admin"))
        {
            reservas = reservas.Where(r => r.UsuarioId == user.Id);
        }

        return View(await reservas.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var reserva = await _context.Reservas
            .Include(r => r.Sala)
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (!User.IsInRole("Admin") && reserva.UsuarioId != user.Id)
            return Forbid();

        return View(reserva);
    }

    public IActionResult Create()
    {
        ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nome");
        return View();
    }

    private async Task<bool> ReservaConflita(Reserva reserva)
    {
        return await _context.Reservas.AnyAsync(r =>
            r.SalaId == reserva.SalaId &&
            r.DataReserva == reserva.DataReserva &&
            r.Id != reserva.Id &&
            (
                (reserva.HoraInicio >= r.HoraInicio && reserva.HoraInicio < r.HoraFim) ||
                (reserva.HoraFim > r.HoraInicio && reserva.HoraFim <= r.HoraFim) ||
                (reserva.HoraInicio <= r.HoraInicio && reserva.HoraFim >= r.HoraFim)
            ));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        var user = await _userManager.GetUserAsync(User);
        reserva.UsuarioId = user.Id;

        if (ModelState.IsValid)
        {
            if (reserva.DataReserva < System.DateTime.Today)
            {
                ModelState.AddModelError("DataReserva", "Data inválida.");
            }
            else if (reserva.HoraFim <= reserva.HoraInicio)
            {
                ModelState.AddModelError("HoraFim", "Hora final inválida.");
            }
            else if (await ReservaConflita(reserva))
            {
                ModelState.AddModelError("", "Horário já reservado.");
            }
            else
            {
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
        return View(reserva);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null)
            return NotFound();

        if (!User.IsInRole("Admin"))
            return Forbid();

        ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
        return View(reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Reserva reserva)
    {
        if (id != reserva.Id)
            return NotFound();

        var reservaBanco = await _context.Reservas.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservaBanco == null)
            return NotFound();

        reserva.UsuarioId = reservaBanco.UsuarioId;

        if (ModelState.IsValid)
        {
            if (reserva.HoraFim <= reserva.HoraInicio)
            {
                ModelState.AddModelError("HoraFim", "Hora final inválida.");
            }
            else if (await ReservaConflita(reserva))
            {
                ModelState.AddModelError("", "Horário já reservado.");
            }
            else
            {
                _context.Update(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
        return View(reserva);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var reserva = await _context.Reservas
            .Include(r => r.Sala)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reserva == null)
            return NotFound();

        if (!User.IsInRole("Admin"))
            return Forbid();

        return View(reserva);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reserva = await _context.Reservas.FindAsync(id);
        if (reserva == null)
            return RedirectToAction(nameof(Index));

        if (!User.IsInRole("Admin"))
            return Forbid();

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
