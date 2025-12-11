namespace Biblioteca.Dominio;

/// <summary>
/// Representa un usuario/socio de la biblioteca.
/// Puede realizar préstamos de documentos.
/// </summary>
public class Usuario
{
    /// <summary>
    /// DNI del usuario - identificador único.
    /// </summary>
    public required string DNI { get; set; }

    public required string Nombre { get; set; }

    public string Apellidos { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Direccion { get; set; } = string.Empty;

    public DateTime FechaAlta { get; set; } = DateTime.Now;

    /// <summary>
    /// Indica si el usuario está sancionado y no puede realizar préstamos.
    /// </summary>
    public bool Sancionado { get; set; } = false;

    /// <summary>
    /// Fecha hasta la cual el usuario está sancionado (si aplica).
    /// </summary>
    public DateTime? FechaFinSancion { get; set; }

    /// <summary>
    /// Número máximo de préstamos simultáneos permitidos.
    /// </summary>
    public int MaxPrestamos { get; set; } = 5;

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string NombreCompleto => $"{Nombre} {Apellidos}";

    public override string ToString()
    {
        return $"{DNI} - {NombreCompleto}";
    }
}
