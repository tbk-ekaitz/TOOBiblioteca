using Biblioteca.Dominio;
using Biblioteca.Negocio;
using Biblioteca.Presentacion.Formularios;

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
    private ToolStripStatusLabel lblInfoInventario;

    private ToolStripMenuItem menuArchivo;
    private ToolStripMenuItem menuUsuarios;
    private ToolStripMenuItem menuCatalogo;
    private ToolStripMenuItem menuPrestamos;
    private ToolStripMenuItem menuVentana;
    private ToolStripMenuItem menuAyuda;

    // ToolStrip
    private ToolStripButton tsbNuevoUsuario;
    private ToolStripButton tsbBuscarUsuario;
    private ToolStripButton tsbNuevoDocumento;
    private ToolStripButton tsbVerCatalogo;
    private ToolStripButton tsbEstadisticas;
    private ToolStripButton tsbNuevoPrestamo;
    private ToolStripButton tsbDevolucion;
    private ToolStripButton tsbPrestamosVencidos;
    private ToolStripButton tsbCerrarSesion;
    private ToolStripButton tsbCerrarPrograma;

    private ToolStripMenuItem itemEstadisticas;
    private ToolStripMenuItem itemAltaDocumento;
    private ToolStripMenuItem itemGestionEjemplares;

    private readonly Empleado _empleadoActual;

    public FormPrincipal(Empleado empleado)
    {
        _empleadoActual = empleado;
        InitializeComponent();
        ConfigurarSeguridadMenu();
        ActualizarStatusBar();
    }

    private ToolStripButton CrearBotonToolbar(string texto, EventHandler evento)
    {
        var btn = new ToolStripButton { Text = texto, DisplayStyle = ToolStripItemDisplayStyle.ImageAndText };
        btn.Click += evento;
        return btn;
    }

    private void InitializeComponent()
    {
        Text = "Sistema de Gestión de Biblioteca";
        Size = new Size(1200, 800);
        StartPosition = FormStartPosition.CenterScreen;
        IsMdiContainer = true;
        WindowState = FormWindowState.Maximized;

        menuPrincipal = new MenuStrip();

        menuArchivo = new ToolStripMenuItem("&Archivo");
        menuArchivo.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Cerrar sesión", null, CerrarSesion_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Salir", null, Salir_Click, Keys.Alt | Keys.F4)
        });

        menuUsuarios = new ToolStripMenuItem("&Usuarios");
        menuUsuarios.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Buscar Usuario", null, BuscarUsuario_Click, Keys.Control | Keys.B),
            new ToolStripMenuItem("&Alta Usuario", null, AltaUsuario_Click, Keys.Control | Keys.N),
            new ToolStripMenuItem("Ba&ja Usuario", null, BajaUsuario_Click),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Listado de Usuarios", null, ListadoUsuarios_Click, Keys.Control | Keys.L)
        });

        menuCatalogo = new ToolStripMenuItem("&Documentos");
        itemEstadisticas = new ToolStripMenuItem("&Estadísticas y Análisis", null, VerEstadisticas_Click);
        itemAltaDocumento = new ToolStripMenuItem("&Alta Documento", null, AltaDocumento_Click);
        itemGestionEjemplares = new ToolStripMenuItem("&Gestión Ejemplares", null, GestionEjemplares_Click);
        menuCatalogo.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Ver Catálogo", null, VerCatalogo_Click),
            itemEstadisticas,
            new ToolStripSeparator(),
            itemAltaDocumento,
            itemGestionEjemplares,
            new ToolStripMenuItem("&Disponibilidad", null, ConsultaDisponibilidad_Click),
        });

        menuPrestamos = new ToolStripMenuItem("&Préstamos");
        menuPrestamos.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Nuevo Préstamo", null, NuevoPrestamo_Click, Keys.Control | Keys.P),
            new ToolStripMenuItem("&Registrar Devolución", null, RegistrarDevolucion_Click, Keys.Control | Keys.D),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Consultar Préstamo", null, ConsultarPrestamo_Click),
            new ToolStripMenuItem("&Listado Préstamos", null, ListadoPrestamos_Click),
            new ToolStripSeparator(),
            //new ToolStripMenuItem("Préstamos &Activos", null, PrestamosActivos_Click),
            new ToolStripMenuItem("Préstamos &Vencidos", null, PrestamosVencidos_Click)
        });

        menuVentana = new ToolStripMenuItem("&Ventana");
        menuVentana.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Cascada", null, (s, e) => LayoutMdi(MdiLayout.Cascade)),
            new ToolStripMenuItem("Mosaico &Horizontal", null, (s, e) => LayoutMdi(MdiLayout.TileHorizontal)),
            new ToolStripMenuItem("Mosaico &Vertical", null, (s, e) => LayoutMdi(MdiLayout.TileVertical)),
            new ToolStripSeparator(),
            new ToolStripMenuItem("&Cerrar todas", null, CerrarTodas_Click)
        });

        menuAyuda = new ToolStripMenuItem("A&yuda");
        menuAyuda.DropDownItems.AddRange(new ToolStripItem[]
        {
            new ToolStripMenuItem("&Acerca de...", null, AcercaDe_Click)
        });

        menuPrincipal.Items.AddRange(new ToolStripItem[]
        {
            menuArchivo, menuUsuarios, menuCatalogo, menuPrestamos, menuVentana, menuAyuda
        });
        menuPrincipal.MdiWindowListItem = menuVentana;

        // tsb
        toolStripPrincipal = new ToolStrip { ImageScalingSize = new Size(24, 24) };

        tsbNuevoUsuario = CrearBotonToolbar("Nuevo Usuario", AltaUsuario_Click);
        tsbBuscarUsuario = CrearBotonToolbar("Buscar Usuario", BuscarUsuario_Click);
        tsbNuevoDocumento = CrearBotonToolbar("Nuevo Documento", AltaDocumento_Click);

        tsbVerCatalogo = CrearBotonToolbar("Catálogo", VerCatalogo_Click);
        tsbEstadisticas = CrearBotonToolbar("Estadísticas", VerEstadisticas_Click);

        tsbNuevoPrestamo = CrearBotonToolbar("Nuevo Préstamo", NuevoPrestamo_Click);
        tsbDevolucion = CrearBotonToolbar("Devolución", RegistrarDevolucion_Click);
        tsbPrestamosVencidos = CrearBotonToolbar("Vencidos", PrestamosVencidos_Click);
        tsbCerrarSesion = CrearBotonToolbar("Cerrar sesion", CerrarSesion_Click);
        tsbCerrarPrograma = CrearBotonToolbar("Salir", Salir_Click);
        tsbCerrarSesion.Alignment = ToolStripItemAlignment.Right;
        tsbCerrarPrograma.Alignment = ToolStripItemAlignment.Right;

        toolStripPrincipal.Items.AddRange(new ToolStripItem[] {
            tsbNuevoUsuario, tsbBuscarUsuario, new ToolStripSeparator(),
            tsbNuevoDocumento, tsbVerCatalogo, tsbEstadisticas, new ToolStripSeparator(),
            tsbNuevoPrestamo, tsbDevolucion, tsbPrestamosVencidos, new ToolStripSeparator(),
            tsbCerrarPrograma, tsbCerrarSesion, 
        });

        //;;;

        statusBar = new StatusStrip();
        lblUsuario = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblRol = new ToolStripStatusLabel { BorderSides = ToolStripStatusLabelBorderSides.Right };
        lblFecha = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleRight };

        lblInfoInventario = new ToolStripStatusLabel
        {
            BorderSides = ToolStripStatusLabelBorderSides.Right,
            ForeColor = Color.DarkBlue,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };


        statusBar.Items.AddRange(new ToolStripItem[] { lblUsuario, lblRol, lblInfoInventario, lblFecha });
        Controls.Add(statusBar);
        Controls.Add(toolStripPrincipal);
        Controls.Add(menuPrincipal);

        //;;;

        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (s, e) =>
        {
            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ActualizarContadoresInventario();
        };
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
                break;

            case Role.PersonalSala:
                // Personal de sala: usuarios y préstamos, NO catálogo
                //menuCatalogo.Visible = false;
                itemEstadisticas.Visible = false;
                itemAltaDocumento.Visible = false;
                itemGestionEjemplares.Visible = false;
                //;;;
                tsbNuevoDocumento.Visible = false;
                tsbVerCatalogo.Visible = false;
                tsbEstadisticas.Visible = false;
                break;

            case Role.PersonalAdquisiciones:
                // Personal de adquisiciones: usuarios y catálogo, NO préstamos
                menuPrestamos.Visible = false;
                //;;;
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
        ActualizarContadoresInventario();
    }

    private void ActualizarContadoresInventario()
    {
        var stats = NegocioDocumentos.ObtenerResumenInventario();
        lblInfoInventario.Text = $"Documentos: {stats.TotalDocs} | Ejemplares: [(Total: {stats.TotalEjemplares}), (Prestados: {stats.Prestados}), (Sin prestar: {stats.NoPrestados})]";
    }

    #region Manejadores de Eventos del Menú

    private void CerrarSesion_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("¿Desea cerrar la sesión actual?", "Cerrar Sesión",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            foreach (Form child in MdiChildren) child.Close();
            //Hide();
            //using (var formLogin = new FormLogin())
            //{
            //    if (formLogin.ShowDialog() == DialogResult.OK && formLogin.EmpleadoLogueado != null) Application.Restart();
            //    else Application.Exit();
            //}
            Application.Restart();
            Environment.Exit(0);
        }
    }

    //private void Salir_Click(object? sender, EventArgs e) => Application.Exit();
    private void Salir_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("¿Desea salir de la aplicación?", "Salir",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) Application.Exit();
    }

    private void BuscarUsuario_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormSolicitarDNI>(FormSolicitarDNI.Modo.Buscar);

    private void AltaUsuario_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormSolicitarDNI>(FormSolicitarDNI.Modo.Alta);

    private void BajaUsuario_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormBajaUsuario>();

    private void ListadoUsuarios_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormListados>();

    private void VerCatalogo_Click(object? sender, EventArgs e) => AbrirOCrearDocumentos(0);

    private void VerEstadisticas_Click(object? sender, EventArgs e) => AbrirOCrearDocumentos(1);

    private void AltaDocumento_Click(object? sender, EventArgs e) => new FormGestionDocumentos(FormGestionDocumentos.Modo.Alta, _empleadoActual) { MdiParent = this }.Show();

    private void GestionEjemplares_Click(object? sender, EventArgs e) => new FormGestionEjemplares(_empleadoActual) { MdiParent = this }.Show();

    //private void DocumentoMasLeido_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormDocumentoMasLeido>();

    private void ConsultaDisponibilidad_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormDisponibilidad>();

    private void NuevoPrestamo_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormAltaPrestamo>(this._empleadoActual);

    private void RegistrarDevolucion_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormDevolucion>();

    private void ConsultarPrestamo_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormConsultaPrestamo>();

    private void ListadoPrestamos_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormListadoPrestamos>(_empleadoActual);

    //private void PrestamosActivos_Click(object? sender, EventArgs e)
    //{
    //    var prestamos = NegocioPrestamos.ObtenerActivosOrdenados();
    //    MessageBox.Show($"Préstamos activos: {prestamos.Count}",
    //        "Préstamos Activos", MessageBoxButtons.OK, MessageBoxIcon.Information);
    //}

    private void PrestamosVencidos_Click(object? sender, EventArgs e) => AbrirFormularioHijo<FormPrestamosFueraPlazo>();

    private void CerrarTodas_Click(object? sender, EventArgs e)
    {
        foreach (Form child in MdiChildren) child.Close();
    }

    private void AcercaDe_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Sistema de Gestión de Biblioteca\n\n\n" +
            $"Usuario actual: {_empleadoActual.NombreCompleto}\n" +
            $"Rol: {_empleadoActual.Rol}",
            "Acerca de",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    #endregion

    #region Métodos Auxiliares

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

    private void AbrirFormularioHijo<T>(object modo) where T : Form
    {
        var form = (T)Activator.CreateInstance(typeof(T), modo)!;
        form.MdiParent = this;
        form.Show();
    }

    //      var form = AbrirFormularioUnico<FormDocumentos>();
    //      form.SeleccionarTab(0);
    //private T AbrirFormularioUnico<T>() where T : Form, new()
    //{
    //    foreach (Form child in MdiChildren)
    //    {
    //        if (child is T f)
    //        {
    //            f.Activate();
    //            return f;
    //        }
    //    }
    //    var form = new T();
    //    form.MdiParent = this;
    //    form.Show();
    //    return form;
    //}

    private void AbrirOCrearDocumentos(int indiceTab)
    {
        var formularioExistente = MdiChildren.OfType<FormDocumentos>()
                                             .FirstOrDefault(f => f.TabSeleccionado == indiceTab);

        if (formularioExistente != null)
        {
            formularioExistente.Activate();
        }
        else
        {
            var nuevoForm = new FormDocumentos(_empleadoActual);
            nuevoForm.MdiParent = this;
            nuevoForm.Show();
            nuevoForm.SeleccionarTab(indiceTab);
        }
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
