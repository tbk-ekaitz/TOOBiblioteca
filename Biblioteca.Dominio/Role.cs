namespace Biblioteca.Dominio;

/// <summary>
/// Roles de empleados en la biblioteca.
/// Determina los permisos y accesos en el sistema.
/// </summary>
public enum Role
{
    // préstamos y usuarios.
    PersonalSala,
    // catálogo de documentos.
    PersonalAdquisiciones,
    // todo
    Administrador
}
