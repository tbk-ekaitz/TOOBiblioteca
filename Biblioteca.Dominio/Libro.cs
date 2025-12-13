namespace Biblioteca.Dominio;

/// <summary>
/// Representa un libro físico en la biblioteca.
/// Hereda de Documento e implementa propiedades específicas.
/// </summary>
public class Libro : Documento
{
    /// <summary>
    /// Número de páginas del libro.
    /// </summary>
    public int NumeroPaginas { get; set; }

    /// <summary>
    /// Edición del libro (ej: "1ª edición", "Edición revisada").
    /// </summary>
    public string Edicion { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de encuadernación (Tapa dura, Tapa blanda, etc.).
    /// </summary>
    public string Encuadernacion { get; set; } = "Tapa blanda";

    /// <summary>
    /// Los libros tienen 21 días de préstamo estándar.
    /// </summary>
    public override int GetDiasPrestamo() => 21;

    public override string TipoDocumento => "Libro";

    public override string ToString()
    {
        return $"[Libro] {Titulo} - {Autor} ({NumeroPaginas} págs.)";
    }
}
