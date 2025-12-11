namespace Biblioteca.Dominio;

/// <summary>
/// Representa un ejemplar físico de un documento.
/// Un documento puede tener múltiples ejemplares.
/// </summary>
public class Ejemplar
{
    /// <summary>
    /// Código de barras único del ejemplar (identificador).
    /// </summary>
    public required string CodigoBarras { get; set; }

    /// <summary>
    /// Referencia al documento al que pertenece este ejemplar.
    /// </summary>
    public required Documento Documento { get; set; }

    /// <summary>
    /// Estado actual del ejemplar.
    /// </summary>
    public EstadoEjemplar Estado { get; set; } = EstadoEjemplar.Disponible;

    /// <summary>
    /// Ubicación física en la biblioteca (ej: "Estante A3, Fila 2").
    /// </summary>
    public string Ubicacion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de adquisición del ejemplar.
    /// </summary>
    public DateTime FechaAdquisicion { get; set; } = DateTime.Now;

    /// <summary>
    /// Observaciones sobre el estado físico del ejemplar.
    /// </summary>
    public string Observaciones { get; set; } = string.Empty;

    /// <summary>
    /// Número de veces que este ejemplar ha sido prestado.
    /// </summary>
    public int VecesPrestado { get; set; } = 0;

    /// <summary>
    /// Indica si el ejemplar está disponible para préstamo.
    /// </summary>
    public bool EstaDisponible => Estado == EstadoEjemplar.Disponible;

    /// <summary>
    /// Indica si el ejemplar está actualmente prestado.
    /// </summary>
    public bool EstaPrestado => Estado == EstadoEjemplar.Prestado;

    /// <summary>
    /// Código del documento asociado (para búsquedas).
    /// </summary>
    public string CodigoDocumento => Documento.Codigo;

    public override string ToString()
    {
        return $"{CodigoBarras} - {Documento.Titulo} ({Estado})";
    }
}
