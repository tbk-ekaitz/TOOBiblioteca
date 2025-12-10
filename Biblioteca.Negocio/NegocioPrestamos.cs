using Biblioteca.Datos;
using Biblioteca.Dominio;

namespace Biblioteca.Negocio;

/// <summary>
/// Lógica de negocio para la gestión de préstamos.
/// Implementa validaciones complejas y cálculo de fechas usando polimorfismo.
/// </summary>
public static class NegocioPrestamos
{
    #region Búsquedas de Préstamos

    /// <summary>
    /// Obtiene todos los préstamos ordenados por fecha (más recientes primero).
    /// </summary>
    public static List<Prestamo> ObtenerTodosOrdenados()
    {
        return Repositorio.ObtenerTodosPrestamos()
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();
    }

    /// <summary>
    /// Obtiene préstamos activos ordenados por fecha de devolución prevista.
    /// </summary>
    public static List<Prestamo> ObtenerActivosOrdenados()
    {
        return Repositorio.ObtenerPrestamosActivos()
            .OrderBy(p => p.FechaDevolucionPrevista)
            .ToList();
    }

    /// <summary>
    /// Obtiene préstamos vencidos.
    /// </summary>
    public static List<Prestamo> ObtenerVencidos()
    {
        return Repositorio.ObtenerPrestamosActivos()
            .Where(p => p.EstaVencido)
            .OrderBy(p => p.FechaDevolucionPrevista)
            .ToList();
    }

    /// <summary>
    /// Obtiene préstamos que vencen pronto (próximos N días).
    /// </summary>
    public static List<Prestamo> ObtenerProximosAVencer(int dias = 3)
    {
        var fechaLimite = DateTime.Now.AddDays(dias);

        return Repositorio.ObtenerPrestamosActivos()
            .Where(p => p.FechaDevolucionPrevista <= fechaLimite && !p.EstaVencido)
            .OrderBy(p => p.FechaDevolucionPrevista)
            .ToList();
    }

    /// <summary>
    /// Obtiene préstamos de un usuario.
    /// </summary>
    public static List<Prestamo> ObtenerPorUsuario(int usuarioId)
    {
        return Repositorio.ObtenerPrestamosPorUsuario(usuarioId)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();
    }

    /// <summary>
    /// Obtiene préstamos activos de un usuario.
    /// </summary>
    public static List<Prestamo> ObtenerActivosPorUsuario(int usuarioId)
    {
        return Repositorio.ObtenerPrestamosActivosPorUsuario(usuarioId)
            .OrderBy(p => p.FechaDevolucionPrevista)
            .ToList();
    }

    /// <summary>
    /// Busca un préstamo por código.
    /// </summary>
    public static Prestamo? BuscarPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        return Repositorio.ObtenerPrestamoPorCodigo(codigo.Trim());
    }

    #endregion

    #region Validaciones de Préstamo

    /// <summary>
    /// Valida si se puede realizar un préstamo para un usuario.
    /// </summary>
    public static (bool puede, string motivo) ValidarPrestamo(int usuarioId, List<Ejemplar> ejemplares)
    {
        // Verificar usuario
        var usuario = Repositorio.ObtenerUsuarioPorId(usuarioId);
        if (usuario == null)
            return (false, "Usuario no encontrado.");

        // Verificar sanción
        if (usuario.Sancionado && usuario.FechaFinSancion > DateTime.Now)
            return (false, $"El usuario está sancionado hasta {usuario.FechaFinSancion:dd/MM/yyyy}.");

        // Verificar límite de préstamos
        var prestamosActivos = Repositorio.ObtenerPrestamosActivosPorUsuario(usuarioId);
        int totalEjemplares = prestamosActivos.Sum(p => p.NumeroEjemplares) + ejemplares.Count;

        if (totalEjemplares > usuario.MaxPrestamos)
            return (false, $"Se superaría el límite de {usuario.MaxPrestamos} préstamos. " +
                         $"Actualmente tiene {prestamosActivos.Sum(p => p.NumeroEjemplares)} ejemplar(es) prestado(s).");

        // Verificar ejemplares
        if (ejemplares.Count == 0)
            return (false, "Debe seleccionar al menos un ejemplar.");

        foreach (var ejemplar in ejemplares)
        {
            if (!ejemplar.EstaDisponible)
                return (false, $"El ejemplar {ejemplar.CodigoBarras} no está disponible.");
        }

        return (true, "Préstamo válido.");
    }

    /// <summary>
    /// Valida si un ejemplar puede ser prestado.
    /// </summary>
    public static (bool puede, string motivo) ValidarEjemplar(string codigoBarras)
    {
        var ejemplar = Repositorio.ObtenerEjemplarPorCodigoBarras(codigoBarras);

        if (ejemplar == null)
            return (false, "Ejemplar no encontrado.");

        if (!ejemplar.EstaDisponible)
            return (false, $"El ejemplar está en estado: {ejemplar.Estado}.");

        return (true, "Ejemplar disponible.");
    }

    #endregion

    #region Operaciones de Préstamo

    /// <summary>
    /// Registra un nuevo préstamo.
    /// Utiliza polimorfismo para calcular la fecha de devolución.
    /// </summary>
    public static (bool exito, string mensaje, Prestamo? prestamo) RegistrarPrestamo(
        int usuarioId,
        List<Ejemplar> ejemplares,
        int empleadoId)
    {
        // Validar préstamo
        var validacion = ValidarPrestamo(usuarioId, ejemplares);
        if (!validacion.puede)
            return (false, validacion.motivo, null);

        var usuario = Repositorio.ObtenerUsuarioPorId(usuarioId);
        var empleado = Repositorio.ObtenerEmpleadoPorId(empleadoId);

        // Crear préstamo
        var prestamo = new Prestamo
        {
            UsuarioId = usuarioId,
            Usuario = usuario,
            EmpleadoId = empleadoId,
            Empleado = empleado as Empleado,
            FechaPrestamo = DateTime.Now,
            Estado = EstadoPrestamo.Activo
        };

        // Añadir ejemplares y actualizar su estado
        foreach (var ejemplar in ejemplares)
        {
            ejemplar.Estado = EstadoEjemplar.Prestado;
            ejemplar.VecesPrestado++;
            prestamo.Ejemplares.Add(ejemplar);
            Repositorio.ActualizarEjemplar(ejemplar);
        }

        // Calcular fecha de devolución usando polimorfismo
        prestamo.CalcularFechaDevolucion();

        // Guardar préstamo
        Repositorio.InsertarPrestamo(prestamo);

        return (true,
            $"Préstamo {prestamo.CodigoPrestamo} registrado. " +
            $"Fecha de devolución: {prestamo.FechaDevolucionPrevista:dd/MM/yyyy}",
            prestamo);
    }

    /// <summary>
    /// Registra la devolución de un préstamo.
    /// </summary>
    public static (bool exito, string mensaje) RegistrarDevolucion(int prestamoId)
    {
        var prestamo = Repositorio.ObtenerPrestamoPorId(prestamoId);

        if (prestamo == null)
            return (false, "Préstamo no encontrado.");

        if (prestamo.Estado != EstadoPrestamo.Activo && prestamo.Estado != EstadoPrestamo.Vencido)
            return (false, $"El préstamo no está activo. Estado actual: {prestamo.Estado}.");

        // Actualizar ejemplares
        foreach (var ejemplar in prestamo.Ejemplares)
        {
            ejemplar.Estado = EstadoEjemplar.Disponible;
            Repositorio.ActualizarEjemplar(ejemplar);
        }

        // Verificar si hubo retraso y aplicar sanción
        string mensajeAdicional = "";
        if (prestamo.EstaVencido)
        {
            int diasRetraso = prestamo.DiasRetraso;
            int diasSancion = diasRetraso * 2; // 2 días de sanción por cada día de retraso

            NegocioUsuarios.AplicarSancion(prestamo.UsuarioId, diasSancion);
            mensajeAdicional = $" Se ha aplicado una sanción de {diasSancion} días por {diasRetraso} día(s) de retraso.";
        }

        // Actualizar préstamo
        prestamo.Estado = EstadoPrestamo.Devuelto;
        prestamo.FechaDevolucionReal = DateTime.Now;
        Repositorio.ActualizarPrestamo(prestamo);

        return (true, $"Devolución registrada correctamente.{mensajeAdicional}");
    }

    /// <summary>
    /// Renueva un préstamo activo (extiende la fecha de devolución).
    /// </summary>
    public static (bool exito, string mensaje) RenovarPrestamo(int prestamoId)
    {
        var prestamo = Repositorio.ObtenerPrestamoPorId(prestamoId);

        if (prestamo == null)
            return (false, "Préstamo no encontrado.");

        if (prestamo.Estado != EstadoPrestamo.Activo)
            return (false, "Solo se pueden renovar préstamos activos.");

        if (prestamo.EstaVencido)
            return (false, "No se puede renovar un préstamo vencido. Debe devolverse primero.");

        // Verificar que el usuario no esté sancionado
        var usuario = Repositorio.ObtenerUsuarioPorId(prestamo.UsuarioId);
        if (usuario != null && usuario.Sancionado && usuario.FechaFinSancion > DateTime.Now)
            return (false, "No se puede renovar. El usuario está sancionado.");

        // Extender fecha de devolución
        int diasExtension = prestamo.Ejemplares
            .Where(e => e.Documento != null)
            .Select(e => e.Documento!.GetDiasPrestamo())
            .DefaultIfEmpty(14)
            .Max();

        prestamo.FechaDevolucionPrevista = prestamo.FechaDevolucionPrevista.AddDays(diasExtension);
        Repositorio.ActualizarPrestamo(prestamo);

        return (true, $"Préstamo renovado. Nueva fecha de devolución: {prestamo.FechaDevolucionPrevista:dd/MM/yyyy}");
    }

    /// <summary>
    /// Cancela un préstamo (caso especial).
    /// </summary>
    public static (bool exito, string mensaje) CancelarPrestamo(int prestamoId, string motivo)
    {
        var prestamo = Repositorio.ObtenerPrestamoPorId(prestamoId);

        if (prestamo == null)
            return (false, "Préstamo no encontrado.");

        if (prestamo.Estado != EstadoPrestamo.Activo)
            return (false, "Solo se pueden cancelar préstamos activos.");

        // Liberar ejemplares
        foreach (var ejemplar in prestamo.Ejemplares)
        {
            ejemplar.Estado = EstadoEjemplar.Disponible;
            Repositorio.ActualizarEjemplar(ejemplar);
        }

        prestamo.Estado = EstadoPrestamo.Cancelado;
        prestamo.Observaciones = $"Cancelado: {motivo}";
        Repositorio.ActualizarPrestamo(prestamo);

        return (true, "Préstamo cancelado correctamente.");
    }

    #endregion

    #region Estadísticas

    /// <summary>
    /// Obtiene estadísticas generales de préstamos.
    /// </summary>
    public static (int Total, int Activos, int Vencidos, int DevueltosEsteMes) ObtenerEstadisticas()
    {
        var prestamos = Repositorio.ObtenerTodosPrestamos();
        var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        return (
            Total: prestamos.Count,
            Activos: prestamos.Count(p => p.Estado == EstadoPrestamo.Activo),
            Vencidos: prestamos.Count(p => p.Estado == EstadoPrestamo.Activo && p.EstaVencido),
            DevueltosEsteMes: prestamos.Count(p => p.Estado == EstadoPrestamo.Devuelto &&
                                                   p.FechaDevolucionReal >= inicioMes)
        );
    }

    /// <summary>
    /// Obtiene préstamos realizados por un empleado.
    /// </summary>
    public static List<Prestamo> ObtenerPorEmpleado(int empleadoId)
    {
        return Repositorio.ObtenerTodosPrestamos()
            .Where(p => p.EmpleadoId == empleadoId)
            .OrderByDescending(p => p.FechaPrestamo)
            .ToList();
    }

    /// <summary>
    /// Cuenta préstamos por estado.
    /// </summary>
    public static Dictionary<EstadoPrestamo, int> ContarPorEstado()
    {
        return Repositorio.ObtenerTodosPrestamos()
            .GroupBy(p => p.Estado)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Obtiene el promedio de días de préstamo (para estadísticas).
    /// </summary>
    public static double ObtenerPromedioDiasPrestamo()
    {
        var prestamosDevueltos = Repositorio.ObtenerTodosPrestamos()
            .Where(p => p.Estado == EstadoPrestamo.Devuelto && p.FechaDevolucionReal.HasValue);

        if (!prestamosDevueltos.Any()) return 0;

        return prestamosDevueltos
            .Select(p => (p.FechaDevolucionReal!.Value - p.FechaPrestamo).Days)
            .Average();
    }

    #endregion
}
