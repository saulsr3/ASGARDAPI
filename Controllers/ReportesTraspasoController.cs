using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.StaticFiles;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;

namespace ASGARDAPI.Controllers
{
    public class ReportesTraspasoController : Controller
    {
        private readonly IHostingEnvironment env;

        public ReportesTraspasoController(IHostingEnvironment _evn)
        {
            env = _evn;
        }

        public IActionResult Index()
        {
            return View();
        }


        //INCIO DE REPORTE DE SOLICITUDES DE TRASPASO
        [HttpGet]
        [Route("api/ReportesTraspaso/solicitudestraspasopdf")]
        public async Task<IActionResult> solicitudestraspasopdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte solicitudes de traspaso de activos");
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

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE SOLICITUDES DE TRASPASO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 15f, 30f, 60f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FOLIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA DE SOLICITUD", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudTraspasoAF> lista = (from solicitud in bd.SolicitudTraspaso              
                                                          where solicitud.Estado == 1
                                                          select new SolicitudTraspasoAF
                                                          {
                                                         
                                                              idsolicitud = solicitud.IdSolicitud,
                                                              folio = solicitud.Folio,                                                          
                                                              fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                              descripcion = solicitud.Descripcion,
                                                           
                                                          }).ToList();
                foreach (var solicitudtraspaso in lista)
                {
                    c1.Phrase = new Phrase(solicitudtraspaso.folio, parrafo5);
                    c2.Phrase = new Phrase(solicitudtraspaso.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(solicitudtraspaso.descripcion, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);


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
        //FIN DE REPORTE DE SOLICITUDES DE TRASPASO

        //INICIO DE HISTORIAL DE TRASPASOS
       /* [HttpGet]
        [Route("api/ReportesTraspaso/historialtraspapdf")]
        public async Task<IActionResult> historialtraspapdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de historial de traspasos");
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

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE HISTORIAL DE TRASPASOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 15f, 25f, 25f, 35f, 30f, 30f, 40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FOLIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA SOLICITUD", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA TRASPASO", parrafo2));
            var c4 = new PdfPCell(new Phrase("RESPONSABLE ANTERIOR", parrafo2));
            var c5 = new PdfPCell(new Phrase("ÁREA DE NEGOCIOS ANTERIOR", parrafo2));
            var c6 = new PdfPCell(new Phrase("NUEVO RESPONSABLE", parrafo2));
            var c7 = new PdfPCell(new Phrase("NUEVA ÁREA DE NEGOCIOS", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<SolicitudTraspasoAF> lista = (from solicitud in bd.SolicitudTraspaso
                                                          join activo in bd.ActivoFijo
                                                          on solicitud.IdBien equals activo.IdBien
                                                          where solicitud.Estado == 2

                                                          select new SolicitudTraspasoAF
                                                          {
                                                              idbien = activo.IdBien,
                                                              idsolicitud = solicitud.IdSolicitud,
                                                              folio = solicitud.Folio,
                                                              codigo = activo.CorrelativoBien,
                                                              fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                              descripcion = solicitud.Descripcion,
                                                              responsableanterior = solicitud.ResponsableAnterior,
                                                              areaanterior = solicitud.AreadenegocioAnterior,
                                                              nuevoresponsable = solicitud.NuevoResponsable,
                                                              nuevaarea = solicitud.NuevaAreadenegocio,
                                                              fechacadenatraspaso = solicitud.Fechatraspaso == null ? " " : ((DateTime)solicitud.Fechatraspaso).ToString("dd-MM-yyyy"),
                                                              idresponsable = (int)solicitud.IdResponsable


                                                          }).ToList();

                foreach (var historialtraspaso in lista)
                {
                    c1.Phrase = new Phrase(historialtraspaso.folio, parrafo5);
                    c2.Phrase = new Phrase(historialtraspaso.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(historialtraspaso.fechacadenatraspaso, parrafo5);
                    c4.Phrase = new Phrase(historialtraspaso.responsableanterior.ToString(), parrafo5);
                    c5.Phrase = new Phrase(historialtraspaso.areaanterior.ToString(), parrafo5);
                    c6.Phrase = new Phrase(historialtraspaso.nuevoresponsable.ToString(), parrafo5);
                    c7.Phrase = new Phrase(historialtraspaso.nuevaarea, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);


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
        }*/

        //FIN DE HISTORIAL DE TRASPASOS



        //INICIO DE REPORTE DE TRASPASO CON ID
        [HttpGet]
        [Route("api/ReportesTraspaso/historialtraspasospdf/{idbien}")]
        public async Task<IActionResult> historialtraspasospdf(int idbien)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte historial traspaso");
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

            //Fuente para tarjeta
            BaseFont fuente6 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente7 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo7 = new iTextSharp.text.Font(fuente, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            BaseFont fuente8 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo8 = new iTextSharp.text.Font(fuente2, 9f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


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

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE HISTORIAL DE TRASPASOS ", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                InformeMatenimientoAF odatos = new InformeMatenimientoAF();
                ActivoFijo oActivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();

                odatos.descripcion = oActivo.Desripcion;
                odatos.codigo = oActivo.CorrelativoBien;
                Empleado oempleado = bd.Empleado.Where(p => p.IdEmpleado == oActivo.IdResponsable).First();
                AreaDeNegocio oArea = bd.AreaDeNegocio.Where(p => p.IdAreaNegocio == oempleado.IdAreaDeNegocio).First();
                odatos.encargado = oempleado.Nombres + " " + oempleado.Apellidos;
                odatos.areadenegocio = oArea.Nombre;
               // return odatos;


                //Cuerpo de la tarjeta
                var tbl1 = new PdfPTable(new float[] { 4f, 7f, 5f, 7f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase("Correlativo: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.codigo, parrafo7)) { Border = 0 });        
                tbl1.AddCell(new PdfPCell(new Phrase("Descripción: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl1.AddCell(new PdfPCell(new Phrase(odatos.descripcion, parrafo7)) { Border = 0 });


                var tbl2 = new PdfPTable(new float[] { 4f, 7f, 5f, 7f }) { WidthPercentage = 100f };
                tbl2.AddCell(new PdfPCell(new Phrase("Encargado actual: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl2.AddCell(new PdfPCell(new Phrase(odatos.encargado, parrafo7)) { Border = 0 });
                tbl2.AddCell(new PdfPCell(new Phrase("Área de negocio actual: ", parrafo6)) { Border = 0, Rowspan = 2 });
                tbl2.AddCell(new PdfPCell(new Phrase(odatos.areadenegocio, parrafo7)) { Border = 0 });

         

                doc.Add(tbl1);
                doc.Add(Chunk.Newline);
                doc.Add(tbl2);
                doc.Add(Chunk.Newline);


                //Tabla de transacciones
                // doc.Add(new Paragraph("TABLA HISTORIAL DE TRASPASOS", parrafo2) { Alignment = Element.ALIGN_CENTER });


                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 15f, 25f, 25f, 35f, 30f, 30f, 40f }) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("FOLIO", parrafo2));
                var c2 = new PdfPCell(new Phrase("FECHA SOLICITUD", parrafo2));
                var c3 = new PdfPCell(new Phrase("FECHA TRASPASO", parrafo2));
                var c4 = new PdfPCell(new Phrase("ENCARGADO ANTERIOR", parrafo2));
                var c5 = new PdfPCell(new Phrase("ÁREA DE NEGOCIOS ANTERIOR", parrafo2));
                var c6 = new PdfPCell(new Phrase("NUEVO ENCARGADO", parrafo2));
                var c7 = new PdfPCell(new Phrase("NUEVA ÁREA DE NEGOCIOS", parrafo2));

                //Agregamos a la tabla las celdas
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                tbl.AddCell(c5);
                tbl.AddCell(c6);
                tbl.AddCell(c7);

                IEnumerable<SolicitudTraspasoAF> lista = (from solicitud in bd.SolicitudTraspaso
                                                          join activo in bd.ActivoFijo
                                                          on solicitud.IdBien equals activo.IdBien
                                                          where activo.IdBien == idbien && solicitud.Estado == 2

                                                          select new SolicitudTraspasoAF
                                                          {
                                                              idbien = activo.IdBien,
                                                              idsolicitud = solicitud.IdSolicitud,
                                                              folio = solicitud.Folio,
                                                              codigo = activo.CorrelativoBien,
                                                              fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),
                                                              descripcion = solicitud.Descripcion,
                                                              responsableanterior = solicitud.ResponsableAnterior,
                                                              areaanterior = solicitud.AreadenegocioAnterior,
                                                              nuevoresponsable = solicitud.NuevoResponsable,
                                                              nuevaarea = solicitud.NuevaAreadenegocio,
                                                              fechacadenatraspaso = solicitud.Fechatraspaso == null ? " " : ((DateTime)solicitud.Fechatraspaso).ToString("dd-MM-yyyy"),
                                                              idresponsable = (int)solicitud.IdResponsable


                                                          }).ToList();

                foreach (var historialtraspaso in lista)
                {
                    c1.Phrase = new Phrase(historialtraspaso.folio, parrafo5);
                    c2.Phrase = new Phrase(historialtraspaso.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(historialtraspaso.fechacadenatraspaso, parrafo5);
                    c4.Phrase = new Phrase(historialtraspaso.responsableanterior.ToString(), parrafo5);
                    c5.Phrase = new Phrase(historialtraspaso.areaanterior.ToString(), parrafo5);
                    c6.Phrase = new Phrase(historialtraspaso.nuevoresponsable.ToString(), parrafo5);
                    c7.Phrase = new Phrase(historialtraspaso.nuevaarea, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);


                }
                doc.Add(tbl);
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
            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

        //FIN DE REPORTES DE TRASPASO CON ID


    }
}