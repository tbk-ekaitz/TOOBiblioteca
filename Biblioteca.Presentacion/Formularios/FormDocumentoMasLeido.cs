using Biblioteca.Negocio;

namespace Biblioteca.Presentacion.Formularios;

/// <summary>
/// Formulario para consultar el documento más leído.
/// </summary>
public class FormDocumentoMasLeido : Form
{
    private GroupBox grpGeneral;
    private Label lblMasLeidoGeneral;
    private TextBox txtMasLeidoGeneral;
    private TextBox txtAutorGeneral;

    private GroupBox grpMes;
    private Label lblMes;
    private ComboBox cmbMes;
    private Label lblAnio;
    private NumericUpDown nudAnio;
    private Button btnConsultar;
    private Label lblMasLeidoMes;
    private TextBox txtMasLeidoMes;
    private TextBox txtAutorMes;

    private GroupBox grpTop;
    private DataGridView dgvTop;

    public FormDocumentoMasLeido()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void InitializeComponent()
    {
        Text = "Documento Más Leído";
        Size = new Size(550, 500);
        StartPosition = FormStartPosition.CenterParent;

        // Grupo General
        grpGeneral = new GroupBox { Text = "Más Leído (Histórico)", Location = new Point(20, 20), Size = new Size(495, 90) };

        lblMasLeidoGeneral = new Label { Text = "Título:", Location = new Point(15, 30), AutoSize = true };
        txtMasLeidoGeneral = new TextBox { Location = new Point(80, 27), Width = 350, ReadOnly = true };

        grpGeneral.Controls.Add(new Label { Text = "Autor:", Location = new Point(15, 57), AutoSize = true });
        txtAutorGeneral = new TextBox { Location = new Point(80, 54), Width = 250, ReadOnly = true };

        grpGeneral.Controls.AddRange(new Control[] { lblMasLeidoGeneral, txtMasLeidoGeneral, txtAutorGeneral });

        // Grupo Mes
        grpMes = new GroupBox { Text = "Más Leído por Mes", Location = new Point(20, 120), Size = new Size(495, 120) };

        lblMes = new Label { Text = "Mes:", Location = new Point(15, 30), AutoSize = true };
        cmbMes = new ComboBox { Location = new Point(55, 27), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbMes.Items.AddRange(new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" });
        cmbMes.SelectedIndex = DateTime.Now.Month - 1;

        lblAnio = new Label { Text = "Año:", Location = new Point(200, 30), AutoSize = true };
        nudAnio = new NumericUpDown { Location = new Point(240, 27), Width = 80, Minimum = 2000, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };

        btnConsultar = new Button { Text = "Consultar", Location = new Point(340, 25), Size = new Size(100, 25) };
        btnConsultar.Click += BtnConsultar_Click;

        lblMasLeidoMes = new Label { Text = "Título:", Location = new Point(15, 65), AutoSize = true };
        txtMasLeidoMes = new TextBox { Location = new Point(80, 62), Width = 350, ReadOnly = true };

        grpMes.Controls.Add(new Label { Text = "Autor:", Location = new Point(15, 92), AutoSize = true });
        txtAutorMes = new TextBox { Location = new Point(80, 89), Width = 250, ReadOnly = true };

        grpMes.Controls.AddRange(new Control[] { lblMes, cmbMes, lblAnio, nudAnio, btnConsultar, lblMasLeidoMes, txtMasLeidoMes, txtAutorMes });

        // Grupo Top 5
        grpTop = new GroupBox { Text = "Top 5 Más Prestados", Location = new Point(20, 250), Size = new Size(495, 190) };

        dgvTop = new DataGridView
        {
            Location = new Point(15, 25),
            Size = new Size(465, 150),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        grpTop.Controls.Add(dgvTop);

        Controls.AddRange(new Control[] { grpGeneral, grpMes, grpTop });
    }

    private void CargarDatos()
    {
        // Más leído histórico
        var masLeidoGeneral = NegocioDocumentos.ObtenerMasLeido();
        if (masLeidoGeneral != null)
        {
            txtMasLeidoGeneral.Text = masLeidoGeneral.Titulo;
            txtAutorGeneral.Text = masLeidoGeneral.Autor;
        }
        else
        {
            txtMasLeidoGeneral.Text = "Sin datos";
            txtAutorGeneral.Text = "-";
        }

        // Más leído mes actual
        var masLeidoMes = NegocioDocumentos.ObtenerMasLeidoMes();
        if (masLeidoMes != null)
        {
            txtMasLeidoMes.Text = masLeidoMes.Titulo;
            txtAutorMes.Text = masLeidoMes.Autor;
        }
        else
        {
            txtMasLeidoMes.Text = "Sin préstamos este mes";
            txtAutorMes.Text = "-";
        }

        // Top 5
        var top5 = NegocioDocumentos.ObtenerTopPrestados(5);
        dgvTop.DataSource = top5.Select((t, i) => new
        {
            Posicion = i + 1,
            Titulo = t.Documento.Titulo,
            Autor = t.Documento.Autor,
            Tipo = t.Documento.TipoDocumento,
            VecesPrestado = t.VecesPrestado
        }).ToList();
    }

    private void BtnConsultar_Click(object? sender, EventArgs e)
    {
        int mes = cmbMes.SelectedIndex + 1;
        int anio = (int)nudAnio.Value;

        var masLeido = NegocioDocumentos.ObtenerMasLeidoMes(mes, anio);
        if (masLeido != null)
        {
            txtMasLeidoMes.Text = masLeido.Titulo;
            txtAutorMes.Text = masLeido.Autor;
        }
        else
        {
            txtMasLeidoMes.Text = $"Sin préstamos en {cmbMes.SelectedItem} {anio}";
            txtAutorMes.Text = "-";
        }
    }
}
