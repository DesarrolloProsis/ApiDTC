namespace ApiDTC.Services
{
    using System;
    using System.IO;

    public class ApiLogger
    {
        //TODO -> Por plaza, con código y petición origen
        /*
         1 = SQL Error
         2 = IO Error
         3 = Genérico
         4 = Eventos 
         5 = PdfException
         6 = ArgumentOutOfRange
         7 = DocumentException
         */
        public void WriteLog(string plaza, Exception info, string metodo, int tipo)
        {
            string directorioLogsBitacora = $@"C:\Bitacora\{plaza}";
            string logFile = $@"{directorioLogsBitacora}\{plaza}_log.txt";
            
            if (!Directory.Exists(directorioLogsBitacora))
                Directory.CreateDirectory(directorioLogsBitacora);

            string error = $"{metodo} [{tipo}] {DateTime.Now:dd/MM/yyyy hh:mm:ss}: Line: {Convert.ToInt32(info.StackTrace.Substring(info.StackTrace.LastIndexOf(" ") + 1))} {info.Message}.\n";
            if (File.Exists(logFile))                
                File.AppendAllText(logFile, error);
            else File.WriteAllText(logFile, error);
        }

        public void WriteLog(string plaza, string metodo, int tipo, string info)
        {
            string directorioLogsBitacora = $@"C:\Bitacora\{plaza}";
            string logFile = $@"{directorioLogsBitacora}\{plaza}_log.txt";

            if (!Directory.Exists(directorioLogsBitacora))
                Directory.CreateDirectory(directorioLogsBitacora);

            string evento = $"{metodo} [4] {DateTime.Now:dd/MM/yyyy hh:mm:ss}: {info}\n";
            if (File.Exists(logFile))
                File.AppendAllText(logFile, evento);
            else File.WriteAllText(logFile, evento);
        }
    }
}