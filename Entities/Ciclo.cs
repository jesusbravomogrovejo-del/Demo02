using System;
using System.Collections.Generic;

namespace Demo02.Entities;

public partial class Ciclo
{
    public int CicloId { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Alumno> Alumno { get; set; } = new List<Alumno>();
}
