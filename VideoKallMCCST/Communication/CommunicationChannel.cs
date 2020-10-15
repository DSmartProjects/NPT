using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace VideoKallMCCST.Communication
{
   public class CommunicationChannel
    {
        DatagramSocket _socketUdp = new DatagramSocket();
        public event EventHandler<CommunicationMsg> MessageReceived;
        public event EventHandler<CommunicationMsg> ErrorMessage;
        public void Initialize()
        {
            _socketUdp.MessageReceived += _socketUdp_MessageReceived;
        }

        private void _socketUdp_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            try
            {
                uint stringLength = args.GetDataReader().UnconsumedBufferLength;
                string receivedMessage = args.GetDataReader().ReadString(stringLength);
                MessageReceived?.Invoke(this, new CommunicationMsg(receivedMessage));
            }
            catch (Exception ex)
            {
                ErrorMessage?.Invoke(this, new CommunicationMsg(ex.Message));
            }
        }

        public async void Connect()
        {
            if (_socketUdp == null)
                return;
            try
            {
                HostName hostName = new HostName(IPAddress);
                await _socketUdp.ConnectAsync(hostName, PortNo);
            }catch(Exception ex)
            {
                ErrorMessage?.Invoke(this, new CommunicationMsg(ex.Message));
            }
        }

        public async void SendMessage(string msg)
        {
            try
            {
                if (_socketUdp != null && _socketUdp.OutputStream != null)
                {
                    DataWriter writer = new DataWriter(_socketUdp.OutputStream);
                    writer.WriteString(msg);
                    await writer.StoreAsync();
                    writer.DetachStream();
                }
            }catch(Exception ex)
            {
                ErrorMessage?.Invoke(this, new CommunicationMsg(ex.Message));
            }
        }

        public void Cleanup()
        {
            if(_socketUdp != null)
            {
                _socketUdp.Dispose();
                _socketUdp = null;
            }
        }

        public string IPAddress { get; set; } 
        public string PortNo { get; set; } 
    }

    public class CommunicationMsg: EventArgs
    {
        public string Msg { get; set; }
        public DeviceResponseType Id { get; set; }
        public CommunicationMsg(string msg)
        {
            Msg = msg;
        }

        public CommunicationMsg(DeviceResponseType id, string msg)
        {
            Msg = msg;
            Id = id;
        }
    }
}
