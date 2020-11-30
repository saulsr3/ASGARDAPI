using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ASGARDAPI.Models;
using ASGARDAPI.Clases;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ASGARDAPI.Controllers
{
    public class ReportesControlActivoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Inicio de reportes de control de activos

        //Reporte activos asignados

        [HttpGet]
        [Route("api/Reporte/activosAsignadosPdf")]
        public async Task<IActionResult> activosAsignadosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos asignados");
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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ACTIVOS ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 17f, 11f, 27f, 25f, 20f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("ÁREA DE NEGOCIO", parrafo2));
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
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          join resposable in bd.Empleado
                                          on activo.IdResponsable equals resposable.IdEmpleado
                                          join area in bd.AreaDeNegocio
                                          on resposable.IdAreaDeNegocio equals area.IdAreaNegocio
                                          where (activo.EstadoActual != 0) && activo.EstaAsignado == 1

                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                              AreaDeNegocio = area.Nombre,
                                              Responsable = resposable.Nombres + " " + resposable.Apellidos
                                          }).ToList();

                foreach (var activoA in lista)
                {
                    c1.Phrase = new Phrase(activoA.Codigo, parrafo5);
                    c2.Phrase = new Phrase(activoA.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoA.Descripcion, parrafo5);
                    c4.Phrase = new Phrase(activoA.AreaDeNegocio, parrafo5);
                    c5.Phrase = new Phrase(activoA.Responsable, parrafo5);
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

        //Fin reporte activos asignados

        //Reporte activos no asignados

        [HttpGet]
        [Route("api/Reporte/activosNoAsignadosPdf")]
        public async Task<IActionResult> activosNoAsignadosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos no asignados");
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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ACTIVOS NO ASIGNADOS", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 20f, 11f, 29f, 40f}) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("NO. FORMULARIO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<NoAsignadosAF> lista = (from activo in bd.ActivoFijo
                                                    join noFormulario in bd.FormularioIngreso
                                                    on activo.NoFormulario equals noFormulario.NoFormulario
                                                    join clasif in bd.Clasificacion
                                                    on activo.IdClasificacion equals clasif.IdClasificacion
                                                    where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.EstaAsignado == 0
                                                    select new NoAsignadosAF
                                                    {
                                                        IdBien = activo.IdBien,
                                                        NoFormulario = noFormulario.NoFormulario,
                                                        fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                        Desripcion = activo.Desripcion,
                                                        Clasificacion = clasif.Clasificacion1

                                                    }).ToList();

                foreach (var activoNoA in lista)
                {
                    c1.Phrase = new Phrase(activoNoA.NoFormulario.ToString(), parrafo5);
                    c2.Phrase = new Phrase(activoNoA.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(activoNoA.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(activoNoA.Desripcion, parrafo5);
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

        //Fin reporte activos no asignados

        //Reporte edificios e instalaciones

        [HttpGet]
        [Route("api/Reporte/edificiosInstalacionesPdf")]
        public async Task<IActionResult> edificiosInstalacionesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte edficios e instalaciones");
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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE EDIFICIOS E INSTALACIONES", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 16f, 11f, 30f, 43f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion
                                          where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 1 && activo.EstadoActual == 1
                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();

                foreach (var edificio in lista)
                {
                    c1.Phrase = new Phrase(edificio.Codigo, parrafo5);
                    c2.Phrase = new Phrase(edificio.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(edificio.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(edificio.Descripcion, parrafo5);
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

        //Fin reporte edificios e instalaciones

        //Reporte activos intangibles

        [HttpGet]
        [Route("api/Reporte/activosIntangiblesPdf")]
        public async Task<IActionResult> activosIntangiblesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte activos intangibles");
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
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ACTIVOS INTANGIBLES", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 18f, 11f, 29f, 42f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("FECHA", parrafo2));
            var c3 = new PdfPCell(new Phrase("CLASIFICACIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                List<RegistroAF> lista = (from activo in bd.ActivoFijo
                                          join noFormulario in bd.FormularioIngreso
                                          on activo.NoFormulario equals noFormulario.NoFormulario
                                          join clasif in bd.Clasificacion
                                          on activo.IdClasificacion equals clasif.IdClasificacion

                                          where (activo.EstadoActual == 1 || activo.EstadoActual == 2 || activo.EstadoActual == 3) && activo.TipoActivo == 3 && activo.EstadoActual == 1
                                          orderby activo.CorrelativoBien
                                          select new RegistroAF
                                          {
                                              IdBien = activo.IdBien,
                                              Codigo = activo.CorrelativoBien,
                                              fechacadena = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                              Descripcion = activo.Desripcion,
                                              Clasificacion = clasif.Clasificacion1,
                                          }).ToList();

                foreach (var intangible in lista)
                {
                    c1.Phrase = new Phrase(intangible.Codigo, parrafo5);
                    c2.Phrase = new Phrase(intangible.fechacadena, parrafo5);
                    c3.Phrase = new Phrase(intangible.Clasificacion, parrafo5);
                    c4.Phrase = new Phrase(intangible.Descripcion, parrafo5);
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

        //Fin reporte activos intangibles

        //Reporte tarjeta

        [HttpGet]
        [Route("api/Reporte/tarjetaPdf/{idbien}")]
        public async Task<IActionResult> tarjetaPdf(int idbien)
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte tarjeta");
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
            iTextSharp.text.Font parrafo6 = new iTextSharp.text.Font(fuente2, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente8 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo8 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));


            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE TARJETA", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            doc.Add(new Paragraph("CONTROL DE EXISTENCIAS DE MOBILIARIO, EQUIPO E INSTALACIONES", parrafo2) { Alignment = Element.ALIGN_CENTER });
            doc.Add(Chunk.Newline);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                TarjetaDatosAF odatos = new TarjetaDatosAF();
                ActivoFijo oactivo = bd.ActivoFijo.Where(p => p.IdBien == idbien).First();
                FormularioIngreso oformulario = bd.FormularioIngreso.Where(p => p.NoFormulario == oactivo.NoFormulario).First();
                odatos.fechaAdquicicion = oformulario.FechaIngreso == null ? " " : ((DateTime)oformulario.FechaIngreso).ToString("dd-MM-yyyy");
                odatos.Observaciones = oformulario.Observaciones;
                odatos.codigo = oactivo.CorrelativoBien;
                odatos.descripcion = oactivo.Desripcion;
                odatos.valor = oactivo.ValorAdquicicion.ToString();
                odatos.prima = oactivo.Prima.ToString();
                odatos.plazo = oactivo.PlazoPago;
                odatos.cuota = oactivo.CuotaAsignanda.ToString();
                odatos.interes = oactivo.Intereses.ToString();
                odatos.color = oactivo.Color;
                odatos.modelo = oactivo.Modelo;
                odatos.noSerie = oactivo.NoSerie;
                int tasa = (int)(100 / oactivo.VidaUtil);
                odatos.tasaAnual = tasa.ToString();
                odatos.vidaUtil = oactivo.VidaUtil.ToString();
                odatos.valorResidual = oactivo.ValorResidual.ToString();

                //Cuerpo de la tarjeta
                var tbl1 = new PdfPTable(new float[] { 36f, 35f, 29f }) { WidthPercentage = 100f };
                tbl1.AddCell(new PdfPCell(new Phrase("Fecha de adquisición: " + odatos.fechaAdquicicion, parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Denominación: " + oactivo.Desripcion, parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Código: " + oactivo.CorrelativoBien, parrafo6)) { Border = 0 });
                //*****************************************************
                if (oactivo.IdMarca != null)
                {
                    Marcas oMarcas = bd.Marcas.Where(p => p.IdMarca == oactivo.IdMarca).First();
                    odatos.marca = oMarcas.Marca;
                    tbl1.AddCell(new PdfPCell(new Phrase("Marca: " + oMarcas.Marca, parrafo6)) { Border = 0 });
                }
                else
                {
                    odatos.marca = " ";
                }
                tbl1.AddCell(new PdfPCell(new Phrase("Color: " + oactivo.Color, parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Modelo: " + oactivo.Modelo, parrafo6)) { Border = 0 });
                //*****************************************************
                tbl1.AddCell(new PdfPCell(new Phrase("No de serie: " + odatos.noSerie, parrafo6)) { Border = 0 });
                if (oactivo.IdProveedor != null)
                {
                    Proveedor oProveedor = bd.Proveedor.Where(p => p.IdProveedor == oactivo.IdProveedor).First();
                    string ProvDon;
                    odatos.proveedor = oProveedor.Nombre;
                    odatos.direccion = oProveedor.Direccion;
                    odatos.telefono = oProveedor.Telefono;
                    odatos.isProvDon = 1;
                    if (odatos.isProvDon == 1)
                    {
                        ProvDon = "Proveedor: ";
                    }
                    else
                    {
                        ProvDon = "Donante: ";
                    }
                    tbl1.AddCell(new PdfPCell(new Phrase(ProvDon + oProveedor.Nombre, parrafo6)) { Border = 0 });
                    tbl1.AddCell(new PdfPCell(new Phrase("Dirección: " + oProveedor.Direccion, parrafo6)) { Border = 0 });
                    tbl1.AddCell(new PdfPCell(new Phrase("Teléfono: " + oProveedor.Telefono, parrafo6)) { Border = 0 });
                }
                else
                {
                    Donantes oDonante = bd.Donantes.Where(p => p.IdDonante == oactivo.IdDonante).First();
                    string ProvDon;
                    odatos.proveedor = oDonante.Nombre;
                    odatos.direccion = oDonante.Direccion;
                    odatos.telefono = oDonante.Telefono;
                    odatos.isProvDon = 2;
                    if (odatos.isProvDon == 1)
                    {
                        ProvDon = "Proveedor: ";
                    }
                    else
                    {
                        ProvDon = "Donante: ";
                    }
                    tbl1.AddCell(new PdfPCell(new Phrase(ProvDon + oDonante.Nombre, parrafo6)) { Border = 0 });
                    tbl1.AddCell(new PdfPCell(new Phrase("Dirección: " + oDonante.Direccion, parrafo6)) { Border = 0 });
                    tbl1.AddCell(new PdfPCell(new Phrase("Teléfono: " + oDonante.Telefono, parrafo6)) { Border = 0 });
                }
                tbl1.AddCell(new PdfPCell(new Phrase("Más interes del: " + odatos.interes + "%", parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Vida util segun referencia de fábrica: " + odatos.vidaUtil + " años", parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Tasa anual de depreciación: " + odatos.tasaAnual + "%", parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Valor residual: " + odatos.valorResidual, parrafo6)) { Border = 0 });
                tbl1.AddCell(new PdfPCell(new Phrase("Observaciones: " + odatos.Observaciones, parrafo6)) { Border = 0 });
                doc.Add(tbl1);

                //Tabla de transacciones
                doc.Add(new Paragraph("MOVIMIENTO DEL VALOR - DEPRECIACIONES - GASTOS DE CONSEVACIÓN", parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(Chunk.Newline);

                //Para el cuadro de transacciones
                //Agregamos una tabla
                var tbl = new PdfPTable(new float[] { 11f, 12f, 10f, 26f, 15f, 15f, 11f }) { WidthPercentage = 100f };
                var c1 = new PdfPCell(new Phrase("FECHA", parrafo8));
                var c2 = new PdfPCell(new Phrase("CONCEPTO", parrafo8));
                var c3 = new PdfPCell(new Phrase("VALOR", parrafo8));
                var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN Y MEJORAS", parrafo8));
                var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN ANUAL", parrafo8));
                var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo8));
                var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo8));
                //Agregamos a la tabla las celdas 
                tbl.AddCell(c1);
                tbl.AddCell(c2);
                tbl.AddCell(c3);
                tbl.AddCell(c4);
                tbl.AddCell(c5);
                tbl.AddCell(c6);
                tbl.AddCell(c7);

                IEnumerable<TarjetaTransaccionesAF> ListaTransacciones = (from tarjeta in bd.TarjetaDepreciacion
                                                                          where tarjeta.IdBien == idbien
                                                                          orderby tarjeta.IdTarjeta
                                                                          select new TarjetaTransaccionesAF
                                                                          {
                                                                              id = tarjeta.IdTarjeta,
                                                                              idBien = (int)tarjeta.IdBien,
                                                                              fecha = tarjeta.Fecha == null ? " " : ((DateTime)tarjeta.Fecha).ToString("dd-MM-yyyy"),
                                                                              concepto = tarjeta.Concepto,
                                                                              montoTransaccion = Math.Round((double)tarjeta.Valor, 2),
                                                                              depreciacionAnual = Math.Round((double)tarjeta.DepreciacionAnual, 2),
                                                                              depreciacionAcumulada = Math.Round((double)tarjeta.DepreciacionAcumulada, 2),
                                                                              valorActual = Math.Round((double)tarjeta.ValorActual, 2),
                                                                              valorTransaccion = Math.Round((double)tarjeta.ValorTransaccion, 2)
                                                                          }).ToList();

                foreach (var tarjeta in ListaTransacciones)
                {
                    c1.Phrase = new Phrase(tarjeta.fecha, parrafo5);
                    c2.Phrase = new Phrase(tarjeta.concepto, parrafo5);

                    c3.Phrase = new Phrase("$ " + tarjeta.valorTransaccion.ToString(), parrafo5);

                    c4.Phrase = new Phrase("$ " + tarjeta.montoTransaccion.ToString(), parrafo5);

                    c5.Phrase = new Phrase("$ " + tarjeta.depreciacionAnual.ToString(), parrafo5);

                    c6.Phrase = new Phrase("$ " + tarjeta.depreciacionAcumulada.ToString(), parrafo5);

                    c7.Phrase = new Phrase("$ " + tarjeta.valorActual.ToString(), parrafo5);
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
        //Fin reporte tarjeta

        //Reporte cuadro de control activos asignados

        [HttpGet]
        [Route("api/Reporte/cuadroControlActivosPdf")]
        public async Task<IActionResult> cuadroControlActivosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 7.5f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 10f, 15f, 11f, 11f, 12f, 12f, 8f, 10f, 13f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));
            var c8 = new PdfPCell(new Phrase("UBICACIÓN", parrafo2));
            var c9 = new PdfPCell(new Phrase("RESPONSABLE", parrafo2));

            //Agregamos a la tabla las celdas 
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);
            tbl.AddCell(c5);
            tbl.AddCell(c6);
            tbl.AddCell(c7);
            tbl.AddCell(c8);
            tbl.AddCell(c9);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<CuadroControlAF> listacuadro = (
                                                             from tarjeta in bd.TarjetaDepreciacion
                                                             group tarjeta by tarjeta.IdBien into bar
                                                             join cuadro in bd.ActivoFijo
                                                             on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                             join noFormulario in bd.FormularioIngreso
                                                             on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                             join clasif in bd.Clasificacion
                                                             on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                             join resposable in bd.Empleado
                                                             on cuadro.IdResponsable equals resposable.IdEmpleado
                                                             join area in bd.AreaDeNegocio
                                                             on resposable.IdAreaDeNegocio equals area.IdAreaNegocio

                                                              //where cuadro.EstadoActual == 1 && cuadro.EstaAsignado == 1
                                                              select new CuadroControlAF()
                                                             {
                                                                 idbien = cuadro.IdBien,
                                                                 codigo = cuadro.CorrelativoBien,
                                                                 descripcion = cuadro.Desripcion,
                                                                 valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                 fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                 valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                 depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                 depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),
                                                                 ubicacion = area.Nombre,
                                                                 responsable = resposable.Nombres + " " + resposable.Apellidos

                                                             }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
                    c8.Phrase = new Phrase(cuadro.ubicacion, parrafo5);
                    c9.Phrase = new Phrase(cuadro.responsable, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                    tbl.AddCell(c5);
                    tbl.AddCell(c6);
                    tbl.AddCell(c7);
                    tbl.AddCell(c8);
                    tbl.AddCell(c9);
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

        //Fin reporte cuadro de control activos asignados

        //Reporte cuadro de control de edificios

        [HttpGet]
        [Route("api/Reporte/cuadroControlEdificiosPdf")]
        public async Task<IActionResult> cuadroControlEdificiosPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 12f, 15f, 11f, 11f, 12f, 12f, 8f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));

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
                IEnumerable<CuadroControlAF> listacuadro = (
                                                               from tarjeta in bd.TarjetaDepreciacion
                                                               group tarjeta by tarjeta.IdBien into bar
                                                               join cuadro in bd.ActivoFijo
                                                               on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                               join noFormulario in bd.FormularioIngreso
                                                               on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                               join clasif in bd.Clasificacion
                                                               on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                               where (cuadro.EstadoActual == 1 || cuadro.EstadoActual == 2 || cuadro.EstadoActual == 3) && cuadro.TipoActivo == 1 && cuadro.EstaAsignado == 1
                                                               select new CuadroControlAF()
                                                               {
                                                                   idbien = cuadro.IdBien,
                                                                   codigo = cuadro.CorrelativoBien,
                                                                   descripcion = cuadro.Desripcion,
                                                                   valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                   fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                   valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                   depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                   depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),

                                                               }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
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

        //Fin reporte cuadro de control de edificios

        //Reporte cuadro de control intangibles



        [HttpGet]
        [Route("api/Reporte/cuadroControlIntangiblesPdf")]
        public async Task<IActionResult> cuadroControlIntangiblesPdf()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte cuadro de control");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 12f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente4, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Para las celdas
            BaseFont fuente5 = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, true);
            iTextSharp.text.Font parrafo5 = new iTextSharp.text.Font(fuente5, 8f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                CooperativaAF oCooperativaAF = new CooperativaAF();
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.Dhabilitado == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion.ToUpper(), parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE CUADRO DE CONTROL DE BIENES DE LA PROPIEDAD PLANTA Y EQUIPO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 12f, 15f, 11f, 11f, 12f, 12f, 8f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CÓDIGO", parrafo2));
            var c2 = new PdfPCell(new Phrase("DESCRIPCIÓN", parrafo2));
            var c3 = new PdfPCell(new Phrase("FECHA DE ADQUISICIÓN", parrafo2));
            var c4 = new PdfPCell(new Phrase("VALOR DE ADQUISICIÓN", parrafo2));
            var c5 = new PdfPCell(new Phrase("DEPRECIACIÓN", parrafo2));
            var c6 = new PdfPCell(new Phrase("DEPRECIACIÓN ACUMULADA", parrafo2));
            var c7 = new PdfPCell(new Phrase("VALOR ACTUAL", parrafo2));

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
                IEnumerable<CuadroControlAF> listacuadro = (
                                                             from tarjeta in bd.TarjetaDepreciacion
                                                             group tarjeta by tarjeta.IdBien into bar
                                                             join cuadro in bd.ActivoFijo
                                                             on bar.FirstOrDefault().IdBien equals cuadro.IdBien
                                                             join noFormulario in bd.FormularioIngreso
                                                             on cuadro.NoFormulario equals noFormulario.NoFormulario
                                                             join clasif in bd.Clasificacion
                                                             on cuadro.IdClasificacion equals clasif.IdClasificacion
                                                             where (cuadro.EstadoActual == 1 || cuadro.EstadoActual == 2 || cuadro.EstadoActual == 3) && cuadro.TipoActivo == 3 && cuadro.EstaAsignado == 1
                                                             select new CuadroControlAF()
                                                             {
                                                                 idbien = cuadro.IdBien,
                                                                 codigo = cuadro.CorrelativoBien,
                                                                 descripcion = cuadro.Desripcion,
                                                                 valoradquisicion = (double)cuadro.ValorAdquicicion,
                                                                 fechaadquisicion = noFormulario.FechaIngreso == null ? " " : ((DateTime)noFormulario.FechaIngreso).ToString("dd-MM-yyyy"),
                                                                 valoractual = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().ValorActual), 2),
                                                                 depreciacion = Math.Round(((double)bar.OrderByDescending(x => x.IdTarjeta).First().DepreciacionAnual), 2),
                                                                 depreciacionacumulada = Math.Round(((double)bar.Sum(x => x.DepreciacionAnual)), 2),

                                                             }).ToList();

                foreach (var cuadro in listacuadro)
                {
                    c1.Phrase = new Phrase(cuadro.codigo, parrafo5);
                    c2.Phrase = new Phrase(cuadro.descripcion, parrafo5);
                    c3.Phrase = new Phrase(cuadro.fechaadquisicion, parrafo5);
                    c4.Phrase = new Phrase("$" + cuadro.valoradquisicion.ToString(), parrafo5);
                    c5.Phrase = new Phrase("$" + cuadro.depreciacion.ToString(), parrafo5);
                    c6.Phrase = new Phrase("$" + cuadro.depreciacionacumulada.ToString(), parrafo5);
                    c7.Phrase = new Phrase("$" + cuadro.valoractual.ToString(), parrafo5);
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

        //Fin reporte cuadro de control intangibles
    }
}
