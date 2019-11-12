using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Globalization;

namespace VSSimulator
{
    public partial class VSSim : Form
    {
        public AsynchronousClient AsyncClient { get; set; }
        private byte[] dataToSend;

        public VSSim()
        {
            InitializeComponent();
        }
        private void VSSim_Load(object sender, EventArgs e)
        {
            initAppButton_Click(sender, e);
            connectButton_Click(sender, e);
        }

        private void initAppButton_Click(object sender, EventArgs e)
        {
            int vsPort = int.Parse(vsPortTextBox.Text, NumberStyles.Integer, CultureInfo.DefaultThreadCurrentCulture);
            AsyncClient = new AsynchronousClient(this, vsPort);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            AsyncClient.StartWorking();
        }

        // Send a message to the server
        private async void sendDataButton_Click(object sender, EventArgs e)
        {
            int sendSize = int.Parse(sendCountTextBox.Text, NumberStyles.Integer, 
                CultureInfo.DefaultThreadCurrentCulture);

            BuildDataWithPattern(sendSize);

            await AsyncClient.SendVSData(dataToSend, sendSize).ConfigureAwait(false);
        }


        private void BuildDataWithPattern(int dataSize)
        {
            dataToSend = new byte[dataSize];

            // Put something in the message start with space character and fill
            // with ASCII alphanumerics
            byte ch = 0;
            for (int i = 0; i < dataSize; i++)
            {
                dataToSend[i] = ch++;
            }

            //// with ASCII alphanumerics
            //byte ch = 0x20;
            //for (int i = 0; i < dataSize; i++)
            //{
            //    dataToSend[i] = ch;
            //    if (++ch > 0x7e)
            //    {
            //        ch = 0x20;
            //    }
            //}
        }

        internal void AddStatusText(string statusText)
        {
            DateTime dt = DateTime.Now;
            string finalText = $"{dt:HH:mm:ss.fff}-{statusText}";
            if(statusTextBox.InvokeRequired)
            {
                statusTextBox.Invoke(new Action(() => statusTextBox.AppendText(finalText)));
            }
            else
            {
                statusTextBox.AppendText(finalText);
            }
        }

        //internal void AddReceivedData(string validDataString)
        //{
        //}

        private void echoRecvdCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Received data from CLI
        internal async void ReceivedData(byte[] recvData)
        {
            //VerifyReceivedData();
            // Verify that the data sent is the data received
            if(recvData.Length != dataToSend.Length)
            {
                AddStatusText($"Length of data received {recvData.Length} not equal to length sent {dataToSend.Length}");
                return;
            }

            if(recvData.SequenceEqual(dataToSend))
            {
                AddStatusText($"SUCCESS - Data sent = data received");
            }
            else
            {
                AddStatusText($"FAILED - Data sent != data received");
                return;
            }

            if (echoRecvdCheckBox.Checked)
            {
                await AsyncClient.SendVSData(recvData, recvData.Length).ConfigureAwait(false);
            }
        }
    }
}
