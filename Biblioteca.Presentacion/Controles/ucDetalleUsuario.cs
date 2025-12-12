using System.ComponentModel;
using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Controles;

/// <summary>
/// Control de usuario reutilizable para mostrar y editar datos de usuario.
/// Incluye validación con ErrorProvider (Práctica 8).
/// </summary>
public partial class ucDetalleUsuario : UserControl
{
    private ErrorProvider errorProvider;
    private Usuario? _usuario;

    #region Controles

    private Label lblDNI;
    private Label lblNombre;
    private Label lblApellidos;
    private Label lblEmail;
    private Label lblTelefono;
    private Label lblDireccion;

    private TextBox txtDNI;
    private TextBox txtNombre;
    private TextBox txtApellidos;
    private TextBox txtEmail;
    private TextBox txtTelefono;
    private TextBox txtDireccion;

    private CheckBox chkSancionado;
    private DateTimePicker dtpFechaFinSancion;

    #endregion

    public ucDetalleUsuario()
    {
        InitializeComponent();
        ConfigurarValidaciones();
    }

    private void InitializeComponent()
    {
        errorProvider = new ErrorProvider();
        ((ISupportInitialize)errorProvider).BeginInit();

        // Crear controles
        lblDNI = new Label { Text = "DNI:", Location = new Point(10, 15), AutoSize = true };
        lblNombre = new Label { Text = "Nombre:", Location = new Point(10, 45), AutoSize = true };
        lblApellidos = new Label { Text = "Apellidos:", Location = new Point(10, 75), AutoSize = true };
        lblEmail = new Label { Text = "Email:", Location = new Point(10, 105), AutoSize = true };
        lblTelefono = new Label { Text = "Teléfono:", Location = new Point(10, 135), AutoSize = true };
        lblDireccion = new Label { Text = "Dirección:", Location = new Point(10, 165), AutoSize = true };

        txtDNI = new TextBox { Location = new Point(100, 12), Width = 150, MaxLength = 9 };
        txtNombre = new TextBox { Location = new Point(100, 42), Width = 250 };
        txtApellidos = new TextBox { Location = new Point(100, 72), Width = 250 };
        txtEmail = new TextBox { Location = new Point(100, 102), Width = 250 };
        txtTelefono = new TextBox { Location = new Point(100, 132), Width = 150 };
        txtDireccion = new TextBox { Location = new Point(100, 162), Width = 250 };

        chkSancionado = new CheckBox
        {
            Text = "Usuario sancionado",
            Location = new Point(10, 195),
            AutoSize = true
        };
        chkSancionado.CheckedChanged += ChkSancionado_CheckedChanged;

        dtpFechaFinSancion = new DateTimePicker
        {
            Location = new Point(150, 193),
            Width = 150,
            Format = DateTimePickerFormat.Short,
            Enabled = false
        };

        // Añadir controles
        Controls.AddRange(new Control[]
        {
            lblDNI, lblNombre, lblApellidos, lblEmail, lblTelefono, lblDireccion,
            txtDNI, txtNombre, txtApellidos, txtEmail, txtTelefono, txtDireccion,
            chkSancionado, dtpFechaFinSancion
        });

        // Configurar UserControl
        Size = new Size(380, 230);
        BorderStyle = BorderStyle.FixedSingle;

        ((ISupportInitialize)errorProvider).EndInit();
    }

    private void ConfigurarValidaciones()
    {
        txtDNI.Validating += TxtDNI_Validating;
        txtNombre.Validating += TxtNombre_Validating;
        txtApellidos.Validating += TxtApellidos_Validating;
        txtEmail.Validating += TxtEmail_Validating;
    }

    private void ChkSancionado_CheckedChanged(object? sender, EventArgs e)
    {
        dtpFechaFinSancion.Enabled = chkSancionado.Checked;
        if (chkSancionado.Checked && dtpFechaFinSancion.Value < DateTime.Now)
        {
            dtpFechaFinSancion.Value = DateTime.Now.AddDays(7);
        }
    }

    #region Validaciones con ErrorProvider

    private void TxtDNI_Validating(object? sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "El DNI es obligatorio.");
            e.Cancel = true;
        }
        else if (!NegocioUsuarios.ValidarFormatoDNI(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "Formato de DNI inválido (8 números + 1 letra).");
            e.Cancel = true;
        }
        else
        {
            errorProvider.SetError(txtDNI, "");
        }
    }

    private void TxtNombre_Validating(object? sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            errorProvider.SetError(txtNombre, "El nombre es obligatorio.");
            e.Cancel = true;
        }
        else
        {
            errorProvider.SetError(txtNombre, "");
        }
    }

    private void TxtApellidos_Validating(object? sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            errorProvider.SetError(txtApellidos, "Los apellidos son obligatorios.");
            e.Cancel = true;
        }
        else
        {
            errorProvider.SetError(txtApellidos, "");
        }
    }

    private void TxtEmail_Validating(object? sender, CancelEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains('@'))
        {
            errorProvider.SetError(txtEmail, "El formato del email no es válido.");
            e.Cancel = true;
        }
        else
        {
            errorProvider.SetError(txtEmail, "");
        }
    }

    #endregion

    #region Propiedades Públicas

    /// <summary>
    /// Obtiene o establece el usuario mostrado en el control.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Usuario? Usuario
    {
        get => _usuario;
        set
        {
            _usuario = value;
            CargarDatos();
        }
    }

    /// <summary>
    /// Indica si el control está en modo solo lectura.
    /// </summary>
    public bool SoloLectura
    {
        get => txtDNI.ReadOnly;
        set
        {
            txtDNI.ReadOnly = value;
            txtNombre.ReadOnly = value;
            txtApellidos.ReadOnly = value;
            txtEmail.ReadOnly = value;
            txtTelefono.ReadOnly = value;
            txtDireccion.ReadOnly = value;
            chkSancionado.Enabled = !value;
            dtpFechaFinSancion.Enabled = !value && chkSancionado.Checked;
        }
    }

    /// <summary>
    /// Indica si el DNI puede ser editado (false para edición de usuarios existentes).
    /// </summary>
    public bool DNIEditable
    {
        get => !txtDNI.ReadOnly;
        set => txtDNI.ReadOnly = !value;
    }

    #endregion

    #region Métodos Públicos

    /// <summary>
    /// Carga los datos del usuario en los controles.
    /// </summary>
    private void CargarDatos()
    {
        if (_usuario == null)
        {
            LimpiarCampos();
            return;
        }

        txtDNI.Text = _usuario.DNI;
        txtNombre.Text = _usuario.Nombre;
        txtApellidos.Text = _usuario.Apellidos;
        txtEmail.Text = _usuario.Email;
        txtTelefono.Text = _usuario.Telefono;
        txtDireccion.Text = _usuario.Direccion;
        chkSancionado.Checked = _usuario.Sancionado;

        if (_usuario.FechaFinSancion.HasValue)
            dtpFechaFinSancion.Value = _usuario.FechaFinSancion.Value;
    }

    /// <summary>
    /// Obtiene los datos de los controles y los aplica al usuario.
    /// </summary>
    public Usuario ObtenerDatos()
    {
        // Si no hay usuario existente, crear uno nuevo con los campos obligatorios
        if (_usuario == null)
        {
            _usuario = new Usuario
            {
                DNI = txtDNI.Text.Trim().ToUpper(),
                Nombre = txtNombre.Text.Trim()
            };
        }
        else
        {
            _usuario.DNI = txtDNI.Text.Trim().ToUpper();
            _usuario.Nombre = txtNombre.Text.Trim();
        }

        var usuario = _usuario;
        usuario.Apellidos = txtApellidos.Text.Trim();
        usuario.Email = txtEmail.Text.Trim();
        usuario.Telefono = txtTelefono.Text.Trim();
        usuario.Direccion = txtDireccion.Text.Trim();
        usuario.Sancionado = chkSancionado.Checked;
        usuario.FechaFinSancion = chkSancionado.Checked ? dtpFechaFinSancion.Value : null;

        return usuario;
    }

    /// <summary>
    /// Limpia todos los campos.
    /// </summary>
    public void LimpiarCampos()
    {
        txtDNI.Clear();
        txtNombre.Clear();
        txtApellidos.Clear();
        txtEmail.Clear();
        txtTelefono.Clear();
        txtDireccion.Clear();
        chkSancionado.Checked = false;
        dtpFechaFinSancion.Value = DateTime.Now;

        errorProvider.Clear();
    }

    /// <summary>
    /// Valida todos los campos y retorna si son válidos.
    /// </summary>
    public bool ValidarCampos()
    {
        bool valido = true;

        // Validar DNI
        if (string.IsNullOrWhiteSpace(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "El DNI es obligatorio.");
            valido = false;
        }
        else if (!NegocioUsuarios.ValidarFormatoDNI(txtDNI.Text))
        {
            errorProvider.SetError(txtDNI, "Formato de DNI inválido.");
            valido = false;
        }
        else
        {
            errorProvider.SetError(txtDNI, "");
        }

        // Validar Nombre
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            errorProvider.SetError(txtNombre, "El nombre es obligatorio.");
            valido = false;
        }
        else
        {
            errorProvider.SetError(txtNombre, "");
        }

        // Validar Apellidos
        if (string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            errorProvider.SetError(txtApellidos, "Los apellidos son obligatorios.");
            valido = false;
        }
        else
        {
            errorProvider.SetError(txtApellidos, "");
        }

        return valido;
    }

    /// <summary>
    /// Establece el foco en el campo DNI.
    /// </summary>
    public void EnfocarDNI()
    {
        txtDNI.Focus();
    }

    /// <summary>
    /// Establece el DNI inicial (para alta de usuarios).
    /// </summary>
    public void EstablecerDNI(string dni)
    {
        txtDNI.Text = dni;
    }

    #endregion
}
