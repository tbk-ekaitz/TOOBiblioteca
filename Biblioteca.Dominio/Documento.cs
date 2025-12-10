namespace Biblioteca.Dominio;

/// <summary>
/// Clase abstracta base para todos los documentos de la biblioteca.
/// Implementa el polimorfismo para calcular días de préstamo según tipo.
/// </summary>
public abstract class Documento
{
    public int Id { get; set; }

    /// <summary>
    /// ISBN o código único del documento.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Editorial { get; set; } = string.Empty;

    public int AnioPublicacion { get; set; }

    public string Genero { get; set; } = string.Empty;

    public string Idioma { get; set; } = "Español";

    /// <summary>
    /// Descripción o sinopsis del documento.
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en que el documento fue añadido al catálogo.
    /// </summary>
    public DateTime FechaAlta { get; set; } = DateTime.Now;

    /// <summary>
    /// Indica si el documento está disponible para préstamo.
    /// </summary>
    public bool Disponible { get; set; } = true;

    /// <summary>
    /// Método abstracto que define los días de préstamo según el tipo de documento.
    /// Permite el polimorfismo en la lógica de negocio.
    /// </summary>
    /// <returns>Número de días permitidos para el préstamo.</returns>
    public abstract int GetDiasPrestamo();

    /// <summary>
    /// Obtiene el tipo de documento como cadena legible.
    /// </summary>
    public abstract string TipoDocumento { get; }

    public override string ToString()
    {
        return $"{Codigo} - {Titulo} ({Autor})";
    }
}
