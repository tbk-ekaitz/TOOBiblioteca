using Biblioteca.Dominio;

namespace Biblioteca.Datos;

/// <summary>
/// Repositorio central de datos simulados.
/// Actúa como una base de datos en memoria con datos de prueba precargados.
/// Utiliza identificadores string (DNI, Codigo, CodigoBarras, Id).
/// </summary>
public static class Repositorio
{
    #region Colecciones de Datos

    private static List<Usuario> _usuarios = new();
    private static List<Empleado> _empleados = new();
    private static List<Documento> _documentos = new();
    private static List<Ejemplar> _ejemplares = new();
    private static List<Prestamo> _prestamos = new();

    private static int _nextPrestamoNum = 1;

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
        // Empleado que registró los documentos de prueba (admin)
        var empleadoAlta = _empleados[0];

        // Libros (Codigo = ISBN)
        var libros = new List<Libro>
        {
            new Libro
            {
                Codigo = "978-84-376-0494-7",
                Titulo = "Don Quijote de la Mancha",
                Autor = "Miguel de Cervantes",
                Editorial = "Cátedra",
                AnioPublicacion = 1605,
                Genero = "Novela",
                NumeroPaginas = 1250,
                Edicion = "Edición conmemorativa",
                Encuadernacion = "Tapa dura",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-204-8475-2",
                Titulo = "Cien años de soledad",
                Autor = "Gabriel García Márquez",
                Editorial = "Alfaguara",
                AnioPublicacion = 1967,
                Genero = "Realismo mágico",
                NumeroPaginas = 496,
                Edicion = "50 aniversario",
                Encuadernacion = "Tapa blanda",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-339-7896-3",
                Titulo = "1984",
                Autor = "George Orwell",
                Editorial = "Debolsillo",
                AnioPublicacion = 1949,
                Genero = "Distopía",
                NumeroPaginas = 352,
                Encuadernacion = "Tapa blanda",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-450-7789-4",
                Titulo = "El Principito",
                Autor = "Antoine de Saint-Exupéry",
                Editorial = "Salamandra",
                AnioPublicacion = 1943,
                Genero = "Fábula",
                NumeroPaginas = 96,
                Encuadernacion = "Tapa dura",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-233-4789-5",
                Titulo = "La sombra del viento",
                Autor = "Carlos Ruiz Zafón",
                Editorial = "Planeta",
                AnioPublicacion = 2001,
                Genero = "Misterio",
                NumeroPaginas = 576,
                Encuadernacion = "Tapa blanda",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-672-3456-6",
                Titulo = "Rayuela",
                Autor = "Julio Cortázar",
                Editorial = "Cátedra",
                AnioPublicacion = 1963,
                Genero = "Novela experimental",
                NumeroPaginas = 736,
                Encuadernacion = "Tapa blanda",
                EmpleadoAlta = empleadoAlta
            },
            new Libro
            {
                Codigo = "978-84-9838-789-7",
                Titulo = "El amor en los tiempos del cólera",
                Autor = "Gabriel García Márquez",
                Editorial = "Debolsillo",
                AnioPublicacion = 1985,
                Genero = "Romance",
                NumeroPaginas = 496,
                Encuadernacion = "Tapa blanda",
                EmpleadoAlta = empleadoAlta
            }
        };

        // Audiolibros (Codigo = ISBN de edición audio)
        var audiolibros = new List<Audiolibro>
        {
            new Audiolibro
            {
                Codigo = "978-84-9838-501-5",
                Titulo = "Harry Potter y la piedra filosofal",
                Autor = "J.K. Rowling",
                Editorial = "Salamandra Audio",
                AnioPublicacion = 1997,
                Genero = "Fantasía",
                Narrador = "Carlos Ponce",
                DuracionMinutos = 520,
                Formato = FormatoAudio.MP3,
                EmpleadoAlta = empleadoAlta
            },
            new Audiolibro
            {
                Codigo = "978-84-01-35267-8",
                Titulo = "El nombre del viento",
                Autor = "Patrick Rothfuss",
                Editorial = "Plaza & Janés Audio",
                AnioPublicacion = 2007,
                Genero = "Fantasía épica",
                Narrador = "Raúl Llorens",
                DuracionMinutos = 1680,
                Formato = FormatoAudio.MP3,
                EmpleadoAlta = empleadoAlta
            },
            new Audiolibro
            {
                Codigo = "978-84-9992-468-3",
                Titulo = "Sapiens: De animales a dioses",
                Autor = "Yuval Noah Harari",
                Editorial = "Debate Audio",
                AnioPublicacion = 2011,
                Genero = "Historia",
                Narrador = "Miguel Ángel Jenner",
                DuracionMinutos = 900,
                Formato = FormatoAudio.AAC,
                NumeroDiscos = 2,
                EmpleadoAlta = empleadoAlta
            }
        };

        _documentos.AddRange(libros);
        _documentos.AddRange(audiolibros);
    }

    private static void CargarEjemplares()
    {
        // Empleado que registró los ejemplares de prueba (admin)
        var empleadoAlta = _empleados[0];

        int ejemplarNum = 1;
        foreach (var documento in _documentos)
        {
            // Crear 2-3 ejemplares por documento
            int numEjemplares = ejemplarNum % 2 == 0 ? 3 : 2;

            for (int i = 1; i <= numEjemplares; i++)
            {
                _ejemplares.Add(new Ejemplar
                {
                    CodigoBarras = $"{documento.Codigo}-{i:D2}",
                    Documento = documento,
                    Estado = EstadoEjemplar.Disponible,
                    Ubicacion = $"Estante {(char)('A' + (ejemplarNum % 5))}{ejemplarNum}, Fila {i}",
                    FechaAdquisicion = documento.FechaAlta.AddDays(-30),
                    EmpleadoAlta = empleadoAlta
                });
            }
            ejemplarNum++;
        }
    }

    private static void CargarPrestamosHistoricos()
    {
        // Préstamo 1: Histórico devuelto
        var prestamo1 = new Prestamo
        {
            Id = GenerarIdPrestamo(),
            Usuario = _usuarios[0],
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-30),
            Estado = EstadoPrestamo.Devuelto,
            FechaDevolucionReal = DateTime.Now.AddDays(-10)
        };
        prestamo1.Ejemplares.Add(_ejemplares[0]);
        prestamo1.CalcularFechaDevolucion();
        _ejemplares[0].VecesPrestado++;

        // Préstamo 2: Activo
        var prestamo2 = new Prestamo
        {
            Id = GenerarIdPrestamo(),
            Usuario = _usuarios[1],
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-50),
            Estado = EstadoPrestamo.Activo
        };
        var ejemplar2 = _ejemplares[2];
        ejemplar2.Estado = EstadoEjemplar.Prestado;
        prestamo2.Ejemplares.Add(ejemplar2);
        prestamo2.CalcularFechaDevolucion();
        ejemplar2.VecesPrestado++;

        // Préstamo 3: Activo con múltiples ejemplares
        var prestamo3 = new Prestamo
        {
            Id = GenerarIdPrestamo(),
            Usuario = _usuarios[3],
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

        // Préstamo 4: Vencido (para pruebas de sanciones)
        var prestamo4 = new Prestamo
        {
            Id = GenerarIdPrestamo(),
            Usuario = _usuarios[2],
            Empleado = _empleados[1],
            FechaPrestamo = DateTime.Now.AddDays(-35),
            FechaDevolucionPrevista = DateTime.Now.AddDays(-14),
            Estado = EstadoPrestamo.Vencido
        };
        var ejemplarVencido = _ejemplares[10];
        ejemplarVencido.Estado = EstadoEjemplar.Prestado;
        prestamo4.Ejemplares.Add(ejemplarVencido);
        ejemplarVencido.VecesPrestado++;

        var ejemplarAudio = _ejemplares.FirstOrDefault(e => e.Documento is Audiolibro);

        if (ejemplarAudio != null)
        {
            var prestamo5 = new Prestamo
            {
                Id = GenerarIdPrestamo(),
                Usuario = _usuarios[4],
                Empleado = _empleados[0],
                FechaPrestamo = DateTime.Now.AddDays(-2),
                Estado = EstadoPrestamo.Activo
            };

            ejemplarAudio.Estado = EstadoEjemplar.Prestado;
            prestamo5.Ejemplares.Add(ejemplarAudio);
            prestamo5.CalcularFechaDevolucion();
            ejemplarAudio.VecesPrestado++;

            _prestamos.Add(prestamo5);
        }
        

        _prestamos.AddRange(new[] { prestamo1, prestamo2, prestamo3, prestamo4 });
    }

    private static string GenerarIdPrestamo()
    {
        return $"PRE-{DateTime.Now:yyyy}-{_nextPrestamoNum++:D3}";
    }

    #endregion

    #region Métodos de Acceso - Usuarios

    public static List<Usuario> ObtenerTodosUsuarios() => new(_usuarios);

    public static Usuario? ObtenerUsuarioPorDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return null;
        return _usuarios.FirstOrDefault(u => u.DNI.Equals(dni.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static bool ExisteUsuario(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return false;
        return _usuarios.Any(u => u.DNI.Equals(dni.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static void InsertarUsuario(Usuario usuario)
    {
        _usuarios.Add(usuario);
    }

    public static bool ActualizarUsuario(Usuario usuario)
    {
        var index = _usuarios.FindIndex(u => u.DNI.Equals(usuario.DNI, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _usuarios[index] = usuario;
        return true;
    }

    public static bool EliminarUsuario(string dni)
    {
        var usuario = ObtenerUsuarioPorDNI(dni);
        if (usuario == null) return false;
        return _usuarios.Remove(usuario);
    }

    #endregion

    #region Métodos de Acceso - Empleados

    public static List<Empleado> ObtenerTodosEmpleados() => new(_empleados);

    public static Empleado? ObtenerEmpleadoPorDNI(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return null;
        return _empleados.FirstOrDefault(e => e.DNI.Equals(dni.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static Empleado? ValidarCredenciales(string nombreUsuario, string password) =>
        _empleados.FirstOrDefault(e =>
            e.NombreUsuario.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase) &&
            e.Password == password &&
            e.Activo);

    public static Empleado? ValidarCredencialesPorDNI(string dni, string password) =>
        _empleados.FirstOrDefault(e =>
            e.DNI.Equals(dni, StringComparison.OrdinalIgnoreCase) &&
            e.Password == password &&
            e.Activo);

    public static void InsertarEmpleado(Empleado empleado)
    {
        _empleados.Add(empleado);
    }

    #endregion

    #region Métodos de Acceso - Documentos

    public static List<Documento> ObtenerTodosDocumentos() => new(_documentos);

    public static List<Libro> ObtenerTodosLibros() =>
        _documentos.OfType<Libro>().ToList();

    public static List<Audiolibro> ObtenerTodosAudiolibros() =>
        _documentos.OfType<Audiolibro>().ToList();

    public static Documento? ObtenerDocumentoPorCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;
        return _documentos.FirstOrDefault(d => d.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static bool ExisteDocumento(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return false;
        return _documentos.Any(d => d.Codigo.Equals(codigo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static void InsertarDocumento(Documento documento)
    {
        _documentos.Add(documento);
    }

    public static bool ActualizarDocumento(Documento documento)
    {
        var index = _documentos.FindIndex(d => d.Codigo.Equals(documento.Codigo, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _documentos[index] = documento;
        return true;
    }

    public static bool EliminarDocumento(string codigo)
    {
        var documento = ObtenerDocumentoPorCodigo(codigo);
        if (documento == null) return false;
        return _documentos.Remove(documento);
    }

    #endregion

    #region Métodos de Acceso - Ejemplares

    public static List<Ejemplar> ObtenerTodosEjemplares() => new(_ejemplares);

    public static List<Ejemplar> ObtenerEjemplaresPorDocumento(string codigoDocumento)
    {
        if (string.IsNullOrWhiteSpace(codigoDocumento)) return new List<Ejemplar>();
        return _ejemplares.Where(e => e.Documento.Codigo.Equals(codigoDocumento.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public static List<Ejemplar> ObtenerEjemplaresDisponibles() =>
        _ejemplares.Where(e => e.Estado == EstadoEjemplar.Disponible).ToList();

    public static Ejemplar? ObtenerEjemplarPorCodigoBarras(string codigoBarras)
    {
        if (string.IsNullOrWhiteSpace(codigoBarras)) return null;
        return _ejemplares.FirstOrDefault(e => e.CodigoBarras.Equals(codigoBarras.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static void InsertarEjemplar(Ejemplar ejemplar)
    {
        _ejemplares.Add(ejemplar);
    }

    public static bool ActualizarEjemplar(Ejemplar ejemplar)
    {
        var index = _ejemplares.FindIndex(e => e.CodigoBarras.Equals(ejemplar.CodigoBarras, StringComparison.OrdinalIgnoreCase));
        if (index < 0) return false;
        _ejemplares[index] = ejemplar;
        return true;
    }

    public static bool EliminarEjemplar(string codigoBarras)
    {
        var ejemplar = ObtenerEjemplarPorCodigoBarras(codigoBarras);
        if (ejemplar == null) return false;
        return _ejemplares.Remove(ejemplar);
    }

    #endregion

    #region Métodos de Acceso - Préstamos

    public static List<Prestamo> ObtenerTodosPrestamos() => new(_prestamos);

    public static List<Prestamo> ObtenerPrestamosActivos() =>
        _prestamos.Where(p => p.Estado == EstadoPrestamo.Activo).ToList();

    public static List<Prestamo> ObtenerPrestamosPorUsuario(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return new List<Prestamo>();
        return _prestamos.Where(p => p.Usuario.DNI.Equals(dni.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public static List<Prestamo> ObtenerPrestamosActivosPorUsuario(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni)) return new List<Prestamo>();
        return _prestamos.Where(p =>
            p.Usuario.DNI.Equals(dni.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.Estado == EstadoPrestamo.Activo).ToList();
    }

    public static Prestamo? ObtenerPrestamoPorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return _prestamos.FirstOrDefault(p => p.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public static List<Prestamo> ObtenerPrestamosVencidos() =>
        _prestamos.Where(p => p.Estado == EstadoPrestamo.Activo && p.EstaVencido).ToList();

    public static List<Prestamo> ObtenerPrestamosDeDocumento(string codigoDocumento)
    {
        if (string.IsNullOrWhiteSpace(codigoDocumento)) return new List<Prestamo>();
        return _prestamos.Where(p =>
            p.Ejemplares.Any(e => e.Documento.Codigo.Equals(codigoDocumento.Trim(), StringComparison.OrdinalIgnoreCase))).ToList();
    }

    public static void InsertarPrestamo(Prestamo prestamo)
    {
        if (string.IsNullOrEmpty(prestamo.Id))
        {
            prestamo.Id = GenerarIdPrestamo();
        }
        _prestamos.Add(prestamo);
    }

    public static bool ActualizarPrestamo(Prestamo prestamo)
    {
        var index = _prestamos.FindIndex(p => p.Id.Equals(prestamo.Id, StringComparison.OrdinalIgnoreCase));
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
