using Demo02.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo02.Models
{
    public class AlumnoViewModel
    {
        // Alumno que se muestra en el formulario
        public Alumno Alumno { get; set; } = new Alumno { Fecha = DateTime.Today, Activo = true };

        // false = creando, true = editando
        public bool EsEdicion { get; set; }

        // Datos para la tabla
        public List<Alumno> Lista { get; set; } = new();

        // Datos para los combobox
        public List<SelectListItem> Ciclos { get; set; } = new();
        public List<SelectListItem> Cursos { get; set; } = new();
    }
}
