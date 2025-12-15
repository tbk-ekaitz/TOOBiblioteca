namespace Biblioteca.Dominio;

/// <summary>
/// Clase abstracta base para todos los documentos de la biblioteca.
/// Implementa el polimorfismo para calcular días de préstamo según tipo.
/// </summary>
public abstract class Documento
{
    private string _codigo;
    public required string Codigo
    {
        get => _codigo;
        set => _codigo = value?.Replace("-", "").Replace(" ", "").Trim() ?? string.Empty;
    }

    public required string Titulo { get; set; }
    public required string Autor { get; set; }
    public string Editorial { get; set; } = string.Empty;
    public int AnioPublicacion { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Idioma { get; set; } = "Español";
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; } = DateTime.Now;
    public required Empleado EmpleadoAlta { get; set; }
    public bool Disponible { get; set; } = true;
    public abstract int GetDiasPrestamo();
    public abstract string TipoDocumento { get; }

    public override string ToString()
    {
        return $"{Codigo} - {Titulo} ({Autor})";
    }
}
