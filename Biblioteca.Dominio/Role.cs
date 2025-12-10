namespace Biblioteca.Dominio;

/// <summary>
/// Roles de empleados en la biblioteca.
/// Determina los permisos y accesos en el sistema.
/// </summary>
public enum Role
{
    /// <summary>
    /// Personal de sala: gestiona préstamos y usuarios.
    /// </summary>
    PersonalSala,

    /// <summary>
    /// Personal de adquisiciones: gestiona el catálogo de documentos.
    /// </summary>
    PersonalAdquisiciones,

    /// <summary>
    /// Administrador: acceso completo al sistema.
    /// </summary>
    Administrador
}
