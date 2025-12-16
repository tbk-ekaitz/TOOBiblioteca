using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para mostrar préstamos vencidos.
/// </summary>
public class FormPrestamosFueraPlazo : Form
{
    private DataGridView dgvVencidos;
    private Label lblTotal;
    private Button btnRefrescar;
    private Button btnRegistrarDevolucion;

    public FormPrestamosFueraPlazo()
    {
        InitializeComponent();
        CargarPrestamosVencidos();
    }

    private void InitializeComponent()
    {
        Text = "Préstamos Fuera de Plazo";
        Size = new Size(900, 500);
        StartPosition = FormStartPosition.CenterParent;

        btnRefrescar = new Button { Text = "Refrescar", Location = new Point(20, 20), Size = new Size(100, 30) };
        btnRefrescar.Click += (s, e) => CargarPrestamosVencidos();

        btnRegistrarDevolucion = new Button { Text = "Registrar Devolución", Location = new Point(140, 20), Size = new Size(150, 30) };
        btnRegistrarDevolucion.Click += BtnRegistrarDevolucion_Click;

        dgvVencidos = new DataGridView
        {
            Location = new Point(20, 65),
            Size = new Size(845, 350),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        lblTotal = new Label
        {
            Text = "Total vencidos: 0",
            Location = new Point(20, 425),
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            ForeColor = Color.Red
        };

        Controls.AddRange(new Control[] { btnRefrescar, btnRegistrarDevolucion, dgvVencidos, lblTotal });
    }

    private void CargarPrestamosVencidos()
    {
        var vencidos = NegocioPrestamos.ObtenerVencidos();

        var datos = vencidos.Select(p => new
        {
            p.Id,
            Usuario = p.Usuario.NombreCompleto,
            DNI = p.Usuario.DNI,
            Telefono = p.Usuario.Telefono,
            FechaPrestamo = p.FechaPrestamo.ToString("dd/MM/yyyy"),
            FechaDevolucion = p.FechaDevolucionPrevista.ToString("dd/MM/yyyy"),
            DiasRetraso = p.DiasRetraso,
            Ejemplares = p.NumeroEjemplares,
            SancionEstimada = $"{p.DiasRetraso * 2} días"
        }).ToList();

        dgvVencidos.DataSource = datos;
        lblTotal.Text = $"Total préstamos vencidos: {vencidos.Count}";

        foreach (DataGridViewRow row in dgvVencidos.Rows)
        {
            var diasRetraso = (int)(row.Cells["DiasRetraso"].Value ?? 0);
            if (diasRetraso > 14)
                row.DefaultCellStyle.BackColor = Color.LightCoral;
            else if (diasRetraso > 7)
                row.DefaultCellStyle.BackColor = Color.LightSalmon;
            else
                row.DefaultCellStyle.BackColor = Color.LightYellow;
        }
    }

    private void BtnRegistrarDevolucion_Click(object? sender, EventArgs e)
    {
        if (dgvVencidos.SelectedRows.Count == 0)
        {
            MessageBox.Show("Seleccione un préstamo.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var id = dgvVencidos.SelectedRows[0].Cells["Id"].Value?.ToString();
        if (string.IsNullOrEmpty(id)) return;

        var prestamo = NegocioPrestamos.ObtenerPorId(id);
        if (prestamo == null) return;

        var resultado = MessageBox.Show(
            $"¿Registrar devolución del préstamo {id}?\n\n" +
            $"Usuario: {prestamo.Usuario.NombreCompleto}\n" +
            $"Días de retraso: {prestamo.DiasRetraso}\n" +
            $"Sanción a aplicar: {prestamo.DiasRetraso * 2} días",
            "Confirmar Devolución",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (resultado == DialogResult.Yes)
        {
            var (exito, mensaje) = NegocioPrestamos.RegistrarDevolucion(id);
            if (exito)
            {
                MessageBox.Show(mensaje, "Devolución Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarPrestamosVencidos();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
