using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para Alta de Préstamo (Maestro-Detalle).
/// Selección de usuario + añadir múltiples ejemplares.
/// Implementa la Práctica 13 (relación maestro-detalle).
/// </summary>
public class FormAltaPrestamo : Form
{
    // Sección Usuario (Maestro)
    private GroupBox grpUsuario;
    private ComboBox cboUsuario;
    private Label lblInfoUsuario;
    private Label lblEstadoUsuario;

    // Sección Ejemplares (Detalle)
    private GroupBox grpEjemplares;
    private TextBox txtCodigoBarras;
    private Button btnAgregar;
    private ListBox lstEjemplares;
    private Button btnQuitar;
    private Label lblTotalEjemplares;

    // Sección Resumen
    private GroupBox grpResumen;
    private Label lblFechaPrestamo;
    private Label lblFechaDevolucion;

    // Botones
    private Button btnConfirmar;
    private Button btnCancelar;

    private List<Ejemplar> _ejemplaresSeleccionados = new();
    private Usuario? _usuarioSeleccionado;

    public FormAltaPrestamo()
    {
        InitializeComponent();
        CargarUsuarios();
    }

    private void InitializeComponent()
    {
        Text = "Nuevo Préstamo";
        Size = new Size(650, 550);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // === SECCIÓN USUARIO (MAESTRO) ===
        grpUsuario = new GroupBox
        {
            Text = "1. Seleccionar Usuario",
            Location = new Point(10, 10),
            Size = new Size(610, 100)
        };

        var lblUsuario = new Label
        {
            Text = "Usuario:",
            Location = new Point(15, 30),
            AutoSize = true
        };

        cboUsuario = new ComboBox
        {
            Location = new Point(75, 27),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cboUsuario.SelectedIndexChanged += CboUsuario_SelectedIndexChanged;

        lblInfoUsuario = new Label
        {
            Location = new Point(15, 60),
            Size = new Size(400, 20),
            ForeColor = Color.Gray
        };

        lblEstadoUsuario = new Label
        {
            Location = new Point(430, 27),
            Size = new Size(170, 50),
            TextAlign = ContentAlignment.MiddleCenter
        };

        grpUsuario.Controls.AddRange(new Control[]
        {
            lblUsuario, cboUsuario, lblInfoUsuario, lblEstadoUsuario
        });

        // === SECCIÓN EJEMPLARES (DETALLE) ===
        grpEjemplares = new GroupBox
        {
            Text = "2. Añadir Ejemplares",
            Location = new Point(10, 120),
            Size = new Size(610, 250)
        };

        var lblCodigo = new Label
        {
            Text = "Código de Barras:",
            Location = new Point(15, 30),
            AutoSize = true
        };

        txtCodigoBarras = new TextBox
        {
            Location = new Point(130, 27),
            Width = 200,
            CharacterCasing = CharacterCasing.Upper
        };
        txtCodigoBarras.KeyPress += TxtCodigoBarras_KeyPress;

        btnAgregar = new Button
        {
            Text = "Agregar",
            Location = new Point(340, 25),
            Size = new Size(80, 25)
        };
        btnAgregar.Click += BtnAgregar_Click;

        lstEjemplares = new ListBox
        {
            Location = new Point(15, 65),
            Size = new Size(500, 140)
        };

        btnQuitar = new Button
        {
            Text = "Quitar",
            Location = new Point(520, 65),
            Size = new Size(75, 25)
        };
        btnQuitar.Click += BtnQuitar_Click;

        lblTotalEjemplares = new Label
        {
            Text = "Total: 0 ejemplar(es)",
            Location = new Point(15, 215),
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        grpEjemplares.Controls.AddRange(new Control[]
        {
            lblCodigo, txtCodigoBarras, btnAgregar, lstEjemplares, btnQuitar, lblTotalEjemplares
        });

        // === SECCIÓN RESUMEN ===
        grpResumen = new GroupBox
        {
            Text = "3. Resumen del Préstamo",
            Location = new Point(10, 380),
            Size = new Size(610, 70)
        };

        lblFechaPrestamo = new Label
        {
            Text = $"Fecha de préstamo: {DateTime.Now:dd/MM/yyyy HH:mm}",
            Location = new Point(15, 25),
            AutoSize = true
        };

        lblFechaDevolucion = new Label
        {
            Text = "Fecha de devolución: --",
            Location = new Point(15, 45),
            AutoSize = true,
            ForeColor = Color.Blue
        };

        grpResumen.Controls.AddRange(new Control[] { lblFechaPrestamo, lblFechaDevolucion });

        // === BOTONES ===
        btnConfirmar = new Button
        {
            Text = "Confirmar Préstamo",
            Location = new Point(360, 460),
            Size = new Size(130, 35),
            Enabled = false
        };
        btnConfirmar.Click += BtnConfirmar_Click;

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(500, 460),
            Size = new Size(100, 35)
        };
        btnCancelar.Click += (s, e) => Close();

        // Añadir controles
        Controls.AddRange(new Control[]
        {
            grpUsuario, grpEjemplares, grpResumen, btnConfirmar, btnCancelar
        });
    }

    private void CargarUsuarios()
    {
        var usuarios = NegocioUsuarios.ObtenerTodosOrdenados();
        cboUsuario.DataSource = usuarios;
        cboUsuario.DisplayMember = "NombreCompleto";
        cboUsuario.SelectedIndex = -1;
    }

    private void CboUsuario_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cboUsuario.SelectedItem is Usuario usuario)
        {
            _usuarioSeleccionado = usuario;

            // Mostrar info
            lblInfoUsuario.Text = $"DNI: {usuario.DNI} | Alta: {usuario.FechaAlta:dd/MM/yyyy}";

            // Verificar si puede pedir préstamos
            var (puede, motivo) = NegocioUsuarios.PuedeRealizarPrestamo(usuario.Id);

            if (puede)
            {
                lblEstadoUsuario.Text = "✓ Puede solicitar préstamos";
                lblEstadoUsuario.ForeColor = Color.Green;
                grpEjemplares.Enabled = true;
            }
            else
            {
                lblEstadoUsuario.Text = $"✗ {motivo}";
                lblEstadoUsuario.ForeColor = Color.Red;
                grpEjemplares.Enabled = false;
            }

            ActualizarEstadoBoton();
        }
    }

    private void TxtCodigoBarras_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            BtnAgregar_Click(sender, e);
            e.Handled = true;
        }
    }

    private void BtnAgregar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCodigoBarras.Text))
        {
            MessageBox.Show("Introduzca un código de barras.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Validar ejemplar
        var (puede, motivo) = NegocioPrestamos.ValidarEjemplar(txtCodigoBarras.Text);

        if (!puede)
        {
            MessageBox.Show(motivo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtCodigoBarras.SelectAll();
            txtCodigoBarras.Focus();
            return;
        }

        var ejemplar = NegocioDocumentos.BuscarEjemplarPorCodigoBarras(txtCodigoBarras.Text);

        if (ejemplar == null)
        {
            MessageBox.Show("Ejemplar no encontrado.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Verificar duplicados
        if (_ejemplaresSeleccionados.Any(e => e.Id == ejemplar.Id))
        {
            MessageBox.Show("Este ejemplar ya está en la lista.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCodigoBarras.Clear();
            txtCodigoBarras.Focus();
            return;
        }

        // Añadir a la lista
        _ejemplaresSeleccionados.Add(ejemplar);
        lstEjemplares.Items.Add($"{ejemplar.CodigoBarras} - {ejemplar.Documento?.Titulo ?? "Sin título"}");

        // Actualizar UI
        txtCodigoBarras.Clear();
        txtCodigoBarras.Focus();
        ActualizarResumen();
        ActualizarEstadoBoton();
    }

    private void BtnQuitar_Click(object? sender, EventArgs e)
    {
        if (lstEjemplares.SelectedIndex >= 0)
        {
            int index = lstEjemplares.SelectedIndex;
            _ejemplaresSeleccionados.RemoveAt(index);
            lstEjemplares.Items.RemoveAt(index);
            ActualizarResumen();
            ActualizarEstadoBoton();
        }
    }

    private void ActualizarResumen()
    {
        lblTotalEjemplares.Text = $"Total: {_ejemplaresSeleccionados.Count} ejemplar(es)";

        if (_ejemplaresSeleccionados.Count > 0)
        {
            // Calcular fecha de devolución
            int maxDias = _ejemplaresSeleccionados
                .Where(e => e.Documento != null)
                .Select(e => e.Documento!.GetDiasPrestamo())
                .DefaultIfEmpty(14)
                .Max();

            var fechaDevolucion = DateTime.Now.AddDays(maxDias);
            lblFechaDevolucion.Text = $"Fecha de devolución: {fechaDevolucion:dd/MM/yyyy}";
        }
        else
        {
            lblFechaDevolucion.Text = "Fecha de devolución: --";
        }
    }

    private void ActualizarEstadoBoton()
    {
        btnConfirmar.Enabled = _usuarioSeleccionado != null &&
                              _ejemplaresSeleccionados.Count > 0 &&
                              grpEjemplares.Enabled;
    }

    private void BtnConfirmar_Click(object? sender, EventArgs e)
    {
        if (_usuarioSeleccionado == null || _ejemplaresSeleccionados.Count == 0)
            return;

        // Obtener el empleado actual (en una implementación real vendría del contexto)
        var empleadoId = 2; // ID del empleado logueado

        // Validación final
        var validacion = NegocioPrestamos.ValidarPrestamo(_usuarioSeleccionado.Id, _ejemplaresSeleccionados);
        if (!validacion.puede)
        {
            MessageBox.Show(validacion.motivo, "Error de Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Confirmar
        var confirmacion = MessageBox.Show(
            $"¿Confirmar préstamo de {_ejemplaresSeleccionados.Count} ejemplar(es) a {_usuarioSeleccionado.NombreCompleto}?",
            "Confirmar Préstamo",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion != DialogResult.Yes)
            return;

        // Registrar préstamo
        var (exito, mensaje, prestamo) = NegocioPrestamos.RegistrarPrestamo(
            _usuarioSeleccionado.Id,
            _ejemplaresSeleccionados,
            empleadoId);

        if (exito)
        {
            MessageBox.Show(
                $"{mensaje}\n\nCódigo: {prestamo?.CodigoPrestamo}",
                "Préstamo Registrado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            Close();
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
