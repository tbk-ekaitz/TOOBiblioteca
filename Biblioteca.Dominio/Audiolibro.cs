namespace Biblioteca.Dominio;

/// <summary>
/// Representa un audiolibro en la biblioteca.
/// Hereda de Documento con propiedades específicas de audio.
/// </summary>
public class Audiolibro : Documento
{
    public int DuracionSegundos { get; set; }
    public string Narrador { get; set; } = string.Empty;
    public FormatoAudio Formato { get; set; } = FormatoAudio.MP3;
    // Número de CDs o archivos.
    public int NumeroDiscos { get; set; } = 1;
    public bool VersionAbreviada { get; set; } = false;
    public override int GetDiasPrestamo() => 10;
    public override string TipoDocumento => "Audiolibro";

    public string DuracionFormateada
    {
        get
        {
            TimeSpan t = TimeSpan.FromSeconds(DuracionSegundos);
            return $"{t.Hours:D2}h {t.Minutes:D2}m {t.Seconds:D2}s";
        }
    }

    public override string ToString()
    {
        return $"[Audiolibro] {Titulo} - {Autor} ({DuracionFormateada})";
    }
}
