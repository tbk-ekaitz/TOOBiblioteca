using System.ComponentModel;
using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario de listados con ListBox ordenados y BindingNavigator.
/// Implementa la Práctica 13 (navegación de datos).
/// </summary>
public class FormListados : Form
{
    private SplitContainer splitContainer;

    // Panel izquierdo - ListBox
    private GroupBox grpListado;
    private ListBox lstUsuarios;
    private ComboBox cboOrden;
    private Label lblOrden;
    private TextBox txtBuscar;
    private Button btnBuscar;

    // Panel derecho - Detalle con BindingNavigator
    private GroupBox grpDetalle;
    private BindingNavigator navUsuarios;
    private BindingSource bsUsuarios;

    private Label lblDNI;
    private Label lblNombre;
    private Label lblEmail;
    private Label lblTelefono;
    private Label lblFechaAlta;
    private Label lblEstado;
    private TextBox txtDetalleDNI;
    private TextBox txtDetalleNombre;
    private TextBox txtDetalleEmail;
    private TextBox txtDetalleTelefono;
    private TextBox txtDetalleFechaAlta;
    private TextBox txtDetalleEstado;

    private Button btnEditar;
    private Button btnEliminar;

    public FormListados()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        Text = "Listado de Usuarios";
        Size = new Size(900, 550);
        StartPosition = FormStartPosition.CenterParent;

        // SplitContainer
        splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterDistance = 350,
            FixedPanel = FixedPanel.Panel1
        };

        // === PANEL IZQUIERDO ===
        grpListado = new GroupBox
        {
            Text = "Usuarios",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        // Controles de filtro
        lblOrden = new Label
        {
            Text = "Ordenar por:",
            Location = new Point(15, 25),
            AutoSize = true
        };

        cboOrden = new ComboBox
        {
            Location = new Point(90, 22),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboOrden.Items.AddRange(new object[] { "Apellidos", "Fecha de Alta", "DNI" });
        cboOrden.SelectedIndex = 0;
        cboOrden.SelectedIndexChanged += CboOrden_SelectedIndexChanged;

        txtBuscar = new TextBox
        {
            Location = new Point(15, 55),
            Width = 170,
            PlaceholderText = "Buscar por nombre..."
        };

        btnBuscar = new Button
        {
            Text = "Buscar",
            Location = new Point(190, 53),
            Size = new Size(60, 25)
        };
        btnBuscar.Click += BtnBuscar_Click;

        lstUsuarios = new ListBox
        {
            Location = new Point(15, 90),
            Size = new Size(310, 370)
        };
        lstUsuarios.SelectedIndexChanged += LstUsuarios_SelectedIndexChanged;

        grpListado.Controls.AddRange(new Control[]
        {
            lblOrden, cboOrden, txtBuscar, btnBuscar, lstUsuarios
        });

        // === PANEL DERECHO ===
        grpDetalle = new GroupBox
        {
            Text = "Detalle del Usuario",
            Dock = DockStyle.Fill,
            Padding = new Padding(10)
        };

        // BindingSource y BindingNavigator
        bsUsuarios = new BindingSource();

        navUsuarios = new BindingNavigator(true)
        {
            BindingSource = bsUsuarios,
            Location = new Point(15, 20)
        };
        navUsuarios.AddNewItem.Visible = false;
        navUsuarios.DeleteItem.Visible = false;

        // Campos de detalle
        int y = 70;
        int spacing = 35;

        lblDNI = new Label { Text = "DNI:", Location = new Point(15, y), AutoSize = true };
        txtDetalleDNI = new TextBox { Location = new Point(100, y - 3), Width = 150, ReadOnly = true };

        lblNombre = new Label { Text = "Nombre:", Location = new Point(15, y + spacing), AutoSize = true };
        txtDetalleNombre = new TextBox { Location = new Point(100, y + spacing - 3), Width = 300, ReadOnly = true };

        lblEmail = new Label { Text = "Email:", Location = new Point(15, y + spacing * 2), AutoSize = true };
        txtDetalleEmail = new TextBox { Location = new Point(100, y + spacing * 2 - 3), Width = 300, ReadOnly = true };

        lblTelefono = new Label { Text = "Teléfono:", Location = new Point(15, y + spacing * 3), AutoSize = true };
        txtDetalleTelefono = new TextBox { Location = new Point(100, y + spacing * 3 - 3), Width = 150, ReadOnly = true };

        lblFechaAlta = new Label { Text = "Alta:", Location = new Point(15, y + spacing * 4), AutoSize = true };
        txtDetalleFechaAlta = new TextBox { Location = new Point(100, y + spacing * 4 - 3), Width = 100, ReadOnly = true };

        lblEstado = new Label { Text = "Estado:", Location = new Point(15, y + spacing * 5), AutoSize = true };
        txtDetalleEstado = new TextBox { Location = new Point(100, y + spacing * 5 - 3), Width = 200, ReadOnly = true };

        // Botones
        btnEditar = new Button
        {
            Text = "Editar",
            Location = new Point(100, y + spacing * 6 + 20),
            Size = new Size(80, 30)
        };
        btnEditar.Click += BtnEditar_Click;

        btnEliminar = new Button
        {
            Text = "Eliminar",
            Location = new Point(190, y + spacing * 6 + 20),
            Size = new Size(80, 30)
        };
        btnEliminar.Click += BtnEliminar_Click;

        grpDetalle.Controls.AddRange(new Control[]
        {
            navUsuarios,
            lblDNI, txtDetalleDNI,
            lblNombre, txtDetalleNombre,
            lblEmail, txtDetalleEmail,
            lblTelefono, txtDetalleTelefono,
            lblFechaAlta, txtDetalleFechaAlta,
            lblEstado, txtDetalleEstado,
            btnEditar, btnEliminar
        });

        // Ensamblar
        splitContainer.Panel1.Controls.Add(grpListado);
        splitContainer.Panel2.Controls.Add(grpDetalle);
        Controls.Add(splitContainer);

        // Configurar enlace de datos para navegación
        bsUsuarios.CurrentChanged += BsUsuarios_CurrentChanged;
    }

    private void CargarDatos()
    {
        var usuarios = cboOrden.SelectedIndex switch
        {
            0 => NegocioUsuarios.ObtenerTodosOrdenados(),
            1 => NegocioUsuarios.ObtenerPorFechaAlta(),
            _ => NegocioUsuarios.ObtenerTodosOrdenados()
        };

        // Cargar ListBox
        lstUsuarios.DataSource = null;
        lstUsuarios.DataSource = usuarios;
        lstUsuarios.DisplayMember = "NombreCompleto";

        // Cargar BindingSource para navegador
        bsUsuarios.DataSource = new BindingList<Usuario>(usuarios);

        if (usuarios.Count > 0)
        {
            lstUsuarios.SelectedIndex = 0;
        }
    }

    private void CboOrden_SelectedIndexChanged(object? sender, EventArgs e)
    {
        CargarDatos();
    }

    private void BtnBuscar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBuscar.Text))
        {
            CargarDatos();
            return;
        }

        var usuarios = NegocioUsuarios.BuscarPorNombre(txtBuscar.Text);
        lstUsuarios.DataSource = null;
        lstUsuarios.DataSource = usuarios;
        lstUsuarios.DisplayMember = "NombreCompleto";
        bsUsuarios.DataSource = new BindingList<Usuario>(usuarios);
    }

    private void LstUsuarios_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (lstUsuarios.SelectedItem is Usuario usuario)
        {
            // Sincronizar con BindingSource
            bsUsuarios.Position = lstUsuarios.SelectedIndex;
            MostrarDetalle(usuario);
        }
    }

    private void BsUsuarios_CurrentChanged(object? sender, EventArgs e)
    {
        if (bsUsuarios.Current is Usuario usuario)
        {
            // Sincronizar ListBox con navigator
            if (lstUsuarios.SelectedIndex != bsUsuarios.Position)
            {
                lstUsuarios.SelectedIndex = bsUsuarios.Position;
            }
            MostrarDetalle(usuario);
        }
    }

    private void MostrarDetalle(Usuario usuario)
    {
        txtDetalleDNI.Text = usuario.DNI;
        txtDetalleNombre.Text = usuario.NombreCompleto;
        txtDetalleEmail.Text = usuario.Email;
        txtDetalleTelefono.Text = usuario.Telefono;
        txtDetalleFechaAlta.Text = usuario.FechaAlta.ToString("dd/MM/yyyy");

        if (usuario.Sancionado && usuario.FechaFinSancion > DateTime.Now)
        {
            txtDetalleEstado.Text = $"Sancionado hasta {usuario.FechaFinSancion:dd/MM/yyyy}";
            txtDetalleEstado.BackColor = Color.LightCoral;
        }
        else
        {
            txtDetalleEstado.Text = "Activo";
            txtDetalleEstado.BackColor = Color.LightGreen;
        }
    }

    private void BtnEditar_Click(object? sender, EventArgs e)
    {
        if (bsUsuarios.Current is Usuario usuario)
        {
            var form = new FormGestionUsuario(usuario, true);
            form.MdiParent = MdiParent;
            form.FormClosed += (s, ev) => CargarDatos();
            form.Show();
        }
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (bsUsuarios.Current is Usuario usuario)
        {
            if (MessageBox.Show($"¿Está seguro de eliminar al usuario {usuario.NombreCompleto}?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var resultado = NegocioUsuarios.BajaUsuario(usuario.DNI);
                MessageBox.Show(resultado.mensaje,
                    resultado.exito ? "Éxito" : "Error",
                    MessageBoxButtons.OK,
                    resultado.exito ? MessageBoxIcon.Information : MessageBoxIcon.Error);

                if (resultado.exito)
                {
                    CargarDatos();
                }
            }
        }
    }
}
