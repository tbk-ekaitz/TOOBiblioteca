using Biblioteca.Dominio;
using Biblioteca.Negocio;
using Biblioteca.Presentacion.Controles;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para Alta/Edición de usuarios.
/// Reutiliza el UserControl ucDetalleUsuario.
/// </summary>
public class FormGestionUsuario : Form
{
    private ucDetalleUsuario ucDetalle;
    private GroupBox grpDatos;
    private GroupBox grpPrestamos;
    private ListBox lstPrestamos;
    private Button btnGuardar;
    private Button btnCancelar;
    private Label lblInfo;

    private readonly bool _esNuevo;
    private Usuario? _usuario;

    /// <summary>
    /// Constructor para edición de usuario existente.
    /// </summary>
    public FormGestionUsuario(Usuario usuario, bool editable = true)
    {
        _usuario = usuario;
        _esNuevo = false;
        InitializeComponent();
        CargarDatos(editable);
    }

    /// <summary>
    /// Constructor para alta de nuevo usuario.
    /// </summary>
    public FormGestionUsuario(string dni)
    {
        _usuario = new Usuario { DNI = dni };
        _esNuevo = true;
        InitializeComponent();
        CargarDatos(true);
    }

    private void InitializeComponent()
    {
        Text = _esNuevo ? "Alta de Usuario" : "Detalle de Usuario";
        Size = new Size(450, 500);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // GroupBox Datos
        grpDatos = new GroupBox
        {
            Text = "Datos del Usuario",
            Location = new Point(10, 10),
            Size = new Size(410, 260)
        };

        ucDetalle = new ucDetalleUsuario
        {
            Location = new Point(10, 20)
        };
        grpDatos.Controls.Add(ucDetalle);

        // GroupBox Préstamos (solo en edición)
        grpPrestamos = new GroupBox
        {
            Text = "Préstamos Activos",
            Location = new Point(10, 280),
            Size = new Size(410, 120),
            Visible = !_esNuevo
        };

        lstPrestamos = new ListBox
        {
            Location = new Point(10, 20),
            Size = new Size(390, 90)
        };
        grpPrestamos.Controls.Add(lstPrestamos);

        // Info
        lblInfo = new Label
        {
            Location = new Point(10, 410),
            Size = new Size(300, 20),
            ForeColor = Color.Gray
        };

        // Botones
        btnGuardar = new Button
        {
            Text = _esNuevo ? "Dar de Alta" : "Guardar Cambios",
            Location = new Point(210, 420),
            Size = new Size(100, 30)
        };
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(320, 420),
            Size = new Size(100, 30)
        };
        btnCancelar.Click += (s, e) => Close();

        Controls.AddRange(new Control[]
        {
            grpDatos, grpPrestamos, lblInfo, btnGuardar, btnCancelar
        });
    }

    private void CargarDatos(bool editable)
    {
        ucDetalle.Usuario = _usuario;
        ucDetalle.SoloLectura = !editable;
        ucDetalle.DNIEditable = _esNuevo;

        btnGuardar.Visible = editable;

        if (!_esNuevo && _usuario != null)
        {
            // Cargar préstamos activos
            var prestamos = NegocioPrestamos.ObtenerActivosPorUsuario(_usuario.Id);
            lstPrestamos.Items.Clear();

            if (prestamos.Count == 0)
            {
                lstPrestamos.Items.Add("No tiene préstamos activos");
            }
            else
            {
                foreach (var p in prestamos)
                {
                    lstPrestamos.Items.Add($"{p.CodigoPrestamo} - {p.NumeroEjemplares} ejemplar(es) - Devolver: {p.FechaDevolucionPrevista:dd/MM/yyyy}");
                }
            }

            // Info
            lblInfo.Text = $"Alta: {_usuario.FechaAlta:dd/MM/yyyy} | Préstamos activos: {prestamos.Count}";
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        // Validar campos
        if (!ucDetalle.ValidarCampos())
        {
            MessageBox.Show("Por favor, corrija los errores marcados.",
                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var usuario = ucDetalle.ObtenerDatos();

        if (_esNuevo)
        {
            var resultado = NegocioUsuarios.AltaUsuario(usuario);
            if (resultado.exito)
            {
                MessageBox.Show(resultado.mensaje, "Alta de Usuario",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show(resultado.mensaje, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            usuario.Id = _usuario!.Id;
            var resultado = NegocioUsuarios.ModificarUsuario(usuario);
            if (resultado.exito)
            {
                MessageBox.Show(resultado.mensaje, "Modificación de Usuario",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                MessageBox.Show(resultado.mensaje, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
