namespace Biblioteca.Dominio;

/// <summary>
/// Representa un empleado de la biblioteca.
/// Hereda de Usuario y añade propiedades de autenticación y rol.
/// </summary>
public class Empleado : Usuario
{
    /// <summary>
    /// Nombre de usuario para el login.
    /// </summary>
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña del empleado (en producción debería estar hasheada).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Rol del empleado que determina sus permisos en el sistema.
    /// </summary>
    public Role Rol { get; set; } = Role.PersonalSala;

    /// <summary>
    /// Fecha de contratación del empleado.
    /// </summary>
    public DateTime FechaContratacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Indica si el empleado está activo en el sistema.
    /// </summary>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Número de empleado único.
    /// </summary>
    public string NumeroEmpleado { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{NumeroEmpleado} - {NombreCompleto} ({Rol})";
    }
}
