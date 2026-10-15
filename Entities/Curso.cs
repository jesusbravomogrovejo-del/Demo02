using System;
using System.Collections.Generic;

namespace Demo02.Entities;

public partial class Curso
{
    public int CursoId { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Alumno> Alumno { get; set; } = new List<Alumno>();
}
