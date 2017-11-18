using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace COMChat
{
    class SerialPortController
    {
        public delegate void ShowHandler(string data, bool sender);
        public delegate void InitHandler();

        private SerialPort _inputPort;
        private SerialPort _outputPort;

        private readonly ShowHandler _showHandler;
        private readonly InitHandler _initHandler;
        private readonly PackageManager _packageManager = new PackageManager();
        private readonly List<byte[]> _messageBuffer = new List<byte[]>();
        private byte _source;
        private bool _isRingInited;

        public SerialPortController( ShowHandler showHandler, InitHandler initHandler)
        {
            _showHandler = showHandler;
            _initHandler = initHandler;
        }

        public void Connect(byte source, string inputPortName, string outputPortName, int baudRate)
        {
            _source = source;

            _inputPort = new SerialPort(inputPortName, baudRate, Parity.None, 8, StopBits.One);
            _inputPort.Open();
            _inputPort.DataReceived += DataReceived;

            _outputPort = new SerialPort(outputPortName, baudRate, Parity.None, 8, StopBits.One);
            _outputPort.Open();
        }

        public void Disconnect()
        {
            _inputPort.Close();
            _outputPort.Close();
        }

        public void BeforeSend(byte destination, byte[] data)
        {
            _messageBuffer.Add(_packageManager.CreatePackage(data, _source, destination));
        }

        public void SendData(byte[] data)
        {
            _outputPort.Write(data, 0, data.Length);
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[_inputPort.BytesToRead];
            _inputPort.Read(data, 0, data.Length);
            if (_packageManager.IsToken(data))
            {
                if (!_isRingInited)
                {
                    _isRingInited = true;
                    if(_initHandler != null)
                        _initHandler.Invoke();
                }
                else if (_messageBuffer.Any())
                {
                    SendData(_messageBuffer.First());
                    _messageBuffer.RemoveRange(0,1);
                    return;
                }
                SendData(data);
                return;
            }
            if (_packageManager.IsData(data))
            {
                if (_packageManager.GetSource(data).Equals(_source))
                {
                    SendData(_packageManager.GetToken());
                }
                else if(_packageManager.GetDest(data).Equals(_source))
                {
                    _showHandler.Invoke(System.Text.Encoding.Default.GetString(_packageManager.GetData(data)), false);
                    SendData(data);
                }
                else
                {
                    SendData(data);
                }
            }
        }
    }
}
