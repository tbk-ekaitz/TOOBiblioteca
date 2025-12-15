namespace Biblioteca.Dominio;

/// <summary>
/// Roles de empleados en la biblioteca.
/// Determina los permisos y accesos en el sistema.
/// </summary>
public enum Role
{
    // Gestiona préstamos y usuarios.
    PersonalSala,
    // Gestiona el catálogo de documentos.
    PersonalAdquisiciones,
    // Todo
    Administrador
}
