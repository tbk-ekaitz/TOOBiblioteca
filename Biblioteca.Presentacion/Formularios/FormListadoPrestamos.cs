using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para listar préstamos con filtros.
/// </summary>
public class FormListadoPrestamos : Form
{
    private Label lblFiltro;
    private ComboBox cmbFiltro;
    private Label lblDNI;
    private TextBox txtDNI;
    private Button btnFiltrar;
    private DataGridView dgvPrestamos;
    private Label lblTotal;
    private BindingNavigator bindingNavigator;
    private BindingSource bindingSource;

    public FormListadoPrestamos()
    {
        InitializeComponent();
        CargarPrestamos();
    }

    private void InitializeComponent()
    {
        Text = "Listado de Préstamos";
        Size = new Size(900, 550);
        StartPosition = FormStartPosition.CenterParent;

        // Filtros
        lblFiltro = new Label { Text = "Filtrar por:", Location = new Point(20, 25), AutoSize = true };
        cmbFiltro = new ComboBox
        {
            Location = new Point(90, 22),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFiltro.Items.AddRange(new[] { "Todos", "Activos", "Vencidos", "Devueltos", "Por Usuario" });
        cmbFiltro.SelectedIndex = 0;
        cmbFiltro.SelectedIndexChanged += CmbFiltro_SelectedIndexChanged;

        lblDNI = new Label { Text = "DNI:", Location = new Point(270, 25), AutoSize = true, Visible = false };
        txtDNI = new TextBox { Location = new Point(310, 22), Width = 120, Visible = false };

        btnFiltrar = new Button { Text = "Filtrar", Location = new Point(450, 20), Size = new Size(80, 25) };
        btnFiltrar.Click += BtnFiltrar_Click;

        // BindingSource y Navigator
        bindingSource = new BindingSource();
        bindingNavigator = new BindingNavigator(true)
        {
            BindingSource = bindingSource,
            Location = new Point(0, 55)
        };

        var btnNuevo = bindingNavigator.AddNewItem;
        bindingNavigator.AddNewItem = null;
        if (btnNuevo != null)
        {
            btnNuevo.Visible = true;  // Que se vea
            btnNuevo.Enabled = true;  // Que se pueda pulsar
            btnNuevo.ToolTipText = "Registrar Nuevo Préstamo"; // Un tooltip útil

            // Asignamos el evento click
            btnNuevo.Click += (s, e) =>
            {
                // Abrimos el formulario de Alta
                var form = new FormAltaPrestamo();
                form.MdiParent = this.MdiParent; // Lo mantenemos dentro del MDI

                // Importante: Cuando se cierre el alta, recargamos la lista para ver el nuevo préstamo
                form.FormClosed += (sender, args) => CargarPrestamos();

                form.Show();
            };
        }

        // DataGridView
        dgvPrestamos = new DataGridView
        {
            Location = new Point(20, 90),
            Size = new Size(845, 370),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            DataSource = bindingSource
        };
        dgvPrestamos.CellDoubleClick += DgvPrestamos_CellDoubleClick;

        // Total
        lblTotal = new Label { Text = "Total: 0", Location = new Point(20, 470), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };

        Controls.AddRange(new Control[] { lblFiltro, cmbFiltro, lblDNI, txtDNI, btnFiltrar, bindingNavigator, dgvPrestamos, lblTotal });
    }

    private void CmbFiltro_SelectedIndexChanged(object? sender, EventArgs e)
    {
        bool mostrarDNI = cmbFiltro.SelectedIndex == 4; // "Por Usuario"
        lblDNI.Visible = mostrarDNI;
        txtDNI.Visible = mostrarDNI;
    }

    private void BtnFiltrar_Click(object? sender, EventArgs e)
    {
        CargarPrestamos();
    }

    private void CargarPrestamos()
    {
        List<Prestamo> prestamos;

        switch (cmbFiltro.SelectedIndex)
        {
            case 1: // Activos
                prestamos = NegocioPrestamos.ObtenerActivosOrdenados();
                break;
            case 2: // Vencidos
                prestamos = NegocioPrestamos.ObtenerVencidos();
                break;
            case 3: // Devueltos
                prestamos = NegocioPrestamos.ObtenerTodosOrdenados()
                    .Where(p => p.Estado == EstadoPrestamo.Devuelto).ToList();
                break;
            case 4: // Por Usuario
                if (string.IsNullOrWhiteSpace(txtDNI.Text))
                {
                    MessageBox.Show("Introduzca el DNI del usuario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                prestamos = NegocioPrestamos.ObtenerPorUsuario(txtDNI.Text.Trim());
                break;
            default: // Todos
                prestamos = NegocioPrestamos.ObtenerTodosOrdenados();
                break;
        }

        var datos = prestamos.Select(p => new
        {
            p.Id,
            Usuario = p.Usuario.NombreCompleto,
            DNI = p.Usuario.DNI,
            FechaPrestamo = p.FechaPrestamo.ToString("dd/MM/yyyy"),
            FechaDevolucion = p.FechaDevolucionPrevista.ToString("dd/MM/yyyy"),
            Estado = p.Estado.ToString(),
            Ejemplares = p.NumeroEjemplares,
            Vencido = p.EstaVencido ? "Sí" : "No"
        }).ToList();

        bindingSource.DataSource = datos;
        lblTotal.Text = $"Total: {prestamos.Count} préstamo(s)";
    }

    private void DgvPrestamos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var id = dgvPrestamos.Rows[e.RowIndex].Cells["Id"].Value?.ToString();
        if (string.IsNullOrEmpty(id)) return;

        var form = new FormConsultaPrestamo(id);
        form.MdiParent = MdiParent;
        form.Show();
    }
}
