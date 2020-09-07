using Microsoft.AspNetCore.Mvc;
using Renci.SshNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EDISON_EMAIL.Models;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Collections;
using Microsoft.AspNetCore.Http.Features;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;

namespace EDISON_EMAIL.Controllers
{
    public class CorreoController : Controller
    {
        public static MySqlConnection Conexion()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=edison_email;";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            databaseConnection.Open();
            return databaseConnection;

        }

        public IActionResult Index()
        {
            return View("Login");
        }

  

        public IActionResult Iniciar_sesion(string correo, string clave)
        {
            var Consulta = "SELECT * FROM correos WHERE nombre_correo='" + correo + "'";
            var cmd = new MySqlCommand(Consulta, Conexion());
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            Correo CorreoUser = new Correo();

            foreach (DataRow row in dt.Rows)
            {

                CorreoUser.apellido = row.Field<string>(2);
                CorreoUser.pais = row.Field<string>(4);
                CorreoUser.nombre_correo = row.Field<string>(6);

                TempData["nombre"] = row.Field<string>(1).ToString();
                TempData["id_correo"] = row.Field<int>(0).ToString();


                return View("Dashboard");


            }

            return NotFound("Usuario y contraseñas incorrecto");


        }

        public IActionResult Registrar_correo(string nombre, string apellido, string nombre_correo, string clave, string pais)
        {

            var Consulta = "INSERT INTO correos(nombre,apellido,nombre_correo,clave,pais) " +
                "VALUES(@nombre,@apellido,@nombre_correo,@clave,@pais)";
            using var Comandos = new MySqlCommand(Consulta, Conexion());

            Comandos.Parameters.AddWithValue("@nombre", nombre);
            Comandos.Parameters.AddWithValue("@apellido", apellido);
            Comandos.Parameters.AddWithValue("@nombre_correo", nombre_correo);
            Comandos.Parameters.AddWithValue("@clave", clave);
            Comandos.Parameters.AddWithValue("@pais", pais);

            Comandos.Prepare();

            Comandos.ExecuteNonQuery();

            return View("Login");


        }
      

        public IActionResult Crear_correo()
        {

            return View("Crear_correo");
        }

        public IActionResult Correos_Spam(int id_correo)
        {
            var consulta = new MySqlCommand("SELECT  descripcion,titulo,nombre_correo,nombre FROM mensajes INNER JOIN correos ON mensajes.id_emisor=correos.id_correo where tipo_de_correo='spam' AND id_resector=@id_resector",Conexion());
            consulta.Parameters.AddWithValue("@id_resector", id_correo);
            consulta.Prepare();
            consulta.ExecuteNonQuery();
            var data = new MySqlDataAdapter(consulta);
            var dt = new DataTable();
            data.Fill(dt);
            List<Mensaje> Correos = new List<Mensaje>();
            foreach (DataRow row in dt.Rows)
            {
                Correos.Add(new Mensaje()
                {
                    descripcion = row.Field<string>(0),
                    titulo = row.Field<string>(1),
                    correo_emisor = row.Field<string>(2),
                    nombre = row.Field<string>(3)
                   

                });
            }


            return Json(Correos);
        }


        public IActionResult leer_correos(int id_correo,string config)
        {
            var Consulta = "";

            if (config == "leer_recibidos")
            {
                 Consulta = "SELECT titulo,descripcion,nombre,apellido,pais,nombre_correo,ruta_archivo FROM mensajes INNER JOIN correos ON correos.id_correo=mensajes.id_emisor WHERE id_resector='" + id_correo + "' order by id_mensaje desc";

            }
            else
            {   
                /*Cuando no este en leer recibidos la variable de configuracion se leen lo que enviaste
                    para ver los correos que has enviado etc
                */
                 Consulta = "SELECT titulo,descripcion,nombre,apellido,pais,nombre_correo,ruta_archivo FROM mensajes INNER JOIN correos ON correos.id_correo=mensajes.id_emisor WHERE id_emisor='" + id_correo + "' order by id_mensaje desc";

            }

            var cmd = new MySqlCommand(Consulta, Conexion());
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            List<Mensaje> Mensajes = new List<Mensaje>();


            foreach (DataRow row in dt.Rows)
            {

                Mensajes.Add(new Mensaje()
                {
                    titulo = row.Field<string>(0),
                    descripcion = row.Field<string>(1),
                    nombre = row.Field<string>(2),
                    apellido = row.Field<string>(3),
                    pais = row.Field<string>(4),
                    correo_emisor = row.Field<string>(5),
                    ruta_archivo = row.Field<string>(6)
                }

                  );

            }
            return Json(Mensajes);


            //return NotFound("No se pudieron leer los correos");
        }
        private readonly IHostingEnvironment hostingEnvironment;
        public CorreoController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }
        public IActionResult enviar_correos(string titulo,string descripcion,int id_emisor,string correo_resector,IFormFile file)
        {
            
            var fileName = Path.GetFileName(file.FileName);

            var archivo_ruta = Path.Combine(hostingEnvironment.WebRootPath, "Adjuntos");
            var filePath = Path.Combine(archivo_ruta,fileName);

            file.CopyTo(new FileStream(filePath, FileMode.Create));
            
            var consulta = "SELECT id_correo,pais FROM correos WHERE nombre_correo='"+correo_resector+"'";
            var comandos = new MySqlCommand(consulta,Conexion());
            var leer_correo = new MySqlDataAdapter(comandos);
            var dt = new DataTable();
            leer_correo.Fill(dt);
            int id_resector =  dt.Rows[0].Field<int>(0);
            string pais = dt.Rows[0].Field<string>(1);
            
            var sql = "INSERT INTO mensajes(titulo,descripcion,id_emisor,id_resector,tipo_de_correo,ruta_archivo) VALUES(@titulo,@texto,@id_emisor,@id_resector,@tipo_de_correo,@ruta_archivo)";
            using var cmd = new MySqlCommand(sql, Conexion());
            cmd.Parameters.AddWithValue("@titulo",titulo);
            cmd.Parameters.AddWithValue("@texto",descripcion);
            cmd.Parameters.AddWithValue("@id_emisor",id_emisor);
            cmd.Parameters.AddWithValue("@id_resector", id_resector);
            cmd.Parameters.AddWithValue("@ruta_archivo",fileName);
            cmd.Parameters.AddWithValue("@tipo_de_correo", "normal");
            
     
            cmd.Prepare();
            if (cmd.ExecuteNonQuery()>0)
            {
                return View("Dashboard");
            }
            else
            {
                return Json("{'Mensaje':'El correo no pudo ser enviado'}");

            }



        }

      
    }
}


