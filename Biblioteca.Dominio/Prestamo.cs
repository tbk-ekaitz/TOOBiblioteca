namespace Biblioteca.Dominio;

/// <summary>
/// Representa un préstamo a un usuario.
/// </summary>
public class Prestamo
{
    public required string Id { get; set; }
    public required Usuario Usuario { get; set; }
    public required Empleado Empleado { get; set; }
    public List<Ejemplar> Ejemplares { get; set; } = new List<Ejemplar>();
    public DateTime FechaPrestamo { get; set; } = DateTime.Now;
    public DateTime FechaDevolucionPrevista { get; set; }
    public DateTime? FechaDevolucionReal { get; set; }
    public EstadoPrestamo Estado { get; set; } = EstadoPrestamo.Activo;
    public string Observaciones { get; set; } = string.Empty;
    public string UsuarioDNI => Usuario.DNI;
    public bool EstaVencido => Estado == EstadoPrestamo.Activo && DateTime.Now >= FechaDevolucionPrevista;

    public int DiasRetraso
    {
        get
        {
            if (Estado != EstadoPrestamo.Activo) return 0;
            var dias = (DateTime.Now - FechaDevolucionPrevista).Days;
            return dias > 0 ? dias : 0;
        }
    }

    public int NumeroEjemplares => Ejemplares.Count;

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
