using Biblioteca.Datos;
using Biblioteca.Dominio;

namespace Biblioteca.Negocio;

/// <summary>
/// Lógica de negocio para la gestión del catálogo de documentos.
/// Implementa CRUD, búsquedas avanzadas y estadísticas con LINQ.
/// Utiliza Codigo como identificador único para documentos.
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
            .Select(e => e.Documento.Codigo)
            .Distinct()
            .ToHashSet();

        return Repositorio.ObtenerTodosDocumentos()
            .Where(d => documentosConDisponibles.Contains(d.Codigo))
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

    /// <summary>
    /// Verifica si existe un documento con el código dado.
    /// </summary>
    public static bool ExisteDocumento(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return false;
        return Repositorio.ExisteDocumento(codigo.Trim());
    }

    #endregion

    #region Ejemplares

    /// <summary>
    /// Obtiene ejemplares de un documento específico por código.
    /// </summary>
    public static List<Ejemplar> ObtenerEjemplares(string codigoDocumento)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(codigoDocumento)
            .OrderBy(e => e.CodigoBarras)
            .ToList();
    }

    /// <summary>
    /// Obtiene ejemplares disponibles de un documento.
    /// </summary>
    public static List<Ejemplar> ObtenerEjemplaresDisponibles(string codigoDocumento)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(codigoDocumento)
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
    public static int ContarEjemplaresDisponibles(string codigoDocumento)
    {
        return Repositorio.ObtenerEjemplaresPorDocumento(codigoDocumento)
            .Count(e => e.EstaDisponible);
    }

    /// <summary>
    /// Verifica si hay ejemplares disponibles de un documento.
    /// </summary>
    public static bool HayEjemplaresDisponibles(string codigoDocumento)
    {
        return ContarEjemplaresDisponibles(codigoDocumento) > 0;
    }

    /// <summary>
    /// Obtiene la fecha estimada de disponibilidad de un documento.
    /// Busca el préstamo activo con fecha de devolución más próxima.
    /// </summary>
    public static DateTime? ObtenerFechaDisponibilidad(string codigoDocumento)
    {
        if (HayEjemplaresDisponibles(codigoDocumento))
            return DateTime.Now; // Ya está disponible

        var prestamosDelDocumento = Repositorio.ObtenerPrestamosDeDocumento(codigoDocumento)
            .Where(p => p.Estado == EstadoPrestamo.Activo)
            .OrderBy(p => p.FechaDevolucionPrevista)
            .FirstOrDefault();

        return prestamosDelDocumento?.FechaDevolucionPrevista;
    }

    #endregion

    #region CRUD de Documentos

    /// <summary>
    /// Valida los datos de un documento.
    /// </summary>
    public static (bool esValido, string mensaje) ValidarDocumento(Documento documento, bool esNuevo = false)
    {
        if (string.IsNullOrWhiteSpace(documento.Codigo))
            return (false, "El ISBN es obligatorio.");

        if (!ValidarFormatoISBN(documento.Codigo))
            return (false, "El formato del ISBN no es válido. Debe tener 10 o 13 dígitos (puede incluir guiones).");

        if (string.IsNullOrWhiteSpace(documento.Titulo))
            return (false, "El título es obligatorio.");

        if (string.IsNullOrWhiteSpace(documento.Autor))
            return (false, "El autor es obligatorio.");

        // Verificar código duplicado (solo para nuevos documentos)
        if (esNuevo && Repositorio.ExisteDocumento(documento.Codigo))
            return (false, "Ya existe un documento con ese ISBN.");

        return (true, "Datos válidos.");
    }

    /// <summary>
    /// Valida el formato de un ISBN (10 o 13 dígitos, puede incluir guiones).
    /// </summary>
    public static bool ValidarFormatoISBN(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn)) return false;

        // Eliminar guiones y espacios
        var soloDigitos = isbn.Replace("-", "").Replace(" ", "");

        // ISBN-10: 10 caracteres (último puede ser X)
        // ISBN-13: 13 dígitos
        if (soloDigitos.Length == 10)
        {
            // Los primeros 9 deben ser dígitos, el último puede ser dígito o X
            for (int i = 0; i < 9; i++)
            {
                if (!char.IsDigit(soloDigitos[i])) return false;
            }
            return char.IsDigit(soloDigitos[9]) || soloDigitos[9] == 'X' || soloDigitos[9] == 'x';
        }
        else if (soloDigitos.Length == 13)
        {
            // Todos deben ser dígitos
            return soloDigitos.All(char.IsDigit);
        }

        return false;
    }

    /// <summary>
    /// Da de alta un nuevo documento.
    /// </summary>
    public static (bool exito, string mensaje) AltaDocumento(Documento documento)
    {
        var validacion = ValidarDocumento(documento, esNuevo: true);
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
        var existente = Repositorio.ObtenerDocumentoPorCodigo(documento.Codigo);
        if (existente == null)
            return (false, "No se encontró el documento a modificar.");

        var validacion = ValidarDocumento(documento, esNuevo: false);
        if (!validacion.esValido)
            return (false, validacion.mensaje);

        Repositorio.ActualizarDocumento(documento);
        return (true, "Documento modificado correctamente.");
    }

    /// <summary>
    /// Da de baja un documento por código (solo si no tiene ejemplares prestados).
    /// </summary>
    public static (bool exito, string mensaje) BajaDocumento(string codigoDocumento)
    {
        var documento = Repositorio.ObtenerDocumentoPorCodigo(codigoDocumento);
        if (documento == null)
            return (false, "No se encontró el documento.");

        var ejemplares = Repositorio.ObtenerEjemplaresPorDocumento(codigoDocumento);
        var prestados = ejemplares.Count(e => e.Estado == EstadoEjemplar.Prestado);

        if (prestados > 0)
            return (false, $"No se puede dar de baja. Hay {prestados} ejemplar(es) prestado(s).");

        Repositorio.EliminarDocumento(codigoDocumento);
        return (true, $"Documento '{documento.Titulo}' dado de baja correctamente.");
    }

    /// <summary>
    /// Añade un nuevo ejemplar a un documento.
    /// </summary>
    public static (bool exito, string mensaje) AltaEjemplar(string codigoDocumento, string ubicacion)
    {
        var documento = Repositorio.ObtenerDocumentoPorCodigo(codigoDocumento);
        if (documento == null)
            return (false, "No se encontró el documento.");

        var ejemplaresExistentes = Repositorio.ObtenerEjemplaresPorDocumento(codigoDocumento);
        int siguiente = ejemplaresExistentes.Count + 1;

        var ejemplar = new Ejemplar
        {
            CodigoBarras = $"{documento.Codigo}-{siguiente:D2}",
            Documento = documento,
            Ubicacion = ubicacion,
            FechaAdquisicion = DateTime.Now,
            Estado = EstadoEjemplar.Disponible
        };

        Repositorio.InsertarEjemplar(ejemplar);
        return (true, $"Ejemplar {ejemplar.CodigoBarras} creado correctamente.");
    }

    /// <summary>
    /// Da de baja un ejemplar por código de barras (solo si no está prestado).
    /// </summary>
    public static (bool exito, string mensaje) BajaEjemplar(string codigoBarras)
    {
        var ejemplar = Repositorio.ObtenerEjemplarPorCodigoBarras(codigoBarras);
        if (ejemplar == null)
            return (false, "No se encontró el ejemplar.");

        if (ejemplar.Estado == EstadoEjemplar.Prestado)
            return (false, "No se puede dar de baja un ejemplar que está prestado.");

        Repositorio.EliminarEjemplar(codigoBarras);
        return (true, $"Ejemplar {codigoBarras} dado de baja correctamente.");
    }

    #endregion

    #region Estadísticas con LINQ

    /// <summary>
    /// Obtiene el documento más leído (más veces prestado).
    /// </summary>
    public static Documento? ObtenerMasLeido()
    {
        var todosPrestamos = Repositorio.ObtenerTodosPrestamos();
        if (todosPrestamos.Count == 0) return null;

        var codigoMasLeido = todosPrestamos
            .SelectMany(p => p.Ejemplares)
            .GroupBy(e => e.Documento.Codigo)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(codigoMasLeido)) return null;
        return Repositorio.ObtenerDocumentoPorCodigo(codigoMasLeido);
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

        var codigoMasLeido = prestamosDelMes
            .SelectMany(p => p.Ejemplares)
            .GroupBy(e => e.Documento.Codigo)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(codigoMasLeido)) return null;
        return Repositorio.ObtenerDocumentoPorCodigo(codigoMasLeido);
    }

    /// <summary>
    /// Obtiene el documento más leído en un mes y año específicos.
    /// </summary>
    public static Documento? ObtenerMasLeidoMes(int mes, int anio)
    {
        var fechaInicio = new DateTime(anio, mes, 1);
        var fechaFin = fechaInicio.AddMonths(1);

        var prestamosDelMes = Repositorio.ObtenerTodosPrestamos()
            .Where(p => p.FechaPrestamo >= fechaInicio && p.FechaPrestamo < fechaFin);

        if (!prestamosDelMes.Any()) return null;

        var codigoMasLeido = prestamosDelMes
            .SelectMany(p => p.Ejemplares)
            .GroupBy(e => e.Documento.Codigo)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(codigoMasLeido)) return null;
        return Repositorio.ObtenerDocumentoPorCodigo(codigoMasLeido);
    }

    /// <summary>
    /// Obtiene estadísticas de préstamos por género.
    /// </summary>
    public static List<(string Genero, int Cantidad)> ObtenerEstadisticasPorGenero()
    {
        return Repositorio.ObtenerTodosPrestamos()
            .SelectMany(p => p.Ejemplares)
            .GroupBy(e => e.Documento.Genero)
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
            .Where(e => e.VecesPrestado > 0)
            .GroupBy(e => e.Documento.Codigo)
            .Select(g => (
                Documento: Repositorio.ObtenerDocumentoPorCodigo(g.Key)!,
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
