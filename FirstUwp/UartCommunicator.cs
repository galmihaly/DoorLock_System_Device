using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitsNet;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace Communicator
{
    public class UartCommunicator
    {
        private SerialDevice UartPort;
        private DataReader DataReaderObject = null;
        private DataWriter DataWriterObject;
        private CancellationTokenSource ReadCancellationTokenSource;


        private string UartPortNumber = "UART0";
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initialize the uart communicator with a baud rate. Call this function to initialize the things.
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector(UartPortNumber);
                var dis = await DeviceInformation.FindAllAsync(aqs);
                UartPort = await SerialDevice.FromIdAsync(dis[0].Id);


                //Configure serial settings
                UartPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);    //mS before a time-out occurs when a write operation does not finish (default=InfiniteTimeout).
                UartPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);     //mS before a time-out occurs when a read operation does not finish (default=InfiniteTimeout).
                UartPort.BaudRate = 115200;
                UartPort.Parity = SerialParity.None;
                UartPort.StopBits = SerialStopBitCount.One;
                UartPort.DataBits = 8;

                DataReaderObject = new DataReader(UartPort.InputStream);
                DataReaderObject.InputStreamOptions = InputStreamOptions.ReadAhead;
                //DataWriterObject = new DataWriter(UartPort.OutputStream);

                Debug.WriteLine($"UartPort: {UartPort.PortName}");
                Debug.WriteLine($"Baudrate: {UartPort.BaudRate}");


                StartReceive();
            }
            catch (Exception ex)
            {
                throw new Exception("Uart Initialize Error", ex);
            }
        }

        /// <summary>
        /// Create the listening port and start listening
        /// </summary>
        /// <returns></returns>
        public async Task StartReceive()
        {
            ReadCancellationTokenSource = new CancellationTokenSource();

            while (true)
            {
                await Listen();
                if ((ReadCancellationTokenSource.Token.IsCancellationRequested) || (UartPort == null))
                    break;
            }
        }

        /// <summary>
        /// Start listener private function
        /// </summary>
        /// <returns></returns>
        private async Task Listen()
        {
            const int NUMBER_OF_BYTES_TO_RECEIVE = 1; // using 1 byte will endure to receive all when buffer ends

            byte[] ReceiveData = new byte[NUMBER_OF_BYTES_TO_RECEIVE];
            List<byte> mes = new List<byte>();
            UInt32 bytesRead;

            try
            {
                if (UartPort != null)
                {
                    string Message = "";
                    while (true)
                    {
                        //###### WINDOWS IoT MEMORY LEAK BUG 2017-03 - USING CancellationToken WITH LoadAsync() CAUSES A BAD MEMORY LEAK.  WORKAROUND IS
                        //TO BUILD RELEASE WITHOUT USING THE .NET NATIVE TOOLCHAIN OR TO NOT USE A CancellationToken IN THE CALL #####
                        //bytesRead = await DataReaderObject.LoadAsync(NUMBER_OF_BYTES_TO_RECEIVE).AsTask(ReadCancellationTokenSource.Token);	//Wait until buffer is full
                        bytesRead = await DataReaderObject.LoadAsync(NUMBER_OF_BYTES_TO_RECEIVE);//.AsTask(ReadCancellationTokenSource.Token);  //Wait until buffer is full

                        if ((ReadCancellationTokenSource.Token.IsCancellationRequested) || (UartPort == null))
                            break;

                        DataReaderObject.ReadBytes(ReceiveData);

                        if (bytesRead > 0)
                        {
                            ReceiveData[0] = 0;
                            DataReaderObject.ReadBytes(ReceiveData);

                            Debug.Write((char)ReceiveData[0]);


                            /*foreach(var b in ReceiveData)
                            {
                                Debug.Write(b);
                            }
                            Debug.Write("hello\n");*/

                            //if (ReceiveData[0] == 2)
                            //{
                            //    // a new message comes
                            //    //Debug.Assert(!string.IsNullOrEmpty(Message));
                            //    Message = ""; // make the message empty!, it will start to receive things
                            //}
                            //else if (ReceiveData[0] == 3)
                            //{
                            //    // message has been finished from the sender
                            //    await HandleReceivedMessage(Message);
                            //}
                            //else
                            //{
                            //    Message += (char)ReceiveData[0]; // concatenate the string
                            //}
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //We will get here often if the USB serial cable is removed so reset ready for a new connection (otherwise a never ending error occurs)
                if (ReadCancellationTokenSource != null)
                    ReadCancellationTokenSource.Cancel();

                System.Diagnostics.Debug.WriteLine("UART ReadAsync Exception: {0}", e.Message);
            }
        }

        /// <summary>
        /// Send the message as bytes
        /// </summary>
        /// <param name="TxData"></param>
        public async Task SendBytes(byte[] TxData)
        {
            try
            {
                //Send data to UART
                DataWriterObject.WriteByte(2);
                DataWriterObject.WriteBytes(TxData);
                DataWriterObject.WriteByte(3);
                await DataWriterObject.StoreAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Uart Tx Error", ex);
            }
        }

        public async Task SendString(string msg)
        {
            
            {
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(msg);
                await SendBytes(bytes);

                //await base.SendString(msg);
            }
        }


    }

}
