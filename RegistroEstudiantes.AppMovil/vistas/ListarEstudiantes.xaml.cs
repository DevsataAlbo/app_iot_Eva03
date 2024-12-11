using System.Collections.ObjectModel;
using Firebase.Database;
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
        // Con esto obtengo los datos cargados en la BD al inicio de la app
        var estudiantes = await client.Child("Estudiantes").OnceAsync<Estudiante>();
        foreach (var estudiante in estudiantes)
        {
            if (estudiante.Object.Estado)
            {
                Lista.Add(estudiante.Object);
                ListaCompleta.Add(estudiante.Object);
            }
        }

        // Con esto cargo los cambios que se realicen en el listado, carga de nuevos alumnos
        client.Child("Estudiantes").AsObservable<Estudiante>().Subscribe((estudiante) =>
        {
            if (estudiante != null && estudiante.Object.Estado)
            {
                Lista.Add(estudiante.Object);
                ListaCompleta.Add(estudiante.Object);
            }
        });
    }


    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if(filtro.Length > 0)
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
}