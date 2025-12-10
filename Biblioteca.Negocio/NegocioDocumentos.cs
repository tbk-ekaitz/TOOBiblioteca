using Biblioteca.Datos;
using Biblioteca.Dominio;

namespace Biblioteca.Negocio;

/// <summary>
/// Lógica de negocio para la gestión del catálogo de documentos.
/// Implementa CRUD, búsquedas avanzadas y estadísticas con LINQ.
/// </summary>
public static class NegocioDocumentos
{
    #region Búsquedas de Documentos

    /// <summary>
    /// Obtiene todos los documentos ordenados por título.
    /// </summary>
    public static List<Documento> ObtenerTodosOrdenados()
    {
        return Repositorio.ObtenerTodosDocumentos()
            .OrderBy(d => d.Titulo)
            .ToList();
    }

    /// <summary>
    /// Obtiene solo libros ordenados por autor.
    /// </summary>
    public static List<Libro> ObtenerLibrosOrdenados()
    {
        return Repositorio.ObtenerTodosLibros()
            .OrderBy(l => l.Autor)
            .ThenBy(l => l.Titulo)
            .ToList();
    }

    /// <summary>
    /// Obtiene solo audiolibros ordenados por duración.
    /// </summary>
    public static List<Audiolibro> ObtenerAudiolibrosOrdenados()
    {
        return Repositorio.ObtenerTodosAudiolibros()
            .OrderByDescending(a => a.DuracionMinutos)
            .ToList();
    }

    /// <summary>
    /// Busca documentos por título (búsqueda parcial).
    /// </summary>
    public static List<Documento> BuscarPorTitulo(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
            return ObtenerTodosOrdenados();

        termino = termino.Trim().ToLower();

        return Repositorio.ObtenerTodosDocumentos()
            .Where(d => d.Titulo.ToLower().Contains(termino))
            .OrderBy(d => d.Titulo)
            .ToList();
    }

    /// <summary>
    /// Busca documentos por autor.
    /// </summary>
    public static List<Documento> BuscarPorAutor(string autor)
    {
        if (string.IsNullOrWhiteSpace(autor))
            return ObtenerTodosOrdenados();

        autor = autor.Trim().ToLower();

        return Repositorio.ObtenerTodosDocumentos()
            .Where(d => d.Autor.ToLower().Contains(autor))
            .OrderBy(d => d.Titulo)
            .ToList();
    }

    /// <summary>
    /// Busca documentos por género.
    /// </summary>
    public static List<Documento> BuscarPorGenero(string genero)
    {
        if (string.IsNullOrWhiteSpace(genero))
            return ObtenerTodosOrdenados();

        return Repositorio.ObtenerTodosDocumentos()
            .Where(d => d.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase))
            .OrderBy(d => d.Titulo)
            .ToList();
    }

    /// <summary>
    /// Obtiene todos los géneros disponibles.
    /// </summary>
    public static List<string> ObtenerGeneros()
    {
        return Repositorio.ObtenerTodosDocumentos()
            .Select(d => d.Genero)
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    /// <summary>
    /// Obtiene documentos con ejemplares disponibles para préstamo.
    /// </summary>
    public static List<Documento> BuscarDisponibles()
    {
        var ejemplaresDisponibles = Repositorio.ObtenerEjemplaresDisponibles();
        var documentosConDisponibles = ejemplaresDisponibles
            .Select(e => e.DocumentoId)
            .Distinct()
            .ToHashSet();

        return Repositorio.ObtenerTodosDocumentos()
            .Where(d => documentosConDisponibles.Contains(d.Id))
            .OrderBy(d => d.Titulo)
            .ToList();
    }

    /// <summary>
    /// Busca un documento por código.
    /// </summary>
    public static Documento? BuscarPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        return Repositorio.ObtenerDocumentoPorCodigo(codigo.Trim());
    }

    #endregion

    #region Ejemplares

    /// <summary>
    /// Obtiene ejemplares de un documento específico.
    /// </summary>
    public static List<Ejemplar> ObtenerEjemplares(int documentoId)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(documentoId)
            .OrderBy(e => e.CodigoBarras)
            .ToList();
    }

    /// <summary>
    /// Obtiene ejemplares disponibles de un documento.
    /// </summary>
    public static List<Ejemplar> ObtenerEjemplaresDisponibles(int documentoId)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(documentoId)
            .Where(e => e.EstaDisponible)
            .OrderBy(e => e.CodigoBarras)
            .ToList();
    }

    /// <summary>
    /// Busca un ejemplar por código de barras.
    /// </summary>
    public static Ejemplar? BuscarEjemplarPorCodigoBarras(string codigoBarras)
    {
        if (string.IsNullOrWhiteSpace(codigoBarras)) return null;
        return Repositorio.ObtenerEjemplarPorCodigoBarras(codigoBarras.Trim());
    }

    /// <summary>
    /// Cuenta ejemplares disponibles de un documento.
    /// </summary>
    public static int ContarEjemplaresDisponibles(int documentoId)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(documentoId)
            .Count(e => e.EstaDisponible);
    }

    #endregion

    #region CRUD de Documentos

    /// <summary>
    /// Valida los datos de un documento.
    /// </summary>
    public static (bool esValido, string mensaje) ValidarDocumento(Documento documento)
    {
        if (string.IsNullOrWhiteSpace(documento.Codigo))
            return (false, "El código es obligatorio.");

        if (string.IsNullOrWhiteSpace(documento.Titulo))
            return (false, "El título es obligatorio.");

        if (string.IsNullOrWhiteSpace(documento.Autor))
            return (false, "El autor es obligatorio.");

        // Verificar código duplicado
        var existente = Repositorio.ObtenerDocumentoPorCodigo(documento.Codigo);
        if (existente != null && existente.Id != documento.Id)
            return (false, "Ya existe un documento con ese código.");

        return (true, "Datos válidos.");
    }

    /// <summary>
    /// Da de alta un nuevo documento.
    /// </summary>
    public static (bool exito, string mensaje) AltaDocumento(Documento documento)
    {
        var validacion = ValidarDocumento(documento);
        if (!validacion.esValido)
            return (false, validacion.mensaje);

        documento.FechaAlta = DateTime.Now;
        Repositorio.InsertarDocumento(documento);
        return (true, $"Documento '{documento.Titulo}' dado de alta correctamente.");
    }

    /// <summary>
    /// Modifica un documento existente.
    /// </summary>
    public static (bool exito, string mensaje) ModificarDocumento(Documento documento)
    {
        var existente = Repositorio.ObtenerDocumentoPorId(documento.Id);
        if (existente == null)
            return (false, "No se encontró el documento a modificar.");

        var validacion = ValidarDocumento(documento);
        if (!validacion.esValido)
            return (false, validacion.mensaje);

        Repositorio.ActualizarDocumento(documento);
        return (true, "Documento modificado correctamente.");
    }

    /// <summary>
    /// Da de baja un documento (solo si no tiene ejemplares prestados).
    /// </summary>
    public static (bool exito, string mensaje) BajaDocumento(int documentoId)
    {
        var documento = Repositorio.ObtenerDocumentoPorId(documentoId);
        if (documento == null)
            return (false, "No se encontró el documento.");

        var ejemplares = Repositorio.ObtenerEjemplaresPorDocumento(documentoId);
        var prestados = ejemplares.Count(e => e.Estado == EstadoEjemplar.Prestado);

        if (prestados > 0)
            return (false, $"No se puede dar de baja. Hay {prestados} ejemplar(es) prestado(s).");

        Repositorio.EliminarDocumento(documentoId);
        return (true, $"Documento '{documento.Titulo}' dado de baja correctamente.");
    }

    /// <summary>
    /// Añade un nuevo ejemplar a un documento.
    /// </summary>
    public static (bool exito, string mensaje) AltaEjemplar(int documentoId, string ubicacion)
    {
        var documento = Repositorio.ObtenerDocumentoPorId(documentoId);
        if (documento == null)
            return (false, "No se encontró el documento.");

        var ejemplaresExistentes = Repositorio.ObtenerEjemplaresPorDocumento(documentoId);
        int siguiente = ejemplaresExistentes.Count + 1;

        var ejemplar = new Ejemplar
        {
            DocumentoId = documentoId,
            Documento = documento,
            CodigoBarras = $"{documento.Codigo}-{siguiente:D2}",
            Ubicacion = ubicacion,
            FechaAdquisicion = DateTime.Now,
            Estado = EstadoEjemplar.Disponible
        };

        Repositorio.InsertarEjemplar(ejemplar);
        return (true, $"Ejemplar {ejemplar.CodigoBarras} creado correctamente.");
    }

    #endregion

    #region Estadísticas con LINQ

    /// <summary>
    /// Obtiene el documento más leído (más veces prestado).
    /// Utiliza GroupBy y Count.
    /// </summary>
    public static Documento? ObtenerMasLeido()
    {
        var todosPrestamos = Repositorio.ObtenerTodosPrestamos();
        if (todosPrestamos.Count == 0) return null;

        // Contar préstamos por documento (a través de ejemplares)
        var documentoMasLeido = todosPrestamos
            .SelectMany(p => p.Ejemplares)
            .Where(e => e.Documento != null)
            .GroupBy(e => e.DocumentoId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        return Repositorio.ObtenerDocumentoPorId(documentoMasLeido);
    }

    /// <summary>
    /// Obtiene el documento más leído en el último mes.
    /// </summary>
    public static Documento? ObtenerMasLeidoMes()
    {
        var fechaInicio = DateTime.Now.AddMonths(-1);

        var prestamosDelMes = Repositorio.ObtenerTodosPrestamos()
            .Where(p => p.FechaPrestamo >= fechaInicio);

        if (!prestamosDelMes.Any()) return null;

        var documentoMasLeido = prestamosDelMes
            .SelectMany(p => p.Ejemplares)
            .Where(e => e.Documento != null)
            .GroupBy(e => e.DocumentoId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        return Repositorio.ObtenerDocumentoPorId(documentoMasLeido);
    }

    /// <summary>
    /// Obtiene estadísticas de préstamos por género.
    /// </summary>
    public static List<(string Genero, int Cantidad)> ObtenerEstadisticasPorGenero()
    {
        return Repositorio.ObtenerTodosPrestamos()
            .SelectMany(p => p.Ejemplares)
            .Where(e => e.Documento != null)
            .GroupBy(e => e.Documento!.Genero)
            .Select(g => (Genero: g.Key, Cantidad: g.Count()))
            .OrderByDescending(x => x.Cantidad)
            .ToList();
    }

    /// <summary>
    /// Obtiene los N documentos más prestados.
    /// </summary>
    public static List<(Documento Documento, int VecesPrestado)> ObtenerTopPrestados(int cantidad = 5)
    {
        return Repositorio.ObtenerTodosEjemplares()
            .Where(e => e.Documento != null && e.VecesPrestado > 0)
            .GroupBy(e => e.DocumentoId)
            .Select(g => (
                Documento: Repositorio.ObtenerDocumentoPorId(g.Key)!,
                VecesPrestado: g.Sum(e => e.VecesPrestado)
            ))
            .Where(x => x.Documento != null)
            .OrderByDescending(x => x.VecesPrestado)
            .Take(cantidad)
            .ToList();
    }

    /// <summary>
    /// Cuenta documentos por tipo (Libro vs Audiolibro).
    /// </summary>
    public static (int Libros, int Audiolibros) ContarPorTipo()
    {
        var documentos = Repositorio.ObtenerTodosDocumentos();
        return (
            Libros: documentos.OfType<Libro>().Count(),
            Audiolibros: documentos.OfType<Audiolibro>().Count()
        );
    }

    /// <summary>
    /// Obtiene documentos añadidos recientemente.
    /// </summary>
    public static List<Documento> ObtenerRecientes(int cantidad = 5)
    {
        return Repositorio.ObtenerTodosDocumentos()
            .OrderByDescending(d => d.FechaAlta)
            .Take(cantidad)
            .ToList();
    }

    #endregion
}
