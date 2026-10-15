using System;
using System.Collections.Generic;

namespace Demo02.Entities;

public partial class Alumno
{
    public int AlumnoId { get; set; }

    public string? Codigo { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public int? CicloId { get; set; }

    public int? CursoId { get; set; }

    public decimal? Beca { get; set; }

    public DateTime? Fecha { get; set; }

    public bool? Activo { get; set; }

    public virtual Ciclo? Ciclo { get; set; }

    public virtual Curso? Curso { get; set; }
}
