using System;
using UnityEngine;

#if WINDOWS_UWP
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class UDPReceiver : MonoBehaviour, IDisposable
{
    public string message = String.Empty;

    private Methodology _methodology;

#if UNITY_EDITOR
        
        public void Ready()
        {
            Debug.Log("Clack");
        }
#endif

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Build and run on Hololens only");
#else
        Receive(5001);
        _methodology = GameObject.Find("MethodologyScripts").GetComponent<Methodology>();
#endif
    }

#if WINDOWS_UWP

    //OnMessageReceived
    public delegate void AddOnMessageReceivedDelegate(string message, IPEndPoint remoteEndpoint);
    public event AddOnMessageReceivedDelegate MessageReceived;

    private void OnMessageReceivedEvent(string message, IPEndPoint remoteEndpoint)
    {
        if (MessageReceived != null)
            MessageReceived(message, remoteEndpoint);
    }

    DatagramSocket _Socket = null;
    

    public async void Receive(int port)
    {
        string portStr = port.ToString();
        // start the client
        try
        {
            _Socket = new DatagramSocket();
            _Socket.MessageReceived += _Socket_MessageReceived;

            await _Socket.BindServiceNameAsync(portStr);

            //Debug.Log(string.Format("Listening on {0}", portStr));

            await Task.Delay(3000);
            // send out a message, otherwise receiving does not work ?!
            var outputStream = await _Socket.GetOutputStreamAsync(new HostName("192.168.8.255"), portStr);
            DataWriter writer = new DataWriter(outputStream);
            //writer.WriteString("Listening...");
            await writer.StoreAsync();
        }
        catch (Exception ex)
        {
            _Socket.Dispose();
            _Socket = null;
            Debug.LogError(ex.ToString());
            Debug.LogError(SocketError.GetStatus(ex.HResult).ToString());
        }
    }

    private async void _Socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        message = String.Empty;
        try
        {
            Stream streamIn = args.GetDataStream().AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn, Encoding.UTF8);

            message = await reader.ReadLineAsync();
            Debug.Log("Got: " + message);

            if ( message.Equals("U") || message.Equals("U'")
                || message.Equals("D") || message.Equals("D'")
                || message.Equals("L") || message.Equals("L'")
                || message.Equals("R") || message.Equals("R'")
                || message.Equals("F") || message.Equals("F'")
                || message.Equals("B") || message.Equals("B'") )
            //if ( message.Equals("b_phase") || message.Equals("p_phase") || message.Equals("t_phase") || message.Equals("r_phase")                               
            //  || message.Equals("finish") || message.Equals("stop_reset") || message.Equals("restart") )
            {
                if (Methodology.UDPListen)
                {
                    Methodology.ActualRotation = message;
                }
                else
                {
                    message = String.Empty;
                }
            }
            else
            {                
                Methodology.PhaseControl = message;

                var outputStream = await _Socket.GetOutputStreamAsync(new HostName("192.168.8.100"), 5001.ToString());
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteString(message);
                await writer.StoreAsync();
            }            
            
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Parse(args.RemoteAddress.RawName), Convert.ToInt32(args.RemotePort));
            OnMessageReceivedEvent(message, remoteEndpoint);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

   /* public async void Ready()
    {
        Receive(5001);
        
        DatagramSocket MySocket = new DatagramSocket();
        var outputStream = await MySocket.GetOutputStreamAsync(new HostName("192.168.8.100"), 5001.ToString());
        DataWriter writer = new DataWriter(outputStream);
        writer.WriteString("HL2 is ready to begin baseline.");        
        Debug.Log("HERE1");
        await writer.StoreAsync();            
    }*/
   
#endif

    public void Dispose()
    {
#if WINDOWS_UWP
        if (_Socket != null)
        {
            _Socket.Dispose();
            _Socket = null;
        }
#else
        Debug.Log("Adios");        
#endif
    }
}

