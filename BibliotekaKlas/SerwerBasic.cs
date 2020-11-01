using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BibliotekaKlas
{
    public abstract class ServerBasic
    {
        /// <summary>
        /// rozmiar buffera
        /// </summary>
        public static int sizeOfBuffer { get; set; } = 1024;
        /// <summary>
        /// Port
        /// </summary>
        public static Int32 port { get; set; } = 2137;
        /// <summary>
        /// obiekt serwera tcp
        /// </summary>
        public TcpListener tcpServer { get; set; } = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        /// <summary>
        /// tablica byte otrzymujaca dane od klienta
        /// </summary>
        public byte[] buffer { get; set; } = new byte[sizeOfBuffer];
       /// <summary>
       /// Obsluga dzialania serwera
       /// </summary>
        public abstract void Server();
    }
}
