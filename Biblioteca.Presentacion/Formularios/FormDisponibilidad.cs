using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para consultar disponibilidad de documentos.
/// </summary>
public class FormDisponibilidad : Form
{
    private Label lblCodigo;
    private TextBox txtCodigo;
    private Button btnConsultar;
    private GroupBox grpResultado;
    private Label lblTitulo;
    private Label lblAutor;
    private Label lblTipo;
    private Label lblDisponibilidad;
    private Label lblFechaDisponible;
    private TextBox txtTitulo;
    private TextBox txtAutor;
    private TextBox txtTipo;
    private TextBox txtDisponibilidad;
    private TextBox txtFechaDisponible;
    private DataGridView dgvEjemplares;
    private ErrorProvider errorProvider;

    public FormDisponibilidad()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Consulta de Disponibilidad";
        Size = new Size(600, 450);
        StartPosition = FormStartPosition.CenterParent;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // Búsqueda
        lblCodigo = new Label { Text = "Código Documento:", Location = new Point(20, 25), AutoSize = true };
        txtCodigo = new TextBox { Location = new Point(140, 22), Width = 150 };
        btnConsultar = new Button { Text = "Consultar", Location = new Point(310, 20), Size = new Size(100, 25) };
        btnConsultar.Click += BtnConsultar_Click;

        // Grupo resultado
        grpResultado = new GroupBox { Text = "Información del Documento", Location = new Point(20, 60), Size = new Size(545, 330), Enabled = false };

        int y = 25;
        lblTitulo = new Label { Text = "Título:", Location = new Point(15, y), AutoSize = true };
        txtTitulo = new TextBox { Location = new Point(120, y - 3), Width = 350, ReadOnly = true };

        y += 30;
        lblAutor = new Label { Text = "Autor:", Location = new Point(15, y), AutoSize = true };
        txtAutor = new TextBox { Location = new Point(120, y - 3), Width = 250, ReadOnly = true };

        y += 30;
        lblTipo = new Label { Text = "Tipo:", Location = new Point(15, y), AutoSize = true };
        txtTipo = new TextBox { Location = new Point(120, y - 3), Width = 100, ReadOnly = true };

        y += 30;
        lblDisponibilidad = new Label { Text = "Disponibilidad:", Location = new Point(15, y), AutoSize = true };
        txtDisponibilidad = new TextBox { Location = new Point(120, y - 3), Width = 200, ReadOnly = true };

        y += 30;
        lblFechaDisponible = new Label { Text = "Disponible desde:", Location = new Point(15, y), AutoSize = true };
        txtFechaDisponible = new TextBox { Location = new Point(120, y - 3), Width = 150, ReadOnly = true };

        y += 35;
        grpResultado.Controls.Add(new Label { Text = "Ejemplares:", Location = new Point(15, y), AutoSize = true });

        y += 20;
        dgvEjemplares = new DataGridView
        {
            Location = new Point(15, y),
            Size = new Size(515, 120),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        grpResultado.Controls.AddRange(new Control[] { lblTitulo, txtTitulo, lblAutor, txtAutor, lblTipo, txtTipo,
            lblDisponibilidad, txtDisponibilidad, lblFechaDisponible, txtFechaDisponible, dgvEjemplares });

        Controls.AddRange(new Control[] { lblCodigo, txtCodigo, btnConsultar, grpResultado });
    }

    private void BtnConsultar_Click(object? sender, EventArgs e)
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
        txtTipo.Text = documento.TipoDocumento;

        // Usar solo ejemplares activos (excluir dados de baja)
        var ejemplaresDisponibles = NegocioDocumentos.ContarEjemplaresDisponibles(documento.Codigo);
        var totalEjemplaresActivos = NegocioDocumentos.ContarEjemplaresActivos(documento.Codigo);

        if (ejemplaresDisponibles > 0)
        {
            txtDisponibilidad.Text = $"Disponible ({ejemplaresDisponibles} de {totalEjemplaresActivos})";
            txtDisponibilidad.BackColor = Color.LightGreen;
            txtFechaDisponible.Text = "Ahora";
        }
        else
        {
            txtDisponibilidad.Text = $"No disponible (0 de {totalEjemplaresActivos})";
            txtDisponibilidad.BackColor = Color.LightCoral;

            var fechaDisponible = NegocioDocumentos.ObtenerFechaDisponibilidad(documento.Codigo);
            txtFechaDisponible.Text = fechaDisponible?.ToString("dd/MM/yyyy") ?? "Desconocida";
        }

        // Cargar solo ejemplares activos (sin los dados de baja)
        var ejemplares = NegocioDocumentos.ObtenerEjemplaresActivos(documento.Codigo);
        dgvEjemplares.DataSource = ejemplares.Select(ej => new
        {
            ej.CodigoBarras,
            Estado = ej.Estado.ToString(),
            ej.Ubicacion,
            ej.VecesPrestado,
            //RegistradoPor = ej.EmpleadoAlta.NombreCompleto // naah, innecesario
        }).ToList();

        // Colorear según estado
        foreach (DataGridViewRow row in dgvEjemplares.Rows)
        {
            var estado = row.Cells["Estado"].Value?.ToString();
            row.DefaultCellStyle.BackColor = estado == "Disponible" ? Color.LightGreen : Color.LightCoral;
        }

        grpResultado.Enabled = true;
    }

    private void LimpiarDatos()
    {
        txtTitulo.Clear();
        txtAutor.Clear();
        txtTipo.Clear();
        txtDisponibilidad.Clear();
        txtDisponibilidad.BackColor = SystemColors.Control;
        txtFechaDisponible.Clear();
        dgvEjemplares.DataSource = null;
        grpResultado.Enabled = false;
    }
}
