using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotekaSerwera;
using BibliotekaKlas;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerBasic s = new ServerAsync();
            s.Server();

            Console.ReadKey();
        }
    }
}
