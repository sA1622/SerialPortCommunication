using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace Serial_Recive_Control_Panel
{
    public partial class Form1 : Form
    {
        SerialPort serialPort;
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
            // System.Windows.Forms.Timer always runs on GUI thread 
        }
        void getAvailablePorts()
        {
            String[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
        }//gets the available ports
        

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "") MessageBox.Show("Please select a port and/or baud rate", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.Open();
                    button1.Enabled = false;
                    button2.Enabled= true;
                    backgroundWorker1.RunWorkerAsync(2000);
                }
            }
            catch(UnauthorizedAccessException) {
                MessageBox.Show("Unauthorized Access => COM port busy", "Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync(); // WorkerSupportsCancellation = true (required to calcelasync)
            textBox1.Text = "";
            serialPort1.Close();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            bool datainput = false;
            string data = string.Empty; // the data string
            while (!datainput)
            {
                data = serialPort1.ReadLine(); //reads line from the serial port
                if (!string.IsNullOrEmpty(data)) datainput = true; // if data is null then datainput = true => it will 
            }
            if (datainput)
            {
                textBox1.Invoke((MethodInvoker)(() => textBox1.Text += "Data is:" + data + Environment.NewLine));//Lambda expression
                // we invoke the function from another thread https://www.youtube.com/watch?v=rvMIIuRXmU4
                try
                {
                    StreamWriter writer = new StreamWriter(Application.StartupPath,);
                    //write to txt to update xml file 
                }
                catch(Exception)
                {
                    //blank
                }
            }
            textBox1.Invoke((MethodInvoker)(() => textBox1.SelectionStart = textBox1.TextLength));
            textBox1.Invoke((MethodInvoker)(() => textBox1.ScrollToCaret())); // autoscrolls the text in the textbox
            /*try
            {
                textBox1.Text = serialPort1.ReadLine();

            }
            catch(TimeoutException) {
                MessageBox.Show("Timeout", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }*/ // displays one line at a time
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           e.Cancel = true; // prevents Software from closing by pressing the "X" button
            MessageBox.Show("Close the Software from the close button", "Exit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                if (!backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync(); // WorkerSupportsCancellation = true (required to cancelasync)
                serialPort1.Close();
                button1.Enabled = true;
                button2.Enabled = false;
                textBox1.Text = "";
                MessageBox.Show("SERIAL PORT CLOSED", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information); //if program is being closed and if the serialPort is still opened => close the serial port and notify the user with a MessageBox
            }
            System.Windows.Forms.Application.ExitThread(); // exits Software
        }
    }
}