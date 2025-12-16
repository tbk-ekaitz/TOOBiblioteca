using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario de Login Modal.
/// Valida credenciales por DNI y devuelve el empleado logueado.
/// </summary>
public class FormLogin : Form
{
    private Label lblTitulo;
    private Label lblDNI;
    private Label lblPassword;
    private TextBox txtDNI;
    private TextBox txtPassword;
    private Button btnEntrar;
    private Button btnCancelar;
    private Label lblMensaje;
    private PictureBox picLogo;
    private ErrorProvider errorProvider;

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
        Text = "Biblioteca - Iniciar Sesión";
        Size = new Size(400, 320);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        AcceptButton = null;

        errorProvider = new ErrorProvider
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        // logo
        picLogo = new PictureBox
        {
            Location = new Point(160, 10),
            Size = new Size(64, 64),
            SizeMode = PictureBoxSizeMode.CenterImage,
            BackColor = Color.LightSteelBlue
        };

        // titulo
        lblTitulo = new Label
        {
            Text = "Sistema de Gestión de Biblioteca",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(50, 80),
            AutoSize = true,
            ForeColor = Color.DarkSlateGray
        };

        // DNI
        lblDNI = new Label
        {
            Text = "DNI:",
            Location = new Point(50, 130),
            AutoSize = true
        };

        txtDNI = new TextBox
        {
            Location = new Point(130, 127),
            Width = 200,
            MaxLength = 9
        };
        txtDNI.TextChanged += TxtDNI_TextChanged;

        // contrasena
        lblPassword = new Label
        {
            Text = "Contraseña:",
            Location = new Point(50, 165),
            AutoSize = true
        };

        txtPassword = new TextBox
        {
            Location = new Point(130, 162),
            Width = 200,
            PasswordChar = '\u25CF'
        };

        // botones
        btnEntrar = new Button
        {
            Text = "Entrar",
            Location = new Point(130, 205),
            Size = new Size(90, 30),
            DialogResult = DialogResult.None
        };
        btnEntrar.Click += BtnEntrar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(240, 205),
            Size = new Size(90, 30),
            DialogResult = DialogResult.Cancel
        };

        lblMensaje = new Label
        {
            Location = new Point(50, 245),
            Size = new Size(300, 20),
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.AddRange(new Control[]
        {
            picLogo, lblTitulo, lblDNI, txtDNI,
            lblPassword, txtPassword, btnEntrar, btnCancelar, lblMensaje
        });

        AcceptButton = btnEntrar;
        CancelButton = btnCancelar;

        txtPassword.KeyPress += TxtPassword_KeyPress;

        //! precargar datos 
        #if DEBUG
        txtDNI.Text = "12345678A";
        txtPassword.Text = "admin123";
        #endif
    }

    private void TxtDNI_TextChanged(object? sender, EventArgs e)
    {
        errorProvider.SetError(txtDNI, "");
        lblMensaje.Text = "";
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
        errorProvider.SetError(txtDNI, "");
        errorProvider.SetError(txtPassword, "");

        if (string.IsNullOrWhiteSpace(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "Introduzca el DNI.");
            lblMensaje.Text = "Introduzca el DNI.";
            txtDNI.Focus();
            return;
        }

        if (!NegocioUsuarios.ValidarFormatoDNI(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "El formato del DNI no es válido (8 números + 1 letra).");
            lblMensaje.Text = "El formato del DNI no es válido.";
            txtDNI.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            errorProvider.SetError(txtPassword, "Introduzca la contraseña.");
            lblMensaje.Text = "Introduzca la contraseña.";
            txtPassword.Focus();
            return;
        }

        var empleado = NegocioUsuarios.ValidarLoginPorDNI(txtDNI.Text.Trim(), txtPassword.Text);

        if (empleado != null)
        {
            EmpleadoLogueado = empleado;
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            lblMensaje.Text = "DNI o contraseña incorrectos.";
            txtPassword.Clear();
            txtPassword.Focus();
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        txtDNI.Focus();
    }
}
