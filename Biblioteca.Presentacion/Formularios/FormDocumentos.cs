using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario unificado de Documentos.
/// Tab 0: Catálogo (Búsqueda y listado).
/// Tab 1: Estadísticas (Dashboard y análisis).
/// </summary>
public class FormDocumentos : Form
{
    private TabControl tabControl;
    private TabPage tabCatalogo;
    private TabPage tabEstadisticas;

    // === CONTROLES TAB CATÁLOGO ===
    private DataGridView dgvDocumentos;
    private ComboBox cboTipo;
    private ComboBox cboGenero;
    private TextBox txtBuscar;
    private Button btnBuscar;
    private Button btnLimpiar;
    private Label lblResultados;

    // === CONTROLES TAB ESTADÍSTICAS ===
    // Sección 1: Totales
    private Label lblTotalLibros;
    private Label lblTotalAudiolibros;
    private Label lblMasLeidoHistorico;

    // Sección 2: Filtro Mensual (Lo que antes era el popup FormDocumentoMasLeido)
    private GroupBox grpMes;
    private ComboBox cmbMes;
    private NumericUpDown nudAnio;
    private Button btnConsultarMes;
    private TextBox txtResultadoMes; // Para mostrar el título
    private TextBox txtAutorMes;     // Para mostrar el autor

    // Sección 3: Gráficos/Tablas
    private DataGridView dgvTopPrestados; // Top 5
    private DataGridView dgvPorGenero;    // Distribución

    public int TabSeleccionado => tabControl.SelectedIndex;

    public FormDocumentos()
    {
        InitializeComponent();
        CargarDatosCatalogo();
        CargarDatosEstadisticas();
    }

    private void InitializeComponent()
    {
        Text = "Documentos"; // Renombrado como pediste
        Size = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterParent;

        tabControl = new TabControl { Dock = DockStyle.Fill };

        // ==========================================
        // TAB 1: CATÁLOGO (Manteniendo lo que ya tenías)
        // ==========================================
        tabCatalogo = new TabPage("Catálogo");
        
        var pnlFiltros = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };
        
        var lblTipo = new Label { Text = "Tipo:", Location = new Point(15, 20), AutoSize = true };
        cboTipo = new ComboBox { Location = new Point(55, 17), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        cboTipo.Items.AddRange(new object[] { "Todos", "Libros", "Audiolibros" });
        cboTipo.SelectedIndex = 0;
        cboTipo.SelectedIndexChanged += (s, e) => AplicarFiltros();

        var lblGenero = new Label { Text = "Género:", Location = new Point(190, 20), AutoSize = true };
        cboGenero = new ComboBox { Location = new Point(245, 17), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        cboGenero.SelectedIndexChanged += (s, e) => AplicarFiltros();

        txtBuscar = new TextBox { Location = new Point(410, 17), Width = 250, PlaceholderText = "Título o Autor..." };
        
        btnBuscar = new Button { Text = "🔍 Buscar", Location = new Point(670, 15), Size = new Size(90, 27) };
        btnBuscar.Click += (s, e) => AplicarFiltros();

        btnLimpiar = new Button { Text = "Limpiar", Location = new Point(770, 15), Size = new Size(80, 27) };
        btnLimpiar.Click += BtnLimpiar_Click;

        lblResultados = new Label { Location = new Point(870, 20), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };

        pnlFiltros.Controls.AddRange(new Control[] { lblTipo, cboTipo, lblGenero, cboGenero, txtBuscar, btnBuscar, btnLimpiar, lblResultados });

        dgvDocumentos = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false
        };
        dgvDocumentos.CellFormatting += DgvDocumentos_CellFormatting;

        tabCatalogo.Controls.Add(dgvDocumentos);
        tabCatalogo.Controls.Add(pnlFiltros);

        // ==========================================
        // TAB 2: ESTADÍSTICAS (El nuevo Dashboard unificado)
        // ==========================================
        tabEstadisticas = new TabPage("Estadísticas");
        tabEstadisticas.Padding = new Padding(10);

        // -- Panel Superior: Resumen --
        var pnlResumen = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.AliceBlue };
        pnlResumen.Padding = new Padding(10);

        lblTotalLibros = new Label { Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
        lblTotalAudiolibros = new Label { Location = new Point(20, 45), AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
        lblMasLeidoHistorico = new Label { Location = new Point(300, 30), AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.DarkBlue };

        pnlResumen.Controls.AddRange(new Control[] { lblTotalLibros, lblTotalAudiolibros, lblMasLeidoHistorico });

        // -- Panel Izquierdo: Filtro Mes y Top 5 --
        var splitStats = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 450 };
        splitStats.Panel1.Padding = new Padding(10);
        splitStats.Panel2.Padding = new Padding(10);

        // Grupo Filtro Mes (Absorbido de FormDocumentoMasLeido)
        grpMes = new GroupBox { Text = "🔎 Más Leído por Mes", Dock = DockStyle.Top, Height = 180 };
        
        var lblMes = new Label { Text = "Mes:", Location = new Point(20, 30), AutoSize = true };
        cmbMes = new ComboBox { Location = new Point(60, 27), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbMes.Items.AddRange(new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" });
        cmbMes.SelectedIndex = DateTime.Now.Month - 1;

        var lblAnio = new Label { Text = "Año:", Location = new Point(200, 30), AutoSize = true };
        nudAnio = new NumericUpDown { Location = new Point(240, 27), Width = 80, Minimum = 2000, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };

        btnConsultarMes = new Button { Text = "Consultar", Location = new Point(340, 25), Size = new Size(80, 25) };
        btnConsultarMes.Click += BtnConsultarMes_Click;

        txtResultadoMes = new TextBox { Location = new Point(20, 70), Width = 400, ReadOnly = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = HorizontalAlignment.Center };
        txtAutorMes = new TextBox { Location = new Point(20, 105), Width = 400, ReadOnly = true, TextAlign = HorizontalAlignment.Center };
        
        grpMes.Controls.AddRange(new Control[] { lblMes, cmbMes, lblAnio, nudAnio, btnConsultarMes, txtResultadoMes, txtAutorMes });

        // Grupo Top 5
        var grpTop = new GroupBox { Text = "🏆 Top 5 Más Prestados", Dock = DockStyle.Fill }; // Fill el resto del panel izquierdo
        dgvTopPrestados = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            BackgroundColor = SystemColors.Control
        };
        grpTop.Controls.Add(dgvTopPrestados);

        splitStats.Panel1.Controls.Add(grpTop);
        splitStats.Panel1.Controls.Add(grpMes); // Orden: Mes arriba, Top abajo (por Dock)
        grpTop.BringToFront(); // Asegurar que Top llene el espacio restante debajo de Mes

        // -- Panel Derecho: Géneros --
        var grpGeneros = new GroupBox { Text = "📊 Préstamos por Género", Dock = DockStyle.Fill };
        dgvPorGenero = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            BackgroundColor = SystemColors.Control
        };
        grpGeneros.Controls.Add(dgvPorGenero);
        splitStats.Panel2.Controls.Add(grpGeneros);

        tabEstadisticas.Controls.Add(splitStats);
        tabEstadisticas.Controls.Add(pnlResumen);

        tabControl.TabPages.Add(tabCatalogo);
        tabControl.TabPages.Add(tabEstadisticas);
        Controls.Add(tabControl);
    }

    /// <summary>
    /// Permite seleccionar qué pestaña mostrar al abrir el formulario.
    /// </summary>
    public void SeleccionarTab(int index)
    {
        if (index >= 0 && index < tabControl.TabCount)
        {
            tabControl.SelectedIndex = index;
            if(index == 1) CargarDatosEstadisticas(); // Recargar al mostrar
        }
    }

    // ==========================================
    // LÓGICA TAB CATÁLOGO
    // ==========================================
    private void CargarDatosCatalogo()
    {
        var generos = NegocioDocumentos.ObtenerGeneros();
        cboGenero.Items.Clear();
        cboGenero.Items.Add("Todos");
        foreach (var g in generos) cboGenero.Items.Add(g);
        cboGenero.SelectedIndex = 0;
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        List<Documento> documentos;
        string tipo = cboTipo.SelectedItem?.ToString() ?? "Todos";
        
        if (tipo == "Libros") documentos = NegocioDocumentos.ObtenerLibrosOrdenados().Cast<Documento>().ToList();
        else if (tipo == "Audiolibros") documentos = NegocioDocumentos.ObtenerAudiolibrosOrdenados().Cast<Documento>().ToList();
        else documentos = NegocioDocumentos.ObtenerTodosOrdenados();

        string genero = cboGenero.SelectedItem?.ToString() ?? "Todos";
        if (genero != "Todos") documentos = documentos.Where(d => d.Genero == genero).ToList();

        if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
        {
            string busqueda = txtBuscar.Text.ToLower();
            documentos = documentos.Where(d => d.Titulo.ToLower().Contains(busqueda) || d.Autor.ToLower().Contains(busqueda)).ToList();
        }

        dgvDocumentos.DataSource = documentos.Select(d => new {
            d.Codigo, d.Titulo, d.Autor, d.Genero, Año = d.AnioPublicacion,
            Tipo = d.TipoDocumento,
            Disponibles = NegocioDocumentos.ContarEjemplaresDisponibles(d.Codigo)
        }).ToList();

        lblResultados.Text = $"{documentos.Count} resultados";
    }

    private void BtnLimpiar_Click(object? sender, EventArgs e)
    {
        cboTipo.SelectedIndex = 0;
        cboGenero.SelectedIndex = 0;
        txtBuscar.Clear();
        AplicarFiltros();
    }

    private void DgvDocumentos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvDocumentos.Columns[e.ColumnIndex].Name == "Disponibles" && e.Value is int disp)
        {
            e.CellStyle.BackColor = disp == 0 ? Color.LightCoral : Color.LightGreen;
        }
    }

    // ==========================================
    // LÓGICA TAB ESTADÍSTICAS
    // ==========================================
    private void CargarDatosEstadisticas()
    {
        // 1. Resumen Superior
        var (libros, audiolibros) = NegocioDocumentos.ContarPorTipo();
        lblTotalLibros.Text = $"📚 Libros: {libros}";
        lblTotalAudiolibros.Text = $"🎧 Audiolibros: {audiolibros}";

        var masLeidoHist = NegocioDocumentos.ObtenerMasLeido();
        lblMasLeidoHistorico.Text = masLeidoHist != null 
            ? $"⭐ Estrella Histórica: {masLeidoHist.Titulo} ({masLeidoHist.Autor})" 
            : "⭐ Estrella Histórica: -";

        // 2. Ejecutar consulta del mes actual por defecto
        BtnConsultarMes_Click(null, null);

        // 3. Cargar Grids
        // Top 5
        var top5 = NegocioDocumentos.ObtenerTopPrestados(5);
        dgvTopPrestados.DataSource = top5.Select(t => new { 
            Título = t.Documento.Titulo, 
            Autor = t.Documento.Autor, 
            Préstamos = t.VecesPrestado 
        }).ToList();

        // Por Género
        var porGenero = NegocioDocumentos.ObtenerEstadisticasPorGenero();
        dgvPorGenero.DataSource = porGenero.Select(x => new { 
            Género = x.Genero, 
            Cantidad = x.Cantidad 
        }).ToList();
    }

    private void BtnConsultarMes_Click(object? sender, EventArgs e)
    {
        int mes = cmbMes.SelectedIndex + 1;
        int anio = (int)nudAnio.Value;

        var doc = NegocioDocumentos.ObtenerMasLeidoMes(mes, anio);
        if (doc != null)
        {
            txtResultadoMes.Text = doc.Titulo;
            txtAutorMes.Text = doc.Autor;
            txtResultadoMes.BackColor = Color.LightGreen;
        }
        else
        {
            txtResultadoMes.Text = "Sin datos en este periodo";
            txtAutorMes.Text = "-";
            txtResultadoMes.BackColor = Color.WhiteSmoke;
        }
    }
}