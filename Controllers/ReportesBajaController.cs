﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using System.IO;
using Microsoft.AspNetCore.Hosting;


namespace ASGARDAPI.Controllers
{
    public class ReportesBajaController : Controller
    {
        private readonly IHostingEnvironment env;

        public ReportesBajaController(IHostingEnvironment _evn)
        {
            env = _evn;
        }

        public IActionResult Index()
        {
            return View();
        }

        //INICIO DE REPORTE DE SOLICITUD DE DESCARGO
        [HttpGet]
        [Route("api/ReportesBaja/solicitudbajapdf")]
        public async Task<IActionResult> solicitudbajapdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte solicitudes de descargo de activos");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE SOLICITUDES DE DESCARGO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 15f, 30f, 40f,40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FOLIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA DE SOLICITUD", parrafo2));
            var c3 = new PdfPCell(new Phrase("MOTIVO", parrafo2));
            var c4 = new PdfPCell(new Phrase("OBSERVACIONES", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudBajaAF> lista = (from activo in bd.ActivoFijo
                                                      join solicitud in bd.SolicitudBaja
                                                      on activo.IdBien equals solicitud.IdBien
                                                      join descargo in bd.TipoDescargo
                                                      on solicitud.IdTipoDescargo equals descargo.IdTipo
                                                      where solicitud.Estado == 1
                                                      select new SolicitudBajaAF
                                                      {
                                                          idbien = activo.IdBien,
                                                          idsolicitud = solicitud.IdSolicitud,
                                                          folio = solicitud.Folio,
                                                          fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                          observaciones = solicitud.Observaciones,
                                                          nombredescargo = descargo.Nombre,
                                                          entidadbeneficiaria = solicitud.EntidadBeneficiaria,
                                                          domicilio = solicitud.Domicilio,
                                                          contacto = solicitud.Contacto,
                                                          telefono = solicitud.Telefono,

                                                      }).ToList();
                foreach (var solicitudbaja in lista)
                {
                    c1.Phrase = new Phrase(solicitudbaja.folio, parrafo5);
                    c2.Phrase = new Phrase(solicitudbaja.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(solicitudbaja.nombredescargo, parrafo5);
                    c4.Phrase = new Phrase(solicitudbaja.observaciones, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);


                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO



            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTE DE SOLICITUD DE  //INICIO DE REPORTE DE SOLICITUD DE DESCARGO

        //INICIO DE INFORME DE SOLICITUDES DE DESCARGOS

        [HttpGet]
        [Route("api/ReportesBaja/informebajapdf")]
        public async Task<IActionResult> informebajapdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte infomes de solicitudes de descargo de activos");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE INFORME DE SOLICITUDES DE DESCARGO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 15f, 30f, 40f, 40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FOLIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA DE APROBACIÓN DE SOLICITUD", parrafo2));
            var c3 = new PdfPCell(new Phrase("MOTIVO", parrafo2));
            var c4 = new PdfPCell(new Phrase("OBSERVACIONES", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudBajaAF> lista = (from activo in bd.ActivoFijo
                                                      join solicitud in bd.SolicitudBaja
                                                      on activo.IdBien equals solicitud.IdBien
                                                      join descargo in bd.TipoDescargo
                                                      on solicitud.IdTipoDescargo equals descargo.IdTipo
                                                      where solicitud.Estado == 2
                                                      select new SolicitudBajaAF
                                                      {
                                                          idbien = activo.IdBien,
                                                          idsolicitud = solicitud.IdSolicitud,
                                                          folio = solicitud.Folio,
                                                          fechacadena = solicitud.Fechabaja == null ? " " : ((DateTime)solicitud.Fechabaja).ToString("dd-MM-yyyy"),
                                                          observaciones = solicitud.Observaciones,
                                                          nombredescargo = descargo.Nombre,
                                                          entidadbeneficiaria = solicitud.EntidadBeneficiaria,
                                                          domicilio = solicitud.Domicilio,
                                                          contacto = solicitud.Contacto,
                                                          telefono = solicitud.Telefono,

                                                      }).ToList();
                foreach (var solicitudbaja in lista)
                {
                    c1.Phrase = new Phrase(solicitudbaja.folio, parrafo5);
                    c2.Phrase = new Phrase(solicitudbaja.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(solicitudbaja.nombredescargo, parrafo5);
                    c4.Phrase = new Phrase(solicitudbaja.observaciones, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);


                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO



            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTE DE INFORME DE DESCARGO




        //INICIO DE REPORTE DE ACTIVOS DADOS DE BAJA ASIGNADOS
        [HttpGet]
        [Route("api/ReportesBaja/asignadosdebajapdf")]
        public async Task<IActionResult> asignadosdebajapdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de historial de descargos de activos asignados");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE HISTORIAL DE DESCARGOS DE ACTIVOS ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 30f, 35f, 40f, 40f,30f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CORRELATIVO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA INGRESO", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("ÁREA DE NEGOCIOS", parrafo2));
            var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<BajaAF> lista = (from activo in bd.ActivoFijo
                                      join noFormulario in bd.FormularioIngreso
                                       on activo.NoFormulario equals noFormulario.NoFormulario
                                      join resposable in bd.Empleado
                                      on activo.IdResponsable equals resposable.IdEmpleado
                                      join area in bd.AreaDeNegocio
                                      on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                      join cargo in bd.Cargos
                                      on resposable.IdCargo equals cargo.IdCargo
                                      where activo.EstadoActual == 0 && activo.EstaAsignado == 1
                                      orderby activo.CorrelativoBien
                                      select new BajaAF
                                      {
                                          IdBien = activo.IdBien,
                                          Codigo = activo.CorrelativoBien,
                                          fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                          Desripcion = activo.Desripcion,
                                          AreaDeNegocio = area.Nombre,
                                          Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                          //cargo = cargo.Cargo,

                                      }).ToList();
                foreach (var activosbaja in lista)
                {
                    c1.Phrase = new Phrase(activosbaja.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activosbaja.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activosbaja.Desripcion, parrafo5);
                    c4.Phrase = new Phrase(activosbaja.AreaDeNegocio, parrafo5);
                    c5.Phrase = new Phrase(activosbaja.Resposnsable, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);


                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO



            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTE DE ACTIVOS DADOS DE BAJA ASIGNADOS

        //INICIO DE REPORTE DE ACTIVOS DADOS DE BAJA NO ASIGANDOS
        [HttpGet]
        [Route("api/ReportesBaja/noasignadosdebajapdf")]
        public async Task<IActionResult> noasignadosdebajapdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporde de historial de descargos de activos no asignados");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE HISTORIAL DE DESCARGOS DE ACTIVOS NO ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 30f, 35f, 40f, 40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NO. FORMULARIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA INGRESO", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
           

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
         

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<BajaAF> lista = (from activo in bd.ActivoFijo
                                      join noFormulario in bd.FormularioIngreso
                                      on activo.NoFormulario equals noFormulario.NoFormulario
                                      join clasif in bd.Clasificacion
                                      on activo.IdClasificacion equals clasif.IdClasificacion
                                      where activo.EstadoActual == 0 && activo.EstaAsignado == 0

                                      select new BajaAF
                                      {
                                          IdBien = activo.IdBien,
                                          NoFormulario = noFormulario.NoFormulario,
                                          fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                          Desripcion = activo.Desripcion,
                                          Clasificacion = clasif.Clasificacion1
                                                                             
                                      }).ToList();
                foreach (var activosbaja in lista)
                {
                    c1.Phrase = new Phrase(activosbaja.NoFormulario.ToString(), parrafo5);
                    c2.Phrase = new Phrase(activosbaja.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activosbaja.Desripcion, parrafo5);
                    c4.Phrase = new Phrase(activosbaja.Clasificacion, parrafo5);
              



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                 

                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO



            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTE DE ACTIVOS DADOS DE BAJA NO ASIGNAGOS

        //INICIO DE HISTORIAL DE ACTIVOS DESCARGADOS
        [HttpGet]
        [Route("api/ReportesBaja/activosDescargadosJefePdf/{idJefe}")]
        public async Task<IActionResult> activosDescargadosJefePdf(int idJefe)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de historial de descargos de activos asignados");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;

                //Se agrega el encabezado
                var tbl1 = new PdfPTable(new float[] { 11f, 89f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase(" ", parrafo2)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Descripcion.ToUpper(), parrafo2)) { Border = 0, HorizontalAlignment = 1 });
                tbl1.AddCell(new PdfPCell(new Phrase(oCooperativa.Nombre.ToUpper(), parrafo3)) { Border = 0, HorizontalAlignment = 1 });
                doc.Add(tbl1);
                doc.Add(new Phrase("\n"));
            }
            doc.Add(new Phrase("\n"));
            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE HISTORIAL DE DESCARGOS DE ACTIVOS ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 30f, 35f, 40f, 40f, 30f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CORRELATIVO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA INGRESO", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("ÁREA DE NEGOCIOS", parrafo2));
            var c5 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == idJefe).FirstOrDefault();
                AreaDeNegocio oarea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).FirstOrDefault();
                List<BajaAF> lista = (from activo in bd.ActivoFijo
                                      join noFormulario in bd.FormularioIngreso
                                       on activo.NoFormulario equals noFormulario.NoFormulario
                                      join resposable in bd.Empleado
                                      on activo.IdResponsable equals resposable.IdEmpleado
                                      join area in bd.AreaDeNegocio
                                      on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                      join cargo in bd.Cargos
                                      on resposable.IdCargo equals cargo.IdCargo

                                      where activo.EstadoActual == 0 && activo.EstaAsignado == 1 && area.IdAreaNegocio == oarea.IdAreaNegocio
                                      orderby activo.CorrelativoBien
                                      select new BajaAF
                                      {
                                          IdBien = activo.IdBien,
                                          Codigo = activo.CorrelativoBien,
                                          fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                          Desripcion = activo.Desripcion,
                                          AreaDeNegocio = area.Nombre,
                                          Resposnsable = resposable.Nombres + " " + resposable.Apellidos,
                                          //cargo = cargo.Cargo,

                                      }).ToList();
                foreach (var activosbaja in lista)
                {
                    c1.Phrase = new Phrase(activosbaja.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activosbaja.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activosbaja.Desripcion, parrafo5);
                    c4.Phrase = new Phrase(activosbaja.AreaDeNegocio, parrafo5);
                    c5.Phrase = new Phrase(activosbaja.Resposnsable, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);


                }

                //INICIO DE ADICIÓN DE LOGO
                CooperativaAF oCooperativaAF = new CooperativaAF();

                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;


                try
                {
                    iTextSharp.text.Image logo = null;
                    logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo.ToString());
                    logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    logo.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    logo.BorderColor = iTextSharp.text.BaseColor.White;
                    logo.ScaleToFit(170f, 100f);

                    float ancho = logo.Width;
                    float alto = logo.Height;
                    float proporcion = alto / ancho;

                    logo.ScaleAbsoluteWidth(80);
                    logo.ScaleAbsoluteHeight(80 * proporcion);

                    logo.SetAbsolutePosition(40f, 695f);

                    doc.Add(logo);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }

                //FIN DE ADICIÓN DE LOGO

            }
            doc.Add(tbl);
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTES DE ACTIVOS DESCARGADOS



    }
}