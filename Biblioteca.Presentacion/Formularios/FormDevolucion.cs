using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para registrar devolución de préstamos o ejemplares.
/// </summary>
public class FormDevolucion : Form
{
    private TabControl tabControl;
    private TabPage tabPrestamo;
    private TabPage tabEjemplar;

    // Tab Préstamo
    private Label lblIdPrestamo;
    private TextBox txtIdPrestamo;
    private Button btnBuscarPrestamo;
    private GroupBox grpPrestamo;
    private Label lblUsuarioPrestamo;
    private Label lblFechaPrestamo;
    private Label lblFechaDevolucion;
    private Label lblEstadoPrestamo;
    private TextBox txtUsuarioPrestamo;
    private TextBox txtFechaPrestamo;
    private TextBox txtFechaDevolucion;
    private TextBox txtEstadoPrestamo;
    private DataGridView dgvEjemplaresPrestamo;
    private Button btnDevolverPrestamo;

    // Tab Ejemplar
    private Label lblCodigoBarras;
    private TextBox txtCodigoBarras;
    private Button btnBuscarEjemplar;
    private GroupBox grpEjemplar;
    private Label lblDocumento;
    private Label lblEstadoEjemplar;
    private TextBox txtDocumento;
    private TextBox txtEstadoEjemplar;
    private Button btnDevolverEjemplar;

    private ErrorProvider errorProvider;

    public FormDevolucion()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Registrar Devolución";
        Size = new Size(600, 450);
        StartPosition = FormStartPosition.CenterParent;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        tabControl = new TabControl { Location = new Point(10, 10), Size = new Size(565, 390) };

        // Tab Préstamo Completo
        tabPrestamo = new TabPage("Por Préstamo");

        lblIdPrestamo = new Label { Text = "ID Préstamo:", Location = new Point(20, 25), AutoSize = true };
        txtIdPrestamo = new TextBox { Location = new Point(110, 22), Width = 200 };
        btnBuscarPrestamo = new Button { Text = "Buscar", Location = new Point(330, 20), Size = new Size(80, 25) };
        btnBuscarPrestamo.Click += BtnBuscarPrestamo_Click;

        grpPrestamo = new GroupBox { Text = "Datos del Préstamo", Location = new Point(20, 60), Size = new Size(520, 240), Enabled = false };

        lblUsuarioPrestamo = new Label { Text = "Usuario:", Location = new Point(15, 25), AutoSize = true };
        txtUsuarioPrestamo = new TextBox { Location = new Point(110, 22), Width = 200, ReadOnly = true };

        lblFechaPrestamo = new Label { Text = "Fecha Préstamo:", Location = new Point(15, 55), AutoSize = true };
        txtFechaPrestamo = new TextBox { Location = new Point(110, 52), Width = 120, ReadOnly = true };

        lblFechaDevolucion = new Label { Text = "Fecha Devolución:", Location = new Point(250, 55), AutoSize = true };
        txtFechaDevolucion = new TextBox { Location = new Point(370, 52), Width = 120, ReadOnly = true };

        lblEstadoPrestamo = new Label { Text = "Estado:", Location = new Point(15, 85), AutoSize = true };
        txtEstadoPrestamo = new TextBox { Location = new Point(110, 82), Width = 100, ReadOnly = true };

        dgvEjemplaresPrestamo = new DataGridView
        {
            Location = new Point(15, 115),
            Size = new Size(490, 80),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        btnDevolverPrestamo = new Button { Text = "Registrar Devolución", Location = new Point(180, 200), Size = new Size(150, 30), Enabled = false };
        btnDevolverPrestamo.Click += BtnDevolverPrestamo_Click;

        grpPrestamo.Controls.AddRange(new Control[] { lblUsuarioPrestamo, txtUsuarioPrestamo, lblFechaPrestamo, txtFechaPrestamo,
            lblFechaDevolucion, txtFechaDevolucion, lblEstadoPrestamo, txtEstadoPrestamo, dgvEjemplaresPrestamo, btnDevolverPrestamo });

        tabPrestamo.Controls.AddRange(new Control[] { lblIdPrestamo, txtIdPrestamo, btnBuscarPrestamo, grpPrestamo });

        // Tab Ejemplar Individual
        tabEjemplar = new TabPage("Por Ejemplar");

        lblCodigoBarras = new Label { Text = "Código Barras:", Location = new Point(20, 25), AutoSize = true };
        txtCodigoBarras = new TextBox { Location = new Point(120, 22), Width = 200 };
        btnBuscarEjemplar = new Button { Text = "Buscar", Location = new Point(340, 20), Size = new Size(80, 25) };
        btnBuscarEjemplar.Click += BtnBuscarEjemplar_Click;

        grpEjemplar = new GroupBox { Text = "Datos del Ejemplar", Location = new Point(20, 60), Size = new Size(520, 150), Enabled = false };

        lblDocumento = new Label { Text = "Documento:", Location = new Point(15, 30), AutoSize = true };
        txtDocumento = new TextBox { Location = new Point(110, 27), Width = 350, ReadOnly = true };

        lblEstadoEjemplar = new Label { Text = "Estado:", Location = new Point(15, 65), AutoSize = true };
        txtEstadoEjemplar = new TextBox { Location = new Point(110, 62), Width = 100, ReadOnly = true };

        btnDevolverEjemplar = new Button { Text = "Devolver Ejemplar", Location = new Point(190, 100), Size = new Size(130, 30), Enabled = false };
        btnDevolverEjemplar.Click += BtnDevolverEjemplar_Click;

        grpEjemplar.Controls.AddRange(new Control[] { lblDocumento, txtDocumento, lblEstadoEjemplar, txtEstadoEjemplar, btnDevolverEjemplar });

        tabEjemplar.Controls.AddRange(new Control[] { lblCodigoBarras, txtCodigoBarras, btnBuscarEjemplar, grpEjemplar });

        tabControl.TabPages.AddRange(new TabPage[] { tabPrestamo, tabEjemplar });
        Controls.Add(tabControl);
    }

    private void BtnBuscarPrestamo_Click(object? sender, EventArgs e)
    {
        errorProvider.SetError(txtIdPrestamo, "");

        if (string.IsNullOrWhiteSpace(txtIdPrestamo.Text))
        {
            errorProvider.SetError(txtIdPrestamo, "Introduzca el ID del préstamo.");
            return;
        }

        var prestamo = NegocioPrestamos.ObtenerPorId(txtIdPrestamo.Text.Trim());
        if (prestamo == null)
        {
            MessageBox.Show("No se encontró ningún préstamo con ese ID.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtUsuarioPrestamo.Text = prestamo.Usuario.NombreCompleto;
        txtFechaPrestamo.Text = prestamo.FechaPrestamo.ToString("dd/MM/yyyy");
        txtFechaDevolucion.Text = prestamo.FechaDevolucionPrevista.ToString("dd/MM/yyyy");
        txtEstadoPrestamo.Text = prestamo.Estado.ToString();
        txtEstadoPrestamo.BackColor = prestamo.EstaVencido ? Color.LightCoral : Color.LightGreen;

        dgvEjemplaresPrestamo.DataSource = prestamo.Ejemplares.Select(ej => new
        {
            ej.CodigoBarras,
            Documento = ej.Documento.Titulo,
            Estado = ej.Estado.ToString()
        }).ToList();

        grpPrestamo.Enabled = true;
        btnDevolverPrestamo.Enabled = prestamo.Estado == Biblioteca.Dominio.EstadoPrestamo.Activo || prestamo.Estado == Biblioteca.Dominio.EstadoPrestamo.Vencido;
    }

    private void BtnDevolverPrestamo_Click(object? sender, EventArgs e)
    {
        var (exito, mensaje) = NegocioPrestamos.RegistrarDevolucion(txtIdPrestamo.Text.Trim());

        if (exito)
        {
            MessageBox.Show(mensaje, "Devolución Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtIdPrestamo.Clear();
            LimpiarPrestamo();
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnBuscarEjemplar_Click(object? sender, EventArgs e)
    {
        errorProvider.SetError(txtCodigoBarras, "");

        if (string.IsNullOrWhiteSpace(txtCodigoBarras.Text))
        {
            errorProvider.SetError(txtCodigoBarras, "Introduzca el código de barras.");
            return;
        }

        var ejemplar = NegocioDocumentos.BuscarEjemplarPorCodigoBarras(txtCodigoBarras.Text.Trim());
        if (ejemplar == null)
        {
            MessageBox.Show("No se encontró ningún ejemplar con ese código.", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtDocumento.Text = ejemplar.Documento.ToString();
        txtEstadoEjemplar.Text = ejemplar.Estado.ToString();
        txtEstadoEjemplar.BackColor = ejemplar.EstaPrestado ? Color.LightCoral : Color.LightGreen;

        grpEjemplar.Enabled = true;
        btnDevolverEjemplar.Enabled = ejemplar.EstaPrestado;
    }

    private void BtnDevolverEjemplar_Click(object? sender, EventArgs e)
    {
        var (exito, mensaje) = NegocioPrestamos.DevolverEjemplar(txtCodigoBarras.Text.Trim());

        if (exito)
        {
            MessageBox.Show(mensaje, "Devolución Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCodigoBarras.Clear();
            LimpiarEjemplar();
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LimpiarPrestamo()
    {
        txtUsuarioPrestamo.Clear();
        txtFechaPrestamo.Clear();
        txtFechaDevolucion.Clear();
        txtEstadoPrestamo.Clear();
        txtEstadoPrestamo.BackColor = SystemColors.Control;
        dgvEjemplaresPrestamo.DataSource = null;
        grpPrestamo.Enabled = false;
        btnDevolverPrestamo.Enabled = false;
    }

    private void LimpiarEjemplar()
    {
        txtDocumento.Clear();
        txtEstadoEjemplar.Clear();
        txtEstadoEjemplar.BackColor = SystemColors.Control;
        grpEjemplar.Enabled = false;
        btnDevolverEjemplar.Enabled = false;
    }
}
