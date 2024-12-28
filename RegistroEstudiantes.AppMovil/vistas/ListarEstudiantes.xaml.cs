using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.Arm;
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil.vistas;

public partial class ListarEstudiantes : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroestudiantes-b3ffb-default-rtdb.firebaseio.com/");
    public ObservableCollection<Estudiante> Lista { get; set; } = new ObservableCollection<Estudiante>();
    private List<Estudiante> ListaCompleta { get; set; } = new List<Estudiante>();

    public ListarEstudiantes()
    {
        InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    private async void CargarLista()
    {
        // para limpiar las listas y evitar duplicados en recargas
        Lista.Clear();
        ListaCompleta.Clear();

        // Para mostrar todos los estudiantes de Firebase
        var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();

        // para recorrer la lista de estudintes y solo agregar los estudiantes activos a ambas listas
        foreach (var estudiante in estudiantes)
        {
            if (estudiante.Object.Estado)
            {
                Lista.Add(estudiante.Object);
                ListaCompleta.Add(estudiante.Object);
            }
        }

        // Para poder reflejar los cambios en el incio, esto lee constantemente los datos en la BD
        client.Child("Estudiantes").AsObservable<Estudiante>().Subscribe((cambio) =>
        {
            if (cambio?.Object != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Buscar si el estudiante ya existe en la lista
                    var existente = Lista.FirstOrDefault(e =>
                        e.CorreoElectronico == cambio.Object.CorreoElectronico);
                    // Si existe se elimina para actualizarlo
                    if (existente != null)
                    {
                        Lista.Remove(existente);
                        ListaCompleta.Remove(existente);
                    }

                    // Solo agrega al estudiete si 'este está activo
                    if (cambio.Object.Estado)
                    {
                        Lista.Add(cambio.Object);
                        ListaCompleta.Add(cambio.Object);
                    }
                });
            }
        });
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text?.ToLower() ?? "";

        if (filtro.Length > 0)
        {
            listaCollection.ItemsSource = ListaCompleta
                .Where(x => x.Estado && x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = ListaCompleta.Where(x => x.Estado);
        }
    }

    private async void NuevoEstudianteBtn_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearEstudiante());
    }

    private async void EditarEstudiante_Clicked(object sender, EventArgs e)
    {
        // Eto reconoce al estudiante que se quiere editar
        var button = sender as Button;
        var estudiante = button?.BindingContext as Estudiante;

        if (estudiante != null)
        {
            // Busca el ID del estudiante en Firebase usando su correo
            var estudianteFirebase = await client.Child("Estudiantes")
                .OnceAsync<Estudiante>();
            var estudianteId = estudianteFirebase
                .Where(x => x.Object.CorreoElectronico == estudiante.CorreoElectronico)
                .FirstOrDefault()?.Key;

            if (estudianteId != null)
            {
                // Navega a la página de edición con el ID y datos del estudiante
                await Navigation.PushAsync(new EditarEstudiante(estudianteId, estudiante));
            }
        }
    }

    private async void DeshabilitarEstudiante_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var estudiante = button?.BindingContext as Estudiante;
        if (estudiante != null)
        {
            bool confirmar = await DisplayAlert("Confirmar",
                "¿Está seguro que desea deshabilitar este estudiante?",
                "Sí", "No");

            if (confirmar)
            {
                try
                {
                    // Busca el ID del estudiante en Firebase usando su correo
                    var estudianteFirebase = await client.Child("Estudiantes")
                        .OnceAsync<Estudiante>();
                    var estudianteId = estudianteFirebase
                        .Where(x => x.Object.CorreoElectronico == estudiante.CorreoElectronico)
                        .FirstOrDefault()?.Key;

                    if (estudianteId != null)
                    {
                        // Cambia el estado a false y actualiza en Firebase
                        estudiante.Estado = false;
                        await client.Child("Estudiantes")
                            .Child(estudianteId)
                            .PutAsync(estudiante);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }
    }
}