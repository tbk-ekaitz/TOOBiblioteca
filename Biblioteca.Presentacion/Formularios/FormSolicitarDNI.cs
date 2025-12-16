using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Diálogo pequeño para solicitar DNI antes de buscar/dar de alta un usuario.
/// pr12
/// </summary>
public class FormSolicitarDNI : Form
{
    public enum Modo { Buscar, Alta, Baja }

    private Label lblInstruccion;
    private Label lblDNI;
    private TextBox txtDNI;
    private Button btnAceptar;
    private Button btnCancelar;
    private Label lblMensaje;

    private readonly Modo _modo;

    public FormSolicitarDNI(Modo modo)
    {
        _modo = modo;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        string titulo = _modo switch
        {
            Modo.Buscar => "Buscar Usuario",
            Modo.Alta => "Alta de Usuario",
            Modo.Baja => "Baja de Usuario",
            _ => "Usuario"
        };

        Text = titulo;
        Size = new Size(350, 200);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        string instruccion = _modo switch
        {
            Modo.Buscar => "Introduzca el DNI del usuario a buscar:",
            Modo.Alta => "Introduzca el DNI del nuevo usuario:",
            Modo.Baja => "Introduzca el DNI del usuario a dar de baja:",
            _ => "Introduzca el DNI:"
        };

        lblInstruccion = new Label
        {
            Text = instruccion,
            Location = new Point(20, 20),
            AutoSize = true
        };

        lblDNI = new Label
        {
            Text = "DNI:",
            Location = new Point(20, 55),
            AutoSize = true
        };

        txtDNI = new TextBox
        {
            Location = new Point(60, 52),
            Width = 150,
            MaxLength = 9,
            CharacterCasing = CharacterCasing.Upper
        };

        btnAceptar = new Button
        {
            Text = "Aceptar",
            Location = new Point(80, 100),
            Size = new Size(80, 28)
        };
        btnAceptar.Click += BtnAceptar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(170, 100),
            Size = new Size(80, 28),
            DialogResult = DialogResult.Cancel
        };
        btnCancelar.Click += BtnCancelar_Click;

        lblMensaje = new Label
        {
            Location = new Point(20, 135),
            Size = new Size(300, 20),
            ForeColor = Color.Red
        };

        Controls.AddRange(new Control[]
        {
            lblInstruccion, lblDNI, txtDNI, btnAceptar, btnCancelar, lblMensaje
        });

        AcceptButton = btnAceptar;
        CancelButton = btnCancelar;
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        txtDNI.Text = "";
    }

    private void BtnAceptar_Click(object? sender, EventArgs e)
    {
        lblMensaje.Text = "";

        if (string.IsNullOrWhiteSpace(txtDNI.Text))
        {
            lblMensaje.Text = "Debe introducir un DNI.";
            txtDNI.Focus();
            return;
        }

        if (!NegocioUsuarios.ValidarFormatoDNI(txtDNI.Text))
        {
            lblMensaje.Text = "Formato de DNI inválido (8 números + 1 letra).";
            txtDNI.Focus();
            return;
        }

        var usuario = NegocioUsuarios.BuscarPorDNI(txtDNI.Text);

        switch (_modo)
        {
            case Modo.Buscar:
                if (usuario == null)
                {
                    lblMensaje.Text = "No se encontró ningún usuario con ese DNI.";
                    return;
                }
                var formDetalle = new FormGestionUsuario(usuario, false);
                formDetalle.MdiParent = MdiParent;
                formDetalle.Show();
                Close();
                break;

            case Modo.Alta:
                if (usuario != null)
                {
                    lblMensaje.Text = "Ya existe un usuario con ese DNI.";
                    return;
                }
                var formAlta = new FormGestionUsuario(txtDNI.Text);
                formAlta.MdiParent = MdiParent;
                formAlta.Show();
                Close();
                break;

            case Modo.Baja:
                if (usuario == null)
                {
                    lblMensaje.Text = "No se encontró ningún usuario con ese DNI.";
                    return;
                }
                var resultado = NegocioUsuarios.BajaUsuario(usuario.DNI);
                if (resultado.exito)
                {
                    MessageBox.Show(resultado.mensaje, "Baja de Usuario",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    lblMensaje.Text = resultado.mensaje;
                }
                break;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        txtDNI.Focus();
    }
}
