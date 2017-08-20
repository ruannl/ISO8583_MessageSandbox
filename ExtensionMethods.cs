using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ISO_8583_MessageSandbox
{
    /// <summary>
    /// Extension class for convert int binary, hex and Ascii
    /// </summary>
    public static class ExtensionMethods
    {
        private static readonly StringDictionary ResponseCodes;

        static ExtensionMethods()
        {
            ResponseCodes = new StringDictionary
            {
                {"00", "Approved"},
                {"03", "Invalid Service Provider Id"},
                {"10", "Delay in processing the recharge"},
                {"12", "Invalid Recharge (e.g. Recharge attempt without activation, Business rule violation)"},
                {"13", "Invalid Recharge Denomination"},
                {"15", "Invalid Financial Institution Id"},
                {"22", "System Related Problem. Transaction Failed"},
                {"26", "Duplicate recharge attempted"},
                {"42", "Invalid MSISDN"},
                {"91", "back-end system is either unavailable or did not respond in time."}
            };

        }

        /// <summary>
        /// Converts Binary string to Hex string.
        /// </summary>
        /// <param name="binary">The binary string.</param>
        /// <returns>Hex string</returns>
        public static string BinaryToHexString(this string binary)
        {
            var result = new StringBuilder((binary.Length / 8) + 1);

            var mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (var i = 0; i < binary.Length; i += 8)
            {
                var eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts the byte array to hex.
        /// </summary>
        /// <param name="ba">The byte array.</param>
        /// <returns>Hex value</returns>
        private static string ByteArrayToHex(this byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts the Hex string to binary.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>Binary representation</returns>
        private static string HexStringToBinary(this string hex)
        {
            var binaryval = String.Join(String.Empty, hex.Select(c => Convert.ToString(Convert.ToInt64(c.ToString(CultureInfo.InvariantCulture), 16), 2).PadLeft(4, '0')));
            return binaryval;
        }

        /// <summary>
        /// Converts Hex to ASCII.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>The Ascii representation</returns>
        public static string HexToAscii(this string hex)
        {
            var sb = new StringBuilder();
            for (var i = 0; i <= hex.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(int.Parse(hex.Substring(i, 2), NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts ASCII text to Hex String.
        /// </summary>
        /// <param name="ascii">The ASCII string.</param>
        /// <returns>THe Hex representation</returns>
        public static string AsciiToHex(this string ascii)
        {
            var hex = string.Join(string.Empty, ascii.Select(c => ((int)c).ToString("X")).ToArray());
            return hex;
        }

        public static byte[] HexToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string With2DigitLengthIndicator(this string value)
        {
            return string.Concat(value.Length.ToString().PadLeft(2, '0'), value);
        }

        public static string With3DigitLengthIndicator(this string value)
        {
            return string.Concat(value.Length.ToString().PadLeft(3, '0'), value);
        }

        /// <summary>
        ///     Decodes the message received from socket.
        /// </summary>
        /// <param name="message">The message.</param>
        public static string DecodeMessage(this byte[] message)
        {
            try
            {
                // =================================================================================
                // Note: For comments on creating the message structure please refer to Echo Message
                // =================================================================================
                Log.WriteLine("=======================================");
                Log.WriteLine("response");
                Log.WriteLine("=======================================");

                // declare message structure byte arrays
                var messageHeader = new byte[2];
                var mti = new byte[4];
                var primaryBitmap = new byte[8];
                var secondaryBitmap = new byte[8];
                var secondaryBitmapAvailable = false;
                var msg = new byte[message.Length - 2];
                byte[] elements;
                byte[] messageBitmap;

                // populate the message structure objects
                Buffer.BlockCopy(message, 0, messageHeader, 0, 2);
                Buffer.BlockCopy(message, 2, msg, 0, message.Length - 2);
                Buffer.BlockCopy(message, 2, mti, 0, 4);
                Buffer.BlockCopy(message, 6, primaryBitmap, 0, 8);

                var primaryBitmapBinary = primaryBitmap.ByteArrayToHex()
                    .HexStringToBinary();

                // determine if secondary bitmap is available
                // copy primary bitmap and secondary bitmap into one byte array
                if (primaryBitmapBinary.Substring(0, 1) == "1")
                {
                    secondaryBitmapAvailable = true;
                    Buffer.BlockCopy(message, 14, secondaryBitmap, 0, 8);
                }

                if (secondaryBitmapAvailable)
                {
                    messageBitmap = new byte[16];
                    Buffer.BlockCopy(message, 6, messageBitmap, 0, 16);

                    elements = new byte[message.Length - 22];
                    Buffer.BlockCopy(message, 22, elements, 0, message.Length - 22);
                }
                else
                {
                    messageBitmap = new byte[8];
                    Buffer.BlockCopy(message, 6, messageBitmap, 0, 8);

                    elements = new byte[message.Length - 14];
                    Buffer.BlockCopy(message, 14, elements, 0, message.Length - 14);
                }

                Log.WriteLine($"MTI = {Encoding.Default.GetString(mti)}");

                // create the data elements object and parses the elements and bitmap
                var dataElements = new DataElements();
                var dataElement = dataElements.Parse(elements, messageBitmap);

                // write all populated data elements to log
                for (var i = 0; i < dataElement.Length; i++)
                    if (dataElement[i] != null)
                        Log.WriteLine($"DE #{i.ToString(CultureInfo.InvariantCulture).PadLeft(3, ' ')} =  {dataElement[i]}");

                Log.WriteLine($"DE #127.3 = {dataElements.Field127_3}");
                Log.WriteLine($"DE #127.6 = {dataElements.Field127_6}");
                Log.WriteLine($"DE #127.9 = {dataElements.Field127_9}");

                // response code (data elements 39)
                var responseMessage = string.Empty;
                if (dataElement[39] != null)
                {
                    // get the response code description 
                    responseMessage = ResponseCodes[dataElement[39]];

                    //if (dataElement[39] == "00")
                       // _authRequestApproved = true;
                }
                Log.WriteLine($"responseMessage = {responseMessage}");

                return responseMessage;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                return ex.ToString();
            }
        }
    }
}
