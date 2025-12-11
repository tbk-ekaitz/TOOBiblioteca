namespace Biblioteca.Dominio;

/// <summary>
/// Representa un audiolibro en la biblioteca.
/// Hereda de Documento con propiedades específicas de audio.
/// </summary>
public class Audiolibro : Documento
{
    /// <summary>
    /// Duración total del audiolibro en minutos.
    /// </summary>
    public int DuracionMinutos { get; set; }

    /// <summary>
    /// Nombre del narrador del audiolibro.
    /// </summary>
    public string Narrador { get; set; } = string.Empty;

    /// <summary>
    /// Formato de audio del audiolibro.
    /// </summary>
    public FormatoAudio Formato { get; set; } = FormatoAudio.MP3;

    /// <summary>
    /// Número de CDs o archivos.
    /// </summary>
    public int NumeroDiscos { get; set; } = 1;

    /// <summary>
    /// Indica si es una versión abreviada del libro original.
    /// </summary>
    public bool VersionAbreviada { get; set; } = false;

    /// <summary>
    /// Los audiolibros tienen 14 días de préstamo.
    /// </summary>
    public override int GetDiasPrestamo() => 14;

    public override string TipoDocumento => "Audiolibro";

    /// <summary>
    /// Obtiene la duración formateada en horas y minutos.
    /// </summary>
    public string DuracionFormateada
    {
        get
        {
            int horas = DuracionMinutos / 60;
            int minutos = DuracionMinutos % 60;
            return $"{horas}h {minutos}min";
        }
    }

    public override string ToString()
    {
        return $"[Audiolibro] {Titulo} - {Autor} ({DuracionFormateada})";
    }
}
