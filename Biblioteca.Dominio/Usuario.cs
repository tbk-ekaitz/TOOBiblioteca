namespace Biblioteca.Dominio;

/// <summary>
/// Representa un usuario/socio de la biblioteca.
/// Puede realizar préstamos de documentos.
/// </summary>
public class Usuario
{
    public required string DNI { get; set; }
    public required string Nombre { get; set; }
    public string Apellidos { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; } = DateTime.Now;
    public bool Sancionado { get; set; } = false;
    public DateTime? FechaFinSancion { get; set; }
    public int MaxPrestamos { get; set; } = 5;
    public string NombreCompleto => $"{Nombre} {Apellidos}";

    public override string ToString()
    {
        return $"{DNI} - {NombreCompleto}";
    }
}
