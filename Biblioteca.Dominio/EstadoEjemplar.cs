namespace Biblioteca.Dominio;

/// <summary>
/// Estados posibles de un ejemplar físico.
/// </summary>
public enum EstadoEjemplar
{
    /// <summary>
    /// Disponible para préstamo.
    /// </summary>
    Disponible,

    /// <summary>
    /// Actualmente prestado.
    /// </summary>
    Prestado,

    /// <summary>
    /// En proceso de reparación o restauración.
    /// </summary>
    EnReparacion,

    /// <summary>
    /// Dado de baja del inventario.
    /// </summary>
    Baja,

    /// <summary>
    /// Reservado por un usuario.
    /// </summary>
    Reservado
}
