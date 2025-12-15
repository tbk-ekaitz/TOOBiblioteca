namespace Biblioteca.Dominio;

/// <summary>
/// Estados posibles de un ejemplar físico.
/// </summary>
public enum EstadoEjemplar
{
    Disponible,
    Prestado,
    EnReparacion, // No usado
    Baja,
    Reservado
}
