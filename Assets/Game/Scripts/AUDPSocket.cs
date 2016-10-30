using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

public class AUDPSocket : MonoBehaviour
{
    private static AUDPSocket s_instance;
    public static AUDPSocket Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new AUDPSocket();
            }
            return s_instance;
        }
    }
    Socket socket;
    public void StartAsServer(int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(new IPEndPoint(IPAddress.Any, port));

        new Thread(new ThreadStart(ReceiveHandler)).Start();
    }

    private void ReceiveHandler()
    {
        while (true)
        {
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            byte[] data = new byte[1024];
            int recv = socket.ReceiveFrom(data, ref Remote);

            string sdataFromClient = Encoding.ASCII.GetString(data, 0, recv);

            //we have received something from client, and now we give client some responses.
            data = Encoding.ASCII.GetBytes("server received success");
            socket.SendTo(data, data.Length, SocketFlags.None, Remote);
        }
    }

    public void StartAsClient()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }
    public string SendToServer(string ip, int port, string sContent)
    {
        if (socket == null)
        {
            return "socket is null.";
        }
        if (string.IsNullOrEmpty(sContent))
        {
            return "";
        }
        byte[] bytes = Encoding.ASCII.GetBytes(sContent);
        socket.SendTo(bytes, bytes.Length, SocketFlags.None, new IPEndPoint(IPAddress.Parse(ip), port));

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;
        bytes = new byte[1024];
        int recv = socket.ReceiveFrom(bytes, ref Remote);
        string sdataFromServer = Encoding.ASCII.GetString(bytes, 0, recv);
        return sdataFromServer;
    }
}