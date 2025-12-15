namespace Biblioteca.Dominio;

/// <summary>
/// Estados posibles de un préstamo.
/// Si, cambie los nombres, no, no es AI slop, es que me liaba entre Finalizado y Cancelado. 
/// Y no me gusta tener que estar haciendo cAmElCaSe, ya que estaba lo cambiaba por activo
/// No esque me la veo venir la de "ui, porque los llamaste asi?? donde lo copiaste/chatgpt, 
/// asique mejor me pre-explico
/// </summary>
public enum EstadoPrestamo
{
    Activo,
    Devuelto,
    // No estado Vencido
    Cancelado
}
