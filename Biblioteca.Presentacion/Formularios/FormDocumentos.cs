using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario de catálogo con DataGridView y estadísticas.
/// Implementa la Práctica 13 (visualización de datos en grid).
/// </summary>
public class FormDocumentos : Form
{
    private TabControl tabControl;
    private TabPage tabCatalogo;
    private TabPage tabEstadisticas;

    // Tab Catálogo
    private DataGridView dgvDocumentos;
    private ComboBox cboTipo;
    private ComboBox cboGenero;
    private TextBox txtBuscar;
    private Button btnBuscar;
    private Button btnLimpiar;
    private Label lblResultados;

    // Tab Estadísticas
    private GroupBox grpEstadisticas;
    private ListBox lstTopPrestados;
    private Label lblMasLeido;
    private Label lblMasLeidoMes;
    private Label lblTotalLibros;
    private Label lblTotalAudiolibros;
    private DataGridView dgvPorGenero;

    public FormDocumentos()
    {
        InitializeComponent();
        CargarDatos();
        CargarEstadisticas();
    }

    private void InitializeComponent()
    {
        Text = "Catálogo de Documentos";
        Size = new Size(1000, 650);
        StartPosition = FormStartPosition.CenterParent;

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill
        };

        // === TAB CATÁLOGO ===
        tabCatalogo = new TabPage("Catálogo");

        // Filtros
        var pnlFiltros = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50
        };

        var lblTipo = new Label { Text = "Tipo:", Location = new Point(10, 15), AutoSize = true };
        cboTipo = new ComboBox
        {
            Location = new Point(45, 12),
            Width = 120,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboTipo.Items.AddRange(new object[] { "Todos", "Libros", "Audiolibros" });
        cboTipo.SelectedIndex = 0;
        cboTipo.SelectedIndexChanged += (s, e) => AplicarFiltros();

        var lblGenero = new Label { Text = "Género:", Location = new Point(180, 15), AutoSize = true };
        cboGenero = new ComboBox
        {
            Location = new Point(230, 12),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboGenero.SelectedIndexChanged += (s, e) => AplicarFiltros();

        txtBuscar = new TextBox
        {
            Location = new Point(400, 12),
            Width = 200,
            PlaceholderText = "Buscar por título o autor..."
        };

        btnBuscar = new Button
        {
            Text = "Buscar",
            Location = new Point(610, 10),
            Size = new Size(70, 25)
        };
        btnBuscar.Click += (s, e) => AplicarFiltros();

        btnLimpiar = new Button
        {
            Text = "Limpiar",
            Location = new Point(690, 10),
            Size = new Size(70, 25)
        };
        btnLimpiar.Click += BtnLimpiar_Click;

        lblResultados = new Label
        {
            Location = new Point(780, 15),
            AutoSize = true
        };

        pnlFiltros.Controls.AddRange(new Control[]
        {
            lblTipo, cboTipo, lblGenero, cboGenero, txtBuscar, btnBuscar, btnLimpiar, lblResultados
        });

        // DataGridView
        dgvDocumentos = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false
        };
        dgvDocumentos.CellFormatting += DgvDocumentos_CellFormatting;

        tabCatalogo.Controls.Add(dgvDocumentos);
        tabCatalogo.Controls.Add(pnlFiltros);

        // === TAB ESTADÍSTICAS ===
        tabEstadisticas = new TabPage("Estadísticas");

        var splitStats = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 200
        };

        // Panel superior - Resumen
        var pnlResumen = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(20)
        };

        lblTotalLibros = new Label { AutoSize = true, Font = new Font("Segoe UI", 12) };
        lblTotalAudiolibros = new Label { AutoSize = true, Font = new Font("Segoe UI", 12) };
        lblMasLeido = new Label { AutoSize = true, Font = new Font("Segoe UI", 12) };
        lblMasLeidoMes = new Label { AutoSize = true, Font = new Font("Segoe UI", 12) };

        pnlResumen.Controls.AddRange(new Control[]
        {
            lblTotalLibros, lblTotalAudiolibros, lblMasLeido, lblMasLeidoMes
        });

        // Panel inferior - Top y por género
        var splitInferior = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 400
        };

        // Top prestados
        var grpTop = new GroupBox
        {
            Text = "Top 5 Más Prestados",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        lstTopPrestados = new ListBox
        {
            Dock = DockStyle.Fill
        };
        grpTop.Controls.Add(lstTopPrestados);

        // Por género
        var grpGenero = new GroupBox
        {
            Text = "Préstamos por Género",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        dgvPorGenero = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false
        };
        grpGenero.Controls.Add(dgvPorGenero);

        splitInferior.Panel1.Controls.Add(grpTop);
        splitInferior.Panel2.Controls.Add(grpGenero);

        splitStats.Panel1.Controls.Add(pnlResumen);
        splitStats.Panel2.Controls.Add(splitInferior);

        tabEstadisticas.Controls.Add(splitStats);

        // Añadir tabs
        tabControl.TabPages.Add(tabCatalogo);
        tabControl.TabPages.Add(tabEstadisticas);
        Controls.Add(tabControl);
    }

    private void CargarDatos()
    {
        // Cargar géneros
        var generos = NegocioDocumentos.ObtenerGeneros();
        cboGenero.Items.Clear();
        cboGenero.Items.Add("Todos");
        foreach (var g in generos)
        {
            cboGenero.Items.Add(g);
        }
        cboGenero.SelectedIndex = 0;

        // Cargar documentos
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        List<Documento> documentos;

        // Filtrar por tipo
        string tipo = cboTipo.SelectedItem?.ToString() ?? "Todos";
        if (tipo == "Libros")
        {
            documentos = NegocioDocumentos.ObtenerLibrosOrdenados().Cast<Documento>().ToList();
        }
        else if (tipo == "Audiolibros")
        {
            documentos = NegocioDocumentos.ObtenerAudiolibrosOrdenados().Cast<Documento>().ToList();
        }
        else
        {
            documentos = NegocioDocumentos.ObtenerTodosOrdenados();
        }

        // Filtrar por género
        string genero = cboGenero.SelectedItem?.ToString() ?? "Todos";
        if (genero != "Todos")
        {
            documentos = documentos.Where(d => d.Genero == genero).ToList();
        }

        // Filtrar por búsqueda
        if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
        {
            string busqueda = txtBuscar.Text.ToLower();
            documentos = documentos.Where(d =>
                d.Titulo.ToLower().Contains(busqueda) ||
                d.Autor.ToLower().Contains(busqueda))
                .ToList();
        }

        // Mostrar en grid
        dgvDocumentos.DataSource = documentos.Select(d => new
        {
            Tipo = d.TipoDocumento,
            d.Codigo,
            d.Titulo,
            d.Autor,
            d.Genero,
            Año = d.AnioPublicacion,
            Disponibles = NegocioDocumentos.ContarEjemplaresDisponibles(d.Codigo),
            Detalles = d is Libro libro ? $"{libro.NumeroPaginas} págs." :
                      d is Audiolibro audio ? audio.DuracionFormateada : "",
            RegistradoPor = d.EmpleadoAlta?.NombreCompleto ?? "N/A"
        }).ToList();

        lblResultados.Text = $"{documentos.Count} documento(s)";
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
        if (dgvDocumentos.Columns[e.ColumnIndex].Name == "Disponibles")
        {
            if (e.Value is int disponibles)
            {
                if (disponibles == 0)
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                }
                else
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                }
            }
        }
    }

    private void CargarEstadisticas()
    {
        var (libros, audiolibros) = NegocioDocumentos.ContarPorTipo();
        lblTotalLibros.Text = $"📚 Total de Libros: {libros}";
        lblTotalAudiolibros.Text = $"🎧 Total de Audiolibros: {audiolibros}";

        var masLeido = NegocioDocumentos.ObtenerMasLeido();
        lblMasLeido.Text = $"⭐ Más leído (histórico): {masLeido?.Titulo ?? "N/A"}";

        var masLeidoMes = NegocioDocumentos.ObtenerMasLeidoMes();
        lblMasLeidoMes.Text = $"📅 Más leído (este mes): {masLeidoMes?.Titulo ?? "N/A"}";

        // Top prestados
        lstTopPrestados.Items.Clear();
        var topPrestados = NegocioDocumentos.ObtenerTopPrestados(5);
        foreach (var (doc, veces) in topPrestados)
        {
            lstTopPrestados.Items.Add($"{doc.Titulo} - {veces} préstamo(s)");
        }

        if (topPrestados.Count == 0)
        {
            lstTopPrestados.Items.Add("No hay datos de préstamos");
        }

        // Por género
        var porGenero = NegocioDocumentos.ObtenerEstadisticasPorGenero();
        dgvPorGenero.DataSource = porGenero.Select(x => new { x.Genero, Préstamos = x.Cantidad }).ToList();
    }
}
