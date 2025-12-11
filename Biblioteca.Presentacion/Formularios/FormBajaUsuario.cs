using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para dar de baja usuarios.
/// Solicita el DNI y confirma la baja.
/// </summary>
public class FormBajaUsuario : Form
{
    private Label lblDNI;
    private TextBox txtDNI;
    private Button btnBuscar;
    private GroupBox grpUsuario;
    private Label lblNombre;
    private Label lblApellidos;
    private Label lblEmail;
    private Label lblEstado;
    private TextBox txtNombre;
    private TextBox txtApellidos;
    private TextBox txtEmail;
    private TextBox txtEstado;
    private Button btnBaja;
    private Button btnCancelar;
    private ErrorProvider errorProvider;

    public FormBajaUsuario()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "Baja de Usuario";
        Size = new Size(450, 350);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        // DNI
        lblDNI = new Label { Text = "DNI:", Location = new Point(20, 25), AutoSize = true };
        txtDNI = new TextBox { Location = new Point(80, 22), Width = 150, MaxLength = 9 };
        btnBuscar = new Button { Text = "Buscar", Location = new Point(250, 20), Size = new Size(80, 25) };
        btnBuscar.Click += BtnBuscar_Click;

        // Grupo datos usuario
        grpUsuario = new GroupBox
        {
            Text = "Datos del Usuario",
            Location = new Point(20, 60),
            Size = new Size(400, 180),
            Enabled = false
        };

        lblNombre = new Label { Text = "Nombre:", Location = new Point(15, 30), AutoSize = true };
        txtNombre = new TextBox { Location = new Point(100, 27), Width = 280, ReadOnly = true };

        lblApellidos = new Label { Text = "Apellidos:", Location = new Point(15, 60), AutoSize = true };
        txtApellidos = new TextBox { Location = new Point(100, 57), Width = 280, ReadOnly = true };

        lblEmail = new Label { Text = "Email:", Location = new Point(15, 90), AutoSize = true };
        txtEmail = new TextBox { Location = new Point(100, 87), Width = 280, ReadOnly = true };

        lblEstado = new Label { Text = "Estado:", Location = new Point(15, 120), AutoSize = true };
        txtEstado = new TextBox { Location = new Point(100, 117), Width = 280, ReadOnly = true };

        grpUsuario.Controls.AddRange(new Control[] { lblNombre, txtNombre, lblApellidos, txtApellidos, lblEmail, txtEmail, lblEstado, txtEstado });

        // Botones
        btnBaja = new Button { Text = "Dar de Baja", Location = new Point(120, 260), Size = new Size(100, 30), Enabled = false };
        btnBaja.Click += BtnBaja_Click;

        btnCancelar = new Button { Text = "Cancelar", Location = new Point(240, 260), Size = new Size(100, 30) };
        btnCancelar.Click += (s, e) => Close();

        Controls.AddRange(new Control[] { lblDNI, txtDNI, btnBuscar, grpUsuario, btnBaja, btnCancelar });
    }

    private void BtnBuscar_Click(object? sender, EventArgs e)
    {
        errorProvider.SetError(txtDNI, "");

        if (string.IsNullOrWhiteSpace(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "Introduzca el DNI.");
            return;
        }

        if (!NegocioUsuarios.ValidarFormatoDNI(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "Formato de DNI inválido.");
            return;
        }

        var usuario = NegocioUsuarios.BuscarPorDNI(txtDNI.Text.Trim());
        if (usuario == null)
        {
            MessageBox.Show("No se encontró ningún usuario con ese DNI.", "Usuario no encontrado",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LimpiarDatos();
            return;
        }

        // Mostrar datos
        txtNombre.Text = usuario.Nombre;
        txtApellidos.Text = usuario.Apellidos;
        txtEmail.Text = usuario.Email;
        txtEstado.Text = usuario.Sancionado ? $"Sancionado hasta {usuario.FechaFinSancion:dd/MM/yyyy}" : "Activo";

        grpUsuario.Enabled = true;
        btnBaja.Enabled = true;
    }

    private void BtnBaja_Click(object? sender, EventArgs e)
    {
        var resultado = MessageBox.Show(
            $"¿Está seguro de que desea dar de baja al usuario {txtNombre.Text} {txtApellidos.Text}?\n\nEsta acción no se puede deshacer.",
            "Confirmar Baja",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (resultado == DialogResult.Yes)
        {
            var (exito, mensaje) = NegocioUsuarios.BajaUsuario(txtDNI.Text.Trim());

            if (exito)
            {
                MessageBox.Show(mensaje, "Baja Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarDatos();
                txtDNI.Clear();
                txtDNI.Focus();
            }
            else
            {
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void LimpiarDatos()
    {
        txtNombre.Clear();
        txtApellidos.Clear();
        txtEmail.Clear();
        txtEstado.Clear();
        grpUsuario.Enabled = false;
        btnBaja.Enabled = false;
    }
}
