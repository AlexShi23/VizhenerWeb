using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VizhenerWeb.Models;

namespace VizhenerWeb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Update(VizhenerViewModel model, IFormFile file)
        {
            if (file == null) 
            {
                if (model == null || string.IsNullOrEmpty(model.Message))
                    return RedirectToAction("Index");
            }
            else
            {
                if (file.Length > 0)
                {
                    try
                    {
                        model.Message = GetMessageFromFile(file);
                    }
                    catch (NotSupportedException)
                    {
                        model.Error = "Ошибка! Файл имеет расширение отличное от .docx!";
                    }
                    catch (Exception)
                    {
                        model.Error = "Возникла ошибка при чтении файла!";
                    }
                }
                else
                {
                    model.Error = "Ошибка! Файл пустой!";
                }
            }
            return View("Index", model);
        }
        private string GetMessageFromFile(IFormFile file)
        {
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == ".docx")
            {
                using (var fileStream = file.OpenReadStream())
                {
                    WordprocessingDocument wordDocument = WordprocessingDocument.Open(fileStream, false);
                    Body body = wordDocument.MainDocumentPart.Document.Body;
                    return body.InnerText;
                }
            }
            else
            { 
                throw new NotSupportedException("Ошибка! Файл имеет расширение отличное от .docx!");
            }
        }
        public IActionResult DownloadFile(VizhenerViewModel model)
        {
            try
            {
                MemoryStream mem = new MemoryStream();
                WordprocessingDocument wordDocument = WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true);

                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text(model.Result));
                mainPart.Document.Save();
                wordDocument.Close();
                mem.Position = 0;

                return File(mem, "application/docx", "result.docx");

            }
            catch (Exception)
            {
                model.Error = "Ошибка при создании файла";
                return View("Index", model);
            }
        }
    }
}
