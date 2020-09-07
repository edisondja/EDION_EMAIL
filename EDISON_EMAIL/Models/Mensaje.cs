using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDISON_EMAIL.Models
{
    public class Mensaje
    {
        public int id_mensaje { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public string fecha_creacion { get; set; }
        public string tipo_de_correo { get; set; }

        public string correo_emisor { get; set; }

        public string nombre { get; set; }

        public string apellido { get; set; }

        public string pais { get; set; }

        public string correo_resector { get; set; }

        public string ruta_archivo { get; set; }


    }
}
