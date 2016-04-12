﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPcap;
using SharpPcap.LibPcap;
using PacketDotNet;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace SharpSniffer
{
    /// <summary>
    /// 公共变量、方法和设置
    /// </summary>
    class Common
    {        
        public static int queueSize = 1000;
        public static string filter = string.Empty;
        public static List<RawCapture> queue = new List<RawCapture>();
        public static ComboBox comboBox = new ComboBox();
        public static CaptureDeviceList devices;
        public static Thread captureThread;
        public static List<Packet> packetQueue = new List<Packet>();
        public static ICaptureDevice device;
        public static int cnt = 0;
        private static CaptureFileWriterDevice deviceWriteFile;
        public static void LoadDevices(BackgroundWorker bw)
        {
            bw.ReportProgress(25);
            devices = CaptureDeviceList.Instance;
            bw.ReportProgress(50);
            if (devices.Count >= 1)
            {
                int cnt = 25 / devices.Count;
                int cntrep = 75;
                foreach (var dev in devices)
                {
                    cntrep += cnt;
                    bw.ReportProgress(cntrep);
                    string name = dev.ToString();
                    name = name.Substring(name.IndexOf("FriendlyName"));
                    name = name.Substring(13, name.IndexOf('\n') - 13);
                    comboBox.Items.Add(name);
                    Thread.Sleep(100);
                }
            }
        }
        public static void LoadCapFile(string capFileName)
        {
            if (device != null) device.Close();
            try
            {
                device = new CaptureFileReaderDevice(capFileName);
                device.Open();
            }
            catch(Exception)
            {
                MessageBox.Show("Error while opening capFile");
                return;
            }
        }
        public static void CreatecapFile(string capFileName)
        {
            deviceWriteFile = new CaptureFileWriterDevice(capFileName);
            for (int i = 0; i < queue.Count; i++)
            {
                deviceWriteFile.Write(queue[i]);
            }
            deviceWriteFile.Close();
        }
        public static void ShowDetail(int index)
        {
            RawCapture rawCapture = null;
            try
            {
                rawCapture = queue[index];
            }
            catch(Exception)
            {
                MessageBox.Show("Error while displying details");
                return;
            }
            Packet packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
            PacketDetials pd = new PacketDetials(packet);
            CellDetails cellDetails = new CellDetails();
            cellDetails.rawCapture = rawCapture;
            if (pd.ethernetPacket != null)
            {
                cellDetails.richTextBox.Text = pd.ethernetPacket.ToString(StringOutputType.VerboseColored) + Environment.NewLine + pd.ethernetPacket.PrintHex();
            }
            cellDetails.Show();
        }
    }
}
