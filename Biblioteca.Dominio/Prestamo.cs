namespace Biblioteca.Dominio;

/// <summary>
/// Representa un préstamo de uno o más ejemplares a un usuario.
/// Implementa el patrón Maestro-Detalle (un préstamo, múltiples ejemplares).
/// </summary>
public class Prestamo
{
    /// <summary>
    /// Identificador único del préstamo (ej: "PRE-2024-001").
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Referencia al usuario que realiza el préstamo.
    /// </summary>
    public required Usuario Usuario { get; set; }

    /// <summary>
    /// Referencia al empleado que registra el préstamo.
    /// </summary>
    public required Empleado Empleado { get; set; }

    /// <summary>
    /// Lista de ejemplares incluidos en este préstamo (Detalle del Maestro-Detalle).
    /// </summary>
    public List<Ejemplar> Ejemplares { get; set; } = new List<Ejemplar>();

    /// <summary>
    /// Fecha y hora en que se realizó el préstamo.
    /// </summary>
    public DateTime FechaPrestamo { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha límite para la devolución.
    /// </summary>
    public DateTime FechaDevolucionPrevista { get; set; }

    /// <summary>
    /// Fecha real de devolución (null si no se ha devuelto).
    /// </summary>
    public DateTime? FechaDevolucionReal { get; set; }

    /// <summary>
    /// Estado actual del préstamo.
    /// </summary>
    public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Activo;

    /// <summary>
    /// Observaciones adicionales sobre el préstamo.
    /// </summary>
    public string Observaciones { get; set; } = string.Empty;

    /// <summary>
    /// DNI del usuario (para búsquedas).
    /// </summary>
    public string UsuarioDNI => Usuario.DNI;

    /// <summary>
    /// Indica si el préstamo está vencido.
    /// </summary>
    public bool EstaVencido => Estado == EstadoPrestamo.Activo && DateTime.Now > FechaDevolucionPrevista;

    /// <summary>
    /// Días de retraso en la devolución (0 si no hay retraso).
    /// </summary>
    public int DiasRetraso
    {
        get
        {
            if (Estado != EstadoPrestamo.Activo) return 0;
            var dias = (DateTime.Now - FechaDevolucionPrevista).Days;
            return dias > 0 ? dias : 0;
        }
    }

    /// <summary>
    /// Número de ejemplares en el préstamo.
    /// </summary>
    public int NumeroEjemplares => Ejemplares.Count;

    /// <summary>
    /// Calcula la fecha de devolución basándose en el documento con más días de préstamo.
    /// Utiliza el polimorfismo de Documento.GetDiasPrestamo().
    /// </summary>
    public void CalcularFechaDevolucion()
    {
        if (Ejemplares.Count == 0)
        {
            FechaDevolucionPrevista = FechaPrestamo.AddDays(14);
            return;
        }

        int maxDias = Ejemplares
            .Select(e => e.Documento.GetDiasPrestamo())
            .Max();

        FechaDevolucionPrevista = FechaPrestamo.AddDays(maxDias);
    }

    public override string ToString()
    {
        return $"{Id} - {Usuario.NombreCompleto} ({NumeroEjemplares} ejemplares) - {Estado}";
    }
}
