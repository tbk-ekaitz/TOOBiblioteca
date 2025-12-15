namespace Biblioteca.Dominio;

/// <summary>
/// Estados posibles de un préstamo.
/// Si, cambie los nombres, me liaba entre Finalizado y Cancelado. Y no me gusta tener que estar haciendo cAmElCaSe...
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
