namespace Biblioteca.Dominio;

/// <summary>
/// Representa un empleado de la biblioteca.
/// Hereda de Usuario y añade propiedades de autenticación y rol.
/// </summary>
public class Empleado : Usuario
{
    public string NombreUsuario { get; set; } = string.Empty;
    public required string Password { get; set; }
    public required Role Rol { get; set; }
    public DateTime FechaContratacion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
    public string NumeroEmpleado { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{NumeroEmpleado} - {NombreCompleto} ({Rol})";
    }
}
