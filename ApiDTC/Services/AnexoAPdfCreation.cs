using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using ApiDTC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ApiDTC.Services
{
    public class AnexoAPdfCreation
    {

        #region Attributes
        private readonly string _clavePlaza;

        private readonly ApiLogger _apiLogger;
        #endregion

        #region Constructors

        public AnexoAPdfCreation(string clavePlaza, ApiLogger apiLogger)
        {
            _clavePlaza = clavePlaza;
            _apiLogger = apiLogger;
        }

        #endregion

        #region Methods

        public Response NewPdf(string folder)
        {
            string directory, filename, path;
            directory = $@"{folder}\{_clavePlaza.ToUpper()}\Almacén";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);

            }

            filename = $"AnexoA.pdf";

            path = Path.Combine(directory, filename);

            //File in use
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                if (File.Exists(path))
                {
                    if (FileInUse(path))
                    {
                        return new Response
                        {
                            Message = $"Error: Archivo {filename}.pdf en uso o inaccesible",
                            Result = null
                        };
                    }
                }
            }
            catch (IOException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "AnexoAPdfCreation: NewPdf", 2);
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                };
            }

            Document doc = new Document();
            try
            {
                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    doc.SetPageSize(new Rectangle(793.701f, 609.4488f));
                    doc.SetMargins(30f, 30f, 30f, 30f);
                    doc.AddAuthor("PROSIS");
                    doc.AddTitle("ANEXO A ACTA ADMINISTRATIVA INFORMATIVA (ENTREGA-RECEPCIÓN)");


                    PdfWriter writer = PdfWriter.GetInstance(doc, myMemoryStream);
                    writer.PageEvent = new PageEventHelperVerticalAnexo();
                    writer.Open();
                    doc.Open();

                    doc.Close();
                    writer.Close();
                    byte[] content = myMemoryStream.ToArray();

                    using (FileStream fs = File.Create(path))
                    {
                        fs.Write(content, 0, (int)content.Length);
                    }
                }
            }
            catch (IOException ex)
            {
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: NewPdf", 2);
                return new Response
                {
                    Message = $"Error: {ex.Message}.",
                    Result = null
                };
            }
            return new Response
            {
                Message = "Ok",
                Result = path
            };
        }


        private bool FileInUse(string file)
        {
            bool fileInUse = false;
            try
            {
                FileStream fs = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (PdfException ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: FileInUse", 5);
                return true;
            }
            catch (Exception ex)
            {
                _apiLogger.WriteLog(_clavePlaza, ex, "InventarioPdfCreation: FileInUse", 3);
                return true;
            }
            return fileInUse;
        }
        #endregion

    }
 }
