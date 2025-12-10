using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario de Login Modal.
/// Valida credenciales y devuelve el empleado logueado.
/// </summary>
public class FormLogin : Form
{
    private Label lblTitulo;
    private Label lblUsuario;
    private Label lblPassword;
    private TextBox txtUsuario;
    private TextBox txtPassword;
    private Button btnEntrar;
    private Button btnCancelar;
    private Label lblMensaje;
    private PictureBox picLogo;

    /// <summary>
    /// Empleado que ha iniciado sesión correctamente.
    /// </summary>
    public Empleado? EmpleadoLogueado { get; private set; }

    public FormLogin()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Configuración del formulario
        Text = "Biblioteca - Iniciar Sesión";
        Size = new Size(400, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        AcceptButton = null; // Se asignará después de crear el botón

        // Logo/Icono
        picLogo = new PictureBox
        {
            Location = new Point(160, 10),
            Size = new Size(64, 64),
            SizeMode = PictureBoxSizeMode.CenterImage,
            BackColor = Color.LightSteelBlue
        };

        // Título
        lblTitulo = new Label
        {
            Text = "Sistema de Gestión de Biblioteca",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(50, 80),
            AutoSize = true,
            ForeColor = Color.DarkSlateGray
        };

        // Usuario
        lblUsuario = new Label
        {
            Text = "Usuario:",
            Location = new Point(50, 130),
            AutoSize = true
        };

        txtUsuario = new TextBox
        {
            Location = new Point(130, 127),
            Width = 200
        };

        // Password
        lblPassword = new Label
        {
            Text = "Contraseña:",
            Location = new Point(50, 160),
            AutoSize = true
        };

        txtPassword = new TextBox
        {
            Location = new Point(130, 157),
            Width = 200,
            PasswordChar = '●'
        };

        // Botones
        btnEntrar = new Button
        {
            Text = "Entrar",
            Location = new Point(130, 200),
            Size = new Size(90, 30),
            DialogResult = DialogResult.None
        };
        btnEntrar.Click += BtnEntrar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(240, 200),
            Size = new Size(90, 30),
            DialogResult = DialogResult.Cancel
        };

        // Mensaje de error
        lblMensaje = new Label
        {
            Location = new Point(50, 240),
            Size = new Size(300, 20),
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // Añadir controles
        Controls.AddRange(new Control[]
        {
            picLogo, lblTitulo, lblUsuario, txtUsuario,
            lblPassword, txtPassword, btnEntrar, btnCancelar, lblMensaje
        });

        // Asignar AcceptButton después de crear el botón
        AcceptButton = btnEntrar;
        CancelButton = btnCancelar;

        // Eventos
        txtPassword.KeyPress += TxtPassword_KeyPress;

        // Pre-cargar usuario de prueba (solo para desarrollo)
        #if DEBUG
        txtUsuario.Text = "admin";
        txtPassword.Text = "admin123";
        #endif
    }

    private void TxtPassword_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            BtnEntrar_Click(sender, e);
            e.Handled = true;
        }
    }

    private void BtnEntrar_Click(object? sender, EventArgs e)
    {
        lblMensaje.Text = "";

        // Validar campos vacíos
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            lblMensaje.Text = "Introduzca el nombre de usuario.";
            txtUsuario.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblMensaje.Text = "Introduzca la contraseña.";
            txtPassword.Focus();
            return;
        }

        // Intentar login
        var empleado = NegocioUsuarios.ValidarLogin(txtUsuario.Text, txtPassword.Text);

        if (empleado != null)
        {
            EmpleadoLogueado = empleado;
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            lblMensaje.Text = "Usuario o contraseña incorrectos.";
            txtPassword.Clear();
            txtPassword.Focus();
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        txtUsuario.Focus();
    }
}
