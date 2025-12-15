using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para gestión de ejemplares (alta y baja).
/// </summary>
public class FormGestionEjemplares : Form
{
    private Empleado _empleado;
    private Label lblCodigo;
    private TextBox txtCodigo;
    private Button btnBuscar;
    private GroupBox grpDocumento;
    private Label lblTitulo;
    private Label lblAutor;
    private TextBox txtTitulo;
    private TextBox txtAutor;
    private DataGridView dgvEjemplares;
    private GroupBox grpNuevoEjemplar;
    private Label lblUbicacion;
    private TextBox txtUbicacion;
    private Button btnAltaEjemplar;
    private Button btnBajaEjemplar;
    private ErrorProvider errorProvider;

    public FormGestionEjemplares(Empleado empleado)
    {
        _empleado = empleado;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Gestión de Ejemplares";
        Size = new Size(700, 500);
        StartPosition = FormStartPosition.CenterParent;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // Búsqueda por código
        lblCodigo = new Label { Text = "Código Documento:", Location = new Point(20, 25), AutoSize = true };
        txtCodigo = new TextBox { Location = new Point(140, 22), Width = 150 };
        btnBuscar = new Button { Text = "Buscar", Location = new Point(310, 20), Size = new Size(80, 25) };
        btnBuscar.Click += BtnBuscar_Click;

        // Grupo documento
        grpDocumento = new GroupBox { Text = "Documento", Location = new Point(20, 60), Size = new Size(640, 80), Enabled = false };
        lblTitulo = new Label { Text = "Título:", Location = new Point(15, 25), AutoSize = true };
        txtTitulo = new TextBox { Location = new Point(70, 22), Width = 300, ReadOnly = true };
        lblAutor = new Label { Text = "Autor:", Location = new Point(15, 50), AutoSize = true };
        txtAutor = new TextBox { Location = new Point(70, 47), Width = 200, ReadOnly = true };
        grpDocumento.Controls.AddRange(new Control[] { lblTitulo, txtTitulo, lblAutor, txtAutor });

        // Grid de ejemplares
        dgvEjemplares = new DataGridView
        {
            Location = new Point(20, 150),
            Size = new Size(640, 180),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvEjemplares.CellFormatting += DgvEjemplares_CellFormatting;

        // Grupo nuevo ejemplar
        grpNuevoEjemplar = new GroupBox { Text = "Nuevo Ejemplar", Location = new Point(20, 340), Size = new Size(400, 60), Enabled = false };
        lblUbicacion = new Label { Text = "Ubicación:", Location = new Point(15, 25), AutoSize = true };
        txtUbicacion = new TextBox { Location = new Point(85, 22), Width = 200 };
        btnAltaEjemplar = new Button { Text = "Añadir", Location = new Point(300, 20), Size = new Size(80, 25) };
        btnAltaEjemplar.Click += BtnAltaEjemplar_Click;
        grpNuevoEjemplar.Controls.AddRange(new Control[] { lblUbicacion, txtUbicacion, btnAltaEjemplar });

        // Botón baja ejemplar
        btnBajaEjemplar = new Button { Text = "Dar de Baja Ejemplar Seleccionado", Location = new Point(440, 355), Size = new Size(220, 30), Enabled = false };
        btnBajaEjemplar.Click += BtnBajaEjemplar_Click;

        Controls.AddRange(new Control[] { lblCodigo, txtCodigo, btnBuscar, grpDocumento, dgvEjemplares, grpNuevoEjemplar, btnBajaEjemplar });
    }

    private void BtnBuscar_Click(object? sender, EventArgs e)
    {
        errorProvider.SetError(txtCodigo, "");

        if (string.IsNullOrWhiteSpace(txtCodigo.Text))
        {
            errorProvider.SetError(txtCodigo, "Introduzca el código del documento.");
            return;
        }

        var documento = NegocioDocumentos.BuscarPorCodigo(txtCodigo.Text.Trim());
        if (documento == null)
        {
            MessageBox.Show("No se encontró ningún documento con ese código.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LimpiarDatos();
            return;
        }

        txtTitulo.Text = documento.Titulo;
        txtAutor.Text = documento.Autor;
        grpDocumento.Enabled = true;
        grpNuevoEjemplar.Enabled = true;
        btnBajaEjemplar.Enabled = true;

        CargarEjemplares();
    }

    private void CargarEjemplares()
    {
        var ejemplares = NegocioDocumentos.ObtenerEjemplares(txtCodigo.Text.Trim());
        dgvEjemplares.DataSource = ejemplares.Select(e => new
        {
            e.CodigoBarras,
            e.Ubicacion,
            Estado = e.Estado.ToString(),
            e.VecesPrestado,
            FechaAdquisicion = e.FechaAdquisicion.ToString("dd/MM/yyyy"),
            RegistradoPor = e.EmpleadoAlta.NombreCompleto
        }).ToList();
    }

    private void BtnAltaEjemplar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUbicacion.Text))
        {
            MessageBox.Show("Introduzca la ubicación del ejemplar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var (exito, mensaje) = NegocioDocumentos.AltaEjemplar(txtCodigo.Text.Trim(), txtUbicacion.Text.Trim(), _empleado.DNI);

        if (exito)
        {
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtUbicacion.Clear();
            CargarEjemplares();
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnBajaEjemplar_Click(object? sender, EventArgs e)
    {
        if (dgvEjemplares.SelectedRows.Count == 0)
        {
            MessageBox.Show("Seleccione un ejemplar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var codigoBarras = dgvEjemplares.SelectedRows[0].Cells["CodigoBarras"].Value?.ToString();
        if (string.IsNullOrEmpty(codigoBarras)) return;

        var resultado = MessageBox.Show($"¿Está seguro de dar de baja el ejemplar {codigoBarras}?", "Confirmar Baja",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (resultado == DialogResult.Yes)
        {
            var (exito, mensaje) = NegocioDocumentos.BajaEjemplar(codigoBarras);
            if (exito)
            {
                MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarEjemplares();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void LimpiarDatos()
    {
        txtTitulo.Clear();
        txtAutor.Clear();
        dgvEjemplares.DataSource = null;
        grpDocumento.Enabled = false;
        grpNuevoEjemplar.Enabled = false;
        btnBajaEjemplar.Enabled = false;
    }

    private void DgvEjemplares_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (dgvEjemplares.Columns[e.ColumnIndex].Name == "Estado")
        {
            if (e.Value?.ToString() == "Baja")
            {
                // Colorear toda la fila en gris para indicar que está dado de baja
                dgvEjemplares.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                dgvEjemplares.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkGray;
            }
        }
    }
}
