namespace ApiDTC.Services
{
    using System;
    using System.IO;

    public class ApiLogger
    {
        public void WriteLog(Exception info, string metodo)
        {
            if (File.Exists(@"C:\temporal\Log.txt"))
            {
                string[] lineas = File.ReadAllLines(@"C:\temporal\Log.txt");
                lineas[0] = $"{metodo}. {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {Convert.ToInt32(info.StackTrace.Substring(info.StackTrace.LastIndexOf(" ") + 1))}. {info.Message}.\n{lineas[0]}";
                File.Delete(@"C:\temporal\Log.txt");
                File.WriteAllLines(@"C:\temporal\Log.txt", lineas);
            }
            else File.WriteAllText(@"C:\temporal\Log.txt", $"{metodo}. {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {Convert.ToInt32(info.StackTrace.Substring(info.StackTrace.LastIndexOf(" ") + 1))}. {info.Message}.");
        }
        public void WriteLog(string info)
        {
            if (File.Exists(@"C:\temporal\Log.txt"))
            {
                string[] lineas = File.ReadAllLines(@"C:\temporal\Log.txt");
                lineas[0] = $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {info}.\n{lineas[0]}";
                File.Delete(@"C:\temporal\Log.txt");
                File.WriteAllLines(@"C:\temporal\Log.txt", lineas);
            }
            else File.WriteAllText(@"C:\temporal\Log.txt", $"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}. {info}.");
        }
    }
}