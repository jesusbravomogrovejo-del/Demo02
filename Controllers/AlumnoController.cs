using Demo02.Contexts;
using Demo02.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Demo02.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlumnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTAR + formulario vacío (nuevo).
        // Si llega un id (/Alumno/Index/5) carga ese alumno en el formulario (editar).
        public async Task<IActionResult> Index(int? id)
        {
            var vm = new AlumnoViewModel();

            if (id.HasValue)
            {
                var alumno = await _context.Alumno.AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.AlumnoId == id.Value);
                if (alumno != null)
                {
                    vm.Alumno = alumno;
                    vm.EsEdicion = true;
                }
            }

            await CargarDatos(vm);
            return View(vm);
        }

        // CREAR / ACTUALIZAR (el mismo formulario sirve para los dos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(AlumnoViewModel vm)
        {
            var alumno = vm.Alumno;

            alumno.Codigo = alumno.Codigo?.Trim();
            alumno.Nombres = alumno.Nombres?.Trim();
            alumno.Apellidos = alumno.Apellidos?.Trim();

            // ---------- Validaciones ----------
            Requerido("Alumno.Codigo", alumno.Codigo, "El código es obligatorio.");
            Requerido("Alumno.Nombres", alumno.Nombres, "Los nombres son obligatorios.");
            Requerido("Alumno.Apellidos", alumno.Apellidos, "Los apellidos son obligatorios.");

            if (alumno.CicloId == null)
                ModelState.AddModelError("Alumno.CicloId", "Selecciona un ciclo.");

            if (alumno.CursoId == null)
                ModelState.AddModelError("Alumno.CursoId", "Selecciona un curso.");

            if (alumno.Fecha == null && (ModelState["Alumno.Fecha"]?.Errors.Count ?? 0) == 0)
                ModelState.AddModelError("Alumno.Fecha", "La fecha es obligatoria.");

            if (alumno.Beca < 0)
                ModelState.AddModelError("Alumno.Beca", "La beca no puede ser negativa.");

            // El código no puede repetirse (se excluye el propio alumno al editar)
            if (!string.IsNullOrWhiteSpace(alumno.Codigo) &&
                await _context.Alumno.AnyAsync(a => a.Codigo == alumno.Codigo && a.AlumnoId != alumno.AlumnoId))
            {
                ModelState.AddModelError("Alumno.Codigo", "Ya existe un alumno con ese código.");
            }

            if (!ModelState.IsValid)
            {
                await CargarDatos(vm);
                return View("Index", vm);
            }

            alumno.Beca ??= 0;
            alumno.Activo ??= false;

            // ---------- Guardar ----------
            try
            {
                if (vm.EsEdicion)
                {
                    _context.Alumno.Update(alumno);
                    TempData["Mensaje"] = "Alumno actualizado correctamente.";
                }
                else
                {
                    alumno.AlumnoId = 0; // lo genera SQL Server (IDENTITY)
                    _context.Alumno.Add(alumno);
                    TempData["Mensaje"] = "Alumno registrado correctamente.";
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["Error"] = "El alumno ya no existe (fue eliminado por otro usuario).";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty,
                    "No se pudo guardar: " + (ex.InnerException?.Message ?? ex.Message));
                await CargarDatos(vm);
                return View("Index", vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var alumno = await _context.Alumno.FindAsync(id);

            if (alumno == null)
            {
                TempData["Error"] = "El alumno no existe.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Alumno.Remove(alumno);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "Alumno eliminado correctamente.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se pudo eliminar el alumno.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- Auxiliares ----------

        // Llena la tabla y los combobox de ciclo y curso
        private async Task CargarDatos(AlumnoViewModel vm)
        {
            vm.Lista = await _context.Alumno
                .AsNoTracking()
                .Include(a => a.Ciclo)
                .Include(a => a.Curso)
                .OrderBy(a => a.Apellidos).ThenBy(a => a.Nombres)
                .ToListAsync();

            vm.Ciclos = await _context.Ciclo
                .AsNoTracking()
                .OrderBy(c => c.CicloId)
                .Select(c => new SelectListItem
                {
                    Value = c.CicloId.ToString(),
                    Text = c.Nombre
                })
                .ToListAsync();

            vm.Cursos = await _context.Curso
                .AsNoTracking()
                .OrderBy(c => c.Codigo)
                .Select(c => new SelectListItem
                {
                    Value = c.CursoId.ToString(),
                    Text = c.Codigo + " - " + c.Nombre
                })
                .ToListAsync();
        }

        // Agrega el error solo si el campo está vacío y no tiene ya otro error
        private void Requerido(string campo, string? valor, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(valor) && (ModelState[campo]?.Errors.Count ?? 0) == 0)
                ModelState.AddModelError(campo, mensaje);
        }

    }
}
