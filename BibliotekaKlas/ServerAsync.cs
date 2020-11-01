using BibliotekaKlas;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Data.SQLite;
using BibliotekaSQL;
using static BibliotekaSQL.AccountDataBase;

namespace BibliotekaSerwera
{
    public class ServerAsync : ServerBasic
    {
        /// <summary>
        /// Tworzenie delegata
        /// </summary>
        /// <param name="tcpClient">Polaczenie z klientem</param>
        public delegate void TransmissionDataDelegate(TcpClient tcpClient);
        /// <summary>
        /// Konstruktor Domyslny
        /// </summary>
        public ServerAsync() { }
        /// <summary>
        /// Funkcja odpowiedzialna za wykonywane operacje logowania i rejestracji
        /// </summary>
        /// <param name="tcpClient">Polaczenie z klientem</param>
        public void LogowanieiRejestracja(TcpClient tcpClient)
        {
           
                NetworkStream stream = tcpClient.GetStream();
                stream.ReadTimeout = 60000;
                Console.WriteLine("Polaczono z klientem!");

                var dataBase = new AccountDataBase();
                dataBase.ustawieniePolaczenia(new SQLiteConnection("Data Source=C:\\Users\\damia\\OneDrive\\Pulpit\\C#- projekty\\TCPServer\\dataBase.db"));
                dataBase.CreateBase();
                var listOfAccounts = dataBase.Accounts;
                
                sendToClient(stream, "Polaczono z serwerem!\r\n");
            //switch
            
                sendToClient(stream, "1. Rejestracja\r\n2. Logowanie\r\n");
                stream.Read(buffer, 0, sizeOfBuffer);

                if (toInt(buffer) == 1)
                {
                    //rejestracja
                    sendToClient(stream, "Rejestracja:\r\nWpisz login: ");
                    stream.Read(buffer, 0, sizeOfBuffer);
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, sizeOfBuffer);
                    var nowyLogin = toString(buffer);
                    sendToClient(stream, "\r\nPodaj haslo!!\r\n");
                    stream.Read(buffer, 0, sizeOfBuffer);
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, sizeOfBuffer);
                    var noweHaslo = toString(buffer);
                    dataBase.NewAccount(nowyLogin, noweHaslo);
                    sendToClient(stream, "\r\nUtworzono nowe konto!!\r\n");
                }
                else if (toInt(buffer) == 2)
                {
                    //logowanie
                    sendToClient(stream, "Wpisz login: ");
                    stream.Read(buffer, 0, sizeOfBuffer);
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, sizeOfBuffer);
                    var login = toString(buffer);

                    sendToClient(stream, "Wpisz haslo: ");
                    stream.Read(buffer, 0, sizeOfBuffer);
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.Read(buffer, 0, sizeOfBuffer);
                    var password = toString(buffer);

                    if (checkLoginAndPasswd(listOfAccounts, login, password))
                        sendToClient(stream, "Prawidlowo!");
                    else
                        sendToClient(stream, "Nieprawidlowy login lub haslo!");
                }
            
            
        }
        /// <summary>
        /// Metoda potrzebna do utrzymania polaczenia ze wszystkimi klientami
        /// </summary>
        public override void Server()
        {
            tcpServer.Start();

            Console.WriteLine("Czekam na polaczenie...");
            Console.WriteLine("Adres Serwera: "+ tcpServer.LocalEndpoint);
            while(true)
            {
                TcpClient tcpClient = tcpServer.AcceptTcpClient();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(LogowanieiRejestracja);
                transmissionDelegate.BeginInvoke(tcpClient, TransmissionCallback, tcpClient);
            }

        }
        /// <summary>
        /// Metoda wywolana w chwili rozlaczenia klienta
        /// </summary>
        /// <param name="ar"></param>
        void TransmissionCallback(IAsyncResult ar)
        {
            TcpClient tcpClient = (TcpClient)ar.AsyncState;
            Console.WriteLine("Wymuszone wylaczenie!!!");
            Console.WriteLine("Czyszczenie...");
            tcpClient.Close();
            
        }
        bool checkLoginAndPasswd(List<Account> list, string login, string password)
        {
            foreach (var e in list)
                if (e.Login == login && e.Password == password)
                    return true;
            return false;
        }
        /// <summary>
        /// wysylanie wiadomosci z serwera do klienta
        /// </summary>
        /// <param name="stream">Strumien danych</param>
        /// <param name="msg">Wiadomosc dla klienta</param>
        static void sendToClient(NetworkStream stream, string msg)
        {
            stream.Write(Encoding.UTF8.GetBytes(msg), 0, msg.Length);
        }
        /// <summary>
        /// Konwersja ciagu bajtow do stringa
        /// </summary>
        /// <param name="buffer">tablica bajtow</param>
        /// <returns></returns>
        string toString(byte[] buffer)
        {
            string text = "";
            text = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Trim('\0');
            Array.Clear(buffer, 0, buffer.Length);
            return text;
        }
        /// <summary>
        /// Konwersja ciagu bajtow do inta
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int toInt(byte[] buffer)
        {
            try
            {
                string text = "";
                text = Encoding.UTF8.GetString(buffer, 0, buffer.Length).Trim('\0');
                return Int32.Parse(text);
            }
            catch { return 0; }
        }

        
    }
}
