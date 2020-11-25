using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.StaticFiles;
using ASGARDAPI.Models;
using System.Collections.Generic;
using ASGARDAPI.Clases;
using System.Linq;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;

namespace ASGARDAPI.Controllers
{
    public class ReportesController : Controller
    {
        //Inicializamos (aACA INICIALIZAS LA VARIABLE env)
        private readonly IHostingEnvironment env;

        //Creamos el contructor (ESTE CONSTRUCTOR TE FALTABA)
        public ReportesController(IHostingEnvironment _evn)
        {
            env = _evn;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/Reporte/reporte")]
        public async Task<IActionResult> reporte()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            //Instanciamos la clase para el paginado y la fecha de impresión
            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte PDF");
            doc.Open();

            //Inicia cuerpo del reporte

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 13f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
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
                Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.IdCooperativa == 1).First();
                oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                oCooperativaAF.nombre = oCooperativa.Nombre;
                oCooperativaAF.descripcion = oCooperativa.Descripcion;
                oCooperativaAF.logo = oCooperativa.Logo;

                //Se agrega el encabezado
                doc.Add(new Paragraph(oCooperativa.Descripcion, parrafo2) { Alignment = Element.ALIGN_CENTER });
                doc.Add(new Paragraph(oCooperativa.Nombre, parrafo3) { Alignment = Element.ALIGN_CENTER });
            }

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ÁREAS DE NEGOCIO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 30f, 45f, 50f, 25f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CORRELATIVO", parrafo2));
            var c2 = new PdfPCell(new Phrase("NOMBRE DE ÁREA", parrafo2));
            var c3 = new PdfPCell(new Phrase("NOMBRE DE SUCURSAL", parrafo2));
            var c4 = new PdfPCell(new Phrase("UBICACIÓN", parrafo2));
            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                IEnumerable<AreasDeNegocioAF> lista = (from area in bd.AreaDeNegocio
                                                       join sucursal in bd.Sucursal
                                                        on area.IdSucursal equals sucursal.IdSucursal
                                                       where area.Dhabilitado == 1
                                                       orderby sucursal.Correlativo
                                                       select new AreasDeNegocioAF
                                                       {
                                                           IdAreaNegocio = area.IdAreaNegocio,
                                                           Nombre = area.Nombre,
                                                           Correlativo = area.Correlativo,
                                                           nombreSucursal = sucursal.Nombre,
                                                           ubicacion = sucursal.Ubicacion
                                                       }).ToList();
                foreach (var area in lista)
                {
                    c1.Phrase = new Phrase(area.Correlativo, parrafo5);
                    c2.Phrase = new Phrase(area.Nombre, parrafo5);
                    c3.Phrase = new Phrase(area.nombreSucursal, parrafo5);
                    c4.Phrase = new Phrase(area.ubicacion, parrafo5);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                }
            }
            doc.Add(tbl);

            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");

        }


        [HttpGet]
        [Route("api/Reporte/reporteLogo")]
        public async Task<IActionResult> reporteLogo()
        {
            Document doc = new Document(PageSize.Letter);
            doc.SetMargins(40f, 40f, 40f, 40f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            var pe = new PageEventHelper();
            writer.PageEvent = pe;

            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte PDF");
            doc.Open();


            //Logo de la base de datos
             using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                  CooperativaAF oCooperativaAF = new CooperativaAF();
                  Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.IdCooperativa == 3).First();
                 oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
              var nombre=   oCooperativaAF.nombre = oCooperativa.Nombre;
                // oCooperativaAF.imagen = oCooperativa.Logo;

                //Convertir la imagen en un array
                //     string image = oCooperativa.Logo;
                //     byte[] imageBytes = Encoding.UTF8.GetBytes(image);
                //     var encodeData = Convert.ToBase64String(imageBytes);

                //var image = Convert.FromBase64String(oCooperativa.Logo);

                //     iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(oCooperativaAF.logo);
                BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
                iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 13f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
                doc.Add(new Paragraph(nombre, parrafo) { Alignment = Element.ALIGN_CENTER });
                string base64string = oCooperativa.Logo;
                try{
                    iTextSharp.text.Image gif = null;
                    //  Convert base64string to bytes array
                    //Byte[] bytes = Convert.FromBase64String(base64string);
                    gif = iTextSharp.text.Image.GetInstance(base64string);
                    gif.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
                    gif.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    gif.BorderColor = iTextSharp.text.BaseColor.White;
                    gif.ScaleToFit(170f, 100f);

                    doc.Add(gif);

                }
                catch (DocumentException dex)
                {
                    //log exception here
                }
            
            }

         

            writer.Close();
            doc.Close();
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/pdf");
        }

    }
}
