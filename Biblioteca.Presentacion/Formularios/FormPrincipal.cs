using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario principal MDI.
/// Gestiona el menú con seguridad basada en roles y contiene los formularios hijos.
/// Incluye ToolStrip para acceso rápido a funciones principales.
/// </summary>
public class FormPrincipal : Form
{
    private MenuStrip menuPrincipal;
    private ToolStrip toolStripPrincipal;
    private StatusStrip statusBar;
    private ToolStripStatusLabel lblUsuario;
    private ToolStripStatusLabel lblRol;
    private ToolStripStatusLabel lblFecha;

    private ToolStripMenuItem menuArchivo;
    private ToolStripMenuItem menuUsuarios;
    private ToolStripMenuItem menuCatalogo;
    private ToolStripMenuItem menuPrestamos;
    private ToolStripMenuItem menuVentana;
    private ToolStripMenuItem menuAyuda;

    // ToolStrip buttons
    private ToolStripButton tsbNuevoUsuario;
    private ToolStripButton tsbBuscarUsuario;
    private ToolStripButton tsbNuevoDocumento;
    private ToolStripButton tsbVerCatalogo;
    private ToolStripButton tsbNuevoPrestamo;
    private ToolStripButton tsbDevolucion;
    private ToolStripButton tsbPrestamosVencidos;
    private ToolStripButton tsbCerrarSesion;

    private readonly Empleado _empleadoActual;

    public FormPrincipal(Empleado empleado)
    {
        _empleadoActual = empleado;
        InitializeComponent();
        ConfigurarSeguridadMenu();
        ActualizarStatusBar();
    }

    private void InitializeComponent()
    {
        // Configuración MDI
        Text = "Sistema de Gestión de Biblioteca";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        IsMdiContainer = true;
        WindowState = FormWindowState.Maximized;

        // Crear MenuStrip
        menuPrincipal = new MenuStrip();

        // Menú Archivo
        menuArchivo = new ToolStripMenuItem("&Archivo");
        menuArchivo.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Cerrar sesión", null, CerrarSesion_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Salir", null, Salir_Click, Keys.Alt | Keys.F4)
        });

        // Menú Usuarios (PersonalSala, PersonalAdquisiciones y Admin)
        menuUsuarios = new ToolStripMenuItem("&Usuarios");
        menuUsuarios.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Buscar Usuario", null, BuscarUsuario_Click, Keys.Control | Keys.B),
            new ToolStripMenuItem("&Alta Usuario", null, AltaUsuario_Click, Keys.Control | Keys.N),
            new ToolStripMenuItem("Ba&ja Usuario", null, BajaUsuario_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Listado de Usuarios", null, ListadoUsuarios_Click, Keys.Control | Keys.L)
        });

        // Menú Catálogo (PersonalAdquisiciones y Admin)
        menuCatalogo = new ToolStripMenuItem("&Catálogo");
        menuCatalogo.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Ver Catálogo", null, VerCatalogo_Click),
            new ToolStripMenuItem("&Alta Documento", null, AltaDocumento_Click),
            new ToolStripMenuItem("&Gestión Ejemplares", null, GestionEjemplares_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Documento &Más Leído", null, DocumentoMasLeido_Click),
            new ToolStripMenuItem("&Disponibilidad", null, ConsultaDisponibilidad_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Estadísticas", null, Estadisticas_Click)
        });

        // Menú Préstamos (PersonalSala y Admin)
        menuPrestamos = new ToolStripMenuItem("&Préstamos");
        menuPrestamos.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Nuevo Préstamo", null, NuevoPrestamo_Click, Keys.Control | Keys.P),
            new ToolStripMenuItem("&Registrar Devolución", null, RegistrarDevolucion_Click, Keys.Control | Keys.D),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Consultar Préstamo", null, ConsultarPrestamo_Click),
            new ToolStripMenuItem("&Listado Préstamos", null, ListadoPrestamos_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("Préstamos &Activos", null, PrestamosActivos_Click),
            new ToolStripMenuItem("Préstamos &Vencidos", null, PrestamosVencidos_Click)
        });

        // Menú Ventana
        menuVentana = new ToolStripMenuItem("&Ventana");
        menuVentana.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Cascada", null, (s, e) => LayoutMdi(MdiLayout.Cascade)),
            new ToolStripMenuItem("Mosaico &Horizontal", null, (s, e) => LayoutMdi(MdiLayout.TileHorizontal)),
            new ToolStripMenuItem("Mosaico &Vertical", null, (s, e) => LayoutMdi(MdiLayout.TileVertical)),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Cerrar todas", null, CerrarTodas_Click)
        });

        // Menú Ayuda
        menuAyuda = new ToolStripMenuItem("A&yuda");
        menuAyuda.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Acerca de...", null, AcercaDe_Click)
        });

        // Añadir menús
        menuPrincipal.Items.AddRange(new ToolStripItem[]
        {
            menuArchivo, menuUsuarios, menuCatalogo, menuPrestamos, menuVentana, menuAyuda
        });
        menuPrincipal.MdiWindowListItem = menuVentana;

        // ToolStrip
        toolStripPrincipal = new ToolStrip();
        toolStripPrincipal.ImageScalingSize = new Size(24, 24);

        tsbNuevoUsuario = new ToolStripButton
        {
            Text = "Nuevo Usuario",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Alta de nuevo usuario (Ctrl+N)"
        };
        tsbNuevoUsuario.Click += AltaUsuario_Click;

        tsbBuscarUsuario = new ToolStripButton
        {
            Text = "Buscar Usuario",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Buscar usuario por DNI (Ctrl+B)"
        };
        tsbBuscarUsuario.Click += BuscarUsuario_Click;

        tsbNuevoDocumento = new ToolStripButton
        {
            Text = "Nuevo Documento",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Alta de nuevo documento"
        };
        tsbNuevoDocumento.Click += AltaDocumento_Click;

        tsbVerCatalogo = new ToolStripButton
        {
            Text = "Catálogo",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Ver catálogo de documentos"
        };
        tsbVerCatalogo.Click += VerCatalogo_Click;

        tsbNuevoPrestamo = new ToolStripButton
        {
            Text = "Nuevo Préstamo",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Registrar nuevo préstamo (Ctrl+P)"
        };
        tsbNuevoPrestamo.Click += NuevoPrestamo_Click;

        tsbDevolucion = new ToolStripButton
        {
            Text = "Devolución",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Registrar devolución (Ctrl+D)"
        };
        tsbDevolucion.Click += RegistrarDevolucion_Click;

        tsbPrestamosVencidos = new ToolStripButton
        {
            Text = "Vencidos",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Ver préstamos vencidos"
        };
        tsbPrestamosVencidos.Click += PrestamosVencidos_Click;

        tsbCerrarSesion = new ToolStripButton
        {
            Text = "Cerrar Sesión",
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            ToolTipText = "Cerrar sesión actual",
            Alignment = ToolStripItemAlignment.Right
        };
        tsbCerrarSesion.Click += CerrarSesion_Click;

        toolStripPrincipal.Items.AddRange(new ToolStripItem[]
        {
            tsbNuevoUsuario,
            tsbBuscarUsuario,
            new ToolStripSeparator(),
            tsbNuevoDocumento,
            tsbVerCatalogo,
            new ToolStripSeparator(),
            tsbNuevoPrestamo,
            tsbDevolucion,
            tsbPrestamosVencidos,
            new ToolStripSeparator(),
            tsbCerrarSesion
        });

        // StatusStrip
        statusBar = new StatusStrip();
        lblUsuario = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblRol = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblFecha = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleRight };

        statusBar.Items.AddRange(new ToolStripItem[] { lblUsuario, lblRol, lblFecha });

        // Añadir controles al formulario
        MainMenuStrip = menuPrincipal;
        Controls.Add(toolStripPrincipal);
        Controls.Add(menuPrincipal);
        Controls.Add(statusBar);

        // Timer para actualizar fecha/hora
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        timer.Start();
    }

    /// <summary>
    /// Configura la visibilidad de los menús y ToolStrip según el rol del empleado.
    /// - Administrador: acceso total
    /// - PersonalSala: Usuarios y Préstamos (NO catálogo)
    /// - PersonalAdquisiciones: Usuarios y Catálogo (NO préstamos)
    /// </summary>
    private void ConfigurarSeguridadMenu()
    {
        switch (_empleadoActual.Rol)
        {
            case Role.Administrador:
                // El administrador ve todo
                break;

            case Role.PersonalSala:
                // Personal de sala: usuarios y préstamos, NO catálogo
                menuCatalogo.Visible = false;
                tsbNuevoDocumento.Visible = false;
                tsbVerCatalogo.Visible = false;
                break;

            case Role.PersonalAdquisiciones:
                // Personal de adquisiciones: usuarios y catálogo, NO préstamos
                menuPrestamos.Visible = false;
                tsbNuevoPrestamo.Visible = false;
                tsbDevolucion.Visible = false;
                tsbPrestamosVencidos.Visible = false;
                break;
        }
    }

    private void ActualizarStatusBar()
    {
        lblUsuario.Text = $"Usuario: {_empleadoActual.NombreCompleto}";
        lblRol.Text = $"Rol: {_empleadoActual.Rol}";
        lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    #region Manejadores de Eventos del Menú

    private void CerrarSesion_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("¿Desea cerrar la sesión actual?", "Cerrar Sesión",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            // Cerrar todos los formularios hijos
            foreach (Form child in MdiChildren)
            {
                child.Close();
            }

            // Mostrar nuevo login
            Hide();
            using (var formLogin = new FormLogin())
            {
                if (formLogin.ShowDialog() == DialogResult.OK && formLogin.EmpleadoLogueado != null)
                {
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }

    private void Salir_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("¿Desea salir de la aplicación?", "Salir",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            Application.Exit();
        }
    }

    private void BuscarUsuario_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormSolicitarDNI>(FormSolicitarDNI.Modo.Buscar);
    }

    private void AltaUsuario_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormSolicitarDNI>(FormSolicitarDNI.Modo.Alta);
    }

    private void BajaUsuario_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormBajaUsuario>();
    }

    private void ListadoUsuarios_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormListados>();
    }

    private void VerCatalogo_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormDocumentos>();
    }

    private void AltaDocumento_Click(object? sender, EventArgs e)
    {
        var form = new FormGestionDocumentos(FormGestionDocumentos.Modo.Alta, _empleadoActual);
        form.MdiParent = this;
        form.Show();
    }

    private void GestionEjemplares_Click(object? sender, EventArgs e)
    {
        var form = new FormGestionEjemplares(_empleadoActual);
        form.MdiParent = this;
        form.Show();
    }

    private void DocumentoMasLeido_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormDocumentoMasLeido>();
    }

    private void ConsultaDisponibilidad_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormDisponibilidad>();
    }

    private void Estadisticas_Click(object? sender, EventArgs e)
    {
        MostrarEstadisticas();
    }

    private void NuevoPrestamo_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormAltaPrestamo>();
    }

    private void RegistrarDevolucion_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormDevolucion>();
    }

    private void ConsultarPrestamo_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormConsultaPrestamo>();
    }

    private void ListadoPrestamos_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormListadoPrestamos>();
    }

    private void PrestamosActivos_Click(object? sender, EventArgs e)
    {
        var prestamos = NegocioPrestamos.ObtenerActivosOrdenados();
        MessageBox.Show($"Préstamos activos: {prestamos.Count}",
            "Préstamos Activos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void PrestamosVencidos_Click(object? sender, EventArgs e)
    {
        AbrirFormularioHijo<FormPrestamosFueraPlazo>();
    }

    private void CerrarTodas_Click(object? sender, EventArgs e)
    {
        foreach (Form child in MdiChildren)
        {
            child.Close();
        }
    }

    private void AcercaDe_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Sistema de Gestión de Biblioteca\n" +
            "Versión 1.0\n\n" +
            "Desarrollado para prácticas de Windows Forms\n" +
            "Arquitectura N-Capas con LINQ\n\n" +
            $"Usuario actual: {_empleadoActual.NombreCompleto}\n" +
            $"Rol: {_empleadoActual.Rol}",
            "Acerca de",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Abre un formulario hijo MDI, evitando duplicados.
    /// </summary>
    private void AbrirFormularioHijo<T>() where T : Form, new()
    {
        // Buscar si ya existe
        foreach (Form child in MdiChildren)
        {
            if (child is T)
            {
                child.Activate();
                return;
            }
        }

        // Crear nuevo
        var form = new T();
        form.MdiParent = this;
        form.Show();
    }

    /// <summary>
    /// Abre un formulario hijo con un modo específico.
    /// </summary>
    private void AbrirFormularioHijo<T>(object modo) where T : Form
    {
        var form = (T)Activator.CreateInstance(typeof(T), modo)!;
        form.MdiParent = this;
        form.Show();
    }

    private void MostrarEstadisticas()
    {
        var (libros, audiolibros) = NegocioDocumentos.ContarPorTipo();
        var masLeido = NegocioDocumentos.ObtenerMasLeido();
        var stats = NegocioPrestamos.ObtenerEstadisticas();

        var mensaje =
            $"=== ESTADÍSTICAS DEL CATÁLOGO ===\n\n" +
            $"Libros: {libros}\n" +
            $"Audiolibros: {audiolibros}\n" +
            $"Total documentos: {libros + audiolibros}\n\n" +
            $"Documento más leído: {masLeido?.Titulo ?? "N/A"}\n\n" +
            $"=== ESTADÍSTICAS DE PRÉSTAMOS ===\n\n" +
            $"Total préstamos: {stats.Total}\n" +
            $"Activos: {stats.Activos}\n" +
            $"Vencidos: {stats.Vencidos}\n" +
            $"Devueltos este mes: {stats.DevueltosEsteMes}";

        MessageBox.Show(mensaje, "Estadísticas", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            if (MessageBox.Show("¿Desea salir de la aplicación?", "Confirmar salida",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
        }
        base.OnFormClosing(e);
    }
}
