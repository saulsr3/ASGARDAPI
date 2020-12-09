using ASGARDAPI.Clases;
using ASGARDAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace ASGARDAPI.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        //lista usuarios
        [HttpGet]
        [Route("api/Usuario/listarUsuario")]

        public IEnumerable<UsuarioAF> listarUsuario()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<UsuarioAF> listaUsuario = (from usuario in bd.Usuario
                                                join empleado in bd.Empleado
                                                on usuario.IdEmpleado equals empleado.IdEmpleado
                                                join tipoUsuario in bd.TipoUsuario
                                                on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                                where usuario.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = usuario.IdUsuario,
                                                    nombreusuario = usuario.NombreUsuario,
                                                    nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                                    nombreTipoUsuario = tipoUsuario.TipoUsuario1

                                                }).ToList();
                return listaUsuario;
            }
        }

        [HttpPost]
        [Route("api/Usuario/guardarUsuario")]
        public int guardarUsuario([FromBody]UsuarioAF oUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    using (var transaccion = new TransactionScope())
                        if (oUsuarioAF.iidusuario == 0)
                        {
                            Usuario oUsuario = new Usuario();
                    oUsuario.IdUsuario = oUsuarioAF.iidusuario;
                    oUsuario.NombreUsuario = oUsuarioAF.nombreusuario;
                            //cifrar contraseña
                    SHA256Managed sha = new SHA256Managed();
                    string clave = oUsuarioAF.contra;
                    byte[] dataNoCifrada = Encoding.Default.GetBytes(clave);
                    byte[] dataCifrada = sha.ComputeHash(dataNoCifrada);
                    string claveCifrada = BitConverter.ToString(dataCifrada).Replace("-", "");
                     oUsuario.Contra = claveCifrada;
                            //oUsuario.Contra = oUsuarioAF.contra;
                    oUsuario.IdEmpleado = oUsuarioAF.iidEmpleado;
                    oUsuario.IdTipoUsuario = oUsuarioAF.iidTipousuario;
                    oUsuario.Dhabilitado = 1;
                    bd.Usuario.Add(oUsuario);

                    //para que asigne el valor de 1 al empleado q ya tiene usuario
                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oUsuarioAF.iidEmpleado).First();
                    oEmpleado.BtieneUsuario = 1;
                    bd.SaveChanges();
                            transaccion.Complete();
                            rpta = 1;
                        }
                }
            }

            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }
    
        [HttpGet]
        [Route("api/Usuario/listarEmpleadoCombo")]
        public IEnumerable<ComboAnidadoAF> listarEmpleadoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<ComboAnidadoAF> listarEmpleado = (from empleado in bd.Empleado
                                                          where empleado.Dhabilitado == 1
                                                          && empleado.BtieneUsuario == 0
                                                          select new ComboAnidadoAF
                                                          {
                                                              id = empleado.IdEmpleado,
                                                              nombre = empleado.Nombres + " " + empleado.Apellidos,

                                                          }).ToList();
                return listarEmpleado;

            }
        }

        [HttpGet]
        [Route("api/Usuario/listarTipoCombo")]
        public IEnumerable<TipoUsuarioAF> listarTipoCombo()
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<TipoUsuarioAF> listarTipo = (from tipoUsuario in bd.TipoUsuario
                                                         where tipoUsuario.Dhabilitado == 1
                                                         select new TipoUsuarioAF
                                                         {
                                                             iidtipousuario = tipoUsuario.IdTipoUsuario,
                                                             tipo = tipoUsuario.TipoUsuario1,

                                                         }).ToList();
                return listarTipo;

            }
        }


        [HttpGet]
        [Route("api/Usuario/RecuperarUsuario/{id}")]
        public UsuarioAF RecuperarUsuario(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                UsuarioAF oUsuarioAF = new UsuarioAF();
                Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == id).First();

                oUsuarioAF.iidusuario = oUsuario.IdUsuario;
                oUsuarioAF.nombreusuario = oUsuario.NombreUsuario;
                //oUsuarioAF.iidEmpleado = (int) oUsuario.IdEmpleado;
                oUsuarioAF.iidTipousuario = (int)oUsuario.IdTipoUsuario;

                return oUsuarioAF;
            }
        }
        [HttpGet]
        [Route("api/Usuario/RecuperarDetallesusuarios/{id}")]
        public DetallesUsuariosAF RecuperarDetallesusuarios(int id)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                DetallesUsuariosAF oUsuarioAF = new DetallesUsuariosAF();
                Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == id).First();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oUsuario.IdEmpleado).FirstOrDefault();
                TipoUsuario oTipo = bd.TipoUsuario.Where(p => p.IdTipoUsuario == oUsuario.IdTipoUsuario).FirstOrDefault();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).FirstOrDefault();
                Sucursal oSucursal = bd.Sucursal.Where(p => p.IdSucursal == oArea.IdSucursal).FirstOrDefault();

                oUsuarioAF.nombre = oEmpleado.Nombres+ " "+oEmpleado.Apellidos;
                oUsuarioAF.nombreusuario = oUsuario.NombreUsuario;
                oUsuarioAF.tipousuario = oTipo.TipoUsuario1;
                oUsuarioAF.sucursal = oSucursal.Nombre;
                oUsuarioAF.area = oArea.Nombre;

                return oUsuarioAF;
            }
        }

        [HttpPost]
        [Route("api/Usuario/modificarUsuario")]
        public int modificarUsuario([FromBody]UsuarioAF oUsuarioAF)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    //para editar tenemos que sacar la informacion
                    Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == oUsuarioAF.iidusuario).First();
                    oUsuario.IdUsuario = oUsuarioAF.iidusuario;
                    oUsuario.NombreUsuario = oUsuarioAF.nombreusuario;
                    //oUsuario.Contra = oUsuarioAF.contra;
                    //oUsuario.IdEmpleado = oUsuarioAF.iidEmpleado;
                    oUsuario.IdTipoUsuario = oUsuarioAF.iidTipousuario;
                    //para guardar cambios
                    bd.SaveChanges();
                    //si todo esta bien
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }



        [HttpGet]
        [Route("api/Usuario/validarUsuario/{iidusuario}/{tipo}")]
        public int validarUsuario(int iidusuario, string tipo)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    if (iidusuario == 0)
                    {
                        rpta = bd.Usuario.Where(p => p.NombreUsuario.ToLower() == tipo.ToLower()
                        && p.Dhabilitado == 1).Count();
                    }
                    else
                    {
                        rpta = bd.Usuario.Where(p => p.NombreUsuario.ToLower() == tipo.ToLower() && p.IdUsuario != iidusuario
                        && p.Dhabilitado == 1).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                //si se cae
                rpta = 0;

            }
            return rpta;
        }

        [HttpGet]
        [Route("api/Usuario/buscarUsuario/{buscador?}")]
        public IEnumerable<UsuarioAF> buscarUsuario(string buscador = "")
        {
            List<UsuarioAF> listaUsuario;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                if (buscador == "")
                {
                    listaUsuario = (from usuario in bd.Usuario
                                    join empleado in bd.Empleado
                                    on usuario.IdEmpleado equals empleado.IdEmpleado
                                    join tipoUsuario in bd.TipoUsuario
                                    on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                    where usuario.Dhabilitado == 1
                                    select new UsuarioAF
                                    {
                                        iidusuario = usuario.IdUsuario,
                                        nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                        nombreusuario = usuario.NombreUsuario,
                                        nombreTipoUsuario=tipoUsuario.TipoUsuario1

                                    }).ToList();

                    return listaUsuario;
                }
                else
                {
                    listaUsuario = (from usuario in bd.Usuario
                                    join empleado in bd.Empleado
                                   on usuario.IdEmpleado equals empleado.IdEmpleado
                                    join tipoUsuario in bd.TipoUsuario
                                    on usuario.IdTipoUsuario equals tipoUsuario.IdTipoUsuario
                                    where usuario.Dhabilitado == 1

                                    && ((usuario.NombreUsuario).ToLower().Contains(buscador.ToLower())
                                     
                                      || (empleado.Nombres).Contains(buscador.ToLower())
                                      || (empleado.Apellidos).ToLower().Contains(buscador.ToLower())
                                      || (tipoUsuario.TipoUsuario1).ToLower().Contains(buscador.ToLower())
                                      )
                                    select new UsuarioAF
                                    {
                                        iidusuario = usuario.IdUsuario,
                                        nombreEmpleado = empleado.Nombres + " " + empleado.Apellidos,
                                        nombreusuario = usuario.NombreUsuario,
                                        nombreTipoUsuario = tipoUsuario.TipoUsuario1
                                    }).ToList();

                    return listaUsuario;
                }
            }
        }

        [HttpGet]
        [Route("api/Usuario/validarUsuariosregistrados")]
        public int validarUsuariosregistrados()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<UsuarioAF> lista = (from usuario in bd.Usuario
                                                         where usuario.Dhabilitado == 1
                                                         select new UsuarioAF
                                                         {
                                                             iidusuario = usuario.IdUsuario
                                                         }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
            }
                return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/validarCooperativasRegistradas")]
        public int validarCooperativassregistrados()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<UsuarioAF> lista = (from cooperativa in bd.Cooperativa
                                                where cooperativa.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = cooperativa.IdCooperativa
                                                }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/validarSucursalesRegistradas")]
        public int validarSucursalessRegistradas()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<UsuarioAF> lista = (from sucursal in bd.Sucursal
                                                where sucursal.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = sucursal.IdSucursal
                                                }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/validarAreasRegistradas")]
        public int validarAreasRegistradas()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<UsuarioAF> lista = (from areas in bd.AreaDeNegocio
                                                where areas.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = areas.IdAreaNegocio
                                                }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/validarEmpleadosRegistrados")]
        public int validarEmpleadosRegistrados()
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<UsuarioAF> lista = (from empleados in bd.Empleado
                                                where empleados.Dhabilitado == 1
                                                select new UsuarioAF
                                                {
                                                    iidusuario = empleados.IdEmpleado
                                                }).ToList();
                if (lista.Count() > 0)
                {
                    rpta = 1;
                }
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/TipoUsuario/eliminarUsuario/{idUsuario}")]
        public int eliminarUsuario(int idUsuario)
        {
            int rpta = 0;
            try
            {
                using (BDAcaassAFContext bd = new BDAcaassAFContext())
                {
                    Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == idUsuario).First();
                    Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oUsuario.IdEmpleado).FirstOrDefault();
                    oEmpleado.BtieneUsuario = 0;
                    oUsuario.Dhabilitado = 0;
                    bd.SaveChanges();
                    rpta = 1;
                }
            }
            catch (Exception ex)
            {
                rpta = 0;
            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/listarEmpleadoAsistente/{idUsuario}")]
        public IEnumerable<ComboAnidadoAF> listarEmpleadoAsistente(int idUsuario)
        {
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == idUsuario).FirstOrDefault();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oUsuario.IdEmpleado).FirstOrDefault();
                //AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<ComboAnidadoAF> listarEmpleado = (from empleado in bd.Empleado
                                                          where empleado.Dhabilitado == 1
                                                          && empleado.BtieneUsuario == 0 && empleado.IdAreaDeNegocio==oEmpleado.IdAreaDeNegocio
                                                          select new ComboAnidadoAF
                                                          {
                                                              id = empleado.IdEmpleado,
                                                              nombre = empleado.Nombres + " " + empleado.Apellidos,

                                                          }).ToList();
                return listarEmpleado;

            }
        }
        [HttpGet]
        [Route("api/Usuario/ValidarEmpleadoAsistente/{idUsuario}")]
        public int ValidarEmpleadoAsistente(int idUsuario)
        {
            int rpta = 0;
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Usuario oUsuario = bd.Usuario.Where(p => p.IdUsuario == idUsuario).FirstOrDefault();
                Empleado oEmpleado = bd.Empleado.Where(p => p.IdEmpleado == oUsuario.IdEmpleado).FirstOrDefault();
                //AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oEmpleado.IdAreaDeNegocio).FirstOrDefault();
                IEnumerable<ComboAnidadoAF> listarEmpleado = (from empleado in bd.Empleado
                                                              where empleado.Dhabilitado == 1
                                                              && empleado.BtieneUsuario == 0 && empleado.IdAreaDeNegocio == oEmpleado.IdAreaDeNegocio
                                                              select new ComboAnidadoAF
                                                              {
                                                                  id = empleado.IdEmpleado,
                                                                  nombre = empleado.Nombres + " " + empleado.Apellidos,

                                                              }).ToList();
                if (listarEmpleado.Count() > 0) {
                    rpta = 1;
                } 

            }
            return rpta;
        }
        [HttpGet]
        [Route("api/Usuario/CreateBackup")]
        public int CreateBackup()
        {
            int res = 0;
            try
            {
                
                //string ruta = @"";
                string wwwPath = Environment.CurrentDirectory+ @"\Backups";
                //string contentPath = Environment.GetFolderPath.ToString();
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                if (!Directory.Exists(wwwPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(wwwPath);
                }
                
                string Fecha=DateTime.Now.ToString("dd-MM-yyyy");
                //string fileName = @"C:\Backup\test.bak";
                //string command = @"BACKUP DATABASE DBAcaassAF TO DISK=" + fileName + "";
                SqlCommand oCommand = null;
                SqlConnection oConnection = null;
                oConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DBAcaassAF;database=BDAcaassAF;Integrated Security=true");
                if (oConnection.State != ConnectionState.Open)
                    oConnection.Open();
                oCommand = new SqlCommand("Backup", oConnection);
                oCommand.CommandType=CommandType.StoredProcedure;
                oCommand.Parameters.AddWithValue("@ruta", SqlDbType.VarChar).Value = wwwPath;
                oCommand.Parameters.AddWithValue("@fecha", SqlDbType.VarChar).Value = Fecha;
                oCommand.ExecuteNonQuery();
                res = 1;
            }
            catch (Exception ex)
            {
                res = 0;

            }
            return res;
        }

    }
}