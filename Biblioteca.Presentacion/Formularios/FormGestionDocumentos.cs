using Biblioteca.Dominio;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para alta y modificación de documentos.
/// </summary>
public class FormGestionDocumentos : Form
{
    public enum Modo { Alta, Modificar }

    private Modo _modo;
    private Empleado _empleado;
    private Label lblISBN;
    private Label lblTitulo;
    private Label lblAutor;
    private Label lblEditorial;
    private Label lblGenero;
    private Label lblAnio;
    private Label lblTipo;
    private TextBox txtISBN;
    private TextBox txtTitulo;
    private TextBox txtAutor;
    private TextBox txtEditorial;
    private TextBox txtGenero;
    private NumericUpDown nudAnio;
    private ComboBox cmbTipo;
    private GroupBox grpLibro;
    private GroupBox grpAudiolibro;
    private NumericUpDown nudPaginas;
    private NumericUpDown nudDuracion;
    private TextBox txtNarrador;
    private ComboBox cmbFormato;
    private Button btnGuardar;
    private Button btnCancelar;
    private ErrorProvider errorProvider;

    public FormGestionDocumentos(Modo modo, Empleado empleado)
    {
        _modo = modo;
        _empleado = empleado;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = _modo == Modo.Alta ? "Alta de Documento" : "Modificar Documento";
        Size = new Size(500, 550);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        int y = 20;
        int labelX = 20;
        int controlX = 120;
        int spacing = 35;

        // Tipo de documento
        lblTipo = new Label { Text = "Tipo:", Location = new Point(labelX, y), AutoSize = true };
        cmbTipo = new ComboBox { Location = new Point(controlX, y - 3), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipo.Items.AddRange(new[] { "Libro", "Audiolibro" });
        cmbTipo.SelectedIndex = 0;
        cmbTipo.SelectedIndexChanged += CmbTipo_SelectedIndexChanged;
        y += spacing;

        // ISBN
        lblISBN = new Label { Text = "ISBN:", Location = new Point(labelX, y), AutoSize = true };
        txtISBN = new TextBox { Location = new Point(controlX, y - 3), Width = 200 };
        y += spacing;

        // Título
        lblTitulo = new Label { Text = "Título:", Location = new Point(labelX, y), AutoSize = true };
        txtTitulo = new TextBox { Location = new Point(controlX, y - 3), Width = 300 };
        y += spacing;

        // Autor
        lblAutor = new Label { Text = "Autor:", Location = new Point(labelX, y), AutoSize = true };
        txtAutor = new TextBox { Location = new Point(controlX, y - 3), Width = 300 };
        y += spacing;

        // Editorial
        lblEditorial = new Label { Text = "Editorial:", Location = new Point(labelX, y), AutoSize = true };
        txtEditorial = new TextBox { Location = new Point(controlX, y - 3), Width = 200 };
        y += spacing;

        // Género
        lblGenero = new Label { Text = "Género:", Location = new Point(labelX, y), AutoSize = true };
        txtGenero = new TextBox { Location = new Point(controlX, y - 3), Width = 150 };
        y += spacing;

        // Año
        lblAnio = new Label { Text = "Año:", Location = new Point(labelX, y), AutoSize = true };
        nudAnio = new NumericUpDown { Location = new Point(controlX, y - 3), Width = 80, Minimum = 1000, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };
        y += spacing + 10;

        // Grupo Libro
        grpLibro = new GroupBox { Text = "Datos del Libro", Location = new Point(20, y), Size = new Size(440, 60) };
        grpLibro.Controls.Add(new Label { Text = "Páginas:", Location = new Point(15, 25), AutoSize = true });
        nudPaginas = new NumericUpDown { Location = new Point(100, 22), Width = 80, Minimum = 1, Maximum = 10000, Value = 100 };
        grpLibro.Controls.Add(nudPaginas);

        // Grupo Audiolibro
        grpAudiolibro = new GroupBox { Text = "Datos del Audiolibro", Location = new Point(20, y), Size = new Size(440, 90), Visible = false };
        grpAudiolibro.Controls.Add(new Label { Text = "Duración (min):", Location = new Point(15, 25), AutoSize = true });
        nudDuracion = new NumericUpDown { Location = new Point(120, 22), Width = 80, Minimum = 1, Maximum = 100000, Value = 60 };
        grpAudiolibro.Controls.Add(nudDuracion);
        grpAudiolibro.Controls.Add(new Label { Text = "Narrador:", Location = new Point(15, 55), AutoSize = true });
        txtNarrador = new TextBox { Location = new Point(120, 52), Width = 150 };
        grpAudiolibro.Controls.Add(txtNarrador);
        grpAudiolibro.Controls.Add(new Label { Text = "Formato:", Location = new Point(290, 55), AutoSize = true });
        cmbFormato = new ComboBox { Location = new Point(350, 52), Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbFormato.Items.AddRange(Enum.GetNames(typeof(FormatoAudio)));
        cmbFormato.SelectedIndex = 0;
        grpAudiolibro.Controls.Add(cmbFormato);

        y += 100;

        // Botones
        btnGuardar = new Button { Text = "Guardar", Location = new Point(150, y + 10), Size = new Size(90, 30) };
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar = new Button { Text = "Cancelar", Location = new Point(260, y + 10), Size = new Size(90, 30) };
        btnCancelar.Click += (s, e) => Close();

        Controls.AddRange(new Control[] { lblTipo, cmbTipo, lblISBN, txtISBN, lblTitulo, txtTitulo,
            lblAutor, txtAutor, lblEditorial, txtEditorial, lblGenero, txtGenero, lblAnio, nudAnio,
            grpLibro, grpAudiolibro, btnGuardar, btnCancelar });
    }

    private void CmbTipo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        grpLibro.Visible = cmbTipo.SelectedIndex == 0;
        grpAudiolibro.Visible = cmbTipo.SelectedIndex == 1;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        errorProvider.Clear();

        if (string.IsNullOrWhiteSpace(txtISBN.Text))
        {
            errorProvider.SetError(txtISBN, "El ISBN es obligatorio.");
            return;
        }

        if (!NegocioDocumentos.ValidarFormatoISBN(txtISBN.Text.Trim()))
        {
            errorProvider.SetError(txtISBN, "El formato del ISBN no es válido (10 o 13 dígitos).");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtTitulo.Text))
        {
            errorProvider.SetError(txtTitulo, "El título es obligatorio.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtAutor.Text))
        {
            errorProvider.SetError(txtAutor, "El autor es obligatorio.");
            return;
        }

        Documento documento;
        if (cmbTipo.SelectedIndex == 0)
        {
            documento = new Libro
            {
                Codigo = txtISBN.Text.Trim(),
                Titulo = txtTitulo.Text.Trim(),
                Autor = txtAutor.Text.Trim(),
                Editorial = txtEditorial.Text.Trim(),
                Genero = txtGenero.Text.Trim(),
                AnioPublicacion = (int)nudAnio.Value,
                NumeroPaginas = (int)nudPaginas.Value
            };
        }
        else
        {
            documento = new Audiolibro
            {
                Codigo = txtISBN.Text.Trim(),
                Titulo = txtTitulo.Text.Trim(),
                Autor = txtAutor.Text.Trim(),
                Editorial = txtEditorial.Text.Trim(),
                Genero = txtGenero.Text.Trim(),
                AnioPublicacion = (int)nudAnio.Value,
                DuracionMinutos = (int)nudDuracion.Value,
                Narrador = txtNarrador.Text.Trim(),
                Formato = Enum.Parse<FormatoAudio>(cmbFormato.SelectedItem?.ToString() ?? "MP3")
            };
        }

        var (exito, mensaje) = _modo == Modo.Alta
            ? NegocioDocumentos.AltaDocumento(documento, _empleado.DNI)
            : NegocioDocumentos.ModificarDocumento(documento);

        if (exito)
        {
            MessageBox.Show(mensaje, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (_modo == Modo.Alta) LimpiarFormulario();
        }
        else
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LimpiarFormulario()
    {
        txtISBN.Clear();
        txtTitulo.Clear();
        txtAutor.Clear();
        txtEditorial.Clear();
        txtGenero.Clear();
        nudAnio.Value = DateTime.Now.Year;
        nudPaginas.Value = 100;
        nudDuracion.Value = 60;
        txtNarrador.Clear();
        txtISBN.Focus();
    }
}
