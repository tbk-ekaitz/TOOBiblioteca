using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario principal MDI.
/// Gestiona el menú con seguridad basada en roles y contiene los formularios hijos.
/// </summary>
public class FormPrincipal : Form
{
    private MenuStrip menuPrincipal;
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

        // Menú Usuarios (PersonalSala y Admin)
        menuUsuarios = new ToolStripMenuItem("&Usuarios");
        menuUsuarios.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Buscar Usuario", null, BuscarUsuario_Click, Keys.Control | Keys.B),
            new ToolStripMenuItem("&Alta Usuario", null, AltaUsuario_Click, Keys.Control | Keys.N),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Listado de Usuarios", null, ListadoUsuarios_Click, Keys.Control | Keys.L)
        });

        // Menú Catálogo (PersonalAdquisiciones y Admin)
        menuCatalogo = new ToolStripMenuItem("&Catálogo");
        menuCatalogo.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Ver Catálogo", null, VerCatalogo_Click),
            new ToolStripMenuItem("&Alta Documento", null, AltaDocumento_Click),
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
            new ToolStripMenuItem("&Préstamos Activos", null, PrestamosActivos_Click),
            new ToolStripMenuItem("&Préstamos Vencidos", null, PrestamosVencidos_Click)
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
        menuVentana.MdiWindowListItem = true;

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

        // StatusStrip
        statusBar = new StatusStrip();
        lblUsuario = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblRol = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblFecha = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleRight };

        statusBar.Items.AddRange(new ToolStripItem[] { lblUsuario, lblRol, lblFecha });

        // Añadir controles al formulario
        MainMenuStrip = menuPrincipal;
        Controls.Add(menuPrincipal);
        Controls.Add(statusBar);

        // Timer para actualizar fecha/hora
        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) => lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        timer.Start();
    }

    /// <summary>
    /// Configura la visibilidad de los menús según el rol del empleado.
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
                break;

            case Role.PersonalAdquisiciones:
                // Personal de adquisiciones: catálogo, NO usuarios ni préstamos
                menuUsuarios.Visible = false;
                menuPrestamos.Visible = false;
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
                    // Reiniciar con nuevo usuario (en una aplicación real, se recargaría el formulario)
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
        MessageBox.Show("Funcionalidad de Alta de Documento.\n(Implementación pendiente)",
            "Alta Documento", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        MessageBox.Show("Funcionalidad de Devolución.\n(Implementación pendiente)",
            "Registrar Devolución", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void PrestamosActivos_Click(object? sender, EventArgs e)
    {
        var prestamos = NegocioPrestamos.ObtenerActivosOrdenados();
        MessageBox.Show($"Préstamos activos: {prestamos.Count}",
            "Préstamos Activos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void PrestamosVencidos_Click(object? sender, EventArgs e)
    {
        var prestamos = NegocioPrestamos.ObtenerVencidos();
        MessageBox.Show($"Préstamos vencidos: {prestamos.Count}",
            "Préstamos Vencidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            "Arquitectura N-Capas con LINQ",
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
