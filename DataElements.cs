using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ISO_8583_MessageSandbox
{

    public static class DataElements
    {
        private static readonly int[] _elementVariableLength = new int[130];
        private static readonly int[] _elementFixedLength = new int[130];
        string[] _elementsIndicator = new string[130];

        public string Field127_3 { get; set; }
        public string Field127_6 { get; set; }
        public string Field127_9 { get; set; }

        static DataElements()
        {
            #region variable legths

            _elementVariableLength[2] = 16;
            _elementVariableLength[32] = 2;
            _elementVariableLength[33] = 2;
            _elementVariableLength[34] = 2;
            _elementVariableLength[35] = 2;
            _elementVariableLength[36] = 3;
            _elementVariableLength[44] = 25;
            _elementVariableLength[45] = 2;
            _elementVariableLength[46] = 3;
            _elementVariableLength[47] = 3;
            _elementVariableLength[48] = 3;
            _elementVariableLength[54] = 3;
            _elementVariableLength[55] = 3;
            _elementVariableLength[56] = 3;
            _elementVariableLength[57] = 3;
            _elementVariableLength[58] = 3;
            _elementVariableLength[59] = 3;
            _elementVariableLength[60] = 1;
            _elementVariableLength[61] = 3;
            _elementVariableLength[62] = 3;
            _elementVariableLength[63] = 3;
            _elementVariableLength[72] = 3;
            _elementVariableLength[99] = 2;
            _elementVariableLength[100] = 2;
            _elementVariableLength[102] = 2;
            _elementVariableLength[103] = 2;
            _elementVariableLength[104] = 3;
            _elementVariableLength[105] = 3;
            _elementVariableLength[106] = 3;
            _elementVariableLength[107] = 3;
            _elementVariableLength[108] = 3;
            _elementVariableLength[109] = 3;
            _elementVariableLength[110] = 3;
            _elementVariableLength[111] = 3;
            _elementVariableLength[112] = 3;
            _elementVariableLength[113] = 2;
            _elementVariableLength[114] = 3;
            _elementVariableLength[115] = 3;
            _elementVariableLength[116] = 3;
            _elementVariableLength[117] = 3;
            _elementVariableLength[118] = 3;
            _elementVariableLength[119] = 3;
            _elementVariableLength[120] = 3;
            _elementVariableLength[121] = 3;
            _elementVariableLength[122] = 3;
            _elementVariableLength[123] = 3;
            _elementVariableLength[124] = 3;
            _elementVariableLength[125] = 2;
            _elementVariableLength[126] = 1;
            _elementVariableLength[127] = 3;

            #endregion

            #region fixed length

            // "-" means not numeric.

            _elementFixedLength[0] = 16;
            _elementFixedLength[1] = 16;
            _elementFixedLength[3] = 6;
            _elementFixedLength[4] = 12;
            _elementFixedLength[5] = 12;
            _elementFixedLength[6] = 12;
            _elementFixedLength[7] = 10;
            _elementFixedLength[8] = 8;
            _elementFixedLength[9] = 8;
            _elementFixedLength[10] = 8;
            _elementFixedLength[11] = 6;
            _elementFixedLength[12] = 6;
            _elementFixedLength[13] = 4;
            _elementFixedLength[14] = 4;
            _elementFixedLength[15] = 4;
            _elementFixedLength[16] = 4;
            _elementFixedLength[17] = 4;
            _elementFixedLength[18] = 4;
            _elementFixedLength[19] = 3;
            _elementFixedLength[20] = 3;
            _elementFixedLength[21] = 3;
            _elementFixedLength[22] = 3;
            _elementFixedLength[23] = 3;
            _elementFixedLength[24] = 3;
            _elementFixedLength[25] = 2;
            _elementFixedLength[26] = 2;
            _elementFixedLength[27] = 1;
            _elementFixedLength[28] = 9;
            _elementFixedLength[29] = 8;
            _elementFixedLength[30] = 9;
            _elementFixedLength[31] = 8;
            _elementFixedLength[37] = -12;
            _elementFixedLength[38] = -6;
            _elementFixedLength[39] = -2;
            _elementFixedLength[40] = -3;
            _elementFixedLength[41] = -8;
            _elementFixedLength[42] = -15;
            _elementFixedLength[43] = -40;
            _elementFixedLength[44] = -25;
            _elementFixedLength[49] = -3;
            _elementFixedLength[50] = -3;
            _elementFixedLength[51] = -3;
            _elementFixedLength[52] = -8;
            _elementFixedLength[53] = 18;
            _elementFixedLength[64] = -4;
            _elementFixedLength[65] = -16;
            _elementFixedLength[66] = 1;
            _elementFixedLength[67] = 2;
            _elementFixedLength[68] = 3;
            _elementFixedLength[69] = 3;
            _elementFixedLength[70] = 3;
            _elementFixedLength[71] = 4;
            _elementFixedLength[73] = 6;
            _elementFixedLength[74] = 10;
            _elementFixedLength[75] = 10;
            _elementFixedLength[76] = 10;
            _elementFixedLength[77] = 10;
            _elementFixedLength[78] = 10;
            _elementFixedLength[79] = 10;
            _elementFixedLength[80] = 10;
            _elementFixedLength[81] = 10;
            _elementFixedLength[82] = 12;
            _elementFixedLength[83] = 12;
            _elementFixedLength[84] = 12;
            _elementFixedLength[85] = 12;
            _elementFixedLength[86] = 15;
            _elementFixedLength[87] = 15;
            _elementFixedLength[88] = 15;
            _elementFixedLength[89] = 15;
            _elementFixedLength[90] = 42;
            _elementFixedLength[91] = -1;
            _elementFixedLength[92] = 2;
            _elementFixedLength[93] = 5;
            _elementFixedLength[94] = -7;
            _elementFixedLength[95] = -42;
            _elementFixedLength[96] = -8;
            _elementFixedLength[97] = 16;
            _elementFixedLength[98] = -25;
            _elementFixedLength[101] = -17;
            _elementFixedLength[128] = -16;

            #endregion
        }

        public DataElements(byte[] elements, byte[] bitmap)
        {
            #region variable legths

            _elementVariableLength[2] = 16;
            _elementVariableLength[32] = 2;
            _elementVariableLength[33] = 2;
            _elementVariableLength[34] = 2;
            _elementVariableLength[35] = 2;
            _elementVariableLength[36] = 3;
            _elementVariableLength[44] = 25;
            _elementVariableLength[45] = 2;
            _elementVariableLength[46] = 3;
            _elementVariableLength[47] = 3;
            _elementVariableLength[48] = 3;
            _elementVariableLength[54] = 3;
            _elementVariableLength[55] = 3;
            _elementVariableLength[56] = 3;
            _elementVariableLength[57] = 3;
            _elementVariableLength[58] = 3;
            _elementVariableLength[59] = 3;
            _elementVariableLength[60] = 1;
            _elementVariableLength[61] = 3;
            _elementVariableLength[62] = 3;
            _elementVariableLength[63] = 3;
            _elementVariableLength[72] = 3;
            _elementVariableLength[99] = 2;
            _elementVariableLength[100] = 2;
            _elementVariableLength[102] = 2;
            _elementVariableLength[103] = 2;
            _elementVariableLength[104] = 3;
            _elementVariableLength[105] = 3;
            _elementVariableLength[106] = 3;
            _elementVariableLength[107] = 3;
            _elementVariableLength[108] = 3;
            _elementVariableLength[109] = 3;
            _elementVariableLength[110] = 3;
            _elementVariableLength[111] = 3;
            _elementVariableLength[112] = 3;
            _elementVariableLength[113] = 2;
            _elementVariableLength[114] = 3;
            _elementVariableLength[115] = 3;
            _elementVariableLength[116] = 3;
            _elementVariableLength[117] = 3;
            _elementVariableLength[118] = 3;
            _elementVariableLength[119] = 3;
            _elementVariableLength[120] = 3;
            _elementVariableLength[121] = 3;
            _elementVariableLength[122] = 3;
            _elementVariableLength[123] = 3;
            _elementVariableLength[124] = 3;
            _elementVariableLength[125] = 2;
            _elementVariableLength[126] = 1;
            _elementVariableLength[127] = 3;

            #endregion

            #region fixed length

            // "-" means not numeric.

            _elementFixedLength[0] = 16;
            _elementFixedLength[1] = 16;
            _elementFixedLength[3] = 6;
            _elementFixedLength[4] = 12;
            _elementFixedLength[5] = 12;
            _elementFixedLength[6] = 12;
            _elementFixedLength[7] = 10;
            _elementFixedLength[8] = 8;
            _elementFixedLength[9] = 8;
            _elementFixedLength[10] = 8;
            _elementFixedLength[11] = 6;
            _elementFixedLength[12] = 6;
            _elementFixedLength[13] = 4;
            _elementFixedLength[14] = 4;
            _elementFixedLength[15] = 4;
            _elementFixedLength[16] = 4;
            _elementFixedLength[17] = 4;
            _elementFixedLength[18] = 4;
            _elementFixedLength[19] = 3;
            _elementFixedLength[20] = 3;
            _elementFixedLength[21] = 3;
            _elementFixedLength[22] = 3;
            _elementFixedLength[23] = 3;
            _elementFixedLength[24] = 3;
            _elementFixedLength[25] = 2;
            _elementFixedLength[26] = 2;
            _elementFixedLength[27] = 1;
            _elementFixedLength[28] = 9;
            _elementFixedLength[29] = 8;
            _elementFixedLength[30] = 9;
            _elementFixedLength[31] = 8;
            _elementFixedLength[37] = -12;
            _elementFixedLength[38] = -6;
            _elementFixedLength[39] = -2;
            _elementFixedLength[40] = -3;
            _elementFixedLength[41] = -8;
            _elementFixedLength[42] = -15;
            _elementFixedLength[43] = -40;
            _elementFixedLength[44] = -25;
            _elementFixedLength[49] = -3;
            _elementFixedLength[50] = -3;
            _elementFixedLength[51] = -3;
            _elementFixedLength[52] = -8;
            _elementFixedLength[53] = 18;
            _elementFixedLength[64] = -4;
            _elementFixedLength[65] = -16;
            _elementFixedLength[66] = 1;
            _elementFixedLength[67] = 2;
            _elementFixedLength[68] = 3;
            _elementFixedLength[69] = 3;
            _elementFixedLength[70] = 3;
            _elementFixedLength[71] = 4;
            _elementFixedLength[73] = 6;
            _elementFixedLength[74] = 10;
            _elementFixedLength[75] = 10;
            _elementFixedLength[76] = 10;
            _elementFixedLength[77] = 10;
            _elementFixedLength[78] = 10;
            _elementFixedLength[79] = 10;
            _elementFixedLength[80] = 10;
            _elementFixedLength[81] = 10;
            _elementFixedLength[82] = 12;
            _elementFixedLength[83] = 12;
            _elementFixedLength[84] = 12;
            _elementFixedLength[85] = 12;
            _elementFixedLength[86] = 15;
            _elementFixedLength[87] = 15;
            _elementFixedLength[88] = 15;
            _elementFixedLength[89] = 15;
            _elementFixedLength[90] = 42;
            _elementFixedLength[91] = -1;
            _elementFixedLength[92] = 2;
            _elementFixedLength[93] = 5;
            _elementFixedLength[94] = -7;
            _elementFixedLength[95] = -42;
            _elementFixedLength[96] = -8;
            _elementFixedLength[97] = 16;
            _elementFixedLength[98] = -25;
            _elementFixedLength[101] = -17;
            _elementFixedLength[128] = -16;

            #endregion
        }

        /// <summary>
        /// Parses the specified data elements.
        /// </summary>
        /// <returns>Data Elements string array</returns>
        public string[] Parse(byte[] elements, byte[] bitmap)
        {
            // create the data elements array
            var dataElements = new string[130];
            try
            {
                var primaryBitmap = new byte[8];
                var secondaryBitmap = new byte[8];
                var secondaryBitmapBinary = string.Empty;

                // copy the primary bitmap out of the message bitmap
                Buffer.BlockCopy(bitmap, 0, primaryBitmap, 0, 8);
                var primaryBitmapBinary = HexStringToBinary(ByteArrayToHex(primaryBitmap));

                if (primaryBitmapBinary.Substring(0, 1) == "1")
                {
                    // copy the secondary bitmap out of the message bitmap
                    Buffer.BlockCopy(bitmap, 8, secondaryBitmap, 0, 8);
                    secondaryBitmapBinary = HexStringToBinary(ByteArrayToHex(secondaryBitmap));
                }

                var bitmapBinary = primaryBitmapBinary + secondaryBitmapBinary;
                var dataElementValues = Encoding.Default.GetString(elements);
                var position = 0;

                // loop thru all characters in binary representation
                for (var i = 2; i < bitmapBinary.Length; i++)
                {
                    int length;
                    switch (i)
                    {
                        case 2: // Primary account number. Length n..19 (note" .. indicates 2 digit length indicator)
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                length = Convert.ToInt32(dataElementValues.Substring(position, 2));
                                dataElements[i] = dataElementValues.Substring(0, length);
                                position = dataElements[i].Length + 2;
                            }
                            break;
                        case 32: // Financial Institute Id. Length n..11 
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                length = Convert.ToInt32(dataElementValues.Substring(position, 2));
                                dataElements[i] = dataElementValues.Substring(position + 2, length);
                                position = position + dataElements[i].Length + 2;
                            }
                            break;
                        case 44: // Additional response data. Length ..25
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                length = Convert.ToInt32(dataElementValues.Substring(position, 2));
                                dataElements[i] = dataElementValues.Substring(position + 2, length);
                                position = position + dataElements[i].Length + 2;
                            }
                            break;
                        case 52:
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                dataElements[i] =
                                    dataElementValues.Substring(position, Math.Abs(_elementFixedLength[i]));
                                position = position + Math.Abs(_elementFixedLength[i]);
                            }
                            break;
                        case 59: // Echo Data ..255
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                length = Convert.ToInt32(dataElementValues.Substring(position, 3));
                                dataElements[i] = dataElementValues.Substring(position + 3, length);
                                position = position + dataElements[i].Length + 3;
                            }
                            break;
                        case 123: // Terminal Type. Length ..15
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                // This field and is made up of a number of sub fields
                                // Positions 14 and15 from the left are relevant and must be used by the financial institution to indicate 
                                // which type of terminal the transaction originates from.
                                // The following are possible types
                                // POS : 01
                                // ATM : 02
                                // IVR : 20
                                // Internet : 90
                                length = Convert.ToInt32(dataElementValues.Substring(position, 3));
                                dataElements[i] = dataElementValues.Substring(position + 3, length);
                                position = position + dataElements[i].Length + 3;
                            }
                            break;
                        case 127:
                            // This is a variable-length field, consisting of a 3-digit length indicator,
                            // followed by an 8-byte bit map indicating the presence of fields, followed by the sub-fields.
                            // 127.3 Routing Information
                            // 126.6 Authorization Profile
                            // 127.9 Unique Transaction Id
                            Field127_3 = string.Empty;
                            Field127_6 = string.Empty;
                            Field127_9 = string.Empty;
                            dataElements[i] = string.Empty;
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                // determine field length including the bitmap
                                length = Convert.ToInt32(dataElementValues.Substring(position, 3));
                                var field127Bitmap = new byte[64];

                                // copy field bitmap into field 127 bitmap
                                var fieldBitmap = Encoding.UTF8.GetBytes(dataElementValues.Substring(position + 3, 25).Trim());
                                Buffer.BlockCopy(fieldBitmap, 0, field127Bitmap, 0, fieldBitmap.Length);

                                var field127BitmapBinary = HexStringToBinary(ByteArrayToHex(field127Bitmap));

                                field127BitmapBinary = field127BitmapBinary.Substring(0, 9);
                                position = position + 28;
                                var values = dataElementValues.Substring(position, length - 25);

                                // loop thru each item in binary string
                                for (var ii = 0; ii < field127BitmapBinary.Length + 1; ii++)
                                {
                                    switch (ii)
                                    {
                                        case 3:
                                            Field127_3 = values.Substring(0, 31);
                                            break;
                                        case 6:
                                            Field127_6 = values.Substring(31, 2);
                                            break;
                                        case 9:
                                            var subFieldLength = Convert.ToInt32(values.Substring(33, 3));
                                            Field127_9 = values.Substring(36, subFieldLength);
                                            break;
                                    }
                                }
                            }
                            break;
                        default:
                            // populate all fixed length elements
                            if (bitmapBinary.Substring(i - 1, 1) == "1")
                            {
                                dataElements[i] = dataElementValues.Substring(position, Math.Abs(_elementFixedLength[i]));
                                position = position + Math.Abs(_elementFixedLength[i]);
                            }
                            break;
                    }
                }

                return dataElements;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                return dataElements;
            }
        }

        private static string HexStringToBinary(string hex)
        {
            var binaryValue = string.Join(string.Empty, hex.Select(character => Convert.ToString(Convert.ToInt64(character.ToString(CultureInfo.InvariantCulture), 16), 2)
                                                                                       .PadLeft(4, '0')));
            return binaryValue;
        }

        private static string ByteArrayToHex(byte[] byteArray)
        {
            var hex = BitConverter.ToString(byteArray);
            return hex.Replace("-", "");
        }
    }

    public static string[] Parse(this DataElements)
    {

    }

}