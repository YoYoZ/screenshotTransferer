using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace screeenshotSender
{
    public class Logger
    {
        StreamWriter sr;
        bool debug;
        private string formedString;
        public Logger(string path, bool debug)
        {
            this.debug = debug;
            if (debug)
            {
                string pathh = path + "\\log.txt";
                sr = new StreamWriter(pathh);
            }
        }
        public void writeParamToLog(string input)
        {
            if (debug)
            {
                sr.WriteLine("[" + DateTime.Now + "] " + input);
                sr.Flush();
            }
        }
        public void writeParamToLog(string[] input, modes m, char customSeparator = ' ')
        {
            if (debug)
            {
                switch (m)
                {
                    case modes.WriteInLine:
                        foreach (string inn in input)
                        {
                            formedString = formedString + " ( " + inn + " ) ";
                        }
                        sr.WriteLine("[" + DateTime.Now + "] " + formedString);
                        break;
                    case modes.WriteWithEnters:
                        foreach (string inn in input)
                        {
                            formedString = formedString + " ( " + inn + " ) " + "\n";
                        }
                        sr.WriteLine("[" + DateTime.Now + "] " + formedString);
                        break;
                    case modes.WriteWithCustomSeparator:
                        foreach (string inn in input)
                        {
                            formedString = formedString + customSeparator + " ( " + inn + " ) " + customSeparator;
                        }
                        sr.WriteLine("[" + DateTime.Now + "] " + formedString);

                        break;
                }
                sr.Flush();
            }
        }
        public void stopLogger()
        {
            if (debug)
            {
                sr.Flush();
                sr.Close();
            }
        }
        public enum modes
        {
            WriteInLine, WriteWithEnters, WriteWithCustomSeparator
        }
    }
}
