using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using RegistroEstudiantes.Modelos.Modelos;

namespace RegistroEstudiantes.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            Registrar();
            return builder.Build();
        }

        public static async void Registrar()
        {
            FirebaseClient client = new FirebaseClient("https://registroestudiantes-b3ffb-default-rtdb.firebaseio.com/");

            var nivel = await client.Child("Nivel").OnceAsync<Nivel>();
            if (nivel.Count == 0)
            {
                await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Basica" });
                await client.Child("Nivel").PostAsync(new Nivel { Nombre = "Media" });
            }

            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            if (cursos.Count == 0)
            {
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "4°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "5°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "6°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "7°", Nivel = "Basica" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "8°", Nivel = "Basica" });

                await client.Child("Cursos").PostAsync(new Curso { Nombre = "1°", Nivel = "Media" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "2°", Nivel = "Media" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "3°", Nivel = "Media" });
                await client.Child("Cursos").PostAsync(new Curso { Nombre = "4°", Nivel = "Media" });
            }
        }
    }
}
