using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.StaticFiles;



namespace ASGARDAPI.Controllers
{
    public class ReportesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //Método prueba
        [HttpGet]
        [Route("api/Reporte/report")]
        public async Task<IActionResult> reportePdf()
        {
            // string filename = "prueba";
            string textPDF = "Hola mundo";
            // var dirFinal = @"C:\PDF\file.pdf";

            var mStr = new MemoryStream();
            var msg = "";


            //var objFichero = new System.IO.StreamReader(archivos, System.Text.Encoding.Default);
            // var file = "prueba"; //Creamos el objeto documento PDF
            var documentoPDF = new Document();
            try
            {
                // var writer = PdfWriter.GetInstance(documentoPDF, HttpContext.Current.Response.OutputStream);

                PdfWriter.GetInstance(documentoPDF, mStr);
                //Stream stream = new MemoryStream(); 

                documentoPDF.Open();
                //Escribimos el texto en el objeto documento PDF
                documentoPDF.Add(new Paragraph(textPDF, FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, Font.NORMAL)));

                // file = String.Format("attachment;filename={0}", filename);
                //Cerramos el objeto documento, guardamos y creamos el PDF
                documentoPDF.Close();
                mStr.Seek(0, SeekOrigin.Begin);






                //HttpResponse.

                //System.Web.HttpUtility = "application/pdf";
                //HttpContext.Current.Response.AddHeader("content-disposition", file + ".pdf");
                //HttpContext.Current.Response.Write(documentoPDF);
                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }

            return File(mStr.ToArray(), "application/pdf");




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
            doc.Add(new Phrase("Esto es una prueba descargando el pdf montano se la come"));

            writer.Close();
            doc.Close();
            file.Dispose();
            var pdf = new FileStream("hola mundo.pdf", FileMode.Open, FileAccess.Read);
            return File(pdf, "application/pdf");


        }


        [HttpGet]
        [Route("api/Reporte/reporPDF")]
        public async Task<IActionResult> reporPDF()
        {
            var msg = "";
            FileStream fs;
            var path = "";
            var dirFinal = "";


            try
            {

                dirFinal = @"/Users/kevinreinaldomontanoorantes/Desktop/file.pdf";
                //  var archivos = " Esto es una Prueba";
                //var objFichero = new System.IO.StreamReader(archivos, System.Text.Encoding.Default);
                var filetext = "Esto es una prueba";

                //Creamos el objeto documento PDF
                var documentoPDF = new Document();
                PdfWriter.GetInstance(documentoPDF, new FileStream(dirFinal, FileMode.Create));
                documentoPDF.Open();
                //Escribimos el texto en el objeto documento PDF
                documentoPDF.Add(new Paragraph(filetext, FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, Font.NORMAL)));


                //Cerramos el objeto documento, guardamos y creamos el PDF
                documentoPDF.Close();
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }

            msg = "Exito";


            // Afterwards file is converted into a stream
            //  path = Path.Combine("/Users/kevinreinaldomontanoorantes/desktop", "file.pdf");
            //   fs = new FileStream(path, FileMode.Open);

            // Return the file. A byte array can also be used instead of a stream
            //  return File(fs, "application/pdf", "file.pdf");


               var file = @"/Users/kevinreinaldomontanoorantes/Desktop/file.pdf";
                
                
                
                //var filePath = Path.Combine(uploads, file);
                var filePath = @"/Users/kevinreinaldomontanoorantes/Desktop/file.pdf";
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return File(memory, GetContentType(filePath), file);
            }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}
