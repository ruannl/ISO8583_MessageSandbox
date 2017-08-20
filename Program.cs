using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using ISO_8583_MessageSandbox.Properties;

namespace ISO_8583_MessageSandbox
{
    internal class Program
    {
        private static readonly IPAddress DestinationIp = IPAddress.Parse(Settings.Default.IPAddress);
        private static readonly int Port = Settings.Default.Port;
        private static readonly Socket Socket = new Socket(DestinationIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        private static string _transactionId = "";
        private static string _systemAuditNumber = "";
        private static bool _authRequestApproved;

        private static void Main(string[] args)
        {
            try
            {
                Socket.Connect(new IPEndPoint(DestinationIp, Port));
                Console.WriteLine("Connected to {0}", DestinationIp);

                var msisdn = Settings.Default.msisdn;

                _transactionId = DateTime.Now.ToString("yyyyddMMHHmmss") + msisdn.Substring(msisdn.Length - 3, 3);
                _systemAuditNumber = DateTime.Now.ToString("HHmmss");
                
                SendEcho();
                
                Socket.Disconnect(false);
                Console.WriteLine("Disconnected from {0}", DestinationIp);

                var response = Console.ReadLine();
                if (response == "exit")
                    Environment.Exit(0);
            }
            catch (Exception)
            {
                if (!Socket.Connected) throw;

                Socket.Disconnect(false);
                Console.WriteLine("Disconnected from {0}", DestinationIp);
                throw;
            }
        }

        /// <summary>
        ///     Sends the echo(0800) message to destination IP.
        /// </summary>
        private static void SendEcho()
        {
            try
            {
                // ===================================================================================================================
                // The message comprises of the following: length indicator, message type identifier, and the bitmaps & data elements.
                // ===================================================================================================================

                // ==========================================
                // MTI Message Type Indicator Length
                // ==========================================
                var mti = Encoding.UTF8.GetBytes("0800");

                // =====================================================================================
                // Each bitmap is 8 bytes in length. 
                // Each of the bits in its binary representation corresponds to a field in the message.
                // Bit 1 indicates the most significant bit.
                
                // primary bitmap: Indicating the presence of Data Elements 1 up to 64
                // binary representation: 1000001000111000000000000000000000000000000000000000000000000000

                // secondary bitmap: Indicating the presence of Data Elements 65 up to 128
                // binary representation: 0000010000000000000000000000000000000000000000000000000000000000
                
                // 127 bitmap: Indicating the presence of Data Elements 1 up to 25 on field 127
                // =====================================================================================

                var primaryBitmap = CreateBitmap(true, new[] { 1, 7, 11, 12, 13 }); // Indicates presence of fields 7,11,12,13
                var secondaryBitmap = CreateBitmap(false, new[] { 70 }); // Indicates presence of field 70
                var messageBitmap = new byte[primaryBitmap.Length + secondaryBitmap.Length];

                // ==================================================================
                // combine the primary bitmap and secondary bitmap into 1 byte array
                // ==================================================================
                Buffer.BlockCopy(primaryBitmap, 0, messageBitmap, 0, primaryBitmap.Length);
                Buffer.BlockCopy(secondaryBitmap, 0, messageBitmap, primaryBitmap.Length, secondaryBitmap.Length);

                // add all data elements to one string
                var dataElement7 = DateTime.Now.ToString("MMddHHmmss"); // Transmission date and time Lenght:10
                var dataElement11 = _systemAuditNumber; // Systems trace audit number Lenght:6
                var dataElement12 = DateTime.Now.ToString("HHmmss"); // Time, local transaction Lenght:6
                var dataElement13 = DateTime.Now.ToString("MMdd"); // Date, local transaction Lenght:4
                var dataElement70 = "301"; // Network management info code Lenght:3

                var dataElements = new StringBuilder();
                dataElements.Append(dataElement7);
                dataElements.Append(dataElement11);
                dataElements.Append(dataElement12);
                dataElements.Append(dataElement13);
                dataElements.Append(dataElement70);

                // add elements string to byte array
                var elements = Encoding.UTF8.GetBytes(dataElements.ToString());

                // create a message byte array for the mti, bitmap and elements
                var msg = new byte[mti.Length + messageBitmap.Length + elements.Length];

                // populate the message byte array
                Buffer.BlockCopy(mti, 0, msg, 0, mti.Length);
                Buffer.BlockCopy(messageBitmap, 0, msg, mti.Length, messageBitmap.Length);
                Buffer.BlockCopy(elements, 0, msg, mti.Length + messageBitmap.Length, elements.Length);

                // =================================================================================================
                // Message Length Indicator
                // A 2-byte header is to be prefixed to all messages, indicating the length of the message
                // The first byte contains the quotient of the length of the message (excluding this header) and 256.
                // The second byte contains the remainder of this division
                // =================================================================================================

                // create the message header
                var messageHeader = new byte[2];
                var messageLength = msg.Length;
                messageHeader[0] = Convert.ToByte(messageLength / 256);
                messageHeader[1] = Convert.ToByte(messageLength % 256);

                // create the message including the header
                var message = new byte[messageHeader.Length + msg.Length];
                Buffer.BlockCopy(messageHeader, 0, message, 0, messageHeader.Length);
                Buffer.BlockCopy(msg, 0, message, messageHeader.Length, msg.Length);

                // decode the message again for testing
                message.DecodeMessage();
                
                // send the message to destination ip address
                SendMessage(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        ///     Sends the authorization request.
        /// </summary>
        private static void SendAuthRequest()
        {
            try
            {
                var primaryAccountNumber = "0000000000000000001"; //2
                var processingCode = "220000"; //3
                var amount = "000000000100"; //4
                var transmissionDateTime = DateTime.Now.ToString("MMddHHmmss"); //7
                // var systemAuditNumber = "000002"; //11
                var timeLocalTransaction = DateTime.Now.ToString("HHmmss"); //12
                var dateLocalTransaction = DateTime.Now.ToString("MMdd"); //13
                var dateExpiration = "9911"; //14 YYMM
                var serviceProviderId = "1000"; //18
                var posEntryMode = "021"; //22
                var posConditionCode = "00"; //25
                var posPinCaptureCode = "05"; //26
                var institutionId = Settings.Default.InstitutionId; //32
                var msisdn = Settings.Default.msisdn; //37
                var terminalId = "internet"; //41

                var retailerId = "retailerId"; //42
                retailerId = retailerId.PadRight(15, ' ');
                retailerId = retailerId.Substring(0, 15);

                var locationName = "online"; // 43
                locationName = locationName.PadRight(40, ' ');
                locationName = locationName.Substring(0, 40);

                var currencyCode = "710"; //49
                var pinData = "00012345"; //52
                var echoData = "Echo Data"; //59
                var terminalType = "000000000000090"; //123

                // =====================================
                // Field 127.9 Transaction Id ...17 (... indicates three digit length indicator)
                // =====================================
                _transactionId = _transactionId.Length.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0') + _transactionId;

                var mti = Encoding.UTF8.GetBytes("0100");
                //var tertiaryBitmapLength = "025";

                var primaryBitmap = CreateBitmap(true, new[] { 1, 2, 3, 4, 7, 11, 12, 13, 14, 18, 22, 25, 26, 32, 37, 41, 42, 43, 49, 52, 59 });
                var secondaryBitmap = CreateBitmap(false, new[] { 123, 127 });
                var tertiaryBitmap = CreateBitmap(true, new[] { 9 });
                var messageBitmap = new byte[primaryBitmap.Length + secondaryBitmap.Length];

                // ==================================================================
                // combine the primary bitmap and secondary bitmap into 1 byte array
                // ==================================================================
                Buffer.BlockCopy(primaryBitmap, 0, messageBitmap, 0, primaryBitmap.Length);
                Buffer.BlockCopy(secondaryBitmap, 0, messageBitmap, primaryBitmap.Length, secondaryBitmap.Length);

                // add all data elements to one string
                var dataElements = new StringBuilder();
                dataElements.Append(primaryAccountNumber.With2DigitLengthIndicator());
                dataElements.Append(processingCode);
                dataElements.Append(amount);
                dataElements.Append(transmissionDateTime);
                dataElements.Append(_systemAuditNumber);
                dataElements.Append(timeLocalTransaction);
                dataElements.Append(dateLocalTransaction);
                dataElements.Append(dateExpiration);
                dataElements.Append(serviceProviderId);
                dataElements.Append(posEntryMode);
                dataElements.Append(posConditionCode);
                dataElements.Append(posPinCaptureCode);
                dataElements.Append(institutionId.With2DigitLengthIndicator());
                dataElements.Append(msisdn);
                dataElements.Append(terminalId);
                dataElements.Append(retailerId);
                dataElements.Append(locationName);
                dataElements.Append(currencyCode);
                dataElements.Append(pinData);
                dataElements.Append(echoData.With3DigitLengthIndicator());
                dataElements.Append(terminalType.With3DigitLengthIndicator());

                // add elements string to byte array
                var elements = Encoding.UTF8.GetBytes(dataElements.ToString());

                // ==========================================
                // build field 127.9
                // ==========================================
                var transactionIdBytes =
                    Encoding.UTF8.GetBytes(_transactionId); // the transaction id includes the 3 digit length indicator
                var field127Length = (_transactionId.Length + 25).ToString().PadLeft(3, '0');

                var field127LengthBytes = Encoding.UTF8.GetBytes(field127Length);
                var field127Bytes = new byte[field127LengthBytes.Length + tertiaryBitmap.Length +
                                             transactionIdBytes.Length];

                Buffer.BlockCopy(field127LengthBytes, 0, field127Bytes, 0, field127LengthBytes.Length);
                Buffer.BlockCopy(tertiaryBitmap, 0, field127Bytes, field127LengthBytes.Length, tertiaryBitmap.Length);
                Buffer.BlockCopy(transactionIdBytes, 0, field127Bytes,
                    field127LengthBytes.Length + tertiaryBitmap.Length, transactionIdBytes.Length);

                var totalElements = new byte[elements.Length + field127Bytes.Length];
                Buffer.BlockCopy(elements, 0, totalElements, 0, elements.Length);
                Buffer.BlockCopy(field127Bytes, 0, totalElements, elements.Length, field127Bytes.Length);

                // build the message including field 127.9
                var msg = new byte[mti.Length + messageBitmap.Length + totalElements.Length];
                Buffer.BlockCopy(mti, 0, msg, 0, mti.Length);
                Buffer.BlockCopy(messageBitmap, 0, msg, mti.Length, messageBitmap.Length);
                Buffer.BlockCopy(totalElements, 0, msg, mti.Length + messageBitmap.Length, totalElements.Length);

                // create the message header
                var messageHeader = new byte[2];
                var messageLength = msg.Length;
                messageHeader[0] = Convert.ToByte(messageLength / 256);
                messageHeader[1] = Convert.ToByte(messageLength % 256);

                var message = new byte[messageHeader.Length + msg.Length];
                Buffer.BlockCopy(messageHeader, 0, message, 0, messageHeader.Length);
                Buffer.BlockCopy(msg, 0, message, messageHeader.Length, msg.Length);

                //DecodeMessage(message);

                SendMessage(message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        /// <summary>
        ///     Sends the transaction request.
        /// </summary>
        private static void SendTransactionRequest()
        {
            try
            {
                var primaryAccountNumber = "0000000000000000001"; //2
                var processingCode = "220000"; //3
                var amount = "000000000100"; //4
                var transmissionDateTime = DateTime.Now.ToString("MMddHHmmss"); //7
                // var systemAuditNumber = "000002"; //11
                var timeLocalTransaction = DateTime.Now.ToString("HHmmss"); //12
                var dateLocalTransaction = DateTime.Now.ToString("MMdd"); //13
                var dateExpiration = "9911"; //14 YYMM
                var serviceProviderId = "1000"; //18
                var posEntryMode = "021"; //22
                var posConditionCode = "00"; //25
                var posPinCaptureCode = "05"; //26
                var institutionId = "901051"; //32
                var msisdn = Settings.Default.msisdn; //37
                var terminalId = "internet"; //41

                var retailerId = "retailerId"; //42
                retailerId = retailerId.PadRight(15, ' ');
                retailerId = retailerId.Substring(0, 15);

                var locationName = "online"; // 43
                locationName = locationName.PadRight(40, ' ');
                locationName = locationName.Substring(0, 40);

                var currencyCode = "710"; //49
                var pinData = "00012345"; //52
                var echoData = "Echo Data"; //59
                var terminalType = "000000000000090"; //123

                // =====================================
                // Field 127.9 Transaction Id ...17 (... indicates three digit length indicator)
                // =====================================
                _transactionId = _transactionId.Length.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0') +
                                 _transactionId;

                var mti = Encoding.UTF8.GetBytes("0200");
                var primaryBitmap = CreateBitmap(true,
                    new[] { 1, 2, 3, 4, 7, 11, 12, 13, 14, 18, 22, 25, 26, 32, 37, 41, 42, 43, 49, 52, 59 });
                var secondaryBitmap = CreateBitmap(false, new[] { 123, 127 });
                var tertiaryBitmap = CreateBitmap(true, new[] { 9 });
                var messageBitmap = new byte[primaryBitmap.Length + secondaryBitmap.Length];

                // ==================================================================
                // combine the primary bitmap and secondary bitmap into 1 byte array
                // ==================================================================
                Buffer.BlockCopy(primaryBitmap, 0, messageBitmap, 0, primaryBitmap.Length);
                Buffer.BlockCopy(secondaryBitmap, 0, messageBitmap, primaryBitmap.Length, secondaryBitmap.Length);

                // add all data elements to one string
                var dataElements = new StringBuilder();
                dataElements.Append(primaryAccountNumber.With2DigitLengthIndicator());
                dataElements.Append(processingCode);
                dataElements.Append(amount);
                dataElements.Append(transmissionDateTime);
                dataElements.Append(_systemAuditNumber);
                dataElements.Append(timeLocalTransaction);
                dataElements.Append(dateLocalTransaction);
                dataElements.Append(dateExpiration);
                dataElements.Append(serviceProviderId);
                dataElements.Append(posEntryMode);
                dataElements.Append(posConditionCode);
                dataElements.Append(posPinCaptureCode);
                dataElements.Append(institutionId.With2DigitLengthIndicator());
                dataElements.Append(msisdn);
                dataElements.Append(terminalId);
                dataElements.Append(retailerId);
                dataElements.Append(locationName);
                dataElements.Append(currencyCode);
                dataElements.Append(pinData);
                dataElements.Append(echoData.With3DigitLengthIndicator());
                dataElements.Append(terminalType.With3DigitLengthIndicator());

                // add elements string to byte array
                var elements = Encoding.UTF8.GetBytes(dataElements.ToString());

                // ==========================================
                // build field 127.9
                // ==========================================
                var transactionIdBytes =
                    Encoding.UTF8.GetBytes(_transactionId); // the transaction id includes the 3 digit length indicator
                var field127Length = (_transactionId.Length + 25).ToString().PadLeft(3, '0');

                var field127LengthBytes = Encoding.UTF8.GetBytes(field127Length);
                var field127Bytes = new byte[field127LengthBytes.Length + tertiaryBitmap.Length +
                                             transactionIdBytes.Length];

                Buffer.BlockCopy(field127LengthBytes, 0, field127Bytes, 0, field127LengthBytes.Length);
                Buffer.BlockCopy(tertiaryBitmap, 0, field127Bytes, field127LengthBytes.Length, tertiaryBitmap.Length);
                Buffer.BlockCopy(transactionIdBytes, 0, field127Bytes,
                    field127LengthBytes.Length + tertiaryBitmap.Length, transactionIdBytes.Length);

                var totalElements = new byte[elements.Length + field127Bytes.Length];
                Buffer.BlockCopy(elements, 0, totalElements, 0, elements.Length);
                Buffer.BlockCopy(field127Bytes, 0, totalElements, elements.Length, field127Bytes.Length);

                // build the message including field 127.9
                var msg = new byte[mti.Length + messageBitmap.Length + totalElements.Length];
                Buffer.BlockCopy(mti, 0, msg, 0, mti.Length);
                Buffer.BlockCopy(messageBitmap, 0, msg, mti.Length, messageBitmap.Length);
                Buffer.BlockCopy(totalElements, 0, msg, mti.Length + messageBitmap.Length, totalElements.Length);

                // create the message header
                var messageHeader = new byte[2];
                var messageLength = msg.Length;
                messageHeader[0] = Convert.ToByte(messageLength / 256);
                messageHeader[1] = Convert.ToByte(messageLength % 256);

                var message = new byte[messageHeader.Length + msg.Length];
                Buffer.BlockCopy(messageHeader, 0, message, 0, messageHeader.Length);
                Buffer.BlockCopy(msg, 0, message, messageHeader.Length, msg.Length);

                //DecodeMessage(message);

                SendMessage(message);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        /// <summary>
        ///     Creates the bitmap byte array.
        /// </summary>
        /// <param name="isPrimary">if set to <c>true</c> [is primary].</param>
        /// <param name="values">The values.</param>
        /// <returns>The bitmap byte array</returns>
        private static byte[] CreateBitmap(bool isPrimary, int[] values)
        {
            var bits = new BitArray(new byte[8]);
            var bitmapString = string.Empty;
            if (isPrimary)
                for (var i = 1; i < bits.Length + 1; i++)
                    if (values.Any(indicator => indicator == i))
                        bitmapString += "1";
                    else
                        bitmapString += "0";
            else
                for (var i = 65; i < bits.Length * 2 + 1; i++)
                    if (values.Any(indicator => indicator == i))
                        bitmapString += "1";
                    else
                        bitmapString += "0";

            var bitmap = bitmapString.BinaryToHexString().HexToByteArray();
            return bitmap;
        }

        /// <summary>
        ///     Sends the message to destination IP.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void SendMessage(byte[] message)
        {
            try
            {
                var byteCount = Socket.Send(message, SocketFlags.None);
                Console.WriteLine("Sent {0} bytes.", byteCount);

                // Get reply from the server.
                var buffer = new byte[1024];
                byteCount = Socket.Receive(buffer, SocketFlags.None);
                var receivedBytes = new byte[byteCount];

                if (byteCount > 0)
                {
                    Console.WriteLine("Received {0} bytes.", byteCount);
                    for (var i = 0; i < byteCount; i++)
                        receivedBytes[i] = buffer[i];
                }

                // decode the message received 
                receivedBytes.DecodeMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}