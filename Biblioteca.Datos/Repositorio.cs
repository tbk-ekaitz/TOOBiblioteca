using Biblioteca.Dominio;

namespace Biblioteca.Datos;

/// <summary>
/// Repositorio central de datos simulados.
/// Actúa como una base de datos en memoria con datos de prueba precargados.
/// </summary>
public static class Repositorio
{
    #region Colecciones de Datos

    private static List<Usuario> _usuarios = new();
    private static List<Empleado> _empleados = new();
    private static List<Documento> _documentos = new();
    private static List<Ejemplar> _ejemplares = new();
    private static List<Prestamo> _prestamos = new();

    private static int _nextUsuarioId = 1;
    private static int _nextEmpleadoId = 1;
    private static int _nextDocumentoId = 1;
    private static int _nextEjemplarId = 1;
    private static int _nextPrestamoId = 1;

    #endregion

    #region Constructor Estático - Carga de Datos de Prueba

    static Repositorio()
    {
        CargarEmpleados();
        CargarUsuarios();
        CargarDocumentos();
        CargarEjemplares();
        CargarPrestamosHistoricos();
    }

    private static void CargarEmpleados()
    {
        _empleados = new List<Empleado>
        {
            new Empleado
            {
                Id = _nextEmpleadoId++,
                DNI = "12345678A",
                Nombre = "Ana",
                Apellidos = "García López",
                Email = "ana.garcia@biblioteca.com",
                Telefono = "600111222",
                NombreUsuario = "admin",
                Password = "admin123",
                Rol = Role.Administrador,
                NumeroEmpleado = "EMP001",
                FechaContratacion = new DateTime(2020, 1, 15)
            },
            new Empleado
            {
                Id = _nextEmpleadoId++,
                DNI = "23456789B",
                Nombre = "Carlos",
                Apellidos = "Martínez Ruiz",
                Email = "carlos.martinez@biblioteca.com",
                Telefono = "600222333",
                NombreUsuario = "carlos",
                Password = "sala123",
                Rol = Role.PersonalSala,
                NumeroEmpleado = "EMP002",
                FechaContratacion = new DateTime(2021, 3, 10)
            },
            new Empleado
            {
                Id = _nextEmpleadoId++,
                DNI = "34567890C",
                Nombre = "María",
                Apellidos = "Fernández Díaz",
                Email = "maria.fernandez@biblioteca.com",
                Telefono = "600333444",
                NombreUsuario = "maria",
                Password = "adq123",
                Rol = Role.PersonalAdquisiciones,
                NumeroEmpleado = "EMP003",
                FechaContratacion = new DateTime(2022, 6, 20)
            }
        };
    }

    private static void CargarUsuarios()
    {
        _usuarios = new List<Usuario>
        {
            new Usuario
            {
                Id = _nextUsuarioId++,
                DNI = "11111111A",
                Nombre = "Pedro",
                Apellidos = "Sánchez Pérez",
                Email = "pedro.sanchez@email.com",
                Telefono = "611111111",
                Direccion = "Calle Mayor 1, Madrid",
                FechaAlta = new DateTime(2023, 1, 10)
            },
            new Usuario
            {
                Id = _nextUsuarioId++,
                DNI = "22222222B",
                Nombre = "Laura",
                Apellidos = "López Gómez",
                Email = "laura.lopez@email.com",
                Telefono = "622222222",
                Direccion = "Avenida Principal 25, Madrid",
                FechaAlta = new DateTime(2023, 2, 15)
            },
            new Usuario
            {
                Id = _nextUsuarioId++,
                DNI = "33333333C",
                Nombre = "Miguel",
                Apellidos = "Torres Vega",
                Email = "miguel.torres@email.com",
                Telefono = "633333333",
                Direccion = "Plaza Central 8, Madrid",
                FechaAlta = new DateTime(2023, 3, 20),
                Sancionado = true,
                FechaFinSancion = DateTime.Now.AddDays(7)
            },
            new Usuario
            {
                Id = _nextUsuarioId++,
                DNI = "44444444D",
                Nombre = "Elena",
                Apellidos = "Ruiz Navarro",
                Email = "elena.ruiz@email.com",
                Telefono = "644444444",
                Direccion = "Calle Secundaria 12, Madrid",
                FechaAlta = new DateTime(2023, 5, 5)
            },
            new Usuario
            {
                Id = _nextUsuarioId++,
                DNI = "55555555E",
                Nombre = "Javier",
                Apellidos = "Moreno Castro",
                Email = "javier.moreno@email.com",
                Telefono = "655555555",
                Direccion = "Paseo del Prado 45, Madrid",
                FechaAlta = new DateTime(2023, 6, 12)
            }
        };
    }

    private static void CargarDocumentos()
    {
        // Libros
        var libros = new List<Libro>
        {
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB001",
                ISBN = "978-84-376-0494-7",
                Titulo = "Don Quijote de la Mancha",
                Autor = "Miguel de Cervantes",
                Editorial = "Cátedra",
                AnioPublicacion = 1605,
                Genero = "Novela",
                NumeroPaginas = 1250,
                Edicion = "Edición conmemorativa",
                Encuadernacion = "Tapa dura"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB002",
                ISBN = "978-84-204-8475-2",
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Editorial = "Alfaguara",
                AnioPublicacion = 1967,
                Genero = "Realismo mágico",
                NumeroPaginas = 496,
                Edicion = "50 aniversario",
                Encuadernacion = "Tapa blanda"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB003",
                ISBN = "978-84-339-7896-3",
                Titulo = "1984",
                Autor = "George Orwell",
                Editorial = "Debolsillo",
                AnioPublicacion = 1949,
                Genero = "Distopía",
                NumeroPaginas = 352,
                Encuadernacion = "Tapa blanda"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB004",
                ISBN = "978-84-450-7789-4",
                Titulo = "El Principito",
                Autor = "Antoine de Saint-Exupéry",
                Editorial = "Salamandra",
                AnioPublicacion = 1943,
                Genero = "Fábula",
                NumeroPaginas = 96,
                Encuadernacion = "Tapa dura"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB005",
                ISBN = "978-84-233-4789-5",
                Titulo = "La sombra del viento",
                Autor = "Carlos Ruiz Zafón",
                Editorial = "Planeta",
                AnioPublicacion = 2001,
                Genero = "Misterio",
                NumeroPaginas = 576,
                Encuadernacion = "Tapa blanda"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB006",
                ISBN = "978-84-672-3456-6",
                Titulo = "Rayuela",
                Autor = "Julio Cortázar",
                Editorial = "Cátedra",
                AnioPublicacion = 1963,
                Genero = "Novela experimental",
                NumeroPaginas = 736,
                Encuadernacion = "Tapa blanda"
            },
            new Libro
            {
                Id = _nextDocumentoId++,
                Codigo = "LIB007",
                ISBN = "978-84-9838-789-7",
                Titulo = "El amor en los tiempos del cólera",
                Autor = "Gabriel García Márquez",
                Editorial = "Debolsillo",
                AnioPublicacion = 1985,
                Genero = "Romance",
                NumeroPaginas = 496,
                Encuadernacion = "Tapa blanda"
            }
        };

        // Audiolibros
        var audiolibros = new List<Audiolibro>
        {
            new Audiolibro
            {
                Id = _nextDocumentoId++,
                Codigo = "AUD001",
                Titulo = "Harry Potter y la piedra filosofal",
                Autor = "J.K. Rowling",
                Editorial = "Salamandra Audio",
                AnioPublicacion = 1997,
                Genero = "Fantasía",
                Narrador = "Carlos Ponce",
                DuracionMinutos = 520,
                Formato = FormatoAudio.MP3
            },
            new Audiolibro
            {
                Id = _nextDocumentoId++,
                Codigo = "AUD002",
                Titulo = "El nombre del viento",
                Autor = "Patrick Rothfuss",
                Editorial = "Plaza & Janés Audio",
                AnioPublicacion = 2007,
                Genero = "Fantasía épica",
                Narrador = "Raúl Llorens",
                DuracionMinutos = 1680,
                Formato = FormatoAudio.MP3
            },
            new Audiolibro
            {
                Id = _nextDocumentoId++,
                Codigo = "AUD003",
                Titulo = "Sapiens: De animales a dioses",
                Autor = "Yuval Noah Harari",
                Editorial = "Debate Audio",
                AnioPublicacion = 2011,
                Genero = "Historia",
                Narrador = "Miguel Ángel Jenner",
                DuracionMinutos = 900,
                Formato = FormatoAudio.AAC,
                NumeroDiscos = 2
            }
        };

        _documentos.AddRange(libros);
        _documentos.AddRange(audiolibros);
    }

    private static void CargarEjemplares()
    {
        foreach (var documento in _documentos)
        {
            // Crear 2-3 ejemplares por documento
            int numEjemplares = documento.Id % 2 == 0 ? 3 : 2;

            for (int i = 1; i <= numEjemplares; i++)
            {
                _ejemplares.Add(new Ejemplar
                {
                    Id = _nextEjemplarId++,
                    CodigoBarras = $"{documento.Codigo}-{i:D2}",
                    DocumentoId = documento.Id,
                    Documento = documento,
                    Estado = EstadoEjemplar.Disponible,
                    Ubicacion = $"Estante {(char)('A' + (documento.Id % 5))}{documento.Id}, Fila {i}",
                    FechaAdquisicion = documento.FechaAlta.AddDays(-30)
                });
            }
        }
    }

    private static void CargarPrestamosHistoricos()
    {
        // Préstamo histórico devuelto
        var prestamo1 = new Prestamo
        {
            Id = _nextPrestamoId++,
            CodigoPrestamo = "PRE-2024-001",
            UsuarioId = 1,
            Usuario = _usuarios[0],
            EmpleadoId = 2,
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-30),
            Estado = EstadoPrestamo.Devuelto,
            FechaDevolucionReal = DateTime.Now.AddDays(-10)
        };
        prestamo1.Ejemplares.Add(_ejemplares[0]);
        prestamo1.CalcularFechaDevolucion();
        _ejemplares[0].VecesPrestado++;

        // Préstamo activo
        var prestamo2 = new Prestamo
        {
            Id = _nextPrestamoId++,
            CodigoPrestamo = "PRE-2024-002",
            UsuarioId = 2,
            Usuario = _usuarios[1],
            EmpleadoId = 2,
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-5),
            Estado = EstadoPrestamo.Activo
        };
        var ejemplar2 = _ejemplares[2];
        ejemplar2.Estado = EstadoEjemplar.Prestado;
        prestamo2.Ejemplares.Add(ejemplar2);
        prestamo2.CalcularFechaDevolucion();
        ejemplar2.VecesPrestado++;

        // Préstamo activo con múltiples ejemplares
        var prestamo3 = new Prestamo
        {
            Id = _nextPrestamoId++,
            CodigoPrestamo = "PRE-2024-003",
            UsuarioId = 4,
            Usuario = _usuarios[3],
            EmpleadoId = 2,
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-3),
            Estado = EstadoPrestamo.Activo
        };
        var ejemplar4 = _ejemplares[5];
        var ejemplar5 = _ejemplares[8];
        ejemplar4.Estado = EstadoEjemplar.Prestado;
        ejemplar5.Estado = EstadoEjemplar.Prestado;
        prestamo3.Ejemplares.Add(ejemplar4);
        prestamo3.Ejemplares.Add(ejemplar5);
        prestamo3.CalcularFechaDevolucion();
        ejemplar4.VecesPrestado++;
        ejemplar5.VecesPrestado++;

        // Préstamo vencido (para pruebas de sanciones)
        var prestamo4 = new Prestamo
        {
            Id = _nextPrestamoId++,
            CodigoPrestamo = "PRE-2024-004",
            UsuarioId = 3,
            Usuario = _usuarios[2],
            EmpleadoId = 2,
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-35),
            FechaDevolucionPrevista = DateTime.Now.AddDays(-14),
            Estado = EstadoPrestamo.Vencido
        };
        var ejemplarVencido = _ejemplares[10];
        ejemplarVencido.Estado = EstadoEjemplar.Prestado;
        prestamo4.Ejemplares.Add(ejemplarVencido);
        ejemplarVencido.VecesPrestado++;

        _prestamos.AddRange(new[] { prestamo1, prestamo2, prestamo3, prestamo4 });
    }

    #endregion

    #region Métodos de Acceso - Usuarios

    public static List<Usuario> ObtenerTodosUsuarios() => new(_usuarios);

    public static Usuario? ObtenerUsuarioPorId(int id) =>
        _usuarios.FirstOrDefault(u => u.Id == id);

    public static Usuario? ObtenerUsuarioPorDNI(string dni) =>
        _usuarios.FirstOrDefault(u => u.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase));

    public static void InsertarUsuario(Usuario usuario)
    {
        usuario.Id = _nextUsuarioId++;
        _usuarios.Add(usuario);
    }

    public static bool ActualizarUsuario(Usuario usuario)
    {
        var index = _usuarios.FindIndex(u => u.Id == usuario.Id);
        if (index < 0) return false;
        _usuarios[index] = usuario;
        return true;
    }

    public static bool EliminarUsuario(int id)
    {
        var usuario = _usuarios.FirstOrDefault(u => u.Id == id);
        if (usuario == null) return false;
        return _usuarios.Remove(usuario);
    }

    #endregion

    #region Métodos de Acceso - Empleados

    public static List<Empleado> ObtenerTodosEmpleados() => new(_empleados);

    public static Empleado? ObtenerEmpleadoPorId(int id) =>
        _empleados.FirstOrDefault(e => e.Id == id);

    public static Empleado? ValidarCredenciales(string nombreUsuario, string password) =>
        _empleados.FirstOrDefault(e =>
            e.NombreUsuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) &&
            e.Password == password &&
            e.Activo);

    public static void InsertarEmpleado(Empleado empleado)
    {
        empleado.Id = _nextEmpleadoId++;
        _empleados.Add(empleado);
    }

    #endregion

    #region Métodos de Acceso - Documentos

    public static List<Documento> ObtenerTodosDocumentos() => new(_documentos);

    public static List<Libro> ObtenerTodosLibros() =>
        _documentos.OfType<Libro>().ToList();

    public static List<Audiolibro> ObtenerTodosAudiolibros() =>
        _documentos.OfType<Audiolibro>().ToList();

    public static Documento? ObtenerDocumentoPorId(int id) =>
        _documentos.FirstOrDefault(d => d.Id == id);

    public static Documento? ObtenerDocumentoPorCodigo(string codigo) =>
        _documentos.FirstOrDefault(d => d.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));

    public static void InsertarDocumento(Documento documento)
    {
        documento.Id = _nextDocumentoId++;
        _documentos.Add(documento);
    }

    public static bool ActualizarDocumento(Documento documento)
    {
        var index = _documentos.FindIndex(d => d.Id == documento.Id);
        if (index < 0) return false;
        _documentos[index] = documento;
        return true;
    }

    public static bool EliminarDocumento(int id)
    {
        var documento = _documentos.FirstOrDefault(d => d.Id == id);
        if (documento == null) return false;
        return _documentos.Remove(documento);
    }

    #endregion

    #region Métodos de Acceso - Ejemplares

    public static List<Ejemplar> ObtenerTodosEjemplares() => new(_ejemplares);

    public static List<Ejemplar> ObtenerEjemplaresPorDocumento(int documentoId) =>
        _ejemplares.Where(e => e.DocumentoId == documentoId).ToList();

    public static List<Ejemplar> ObtenerEjemplaresDisponibles() =>
        _ejemplares.Where(e => e.Estado == EstadoEjemplar.Disponible).ToList();

    public static Ejemplar? ObtenerEjemplarPorId(int id) =>
        _ejemplares.FirstOrDefault(e => e.Id == id);

    public static Ejemplar? ObtenerEjemplarPorCodigoBarras(string codigoBarras) =>
        _ejemplares.FirstOrDefault(e => e.CodigoBarras.Equals(codigoBarras, StringComparison.OrdinalIgnoreCase));

    public static void InsertarEjemplar(Ejemplar ejemplar)
    {
        ejemplar.Id = _nextEjemplarId++;
        _ejemplares.Add(ejemplar);
    }

    public static bool ActualizarEjemplar(Ejemplar ejemplar)
    {
        var index = _ejemplares.FindIndex(e => e.Id == ejemplar.Id);
        if (index < 0) return false;
        _ejemplares[index] = ejemplar;
        return true;
    }

    #endregion

    #region Métodos de Acceso - Préstamos

    public static List<Prestamo> ObtenerTodosPrestamos() => new(_prestamos);

    public static List<Prestamo> ObtenerPrestamosActivos() =>
        _prestamos.Where(p => p.Estado == EstadoPrestamo.Activo).ToList();

    public static List<Prestamo> ObtenerPrestamosPorUsuario(int usuarioId) =>
        _prestamos.Where(p => p.UsuarioId == usuarioId).ToList();

    public static List<Prestamo> ObtenerPrestamosActivosPorUsuario(int usuarioId) =>
        _prestamos.Where(p => p.UsuarioId == usuarioId && p.Estado == EstadoPrestamo.Activo).ToList();

    public static Prestamo? ObtenerPrestamoPorId(int id) =>
        _prestamos.FirstOrDefault(p => p.Id == id);

    public static Prestamo? ObtenerPrestamoPorCodigo(string codigo) =>
        _prestamos.FirstOrDefault(p => p.CodigoPrestamo.Equals(codigo, StringComparison.OrdinalIgnoreCase));

    public static void InsertarPrestamo(Prestamo prestamo)
    {
        prestamo.Id = _nextPrestamoId++;
        prestamo.CodigoPrestamo = $"PRE-{DateTime.Now:yyyy}-{prestamo.Id:D3}";
        _prestamos.Add(prestamo);
    }

    public static bool ActualizarPrestamo(Prestamo prestamo)
    {
        var index = _prestamos.FindIndex(p => p.Id == prestamo.Id);
        if (index < 0) return false;
        _prestamos[index] = prestamo;
        return true;
    }

    #endregion

    #region Estadísticas

    public static int ContarUsuarios() => _usuarios.Count;
    public static int ContarDocumentos() => _documentos.Count;
    public static int ContarEjemplares() => _ejemplares.Count;
    public static int ContarPrestamosActivos() => _prestamos.Count(p => p.Estado == EstadoPrestamo.Activo);

    #endregion
}
