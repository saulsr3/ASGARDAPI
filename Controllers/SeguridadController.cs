using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;


namespace ASGARDAPI.Controllers
{
    public class SeguridadController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("api/Usuario/login")]
        public UsuarioAF login([FromBody] UsuarioAF oUsuarioAF) {
            int rpta = 0;
            UsuarioAF oUsuario = new UsuarioAF();
            using (BDAcaassAFContext bd = new BDAcaassAFContext()) {

                SHA256Managed sha = new SHA256Managed();
                byte[] dataNoCifrada = Encoding.Default.GetBytes(oUsuarioAF.contra);
                byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                string claveCifrada = BitConverter.ToString(dataCifrada).Replace("-", "");
                rpta = bd.Usuario.Where(p => p.NombreUsuario.ToUpper() == oUsuarioAF.nombreusuario.ToUpper() && p.Contra == claveCifrada).Count();
                if (rpta == 1)
                {
                    Usuario oUsuarioRecuperar = bd.Usuario.Where(p => p.NombreUsuario.ToUpper() == oUsuarioAF.nombreusuario.ToUpper() && p.Contra == claveCifrada).First();
                    oUsuario.iidusuario = oUsuarioRecuperar.IdUsuario;
                    oUsuario.nombreusuario = oUsuarioRecuperar.NombreUsuario;
                    oUsuario.iidEmpleado = (int)oUsuarioRecuperar.IdEmpleado;
                    oUsuario.iidTipousuario = (int)oUsuarioRecuperar.IdTipoUsuario;
                }
                else {
                    oUsuario.iidusuario = 0;
                    oUsuario.nombreusuario = "";
                }
            }
            return oUsuario;
        }
        [HttpGet]
        [Route("api/Seguridad/listarBitacora")]
        public IEnumerable<TablaBitacoraAF> listarBitacora()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TablaBitacoraAF> listaProveedores = (from bitacora in bd.Bitacora
                                                                 join usuario in bd.Usuario
                                                                  on bitacora.IdUsuario equals usuario.IdUsuario
                                                                 join empleado in bd.Empleado
                                                                 on usuario.IdEmpleado equals empleado.IdEmpleado
                                                                 orderby bitacora.Fecha descending
                                                                 select new TablaBitacoraAF
                                                                 {
                                                                     idBitacora = bitacora.IdBitacora,
                                                                     nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                                                     nombreUsuario = usuario.NombreUsuario,
                                                                     fecha = bitacora.Fecha == null ? " " : ((DateTime)bitacora.Fecha).ToString("dd-MM-yyyy : HH:mm:ss"),
                                                                     descripcion = bitacora.Descripcion,
                                                                 }).ToList();
                return listaProveedores;
            }
        }
        [HttpGet]
        [Route("api/Seguridad/ValidaNumeroUsuarios/{email}")]
        public int ValidaNumeroUsuarios(string email)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                try
                {
                    Empleado oEmpledo = bd.Empleado.Where(p => p.Email == email).First();
                    IEnumerable<ComboAnidadoAF> lista = (from usuario in bd.Usuario
                                                         where usuario.IdEmpleado == oEmpledo.IdEmpleado && usuario.Dhabilitado == 1
                                                         select new ComboAnidadoAF
                                                         {
                                                             id = usuario.IdUsuario
                                                         }).ToList();
                    if (lista.Count() > 1)
                    {
                        rpta = 1;
                    }
                    else if (lista.Count() == 0)
                    {
                        rpta = 0;
                    }
                    else if (lista.Count() == 1)
                    {
                        rpta = 2;
                    }
                }
                catch (Exception)
                {

                    rpta=0;
                }
                
                return rpta;
            }
        }
        [HttpGet]
        [Route("api/Seguridad/ListarUsuarios/{email}")]
        public IEnumerable<UsuarioAF> ListarUsuarios(string email)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oEmpledo = bd.Empleado.Where(p => p.Email == email).First();
                IEnumerable<UsuarioAF> lista = (from usuario in bd.Usuario
                                                join tipo in bd.TipoUsuario
                                                on usuario.IdTipoUsuario equals tipo.IdTipoUsuario
                                                where usuario.IdEmpleado == oEmpledo.IdEmpleado&&usuario.Dhabilitado==1
                                                     select new UsuarioAF
                                                     {
                                                         iidusuario = usuario.IdUsuario,
                                                         nombreusuario=usuario.NombreUsuario,
                                                         nombreTipoUsuario=tipo.TipoUsuario1
                                                     }).ToList();
            
                return lista;
            }
        }
        [HttpGet]
        [Route("api/Seguridad/SendEmail/{idusuario}/{email}")]
        public int SendEmail(int idusuario, string email)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == idusuario).FirstOrDefault();
                    Empleado oEmpleado= bd.Empleado.Where(p => p.Email == email).FirstOrDefault();

                



                    SHA256Managed sha = new SHA256Managed();
                    byte[] dataNoCifrada = Encoding.Default.GetBytes(oUsuario.Contra);
                    byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                    string pass = Convert.ToBase64String(dataCifrada);
                    var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TEAM ASGARD", "asgardrecoverypass@gmail.com"));
                message.To.Add(new MailboxAddress(oUsuario.NombreUsuario,oEmpleado.Email ));
                message.Subject = "Recuperación de contraseña";

                message.Body = new TextPart("plain")
                {
                    Text = $"Hola,{oEmpleado.Nombres}\n\nTu contraseña es:{pass}\n\nGuardala en un lugar seguro.\n\n-- TEAM ASGARD"
                };
   

                    using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("asgardrecoverypass@gmail.com", "asgardproject2020$#199342//2");

                    client.Send(message);
                    client.Disconnect(true);
                }
                rpta = 1;
                }
            }
            catch (Exception)
            {

                rpta = 0;
            }
          
                return rpta;
        }
    }
}
