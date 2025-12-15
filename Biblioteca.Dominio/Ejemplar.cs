namespace Biblioteca.Dominio;

/// <summary>
/// Representa un ejemplar físico de un documento.
/// Un documento puede tener múltiples ejemplares.
/// </summary>
public class Ejemplar
{
    public required string CodigoBarras { get; set; }
    public required Documento Documento { get; set; }
    public EstadoEjemplar Estado { get; set; } = EstadoEjemplar.Disponible;
    public string Ubicacion { get; set; } = string.Empty;
    public DateTime FechaAdquisicion { get; set; } = DateTime.Now;
    public required Empleado EmpleadoAlta { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int VecesPrestado { get; set; } = 0;
    public bool EstaDisponible => Estado == EstadoEjemplar.Disponible;
    public bool EstaPrestado => Estado == EstadoEjemplar.Prestado;

    // para búsquedas
    public string CodigoDocumento => Documento.Codigo;

    public override string ToString()
    {
        return $"{CodigoBarras} - {Documento.Titulo} ({Estado})";
    }
}
