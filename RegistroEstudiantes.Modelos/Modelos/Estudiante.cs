using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroEstudiantes.Modelos.Modelos
{
    public class Estudiante
    {
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string? CorreoElectronico { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? Nivel { get; set; }
        public string? Curso { get; set; }
        public bool Estado { get; set; }
        public string NombreCompleto => $"{PrimerNombre} {PrimerApellido}";
        public string CursoCompleto => $"{Curso} - {Nivel}";
    }
}
