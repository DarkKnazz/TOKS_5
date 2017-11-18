using System;
using System.Text;
using System.Windows.Forms;

namespace COMChat
{
    public partial class ChatForm : Form
    {
        private readonly SerialPortController _serialPort;

        public ChatForm()
        {
            InitializeComponent();
            _serialPort = new SerialPortController(ShowData, RingInited);
        }

        private void CanConnect(object sender, EventArgs e)
        {
            button1.Enabled = (inputPortTextBox.Text.Length != 0) && (comboBox1.SelectedIndex > -1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text)
            {
                case "Connect":
                    Connect();
                    break;
                case "Disconnect":
                    Disconnect();
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _serialPort.BeforeSend((byte) destinationNumericUpDown.Value,Encoding.Default.GetBytes(richTextBox2.Text));
            ShowData(richTextBox2.Text, true);
            richTextBox2.Text = "";
        }

        private void ShowData(string data, bool sender)
        {
            richTextBox1.Text +=
                (sender ? "↑" : "↓") + @" [" + DateTime.Now.ToString("HH:mm:ss tt") + @"] " + data + "\n";
        }

        private void Connect()
        {
            _serialPort.Connect((byte) sourceNumericUpDown.Value, inputPortTextBox.Text, outputPortTextBox.Text,
                Int32.Parse(comboBox1.SelectedItem.ToString()));
            button1.Text = @"Disconnect";
            inputPortTextBox.Enabled = false;
            outputPortTextBox.Enabled = false;
            comboBox1.Enabled = false;
            sourceNumericUpDown.Enabled = false;
            initRingButton.Enabled = true;
        }

        private void Disconnect()
        {
            _serialPort.Disconnect();
            button1.Text = @"Connect";
            inputPortTextBox.Enabled = true;
            outputPortTextBox.Enabled = true;
            comboBox1.Enabled = true;
            sourceNumericUpDown.Enabled = true;
            button2.Enabled = false;
        }

        private void initRingButton_Click(object sender, EventArgs e)
        {
            _serialPort.SendData(new PackageManager().GetToken());
        }

        private void RingInited()
        {
            if (!InvokeRequired)
            {
                initRingButton.Enabled = false;
            }
            else
            {
                Invoke(new Action(RingInited));
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = richTextBox2.Text.Length > 0;
        }
    }
}
