using Biblioteca.Presentacion.Formularios;

namespace Biblioteca.Presentacion;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Mostrar formulario de login primero
        using (var formLogin = new FormLogin())
        {
            if (formLogin.ShowDialog() == DialogResult.OK && formLogin.EmpleadoLogueado != null)
            {
                // Si el login es exitoso, abrir el formulario principal
                Application.Run(new FormPrincipal(formLogin.EmpleadoLogueado));
            }
        }
    }
}
