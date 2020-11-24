using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

        //Creación de reporte
        [HttpGet]
        [Route("api/Reporte/reporte")]
        public async Task<IActionResult> reporte()
        {

            Document doc = new Document(PageSize.Letter);
            FileStream file = new FileStream("hola mundo.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            PdfWriter writer = PdfWriter.GetInstance(doc, file);
            doc.AddAuthor("Asgard");
            doc.AddTitle("Reporte PDF");
            doc.Open();

            //Estilo y fuente personalizada
            BaseFont fuente = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo = new iTextSharp.text.Font(fuente, 13f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente2 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo2 = new iTextSharp.text.Font(fuente2, 10f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente3 = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo3 = new iTextSharp.text.Font(fuente3, 15f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));
            BaseFont fuente4 = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1250, true);
            iTextSharp.text.Font parrafo4 = new iTextSharp.text.Font(fuente, 11f, iTextSharp.text.Font.NORMAL, new BaseColor(0, 0, 0));

            //Encabezado
            doc.Add(new Paragraph("ASOCIACIÓN COOPERTIVA DE APROVICIONAMIENTO AGROPECUARIO, AHORRO,", parrafo2) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph("CRÉDITO Y CONSUMO DE SAN SEBASTIÁN DE RESPONSABILIDAD LIMITADA", parrafo2) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph("ACAASS DE R.L.", parrafo3) { Alignment = Element.ALIGN_CENTER });

            //Línea separadora
            Chunk linea = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.Black, Element.ALIGN_CENTER, 1f));
            doc.Add(linea);
            doc.Add(new Paragraph("REPORTE DE ÁREAS DE NEGOCIO", parrafo) { Alignment = Element.ALIGN_CENTER });

            //Agregamos la fecha y la hora del reporte
            doc.Add(new Paragraph(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), parrafo4) { Alignment = Element.ALIGN_CENTER });
            //Espacio en blanco
            doc.Add(Chunk.Newline);

            //Agregamos una tabla
            var tbl = new PdfPTable(new float[] { 30f, 45f, 50f, 25f }) { WidthPercentage = 100f };
            var c1 = new PdfPCell(new Phrase("CORRELATIVO"));
            var c2 = new PdfPCell(new Phrase("NOMBRE DE ÁREA"));
            var c3 = new PdfPCell(new Phrase("NOMBRE DE SUCURSAL"));
            var c4 = new PdfPCell(new Phrase("UBICACIÓN"));
            //Agregamos a la tabla las celdas
            tbl.AddCell(c1);
            tbl.AddCell(c2);
            tbl.AddCell(c3);
            tbl.AddCell(c4);

            //Extraemos de la base y llenamos las celdas
            using (BDAcaassAFContext bd = new BDAcaassAFContext())
            {
                //Logo
                //     CooperativaAF oCooperativaAF = new CooperativaAF();
                //   Cooperativa oCooperativa = bd.Cooperativa.Where(p => p.IdCooperativa == 2).First();
                //   oCooperativaAF.idcooperativa = oCooperativa.IdCooperativa;
                //   oCooperativaAF.logo = oCooperativa.Logo;

                //   byte[] _imagen_bytes = ctx.Tabla.Select(x => x.imagen).First();

                //  using (var ms = new MemoryStream(_imagen_bytes))
                //  {
                //  Image returnImage = Image.FromStream(ms);
                // }


                //  iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(oCooperativa.Logo);
                //  logo.ScaleAbsolute(75, 75);
                //  doc.Add(logo);

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
                    c1.Phrase = new Phrase(area.Correlativo);
                    c2.Phrase = new Phrase(area.Nombre);
                    c3.Phrase = new Phrase(area.nombreSucursal);
                    c4.Phrase = new Phrase(area.ubicacion);
                    //Agregamos a la tabla
                    tbl.AddCell(c1);
                    tbl.AddCell(c2);
                    tbl.AddCell(c3);
                    tbl.AddCell(c4);
                }
            }
            doc.Add(tbl);

            //YA FUNCIONA EL env.WebRootPath, solo crea otro metodo y proba solo la imagen 
            // iTextSharp.text.Image logo=iTextSharp.text.Image.GetInstance(Path.Combine(env.WebRootPath))

            writer.Close();
            doc.Close();
            file.Dispose();
            var pdf = new FileStream("hola mundo.pdf", FileMode.Open, FileAccess.Read);
            return File(pdf, "application/pdf");


        }

    }
}
