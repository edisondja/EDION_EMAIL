using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EDISON_EMAIL.Models
{
    public class Correo
    {
       public string nombre { get; set; }
       public string apellido { get; set; }
       public string nombre_correo { get; set; }
       public DateTime fecha_creacion { get; set; }
        
       public string pais { get; set; }
       public DateTime edad { get; set; }
       
       public string clave { get; set; }
       
       public int id_correo { get; set; }
        

    }
}
