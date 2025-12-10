namespace Biblioteca.Dominio;

/// <summary>
/// Estados posibles de un préstamo.
/// </summary>
public enum EstadoPrestamo
{
    /// <summary>
    /// Préstamo activo, pendiente de devolución.
    /// </summary>
    Activo,

    /// <summary>
    /// Préstamo devuelto correctamente.
    /// </summary>
    Devuelto,

    /// <summary>
    /// Préstamo con fecha de devolución vencida.
    /// </summary>
    Vencido,

    /// <summary>
    /// Préstamo cancelado por algún motivo.
    /// </summary>
    Cancelado
}
