using Biblioteca.Negocio.Persistencia;
using Biblioteca.Dominio;

namespace Biblioteca.Negocio;

/// <summary>
/// Lógica de negocio para la gestión de usuarios y empleados.
/// </summary>
public static class NegocioUsuarios
{
    #region Autenticación

    /// <summary>
    /// Valida las credenciales de un empleado para el login (por nombre de usuario).
    /// </summary>
    public static Empleado? ValidarLogin(string nombreUsuario, string password)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
            return null;

        return Repositorio.ValidarCredenciales(nombreUsuario, password);
    }

    /// <summary>
    /// Valida las credenciales de un empleado para el login (por DNI).
    /// </summary>
    public static Empleado? ValidarLoginPorDNI(string dni, string password)
    {
        if (string.IsNullOrWhiteSpace(dni) || string.IsNullOrWhiteSpace(password))
            return null;

        return Repositorio.ValidarCredencialesPorDNI(dni.Trim(), password);
    }

    /// <summary>
    /// Verifica si el empleado tiene acceso a una funcionalidad según su rol.
    /// </summary>
    public static bool TieneAcceso(Empleado empleado, Role rolRequerido)
    {
        if (empleado == null) return false;

        if (empleado.Rol == Role.Administrador) return true;
        return empleado.Rol == rolRequerido;
    }

    #endregion

    #region Búsquedas de Usuarios

    /// <summary>
    /// Busca un usuario por su DNI.
    /// </summary>
    public static Usuario? BuscarPorDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return null;
        return Repositorio.ObtenerUsuarioPorDNI(dni.Trim());
    }

    /// <summary>
    /// Verifica si existe un usuario con el DNI dado.
    /// </summary>
    public static bool ExisteUsuario(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;
        return Repositorio.ExisteUsuario(dni.Trim());
    }

    /// <summary>
    /// Obtiene todos los usuarios sin ordenar.
    /// </summary>
    public static List<Usuario> ObtenerTodos()
    {
        return Repositorio.ObtenerTodosUsuarios();
    }

    /// <summary>
    /// Obtiene todos los usuarios ordenados alfabéticamente por apellidos.
    /// </summary>
    public static List<Usuario> ObtenerTodosOrdenados()
    {
        return Repositorio.ObtenerTodosUsuarios()
            .OrderBy(u => u.Apellidos)
            .ThenBy(u => u.Nombre)
            .ToList();
    }

    /// <summary>
    /// Obtiene usuarios ordenados por fecha de alta (más recientes primero).
    /// </summary>
    public static List<Usuario> ObtenerPorFechaAlta()
    {
        return Repositorio.ObtenerTodosUsuarios()
            .OrderByDescending(u => u.FechaAlta)
            .ToList();
    }

    /// <summary>
    /// Busca usuarios por nombre o apellidos (búsqueda parcial).
    /// </summary>
    public static List<Usuario> BuscarPorNombre(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
            return ObtenerTodosOrdenados();

        termino = termino.Trim().ToLower();

        return Repositorio.ObtenerTodosUsuarios()
            .Where(u => u.Nombre.ToLower().Contains(termino) ||
                       u.Apellidos.ToLower().Contains(termino))
            .OrderBy(u => u.Apellidos)
            .ToList();
    }

    /// <summary>
    /// Obtiene usuarios que están sancionados actualmente.
    /// </summary>
    public static List<Usuario> ObtenerSancionados()
    {
        return Repositorio.ObtenerTodosUsuarios()
            .Where(u => u.Sancionado &&
                       (u.FechaFinSancion == null || u.FechaFinSancion > DateTime.Now))
            .OrderBy(u => u.FechaFinSancion)
            .ToList();
    }

    /// <summary>
    /// Obtiene usuarios sin préstamos activos.
    /// </summary>
    public static List<Usuario> ObtenerSinPrestamosActivos()
    {
        var usuariosConPrestamos = Repositorio.ObtenerPrestamosActivos()
            .Select(p => p.Usuario.DNI)
            .Distinct()
            .ToHashSet();

        return Repositorio.ObtenerTodosUsuarios()
            .Where(u => !usuariosConPrestamos.Contains(u.DNI))
            .OrderBy(u => u.Apellidos)
            .ToList();
    }

    #endregion

    #region CRUD de Usuarios

    /// <summary>
    /// Valida los datos de un usuario antes de guardar.
    /// </summary>
    public static (bool esValido, string mensaje) ValidarUsuario(Usuario usuario, bool esNuevo = false)
    {
        if (string.IsNullOrWhiteSpace(usuario.DNI))
            return (false, "El DNI es obligatorio.");

        if (!ValidarFormatoDNI(usuario.DNI))
            return (false, "El formato del DNI no es válido (8 números + 1 letra).");

        if (string.IsNullOrWhiteSpace(usuario.Nombre))
            return (false, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuario.Apellidos))
            return (false, "Los apellidos son obligatorios.");

        if (esNuevo && Repositorio.ExisteUsuario(usuario.DNI))
            return (false, "Ya existe un usuario con ese DNI.");

        return (true, "Datos válidos.");
    }

    /// <summary>
    /// Valida el formato del DNI español (8 números + 1 letra).
    /// </summary>
    public static bool ValidarFormatoDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;
        dni = dni.Trim().ToUpper();

        if (dni.Length != 9) return false;

        // Verificar 8 números + 1 letra
        for (int i = 0; i < 8; i++)
        {
            if (!char.IsDigit(dni[i])) return false;
        }

        return char.IsLetter(dni[8]);
    }

    /// <summary>
    /// Da de alta un nuevo usuario.
    /// </summary>
    public static (bool exito, string mensaje) AltaUsuario(Usuario usuario)
    {
        var validacion = ValidarUsuario(usuario, esNuevo: true);
        if (!validacion.esValido)
            return (false, validacion.mensaje);

        usuario.FechaAlta = DateTime.Now;
        Repositorio.InsertarUsuario(usuario);
        return (true, $"Usuario {usuario.NombreCompleto} dado de alta correctamente.");
    }

    /// <summary>
    /// Modifica los datos de un usuario existente.
    /// </summary>
    public static (bool exito, string mensaje) ModificarUsuario(Usuario usuario)
    {
        var existente = Repositorio.ObtenerUsuarioPorDNI(usuario.DNI);
        if (existente == null)
            return (false, "No se encontró el usuario a modificar.");

        var validacion = ValidarUsuario(usuario, esNuevo: false);
        if (!validacion.esValido)
            return (false, validacion.mensaje);

        Repositorio.ActualizarUsuario(usuario);
        return (true, "Usuario modificado correctamente.");
    }

    /// <summary>
    /// Da de baja un usuario por DNI (solo si no tiene préstamos activos).
    /// </summary>
    public static (bool exito, string mensaje) BajaUsuario(string dni)
    {
        var usuario = Repositorio.ObtenerUsuarioPorDNI(dni);
        if (usuario == null)
            return (false, "No se encontró el usuario.");

        var prestamosActivos = Repositorio.ObtenerPrestamosActivosPorUsuario(dni);
        if (prestamosActivos.Count > 0)
            return (false, $"No se puede dar de baja al usuario. Tiene {prestamosActivos.Count} préstamo(s) activo(s).");

        Repositorio.EliminarUsuario(dni);
        return (true, $"Usuario {usuario.NombreCompleto} dado de baja correctamente.");
    }

    #endregion

    #region Sanciones

    /// <summary>
    /// Aplica una sanción a un usuario por DNI.
    /// </summary>
    public static void AplicarSancion(string dni, int diasSancion)
    {
        var usuario = Repositorio.ObtenerUsuarioPorDNI(dni);
        if (usuario == null) return;

        usuario.Sancionado = true;
        usuario.FechaFinSancion = DateTime.Now.AddDays(diasSancion);
        Repositorio.ActualizarUsuario(usuario);
    }

    /// <summary>
    /// Levanta la sanción de un usuario por DNI.
    /// </summary>
    public static void LevantarSancion(string dni)
    {
        var usuario = Repositorio.ObtenerUsuarioPorDNI(dni);
        if (usuario == null) return;

        usuario.Sancionado = false;
        usuario.FechaFinSancion = null;
        Repositorio.ActualizarUsuario(usuario);
    }

    /// <summary>
    /// Verifica si un usuario puede realizar préstamos.
    /// </summary>
    public static (bool puede, string motivo) PuedeRealizarPrestamo(string dni)
    {
        var usuario = Repositorio.ObtenerUsuarioPorDNI(dni);
        if (usuario == null)
            return (false, "Usuario no encontrado.");

        if (usuario.Sancionado && usuario.FechaFinSancion > DateTime.Now)
            return (false, $"Usuario sancionado hasta {usuario.FechaFinSancion:dd/MM/yyyy}.");

        var prestamosActivos = Repositorio.ObtenerPrestamosActivosPorUsuario(dni);
        if (prestamosActivos.Count >= usuario.MaxPrestamos)
            return (false, $"El usuario ha alcanzado el máximo de {usuario.MaxPrestamos} préstamos simultáneos.");

        return (true, "El usuario puede realizar préstamos.");
    }

    #endregion

    #region Estadísticas

    /// <summary>
    /// Obtiene el usuario con más préstamos históricos.
    /// </summary>
    public static Usuario? ObtenerUsuarioMasActivo()
    {
        var todosLosPrestamos = Repositorio.ObtenerTodosPrestamos();
        if (todosLosPrestamos.Count == 0) return null;

        var dniMasActivo = todosLosPrestamos
            .GroupBy(p => p.Usuario.DNI)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(dniMasActivo)) return null;
        return Repositorio.ObtenerUsuarioPorDNI(dniMasActivo);
    }

    /// <summary>
    /// Cuenta préstamos históricos de un usuario por DNI.
    /// </summary>
    public static int ContarPrestamosUsuario(string dni)
    {
        return Repositorio.ObtenerPrestamosPorUsuario(dni).Count;
    }

    #endregion
}
