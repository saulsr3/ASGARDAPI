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
    public class ReportesMantenimientoController : Controller
    {
        private readonly IHostingEnvironment env;

        public ReportesMantenimientoController(IHostingEnvironment _evn)
        {
            env = _evn;
        }

        public IActionResult Index()
        {
            return View();
        }

        //INICIO DE REPORTE DE SOLICITUDES EN MANTENIMIENTO
        [HttpGet]
        [Route("api/ReportesMantenimiento/solicitudesmantenimientopdf")]
        public async Task<IActionResult> solicitudesmantenimientopdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte solicitudes de mantenimiento");
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
            doc.Add(new Paragraph("REPORTE DE SOLICITUDES DE MANTENIMIENTO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 25f, 30f,50f }) { WidthPercentage = 100f };
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
                IEnumerable<SolicitudMantenimientoPAF> lista = (from solicitud in bd.SolicitudMantenimiento
                                                                 where solicitud.Estado == 1

                                                                 select new SolicitudMantenimientoPAF
                                                                 {
                                                                     idsolicitud = solicitud.IdSolicitud,
                                                                     folio = solicitud.Folio,
                                                                     descripcion = solicitud.Descripcion,
                                                                     fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy")
                                                                 }).ToList();
                foreach (var solicitudmante in lista)
                {
                    c1.Phrase = new Phrase(solicitudmante.folio, parrafo5);
                    c2.Phrase = new Phrase(solicitudmante.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(solicitudmante.descripcion, parrafo5);



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
        //FIN DE REPORTE DE SOLICITUDES EN MANTENIMIENTO

        //INICIO DE REPORTE DE ACTIVOS EN MANTENIMIENTO
        [HttpGet]
        [Route("api/ReportesMantenimiento/activosenmantenimientopdf")]
        public async Task<IActionResult> activosenmantenimientopdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte solicitudes en mantenimiento");
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
            doc.Add(new Paragraph("REPORTE DE ACTIVOS EN MANTENIMIENTO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 25f, 40f, 40f,40f}) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("RAZONES DE MANTENIMIENTO", parrafo2));
            var c4 = new PdfPCell(new Phrase("PERÍODO DE MANTENIMIENTO", parrafo2));

            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<BienesSolicitadosMttoAF> lista = (from bienMtto in bd.BienMantenimiento
                                                              join activo in bd.ActivoFijo
                                                              on bienMtto.IdBien equals activo.IdBien
                                                              join solicitud in bd.SolicitudMantenimiento
                                                              on bienMtto.IdSolicitud equals solicitud.IdSolicitud
                                                              //  join informe in bd.InformeMantenimiento
                                                              // on bienMtto.IdMantenimiento equals informe.IdMantenimiento
                                                              where activo.EstadoActual == 3 && bienMtto.Estado == 1 //ELEMENTO 2 LISTA
                                                              select new BienesSolicitadosMttoAF
                                                              {
                                                                  idBien = activo.IdBien,
                                                                  idmantenimiento = bienMtto.IdMantenimiento,
                                                                  estadoActual = (int)activo.EstadoActual,
                                                                  Codigo = activo.CorrelativoBien,
                                                                  Descripcion = activo.Desripcion,
                                                                  Periodo = bienMtto.PeriodoMantenimiento,
                                                                  Razon = bienMtto.RazonMantenimiento,
                                                                  fechacadena = solicitud.Fecha == null ? " " : ((DateTime)solicitud.Fecha).ToString("dd-MM-yyyy"),


                                                              }).ToList();
                foreach (var activosmante in lista)
                {
                    c1.Phrase = new Phrase(activosmante.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activosmante.Descripcion, parrafo5);
                    c3.Phrase = new Phrase(activosmante.Razon, parrafo5);
                    c4.Phrase = new Phrase(activosmante.Periodo, parrafo5);



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

        //FIN DE REPORTE DE ACTIVOS EN MANTENIMIENTO

        //INCIO DE REPORTE DE INFORMES (PARA DAR REVALORIZACIÓN)
        [HttpGet]
        [Route("api/ReportesMantenimiento/informesmantenimientopdf")]
        public async Task<IActionResult> informesmantenimientopdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte informes de mantenimiento");
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
            doc.Add(new Paragraph("REPORTE DE INFORMES DE MANTENIMIENTO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 20f, 30f, 30f, 20f ,30f,30f,30f}) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("TÉCNICO", parrafo2));
            var c4 = new PdfPCell(new Phrase("MANO DE OBRA", parrafo2));
            var c5 = new PdfPCell(new Phrase("MATERIALES", parrafo2));
            var c6 = new PdfPCell(new Phrase("COSTO TOTAL", parrafo2));
            var c7 = new PdfPCell(new Phrase("DESCRIPCIÓN DE INFORME", parrafo2));

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
                Periodo anioActual = bd.Periodo.Where(p => p.Estado == 1).FirstOrDefault();
                IEnumerable<InformeMatenimientoAF> lista = (from tecnico in bd.Tecnicos
                                                                        join informemante in bd.InformeMantenimiento
                                                                          on tecnico.IdTecnico equals informemante.IdTecnico
                                                                        join bienmante in bd.BienMantenimiento
                                                                        on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                        join activo in bd.ActivoFijo
                                                                        on bienmante.IdBien equals activo.IdBien
                                                                        where informemante.Estado == 1 && (activo.EstadoActual != 0) && (activo.UltimoAnioDepreciacion == null || (activo.UltimoAnioDepreciacion < (anioActual.Anio)))


                                                                        select new InformeMatenimientoAF
                                                                        {
                                                                            idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                            idBien = activo.IdBien,
                                                                            idmantenimiento = (int)informemante.IdMantenimiento,
                                                                            fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                            nombretecnico = tecnico.Nombre,
                                                                            descripcion = informemante.Descripcion,
                                                                            costomateriales = (double)informemante.CostoMateriales,
                                                                            costomo = (double)informemante.CostoMo,
                                                                            costototal = (double)informemante.CostoTotal,
                                                                            vidautil = activo.VidaUtil,
                                                                            bienes = activo.Desripcion


                                                                        }).ToList();

                foreach (var informemante in lista)
                {
                    c1.Phrase = new Phrase(informemante.fechacadena, parrafo5);
                    c2.Phrase = new Phrase(informemante.bienes, parrafo5);
                    c3.Phrase = new Phrase(informemante.nombretecnico, parrafo5);
                    c4.Phrase = new Phrase(informemante.costomo.ToString(), parrafo5);
                    c5.Phrase = new Phrase(informemante.costomateriales.ToString(), parrafo5);
                    c6.Phrase = new Phrase(informemante.costototal.ToString(), parrafo5);
                    c7.Phrase = new Phrase(informemante.descripcion, parrafo5);



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
        }


        //FIN DE REPORTE  DE INFORMES (PARA DAR REVALORIZACIÓN)

        //INICIO DE REPORTE DE HISTORIAL DE MANTENIMIENTO
       /* [HttpGet]
        [Route("api/ReportesMantenimiento/historialmantenimientoSSpdf")]
        public async Task<IActionResult> historialmantenimientoSSpdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte de historial de mantenimiento");
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
            doc.Add(new Paragraph("REPORTE DE HISTORIAL DE MANTENIMIENTO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 35f, 25f, 30f, 25f, 30f, 20f, 40f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CORRELATIVO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("TÉCNICO", parrafo2));
            var c4 = new PdfPCell(new Phrase("MANO DE OBRA", parrafo2));
            var c5 = new PdfPCell(new Phrase("MATERIALES", parrafo2));
            var c6 = new PdfPCell(new Phrase("COSTO TOTAL", parrafo2));
            var c7 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));

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
                IEnumerable<InformeMatenimientoAF> lista = (from tecnico in bd.Tecnicos
                                                                        join informemante in bd.InformeMantenimiento
                                                                        on tecnico.IdTecnico equals informemante.IdTecnico
                                                                        join bienmante in bd.BienMantenimiento
                                                                        on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                                        join bienes in bd.ActivoFijo
                                                                        on bienmante.IdBien equals bienes.IdBien
                                                                        where informemante.Estado==0 || informemante.Estado==2 || informemante.Estado == 1
                                                            orderby bienes.CorrelativoBien

                                                                        select new InformeMatenimientoAF
                                                                        {
                                                                            idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                            idBien = bienes.IdBien,
                                                                            idmantenimiento = (int)informemante.IdMantenimiento,
                                                                            fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                            nombretecnico = tecnico.Nombre,
                                                                            codigo = bienes.CorrelativoBien,
                                                                            descripcion = informemante.Descripcion,
                                                                            costomateriales = (double)informemante.CostoMateriales,
                                                                            costomo = (double)informemante.CostoMo,
                                                                            costototal = (double)informemante.CostoTotal,
                                                                            bienes = bienes.Desripcion

                                                                        }).ToList();

                foreach (var historialmante in lista)
                {
                    c1.Phrase = new Phrase(historialmante.codigo, parrafo5);
                    c2.Phrase = new Phrase(historialmante.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(historialmante.nombretecnico, parrafo5);
                    c4.Phrase = new Phrase(historialmante.costomo.ToString(), parrafo5);
                    c5.Phrase = new Phrase(historialmante.costomateriales.ToString(), parrafo5);
                    c6.Phrase = new Phrase(historialmante.costototal.ToString(), parrafo5);
                    c7.Phrase = new Phrase(historialmante.descripcion, parrafo5);



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

        //FIN DE REPORTE DE HISTORIAL DE MANTENIMIENTO

        //INICIO DE REPORTE DE HISTORIAL DE MANTENIMIENTO CON ID 
        [HttpGet]
        [Route("api/ReportesMantenimiento/historialmantenimientopdf/{idbien}")]
        public async Task<IActionResult> historialmantenimientopdf(int idbien)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte historial mantenimiento");
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
            doc.Add(new Paragraph("REPORTE HISTORIAL DE MANTENIMIENTO ", parrafo) { Alignment = Element.ALIGN_CENTER });

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
                tbl1.AddCell(new PdfPCell(new Phrase("Descripción de activo: ", parrafo6)) { Border = 0, Rowspan = 2 });
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
                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 25f, 30f, 25f, 30f, 20f, 40f }) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("FECHA", parrafo2));
                var c2 = new PdfPCell(new Phrase("TÉCNICO", parrafo2));
                var c3 = new PdfPCell(new Phrase("MANO DE OBRA", parrafo2));
                var c4 = new PdfPCell(new Phrase("MATERIALES", parrafo2));
                var c5 = new PdfPCell(new Phrase("COSTO TOTAL", parrafo2));
                var c6 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));

                //Agregamos a la tabla las celdas
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                tbl.AddCell(c5);
                tbl.AddCell(c6);
              

                IEnumerable<InformeMatenimientoAF> lista = (from tecnico in bd.Tecnicos
                                                            join informemante in bd.InformeMantenimiento
                                                            on tecnico.IdTecnico equals informemante.IdTecnico
                                                            join bienmante in bd.BienMantenimiento
                                                            on informemante.IdMantenimiento equals bienmante.IdMantenimiento
                                                            join bienes in bd.ActivoFijo
                                                            on bienmante.IdBien equals bienes.IdBien
                                                            where bienes.IdBien== idbien //&& informemante.Estado == 0 || informemante.Estado == 2 || informemante.Estado == 1
                                                            orderby bienes.CorrelativoBien

                                                            select new InformeMatenimientoAF
                                                            {
                                                                idinformematenimiento = informemante.IdInformeMantenimiento,
                                                                idBien = bienes.IdBien,
                                                                idmantenimiento = (int)informemante.IdMantenimiento,
                                                                fechacadena = informemante.Fecha == null ? " " : ((DateTime)informemante.Fecha).ToString("dd-MM-yyyy"),
                                                                nombretecnico = tecnico.Nombre,
                                                                codigo = bienes.CorrelativoBien,
                                                                descripcion = informemante.Descripcion,
                                                                costomateriales = (double)informemante.CostoMateriales,
                                                                costomo = (double)informemante.CostoMo,
                                                                costototal = (double)informemante.CostoTotal, 
                                                                bienes = bienes.Desripcion

                                                            }).ToList();

                foreach (var historialmante in lista)
                {
                   
                    c1.Phrase = new Phrase(historialmante.fechacadena, parrafo5);
                    c2.Phrase = new Phrase(historialmante.nombretecnico, parrafo5);
                    c3.Phrase = new Phrase("$" + historialmante.costomo.ToString(), parrafo5);
                    c4.Phrase = new Phrase("$" + historialmante.costomateriales.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + historialmante.costototal.ToString(), parrafo5);
                    c6.Phrase = new Phrase(historialmante.descripcion, parrafo5);



                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                  


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

        //FIN DE REPORTES DE HISTORIAL DE MANTENIMIENTO CON ID



    }
}
