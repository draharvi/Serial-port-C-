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
using System.Security.Cryptography;
using System.Security.Policy;
using System.IO;

namespace ComTest
{
    public partial class Form1 : Form
    {
        string dataOUT;
        string sendWith = "Both";
        string dataIN;
        public static bool comPortConnected = false;
        int dataINLenght;
        int[] dataInDec;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);

             
            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;
            chBoxRtsEnable.Checked = false;
            serialPort1.RtsEnable = false;
            cSendData.Enabled = true;
            toolStripComboBox1.Text = "Add to Old Data";
            toolStripComboBox2.Text = "Both";
            toolStripComboBox3.Text = "TOP";
            toolStripComboBox4.Text = "Char";
        }

        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBautRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxPatityBits.Text);

                serialPort1.Open();
                progressBar1.Value = 100;
                comPortConnected = true;
            }
            catch (IOException ex)
            {
                if (cBoxCOMPORT.Text == "")
                {
                    MessageBox.Show("Не выбран COM порт", "COM PORT SERIAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Обрыв связи. Обмен прекращен.", "COM PORT SERIAL");
                }
            }
        }
        private void cLOSEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;
            }
        }

        private void cSendData_Click(object sender, EventArgs e)
        {
            if(comPortConnected)
            {
                if (serialPort1.IsOpen)
                {
                    dataOUT = tBoxDataOut.Text;
                    if (sendWith == "None")
                    {
                        serialPort1.Write(dataOUT);
                    }
                    else if (sendWith == "Both")
                    {
                        serialPort1.Write(dataOUT + "\r\n");
                    }
                    else if (sendWith == "New Line")
                    {
                        serialPort1.Write(dataOUT + "\n");
                    }
                    else if (sendWith == "Carrige retutn")
                    {
                        serialPort1.Write(dataOUT + "\r");
                    }
                }
            }
            else
            {
                MessageBox.Show("Вы не открыли последовательный порт", "COM PORT SERIAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (clearToolStripMenuItem.Checked)
            {
                if (tBoxDataOut.Text != "")
            {
                tBoxDataOut.Text = "";
            }
            }
        }

        private void toolStripComboBox2_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBox2.Text == "None")
            {
                sendWith = "None";
            }
            else if (toolStripComboBox2.Text == "Both")
            {
                sendWith = "Both";
            }
            else if (toolStripComboBox2.Text == "New Line")
            {
                sendWith = "New Line";
            }
            else if (toolStripComboBox2.Text == "Carrige retutn")
            {
                sendWith = "Carrige retutn";
            }
        }
        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
                MessageBox.Show("DTR Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } 
            else
            {
                serialPort1.DtrEnable = false;
            }
        }

        private void chBoxRtsEnable_CheckedChanged(object sender, EventArgs e)
        {
            if(chBoxRtsEnable.Checked)
            {
                serialPort1.RtsEnable = true;
                MessageBox.Show("RTC Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.RtsEnable = false;
            }
        }
        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (tBoxDataIN.Text != "")
            {
                tBoxDataIN.Text = "";
            }
        }

        private void tBoxDataOut_TextChanged(object sender, EventArgs e)
        {
            int dataOUTlenght = tBoxDataOut.TextLength;
            lblDataOUTLenght.Text = string.Format("{0:00}",dataOUTlenght);
        }

        private void tBoxDataOut_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.doSomesing();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        private void doSomesing()
        {
            if (serialPort1.IsOpen)
            {
                dataOUT = tBoxDataOut.Text;
                if (sendWith == "None")
                {
                    serialPort1.Write(dataOUT);
                }
                else if (sendWith == "Both")
                {
                    serialPort1.Write(dataOUT + "\r\n");
                }
                else if (sendWith == "New Line")
                {
                    serialPort1.Write(dataOUT + "\n");
                }
                else if (sendWith == "Carrige retutn")
                {
                    serialPort1.Write(dataOUT + "\r");
                }
            }
            if (clearToolStripMenuItem.Checked)
            {
                if (tBoxDataOut.Text != "")
                {
                    tBoxDataOut.Text = "";
                }
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<int> databuffer = new List<int>();
            while(serialPort1.BytesToRead > 0)
            {
                try
                {
                    databuffer.Add(serialPort1.ReadByte());
                }
                catch(Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            dataINLenght = databuffer.Count;
            dataInDec = new int[dataINLenght];
            dataInDec = databuffer.ToArray();
            this.Invoke(new EventHandler(ShowData));
        }
        private void ShowData(object sender, EventArgs e)
        {
            dataIN = RxDataFormat(dataInDec);
            lblDataINLenght.Text = string.Format("{0:00}", dataINLenght);
            if (toolStripComboBox1.Text == "Always Update")
            {
                tBoxDataIN.Text = dataIN;
            }
            else if (toolStripComboBox1.Text == "Add to Old Data")
            {
                if (toolStripComboBox3.Text == "TOP")
                {
                    tBoxDataIN.Text = tBoxDataIN.Text.Insert(0, dataIN);
                }
                else if (toolStripComboBox3.Text == "BUTTON")
                {
                    tBoxDataIN.Text += dataIN;
                }               
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Create by Catup P", "COM PORT SERIAL", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы хотите завершить работу?", "COM PORT SERIAL", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.Clear();
            foreach (string port in ports)
            {
                cBoxCOMPORT.Items.Add(port);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Показываем диалоговое окно с вопросом о завершении работы
                DialogResult result = MessageBox.Show("Вы хотите завершить работу?", "COM PORT SERIAL", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Если пользователь нажал "Нет", отменяем закрытие формы
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private string RxDataFormat(int[] dataInput)
        {
            string strOut = "";
            if (toolStripComboBox4.Text == "Hex")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element, 16) + "\t";
                }
            }
            if (toolStripComboBox4.Text == "Decimal")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element) + "\t";
                }
            }
            if (toolStripComboBox4.Text == "Binary")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element, 2) + "\t";
                }
            }
            if (toolStripComboBox4.Text == "Char")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToChar(element);
                }
            }
            return strOut;
        }
    }
}
