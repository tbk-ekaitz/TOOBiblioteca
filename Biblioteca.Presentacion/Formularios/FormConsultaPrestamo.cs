using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para consultar información de un préstamo.
/// </summary>
public class FormConsultaPrestamo : Form
{
    private Label lblIdPrestamo;
    private TextBox txtIdPrestamo;
    private Button btnBuscar;
    private GroupBox grpDatos;
    private Label lblUsuario;
    private Label lblEmpleado;
    private Label lblFechaPrestamo;
    private Label lblFechaDevolucionPrevista;
    private Label lblFechaDevolucionReal;
    private Label lblEstado;
    private TextBox txtUsuario;
    private TextBox txtEmpleado;
    private TextBox txtFechaPrestamo;
    private TextBox txtFechaDevolucionPrevista;
    private TextBox txtFechaDevolucionReal;
    private TextBox txtEstado;
    private Label lblDiasRetraso;
    private TextBox txtDiasRetraso;
    private DataGridView dgvEjemplares;
    private Button btnRenovar;
    private Button btnCancelar;
    private ErrorProvider errorProvider;

    public FormConsultaPrestamo()
    {
        InitializeComponent();
    }

    public FormConsultaPrestamo(string idPrestamo) : this()
    {
        txtIdPrestamo.Text = idPrestamo;
        BtnBuscar_Click(this, EventArgs.Empty);
    }

    private void InitializeComponent()
    {
        Text = "Consultar Préstamo";
        Size = new Size(650, 500);
        StartPosition = FormStartPosition.CenterParent;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // busqueda
        lblIdPrestamo = new Label { Text = "ID Préstamo:", Location = new Point(20, 25), AutoSize = true };
        txtIdPrestamo = new TextBox { Location = new Point(110, 22), Width = 200 };
        btnBuscar = new Button { Text = "Buscar", Location = new Point(330, 20), Size = new Size(80, 25) };
        btnBuscar.Click += BtnBuscar_Click;

        // datos
        grpDatos = new GroupBox { Text = "Datos del Préstamo", Location = new Point(20, 60), Size = new Size(595, 330), Enabled = false };

        int y = 25;
        lblUsuario = new Label { Text = "Usuario:", Location = new Point(15, y), AutoSize = true };
        txtUsuario = new TextBox { Location = new Point(140, y - 3), Width = 250, ReadOnly = true };

        y += 30;
        lblEmpleado = new Label { Text = "Empleado:", Location = new Point(15, y), AutoSize = true };
        txtEmpleado = new TextBox { Location = new Point(140, y - 3), Width = 250, ReadOnly = true };

        y += 30;
        lblFechaPrestamo = new Label { Text = "Fecha Préstamo:", Location = new Point(15, y), AutoSize = true };
        txtFechaPrestamo = new TextBox { Location = new Point(140, y - 3), Width = 100, ReadOnly = true };

        lblFechaDevolucionPrevista = new Label { Text = "Devolución Prevista:", Location = new Point(270, y), AutoSize = true };
        txtFechaDevolucionPrevista = new TextBox { Location = new Point(400, y - 3), Width = 100, ReadOnly = true };

        y += 30;
        lblFechaDevolucionReal = new Label { Text = "Devolución Real:", Location = new Point(15, y), AutoSize = true };
        txtFechaDevolucionReal = new TextBox { Location = new Point(140, y - 3), Width = 100, ReadOnly = true };

        lblEstado = new Label { Text = "Estado:", Location = new Point(270, y), AutoSize = true };
        txtEstado = new TextBox { Location = new Point(400, y - 3), Width = 100, ReadOnly = true };

        y += 30;
        lblDiasRetraso = new Label { Text = "Días de Retraso:", Location = new Point(15, y), AutoSize = true };
        txtDiasRetraso = new TextBox { Location = new Point(140, y - 3), Width = 60, ReadOnly = true };

        y += 35;
        dgvEjemplares = new DataGridView
        {
            Location = new Point(15, y),
            Size = new Size(565, 100),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        y += 110;
        btnRenovar = new Button { Text = "Renovar Préstamo", Location = new Point(150, y), Size = new Size(130, 30), Enabled = false };
        btnRenovar.Click += BtnRenovar_Click;

        btnCancelar = new Button { Text = "Cancelar Préstamo", Location = new Point(300, y), Size = new Size(130, 30), Enabled = false };
        btnCancelar.Click += BtnCancelar_Click;

        grpDatos.Controls.AddRange(new Control[] { lblUsuario, txtUsuario, lblEmpleado, txtEmpleado,
            lblFechaPrestamo, txtFechaPrestamo, lblFechaDevolucionPrevista, txtFechaDevolucionPrevista,
            lblFechaDevolucionReal, txtFechaDevolucionReal, lblEstado, txtEstado, lblDiasRetraso, txtDiasRetraso,
            dgvEjemplares, btnRenovar, btnCancelar });

        Controls.AddRange(new Control[] { lblIdPrestamo, txtIdPrestamo, btnBuscar, grpDatos });
    }

    private void BtnBuscar_Click(object? sender, EventArgs e)
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
            LimpiarDatos();
            return;
        }

        txtUsuario.Text = $"{prestamo.Usuario.DNI} - {prestamo.Usuario.NombreCompleto}";
        txtEmpleado.Text = prestamo.Empleado.NombreCompleto;
        txtFechaPrestamo.Text = prestamo.FechaPrestamo.ToString("dd/MM/yyyy");
        txtFechaDevolucionPrevista.Text = prestamo.FechaDevolucionPrevista.ToString("dd/MM/yyyy");
        txtFechaDevolucionReal.Text = prestamo.FechaDevolucionReal?.ToString("dd/MM/yyyy") ?? "-";
        txtEstado.Text = prestamo.Estado.ToString();
        txtDiasRetraso.Text = prestamo.DiasRetraso.ToString();

        if (prestamo.EstaVencido)
        {
            txtEstado.BackColor = Color.LightCoral;
            txtDiasRetraso.BackColor = Color.LightCoral;
        }
        else
        {
            txtEstado.BackColor = prestamo.Estado == Biblioteca.Dominio.EstadoPrestamo.Activo ? Color.LightGreen : SystemColors.Control;
            txtDiasRetraso.BackColor = SystemColors.Control;
        }

        dgvEjemplares.DataSource = prestamo.Ejemplares.Select(ej => new
        {
            ej.CodigoBarras,
            Documento = ej.Documento.Titulo,
            Autor = ej.Documento.Autor,
            Estado = ej.Estado.ToString()
        }).ToList();

        grpDatos.Enabled = true;
        btnRenovar.Enabled = prestamo.Estado == Biblioteca.Dominio.EstadoPrestamo.Activo && !prestamo.EstaVencido;
        btnCancelar.Enabled = prestamo.Estado == Biblioteca.Dominio.EstadoPrestamo.Activo;
    }

    private void BtnRenovar_Click(object? sender, EventArgs e)
    {
        var resultado = MessageBox.Show("¿Desea renovar este préstamo?", "Confirmar Renovación",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (resultado == DialogResult.Yes)
        {
            var (exito, mensaje) = NegocioPrestamos.RenovarPrestamo(txtIdPrestamo.Text.Trim());
            if (exito)
            {
                MessageBox.Show(mensaje, "Renovación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnBuscar_Click(sender, e);
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        var motivo = Microsoft.VisualBasic.Interaction.InputBox("Introduzca el motivo de la cancelación:", "Cancelar Préstamo", "");
        if (string.IsNullOrWhiteSpace(motivo)) return;

        var (exito, mensaje) = NegocioPrestamos.CancelarPrestamo(txtIdPrestamo.Text.Trim(), motivo);
        if (exito)
        {
            MessageBox.Show(mensaje, "Cancelación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BtnBuscar_Click(sender, e);
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LimpiarDatos()
    {
        txtUsuario.Clear();
        txtEmpleado.Clear();
        txtFechaPrestamo.Clear();
        txtFechaDevolucionPrevista.Clear();
        txtFechaDevolucionReal.Clear();
        txtEstado.Clear();
        txtEstado.BackColor = SystemColors.Control;
        txtDiasRetraso.Clear();
        txtDiasRetraso.BackColor = SystemColors.Control;
        dgvEjemplares.DataSource = null;
        grpDatos.Enabled = false;
        btnRenovar.Enabled = false;
        btnCancelar.Enabled = false;
    }
}
