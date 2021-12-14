using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApiDTC.Services

{
    
    public class VerificarPDF
    {
        #region Attributes
        #endregion
        public VerificarPDF()
        {
        
        }

        public bool IsPDFOk(string fileName)
        {
            byte[] buffer = null;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buffer = br.ReadBytes((int)numBytes);

            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);

            if (buffer[0] == 37 && buffer[1] == 80 && buffer[2] == 68 && buffer[3] == 70 &&
                    buffer[numBytes - 2] == 70 && buffer[numBytes - 3] == 79 && buffer[numBytes - 4] == 69)
            {
                fs.Close();
                return true;
            }
            fs.Close();
            return false;
        }
    }
}
