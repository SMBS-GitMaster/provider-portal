using System;
using System.Collections.Generic;

namespace PortalProveedor.Entities;

/// <summary>
/// Cliente de la plataforma, suele ser una empresa o profesional. Es a quien le vendemos la plataforma.
/// </summary>
public partial class Cliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public DateTime FechaAlta { get; set; }

    public string Email { get; set; } = null!;

    public string Identificador { get; set; } = null!;

    public string TipoIdentificador { get; set; } = null!;

    public virtual ICollection<Sociedad> Sociedads { get; set; } = new List<Sociedad>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
