using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil.vistas;

public partial class CrearEstudiante : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-b3ffb-default-rtdb.firebaseio.com/");
    public List<Nivel>Niveles {  get; set; }
    public List<Curso>Cursos { get; set; }

    public CrearEstudiante()
    {
        InitializeComponent();
        CargarDatos();
        BindingContext = this;
    }

    private async void CargarDatos()
    {
        await ListarNiveles();
        await ListarCursos();
    }

    private async Task ListarCursos()
    {
        var cursos = await client.Child("Cursos").OnceAsync<Curso>();
        Cursos = cursos.Select(x => x.Object).ToList();
        OnPropertyChanged(nameof(Cursos));
    }

    private async Task ListarNiveles()
    {
        var niveles = await client.Child("Nivel").OnceAsync<Nivel>();
        Niveles = niveles.Select(x => x.Object).ToList();
        OnPropertyChanged(nameof(Niveles));

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

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {

        if (nivelPicker.SelectedItem is not Nivel nivelSeleccionado ||
            cursoPicker.SelectedItem is not Curso cursoSeleccionado)
        {
            await DisplayAlert("Error", "Debe seleccionar nivel y curso", "OK");
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
            await client.Child("Estudiantes").PostAsync(estudiante);
            await DisplayAlert("Éxito", "Estudiante guardado correctamente", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }
}