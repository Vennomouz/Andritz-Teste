using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {
            byte[] bytes = null;
            ArrayList dates = new ArrayList();
            string dtReplace;
            int i = 0;

            string linha = null;
            var reader = new StreamReader(input, encoding, true);

            while (null != (linha = await reader.ReadLineAsync()))
            {
                var rx = new Regex(@"\d{2}:\d{2}:\d{2},\d{3}");
                MatchCollection matches = rx.Matches(linha);

                if(matches.Count > 0)
                {
                    foreach (Match match in rx.Matches(linha))
                    {
                        dtReplace = match.Value.Replace(",", ".");
                        dates.Add(dtReplace);
                    }

                    DateTime dt1 = DateTime.ParseExact(dates[i].ToString(), "hh:mm:ss.fff", null);
                    DateTime dt2 = DateTime.ParseExact(dates[i + 1].ToString(), "hh:mm:ss.fff", null);

                    bytes = Encoding.UTF8.GetBytes(dt1.Add(timeSpan).ToString("HH:mm:ss.fff").Replace(".", ",") + " --> " + dt2.Add(timeSpan).ToString("HH:mm:ss.fff").Replace(".", ",") + "\n");
                    i = i+2;
                }
                else
                {
                    bytes = Encoding.UTF8.GetBytes(linha + "\n");
                }

                output.Write(bytes, 0, bytes.Length);

            }


        }
    }
}
