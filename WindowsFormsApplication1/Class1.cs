using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication1
{
    class CookieSaver
    {
        public static void saveCookies(string path, string cookie)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(cookie);
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {

            }

        }

        public static void loadCookies(string path, out string cookie)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                String line = sr.ReadLine();
                cookie = line;
                sr.Close();
            }
            catch(Exception)
            {
                cookie = "";
            }
        }
    }
}
