namespace Biblioteca.Dominio;

/// <summary>
/// Representa un libro físico en la biblioteca.
/// Hereda de Documento e implementa propiedades específicas.
/// </summary>
public class Libro : Documento
{
    public int NumeroPaginas { get; set; }
    public string Edicion { get; set; } = string.Empty;
    public string Encuadernacion { get; set; } = "Tapa blanda";
    public override int GetDiasPrestamo() => 15;
    public override string TipoDocumento => "Libro";
    public override string ToString()
    {
        return $"[Libro] {Titulo} - {Autor} ({NumeroPaginas} págs.)";
    }
}
