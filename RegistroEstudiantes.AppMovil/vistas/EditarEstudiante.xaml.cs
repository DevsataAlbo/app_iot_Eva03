using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil.vistas;

public partial class EditarEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-b3ffb-default-rtdb.firebaseio.com/");
    public List<Nivel> Niveles { get; set; }
    public List<Curso> Cursos { get; set; }
    private string estudianteId;
    private Estudiante estudianteActual;

    public EditarEstudiante(string id, Estudiante estudiante)
    {
        InitializeComponent();
        estudianteId = id;
        estudianteActual = estudiante;
        BindingContext = this;
        InicializarDatos();
    }

    private async void InicializarDatos()
    {
        await CargarDatos();   // Aca espera a que se carguen niveles y cursos
        CargarEstudiante();    // Y luego carga los datos del estudiante
    }

    private async Task CargarDatos()
    {
        try
        {
            await ListarNiveles();  // Para cargar primero los niveles
            await ListarCursos();   // Para cargar luego los cursos segun el nivel seleccionado
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al cargar los datos: " + ex.Message, "OK");
        }
    }

    private void CargarEstudiante()
    {
        try
        {
            if (Niveles == null || !Niveles.Any())
            {
                throw new Exception("No se han cargado los niveles");
            }

            if (Cursos == null || !Cursos.Any())
            {
                throw new Exception("No se han cargado los cursos");
            }

            primerNombreEntry.Text = estudianteActual.PrimerNombre;
            segundoNombreEntry.Text = estudianteActual.SegundoNombre;
            primerApellidoEntry.Text = estudianteActual.PrimerApellido;
            segundoApellidoEntry.Text = estudianteActual.SegundoApellido;
            correoEntry.Text = estudianteActual.CorreoElectronico;
            fechaNacimientoPicker.Date = estudianteActual.FechaNacimiento;
            estadoCheckBox.IsChecked = estudianteActual.Estado;

            var nivelActual = Niveles.FirstOrDefault(n => n.Nombre == estudianteActual.Nivel);
            if (nivelActual != null)
            {
                nivelPicker.SelectedItem = nivelActual;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(200); // Esto lo encontre en StackOverflow, tenia problema con la carga de datos aunque fueran asincronos, aca le agregue una pausa y aparentemente funciona bien :)

                var cursosFiltrados = Cursos.Where(c => c.Nivel == estudianteActual.Nivel).ToList();
                cursoPicker.ItemsSource = cursosFiltrados;
                cursoPicker.IsEnabled = true;

                var cursoActual = cursosFiltrados.FirstOrDefault(c =>
                    c.Nombre == estudianteActual.Curso &&
                    c.Nivel == estudianteActual.Nivel);

                if (cursoActual != null)
                {
                    cursoPicker.SelectedItem = cursoActual;
                }
            });
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Error", "Error al cargar estudiante: " + ex.Message, "OK");
            });
        }
    }

    private async Task ListarNiveles()
    {
        // Con esto obtengo los niveles de Firebase
        var niveles = await client.Child("Nivel").OnceAsync<Nivel>();
        // Convierto los datos en una lista
        Niveles = niveles.Select(x => x.Object).ToList();
        // Con esto notifico que hubo un cambio 
        OnPropertyChanged(nameof(Niveles));
    }

    private async Task ListarCursos()
    {
        // Esto es lo mismo que para listar niveles pero con los cursos
        var cursos = await client.Child("Cursos").OnceAsync<Curso>();
        Cursos = cursos.Select(x => x.Object).ToList();
        OnPropertyChanged(nameof(Cursos));
    }

    private async void nivelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (nivelPicker.SelectedItem is Nivel nivelSeleccionado)
        {
            var cursosFiltrados = Cursos.Where(c => c.Nivel == nivelSeleccionado.Nombre).ToList();
            cursoPicker.ItemsSource = cursosFiltrados;
            cursoPicker.IsEnabled = true;
        }
    }

    private async void actualizarButton_Clicked(object sender, EventArgs e)
    {
        if (nivelPicker.SelectedItem is not Nivel nivelSeleccionado ||
            cursoPicker.SelectedItem is not Curso cursoSeleccionado)
        {
            await DisplayAlert("Error", "Debe seleccionar el nivel y curso", "OK");
            return;
        }

        var estudiante = new Estudiante
        {
            PrimerNombre = primerNombreEntry.Text,
            SegundoNombre = segundoNombreEntry.Text,
            PrimerApellido = primerApellidoEntry.Text,
            SegundoApellido = segundoApellidoEntry.Text,
            CorreoElectronico = correoEntry.Text,
            FechaNacimiento = fechaNacimientoPicker.Date,
            Nivel = nivelSeleccionado.Nombre,
            Curso = cursoSeleccionado.Nombre,
            Estado = estadoCheckBox.IsChecked
        };

        try
        {
            await client.Child("Estudiantes").Child(estudianteId).PutAsync(estudiante);
            await DisplayAlert("Éxito", "Estudiante actualizado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}