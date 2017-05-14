/**
 * Parser for MARC records
 * 
 * This project is based on the File_MARC package
 * (http://pear.php.net/package/File_MARC) by Dan Scott , which was based on PHP
 * MARC package, originally called "php-marc", that is part of the Emilda
 * Project (http://www.emilda.org). Both projects were released under the LGPL
 * which allowed me to port the project to C# for use with the .NET Framework.
 * 
 * GetChars() and AppendUnicodeCharacter based on code from SobekCM() Marc Library
 * https://sourceforge.net/projects/marclibrary/
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * @author    Mattie Schraeder-Urbanowicz <mattie@csharpmarc.net>
 * @copyright 2009-2017 Mattie Schraeder-Urbanowicz and Bound to Stay Bound Books <http://www.btsb.com>
 * @license   http://www.gnu.org/copyleft/lesser.html  LGPL License 3
 * 
 */

using System.Text;

namespace MARC
{
	public class MARC8 : Encoding
	{
		public override string EncodingName => "MARC8";

	    /// <summary>
		/// When overridden in a derived class, calculates the number of bytes produced by encoding a set of characters from the specified character array.
		/// </summary>
		/// <param name="chars">The character array containing the set of characters to encode.</param>
		/// <param name="index">The index of the first character to encode.</param>
		/// <param name="count">The number of characters to encode.</param>
		/// <returns>
		/// The number of bytes produced by encoding the specified characters.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override int GetByteCount(char[] chars, int index, int count)
		{
			byte[] b = new byte[1];
			return GetBytes(chars, index, count, b, -1);
		}

		/// <summary>
		/// When overridden in a derived class, encodes a set of characters from the specified character array into the specified byte array.
		/// </summary>
		/// <param name="chars">The character array containing the set of characters to encode.</param>
		/// <param name="charIndex">The index of the first character to encode.</param>
		/// <param name="charCount">The number of characters to encode.</param>
		/// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
		/// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
		/// <returns>
		/// The actual number of bytes written into <paramref name="bytes" />.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
		{
			int byteCount = 0;
			
			for (int i = 0; i < charCount; i++)
			{
				//A byteIndex of -1 means just keep reusing the same char. This saves memory when only doing counts
				int writeIndex = byteIndex;

                if (writeIndex < 0)
					writeIndex = 0;

                byte?[] convertedBytes = ConvertToMARC8Bytes(chars[charIndex + i]);

				foreach (byte? b in convertedBytes)				
				{
					if (b != null)
					{
						bytes[writeIndex] = (byte)b;
						if (byteIndex >= 0)
						{
							byteIndex++;
							writeIndex++;
						}
						byteCount++;
					}
				}
			}

			return byteCount;
		}

		/// <summary>
		/// When overridden in a derived class, calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
		/// </summary>
		/// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
		/// <param name="index">The index of the first byte to decode.</param>
		/// <param name="count">The number of bytes to decode.</param>
		/// <returns>
		/// The number of characters produced by decoding the specified sequence of bytes.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override int GetCharCount(byte[] bytes, int index, int count)
		{
			char[] c = new char[1];
			return GetChars(bytes, index, count, c, -1);
		}

		/// <summary>
		/// When overridden in a derived class, decodes a sequence of bytes from the specified byte array into the specified character array.
		/// </summary>
		/// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
		/// <param name="byteIndex">The index of the first byte to decode.</param>
		/// <param name="byteCount">The number of bytes to decode.</param>
		/// <param name="chars">The character array to contain the resulting set of characters.</param>
		/// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
		/// <returns>
		/// The actual number of characters written into <paramref name="chars" />.
		/// </returns>
		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
		{
            int marcByte1 = -1;
            int marcByte2 = -1;
			int charCount = 0;

            //Step through all the bytes in the array
            for (int i = 0; i < byteCount; i++)
            {
                //If any previous bytes, save them
                int marcByte3 = -1;
                marcByte3 = marcByte2;
                marcByte2 = marcByte1;
                
                //Get this byte from the array
                marcByte1 = bytes[byteIndex + i];

				//A charIndex of -1 means just keep reusing the same char. This saves memory when only doing counts
				int writeCharIndex = charIndex;
				if (writeCharIndex < 0)
					writeCharIndex = 0;

				//Try to convert the current byte to unicode character
				int charsAppended = AppendUnicodeCharacter(chars, writeCharIndex, marcByte1, marcByte2, marcByte3);
                if (charsAppended > 0)
                {
                    //Since the bytes were handled, clear them
                    marcByte1 = -1;
                    marcByte2 = -1;

					//Update the index and counts
					if (charIndex >= 0) //Only update the index if it wasn't -1
						charIndex += charsAppended;
					charCount += charsAppended;
                }
            }

            //Return the char count
			return charCount;
		}

		/// <summary>
		/// When overridden in a derived class, calculates the maximum number of bytes produced by encoding the specified number of characters.
		/// </summary>
		/// <param name="charCount">The number of characters to encode.</param>
		/// <returns>
		/// The maximum number of bytes produced by encoding the specified number of characters.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override int GetMaxByteCount(int charCount)
		{
			return charCount * 3;
		}

		/// <summary>
		/// When overridden in a derived class, calculates the maximum number of characters produced by decoding the specified number of bytes.
		/// </summary>
		/// <param name="byteCount">The number of bytes to decode.</param>
		/// <returns>
		/// The maximum number of characters produced by decoding the specified number of bytes.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override int GetMaxCharCount(int byteCount)
		{
			return byteCount;
        }

        /// <summary>
        /// Appends the unicode_ character.
        /// Based on code from Mark V. Sullivan's SobekCM MARC Library (https://sourceforge.net/projects/marclibrary/) which is also GPLv3
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="charIndex">Index of the character.</param>
        /// <param name="marcByte1">The marc byte1.</param>
        /// <param name="marcByte2">The marc byte2.</param>
        /// <param name="marcByte3">The marc byte3.</param>
        /// <returns></returns>
        private static int AppendUnicodeCharacter(char[] chars, int charIndex, int marcByte1, int marcByte2, int marcByte3)
        {
            //For the special characters in MARC encoding, return FALSE, indicating the byte was not yet handled. (Need the next byte(s))
            if (marcByte1 >= 224)
                return 0;

            //Case where there is only one byte to handle (and not a special case returned already from above lines)
            if ((marcByte2 == -1) && (marcByte3 == -1))
            {
				switch (marcByte1)
				{
					case 136:
						chars[charIndex] = ((char)0x0098);
						break;
					case 137:
						chars[charIndex] = ((char)0x009C);
						break;
					case 162:
						chars[charIndex] = ((char)0x00D8);
						break;
					case 164:
						chars[charIndex] = ((char)0x00DE);
						break;
					case 165:
						chars[charIndex] = ((char)0x00C6);
						break;
					case 168:
						chars[charIndex] = ((char)0x00B7);
						break;
					case 170:
						chars[charIndex] = ((char)0x00AE);
						break;
					case 171:
						chars[charIndex] = ((char)0x00B1);
						break;
					case 185:
						chars[charIndex] = ((char)0x00A3);
						break;
					case 192:
						chars[charIndex] = ((char)0x00B0);
						break;
					case 195:
						chars[charIndex] = ((char)0x00A9);
						break;
					case 197:
						chars[charIndex] = ((char)0x00BF);
						break;
					case 198:
						chars[charIndex] = ((char)0x00A1);
						break;
					case 199:
						chars[charIndex] = ((char)0x00DF);
						break;
					default:
						chars[charIndex] = ((char)marcByte1);
						break;
				}
                
                return 1;
            }

            //Is this just a two byte combination?
            if (marcByte3 == -1)
            {
                if (marcByte2 != -1)
                {
                    if (marcByte2 == 224)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x1EA2);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x1EBA);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x1EC8);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x1ECE);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x1EE6);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x1EF6);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x1EA3);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x1EBB);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x1EC9);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x1ECF);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x1EE7);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1EF7);
                                return 1;
                            case 172:
                                chars[charIndex] = ((char) 0x1EDE);
                                return 1;
                            case 173:
                                chars[charIndex] = ((char) 0x1EEC);
                                return 1;
                            case 188:
                                chars[charIndex] = ((char) 0x1EDF);
                                return 1;
                            case 189:
                                chars[charIndex] = ((char) 0x1EED);
                                return 1;
                        }
                    }
                    if (marcByte2 == 225)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C0);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x00C8);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x00CC);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x01F8);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x00D2);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x00D9);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x1E80);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x1EF2);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E0);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x00E8);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x00EC);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x01F9);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x00F2);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x00F9);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E81);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1EF3);
                                return 1;
                            case 172:
                                chars[charIndex] = ((char) 0x1EDC);
                                return 1;
                            case 173:
                                chars[charIndex] = ((char) 0x1EEA);
                                return 1;
                            case 188:
                                chars[charIndex] = ((char) 0x1EDD);
                                return 1;
                            case 189:
                                chars[charIndex] = ((char) 0x1EEB);
                                return 1;
                        }
                    }
                    if (marcByte2 == 226)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C1);
                                return 1;
                            case 67:
                                chars[charIndex] = ((char) 0x0106);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x00C9);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x01F4);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x00CD);
                                return 1;
                            case 75:
                                chars[charIndex] = ((char) 0x1E30);
                                return 1;
                            case 76:
                                chars[charIndex] = ((char) 0x0139);
                                return 1;
                            case 77:
                                chars[charIndex] = ((char) 0x1E3E);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x0143);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x00D3);
                                return 1;
                            case 80:
                                chars[charIndex] = ((char) 0x1E54);
                                return 1;
                            case 82:
                                chars[charIndex] = ((char) 0x0154);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x015A);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x00DA);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x1E82);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x00DD);
                                return 1;
                            case 90:
                                chars[charIndex] = ((char) 0x0179);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E1);
                                return 1;
                            case 99:
                                chars[charIndex] = ((char) 0x0107);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x00E9);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x01F5);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x00ED);
                                return 1;
                            case 107:
                                chars[charIndex] = ((char) 0x1E31);
                                return 1;
                            case 108:
                                chars[charIndex] = ((char) 0x013A);
                                return 1;
                            case 109:
                                chars[charIndex] = ((char) 0x1E3F);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x0144);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x00F3);
                                return 1;
                            case 112:
                                chars[charIndex] = ((char) 0x1E55);
                                return 1;
                            case 114:
                                chars[charIndex] = ((char) 0x0155);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x015B);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x00FA);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E83);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x00FD);
                                return 1;
                            case 122:
                                chars[charIndex] = ((char) 0x017A);
                                return 1;
                            case 162:
                                chars[charIndex] = ((char) 0x01FE);
                                return 1;
                            case 165:
                                chars[charIndex] = ((char) 0x01FC);
                                return 1;
                            case 172:
                                chars[charIndex] = ((char) 0x1EDA);
                                return 1;
                            case 173:
                                chars[charIndex] = ((char) 0x1EE8);
                                return 1;
                            case 178:
                                chars[charIndex] = ((char) 0x01FF);
                                return 1;
                            case 181:
                                chars[charIndex] = ((char) 0x01FD);
                                return 1;
                            case 188:
                                chars[charIndex] = ((char) 0x1EDB);
                                return 1;
                            case 189:
                                chars[charIndex] = ((char) 0x1EE9);
                                return 1;
                            case 232:
                                chars[charIndex] = ((char) 0x0344);
                                return 1;
                        }
                    }
                    if (marcByte2 == 227)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C2);
                                return 1;
                            case 67:
                                chars[charIndex] = ((char) 0x0108);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x00CA);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x011C);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x0124);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x00CE);
                                return 1;
                            case 74:
                                chars[charIndex] = ((char) 0x0134);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x00D4);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x015C);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x00DB);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x0174);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x0176);
                                return 1;
                            case 90:
                                chars[charIndex] = ((char) 0x1E90);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E2);
                                return 1;
                            case 99:
                                chars[charIndex] = ((char) 0x0109);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x00EA);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x011D);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x0125);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x00EE);
                                return 1;
                            case 106:
                                chars[charIndex] = ((char) 0x0135);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x00F4);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x015D);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x00FB);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x0175);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x0177);
                                return 1;
                            case 122:
                                chars[charIndex] = ((char) 0x1E91);
                                return 1;
                        }
                    }
                    if (marcByte2 == 228)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C3);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x1EBC);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x0128);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x00D1);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x00D5);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x0168);
                                return 1;
                            case 86:
                                chars[charIndex] = ((char) 0x1E7C);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x1EF8);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E3);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x1EBD);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x0129);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x00F1);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x00F5);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x0169);
                                return 1;
                            case 118:
                                chars[charIndex] = ((char) 0x1E7D);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1EF9);
                                return 1;
                            case 172:
                                chars[charIndex] = ((char) 0x1EE0);
                                return 1;
                            case 173:
                                chars[charIndex] = ((char) 0x1EEE);
                                return 1;
                            case 188:
                                chars[charIndex] = ((char) 0x1EE1);
                                return 1;
                            case 189:
                                chars[charIndex] = ((char) 0x1EEF);
                                return 1;
                        }
                    }
                    if (marcByte2 == 229)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x0100);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x0112);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x1E20);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x012A);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x014C);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x016A);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x0232);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x0101);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x0113);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x1E21);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x012B);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x014D);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x016B);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x0233);
                                return 1;
                            case 165:
                                chars[charIndex] = ((char) 0x01E2);
                                return 1;
                            case 181:
                                chars[charIndex] = ((char) 0x01E3);
                                return 1;
                        }
                    }
                    if (marcByte2 == 230)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x0102);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x0114);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x011E);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x012C);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x014E);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x016C);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x0103);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x0115);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x011F);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x012D);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x014F);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x016D);
                                return 1;
                        }
                    }
                    if (marcByte2 == 231)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x0226);
                                return 1;
                            case 66:
                                chars[charIndex] = ((char) 0x1E02);
                                return 1;
                            case 67:
                                chars[charIndex] = ((char) 0x010A);
                                return 1;
                            case 68:
                                chars[charIndex] = ((char) 0x1E0A);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x0116);
                                return 1;
                            case 70:
                                chars[charIndex] = ((char) 0x1E1E);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x0120);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x1E22);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x0130);
                                return 1;
                            case 77:
                                chars[charIndex] = ((char) 0x1E40);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x1E44);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x022E);
                                return 1;
                            case 80:
                                chars[charIndex] = ((char) 0x1E56);
                                return 1;
                            case 82:
                                chars[charIndex] = ((char) 0x1E58);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x1E60);
                                return 1;
                            case 84:
                                chars[charIndex] = ((char) 0x1E6A);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x1E86);
                                return 1;
                            case 88:
                                chars[charIndex] = ((char) 0x1E8A);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x1E8E);
                                return 1;
                            case 90:
                                chars[charIndex] = ((char) 0x017B);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x0227);
                                return 1;
                            case 98:
                                chars[charIndex] = ((char) 0x1E03);
                                return 1;
                            case 99:
                                chars[charIndex] = ((char) 0x010B);
                                return 1;
                            case 100:
                                chars[charIndex] = ((char) 0x1E0B);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x0117);
                                return 1;
                            case 102:
                                chars[charIndex] = ((char) 0x1E1F);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x0121);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x1E23);
                                return 1;
                            case 109:
                                chars[charIndex] = ((char) 0x1E41);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x1E45);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x022F);
                                return 1;
                            case 112:
                                chars[charIndex] = ((char) 0x1E57);
                                return 1;
                            case 114:
                                chars[charIndex] = ((char) 0x1E59);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x1E61);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x1E6B);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E87);
                                return 1;
                            case 120:
                                chars[charIndex] = ((char) 0x1E8B);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1E8F);
                                return 1;
                            case 122:
                                chars[charIndex] = ((char) 0x017C);
                                return 1;
                        }
                    }
                    if (marcByte2 == 232)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C4);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x00CB);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x1E26);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x00CF);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x00D6);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x00DC);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x1E84);
                                return 1;
                            case 88:
                                chars[charIndex] = ((char) 0x1E8C);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x0178);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E4);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x00EB);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x1E27);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x00EF);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x00F6);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x1E97);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x00FC);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E85);
                                return 1;
                            case 120:
                                chars[charIndex] = ((char) 0x1E8D);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x00FF);
                                return 1;
                        }
                    }
                    if (marcByte2 == 233)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x01CD);
                                return 1;
                            case 67:
                                chars[charIndex] = ((char) 0x010C);
                                return 1;
                            case 68:
                                chars[charIndex] = ((char) 0x010E);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x011A);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x01E6);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x021E);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x01CF);
                                return 1;
                            case 75:
                                chars[charIndex] = ((char) 0x01E8);
                                return 1;
                            case 76:
                                chars[charIndex] = ((char) 0x013D);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x0147);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x01D1);
                                return 1;
                            case 82:
                                chars[charIndex] = ((char) 0x0158);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x0160);
                                return 1;
                            case 84:
                                chars[charIndex] = ((char) 0x0164);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x01D3);
                                return 1;
                            case 90:
                                chars[charIndex] = ((char) 0x017D);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x01CE);
                                return 1;
                            case 99:
                                chars[charIndex] = ((char) 0x010D);
                                return 1;
                            case 100:
                                chars[charIndex] = ((char) 0x010F);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x011B);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x01E7);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x021F);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x01D0);
                                return 1;
                            case 106:
                                chars[charIndex] = ((char) 0x01F0);
                                return 1;
                            case 107:
                                chars[charIndex] = ((char) 0x01E9);
                                return 1;
                            case 108:
                                chars[charIndex] = ((char) 0x013E);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x0148);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x01D2);
                                return 1;
                            case 114:
                                chars[charIndex] = ((char) 0x0159);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x0161);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x0165);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x01D4);
                                return 1;
                            case 122:
                                chars[charIndex] = ((char) 0x017E);
                                return 1;
                        }
                    }
                    if (marcByte2 == 234)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x00C5);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x016E);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x00E5);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x016F);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E98);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1E99);
                                return 1;
                        }
                    }
                    if (marcByte2 == 238)
                    {
                        switch (marcByte1)
                        {
                            case 79:
                                chars[charIndex] = ((char) 0x0150);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x0170);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x0151);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x0171);
                                return 1;
                        }
                    }
                    if (marcByte2 == 240)
                    {
                        switch (marcByte1)
                        {
                            case 67:
                                chars[charIndex] = ((char) 0x00C7);
                                return 1;
                            case 68:
                                chars[charIndex] = ((char) 0x1E10);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x0228);
                                return 1;
                            case 71:
                                chars[charIndex] = ((char) 0x0122);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x1E28);
                                return 1;
                            case 75:
                                chars[charIndex] = ((char) 0x0136);
                                return 1;
                            case 76:
                                chars[charIndex] = ((char) 0x013B);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x0145);
                                return 1;
                            case 82:
                                chars[charIndex] = ((char) 0x0156);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x015E);
                                return 1;
                            case 84:
                                chars[charIndex] = ((char) 0x0162);
                                return 1;
                            case 99:
                                chars[charIndex] = ((char) 0x00E7);
                                return 1;
                            case 100:
                                chars[charIndex] = ((char) 0x1E11);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x0229);
                                return 1;
                            case 103:
                                chars[charIndex] = ((char) 0x0123);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x1E29);
                                return 1;
                            case 107:
                                chars[charIndex] = ((char) 0x0137);
                                return 1;
                            case 108:
                                chars[charIndex] = ((char) 0x013C);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x0146);
                                return 1;
                            case 114:
                                chars[charIndex] = ((char) 0x0157);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x015F);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x0163);
                                return 1;
                        }
                    }
                    if (marcByte2 == 241)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x0104);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x0118);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x012E);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x01EA);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x0172);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x0105);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x0119);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x012F);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x01EB);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x0173);
                                return 1;
                        }
                    }
                    if (marcByte2 == 242)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x1EA0);
                                return 1;
                            case 66:
                                chars[charIndex] = ((char) 0x1E04);
                                return 1;
                            case 68:
                                chars[charIndex] = ((char) 0x1E0C);
                                return 1;
                            case 69:
                                chars[charIndex] = ((char) 0x1EB8);
                                return 1;
                            case 72:
                                chars[charIndex] = ((char) 0x1E24);
                                return 1;
                            case 73:
                                chars[charIndex] = ((char) 0x1ECA);
                                return 1;
                            case 75:
                                chars[charIndex] = ((char) 0x1E32);
                                return 1;
                            case 76:
                                chars[charIndex] = ((char) 0x1E36);
                                return 1;
                            case 77:
                                chars[charIndex] = ((char) 0x1E42);
                                return 1;
                            case 78:
                                chars[charIndex] = ((char) 0x1E46);
                                return 1;
                            case 79:
                                chars[charIndex] = ((char) 0x1ECC);
                                return 1;
                            case 82:
                                chars[charIndex] = ((char) 0x1E5A);
                                return 1;
                            case 83:
                                chars[charIndex] = ((char) 0x1E62);
                                return 1;
                            case 84:
                                chars[charIndex] = ((char) 0x1E6C);
                                return 1;
                            case 85:
                                chars[charIndex] = ((char) 0x1EE4);
                                return 1;
                            case 86:
                                chars[charIndex] = ((char) 0x1E7E);
                                return 1;
                            case 87:
                                chars[charIndex] = ((char) 0x1E88);
                                return 1;
                            case 89:
                                chars[charIndex] = ((char) 0x1EF4);
                                return 1;
                            case 90:
                                chars[charIndex] = ((char) 0x1E92);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x1EA1);
                                return 1;
                            case 98:
                                chars[charIndex] = ((char) 0x1E05);
                                return 1;
                            case 100:
                                chars[charIndex] = ((char) 0x1E0D);
                                return 1;
                            case 101:
                                chars[charIndex] = ((char) 0x1EB9);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x1E25);
                                return 1;
                            case 105:
                                chars[charIndex] = ((char) 0x1ECB);
                                return 1;
                            case 107:
                                chars[charIndex] = ((char) 0x1E33);
                                return 1;
                            case 108:
                                chars[charIndex] = ((char) 0x1E37);
                                return 1;
                            case 109:
                                chars[charIndex] = ((char) 0x1E43);
                                return 1;
                            case 110:
                                chars[charIndex] = ((char) 0x1E47);
                                return 1;
                            case 111:
                                chars[charIndex] = ((char) 0x1ECD);
                                return 1;
                            case 114:
                                chars[charIndex] = ((char) 0x1E5B);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x1E63);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x1E6D);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x1EE5);
                                return 1;
                            case 118:
                                chars[charIndex] = ((char) 0x1E7F);
                                return 1;
                            case 119:
                                chars[charIndex] = ((char) 0x1E89);
                                return 1;
                            case 121:
                                chars[charIndex] = ((char) 0x1EF5);
                                return 1;
                            case 122:
                                chars[charIndex] = ((char) 0x1E93);
                                return 1;
                            case 172:
                                chars[charIndex] = ((char) 0x1EE2);
                                return 1;
                            case 173:
                                chars[charIndex] = ((char) 0x1EF0);
                                return 1;
                            case 188:
                                chars[charIndex] = ((char) 0x1EE3);
                                return 1;
                            case 189:
                                chars[charIndex] = ((char) 0x1EF1);
                                return 1;
                        }
                    }
                    if (marcByte2 == 243)
                    {
                        switch (marcByte1)
                        {
                            case 85:
                                chars[charIndex] = ((char) 0x1E72);
                                return 1;
                            case 117:
                                chars[charIndex] = ((char) 0x1E73);
                                return 1;
                        }
                    }
                    if (marcByte2 == 244)
                    {
                        switch (marcByte1)
                        {
                            case 65:
                                chars[charIndex] = ((char) 0x1E00);
                                return 1;
                            case 97:
                                chars[charIndex] = ((char) 0x1E01);
                                return 1;
                        }
                    }
                    if (marcByte2 == 247)
                    {
                        switch (marcByte1)
                        {
                            case 83:
                                chars[charIndex] = ((char) 0x0218);
                                return 1;
                            case 84:
                                chars[charIndex] = ((char) 0x021A);
                                return 1;
                            case 115:
                                chars[charIndex] = ((char) 0x0219);
                                return 1;
                            case 116:
                                chars[charIndex] = ((char) 0x021B);
                                return 1;
                        }
                    }
                    if (marcByte2 == 249)
                    {
                        switch (marcByte1)
                        {
                            case 72:
                                chars[charIndex] = ((char) 0x1E2A);
                                return 1;
                            case 104:
                                chars[charIndex] = ((char) 0x1E2B);
                                return 1;
                        }
                    }
                }

                return 0;
            }
            else //This is a THREE byte combination
            {
                if (marcByte3 == 224)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EA8);
                                    return 1;
                                case 69:
                                    chars[charIndex] = ((char) 0x1EC2);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1ED4);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EA9);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1EC3);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1ED5);
                                    return 1;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EB2);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EB3);
                                    return 1;
                            }
                            break;
                    }
                }

                if (marcByte3 == 225)
                {
                    switch (marcByte2)
                    {

                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EA6);
                                    return 1;
                                case 69:
                                    chars[charIndex] = ((char) 0x1EC0);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1ED2);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EA7);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1EC1);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1ED3);
                                    return 1;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 69:
                                    chars[charIndex] = ((char) 0x1E14);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1E50);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1E15);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1E51);
                                    return 1;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EB0);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EB1);
                                    return 1;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 85:
                                    chars[charIndex] = ((char) 0x01DB);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x01DC);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 226)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EA4);
                                    return 1;
                                case 69:
                                    chars[charIndex] = ((char) 0x1EBE);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1ED0);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EA5);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1EBF);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1ED1);
                                    return 1;
                            }
                            break;
                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    chars[charIndex] = ((char) 0x1E4C);
                                    return 1;
                                case 85:
                                    chars[charIndex] = ((char) 0x1E78);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1E4D);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x1E79);
                                    return 1;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 69:
                                    chars[charIndex] = ((char) 0x1E16);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1E52);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1E17);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1E53);
                                    return 1;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EAE);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EAF);
                                    return 1;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 73:
                                    chars[charIndex] = ((char) 0x1E2E);
                                    return 1;
                                case 85:
                                    chars[charIndex] = ((char) 0x01D7);
                                    return 1;
                                case 105:
                                    chars[charIndex] = ((char) 0x1E2F);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x01D8);
                                    return 1;
                            }
                            break;
                        case 234:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x01FA);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x01FB);
                                    return 1;
                            }
                            break;
                        case 240:
                            switch (marcByte1)
                            {
                                case 67:
                                    chars[charIndex] = ((char) 0x1E08);
                                    return 1;
                                case 99:
                                    chars[charIndex] = ((char) 0x1E09);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 227)
                {
                    switch (marcByte2)
                    {
                        case 242:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EAC);
                                    return 1;
                                case 69:
                                    chars[charIndex] = ((char) 0x1EC6);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1ED8);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EAD);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1EC7);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1ED9);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 228)
                {
                    switch (marcByte2)
                    {
                        case 227:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EAA);
                                    return 1;
                                case 69:
                                    chars[charIndex] = ((char) 0x1EC4);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x1ED6);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EAB);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1EC5);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1ED7);
                                    return 1;
                            }
                            break;
                        case 230:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EB4);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EB5);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 229)
                {
                    switch (marcByte2)
                    {

                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    chars[charIndex] = ((char) 0x022C);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x022D);
                                    return 1;
                            }
                            break;
                        case 231:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x01E0);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x0230);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x01E1);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x0231);
                                    return 1;
                            }
                            break;
                        case 232:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x01DE);
                                    return 1;
                                case 79:
                                    chars[charIndex] = ((char) 0x022A);
                                    return 1;
                                case 85:
                                    chars[charIndex] = ((char) 0x01D5);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x01DF);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x022B);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x01D6);
                                    return 1;
                            }
                            break;
                        case 241:
                            switch (marcByte1)
                            {
                                case 79:
                                    chars[charIndex] = ((char) 0x01EC);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x01ED);
                                    return 1;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 76:
                                    chars[charIndex] = ((char) 0x1E38);
                                    return 1;
                                case 82:
                                    chars[charIndex] = ((char) 0x1E5C);
                                    return 1;
                                case 108:
                                    chars[charIndex] = ((char) 0x1E39);
                                    return 1;
                                case 114:
                                    chars[charIndex] = ((char) 0x1E5D);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 230)
                {
                    switch (marcByte2)
                    {

                        case 240:
                            switch (marcByte1)
                            {
                                case 69:
                                    chars[charIndex] = ((char) 0x1E1C);
                                    return 1;
                                case 101:
                                    chars[charIndex] = ((char) 0x1E1D);
                                    return 1;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 65:
                                    chars[charIndex] = ((char) 0x1EB6);
                                    return 1;
                                case 97:
                                    chars[charIndex] = ((char) 0x1EB7);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 231)
                {
                    switch (marcByte2)
                    {

                        case 226:
                            switch (marcByte1)
                            {
                                case 83:
                                    chars[charIndex] = ((char) 0x1E64);
                                    return 1;
                                case 115:
                                    chars[charIndex] = ((char) 0x1E65);
                                    return 1;
                            }
                            break;
                        case 233:
                            switch (marcByte1)
                            {
                                case 83:
                                    chars[charIndex] = ((char) 0x1E66);
                                    return 1;
                                case 115:
                                    chars[charIndex] = ((char) 0x1E67);
                                    return 1;
                            }
                            break;
                        case 242:
                            switch (marcByte1)
                            {
                                case 83:
                                    chars[charIndex] = ((char) 0x1E68);
                                    return 1;
                                case 115:
                                    chars[charIndex] = ((char) 0x1E69);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 232)
                {
                    switch (marcByte2)
                    {
                        case 228:
                            switch (marcByte1)
                            {
                                case 79:
                                    chars[charIndex] = ((char) 0x1E4E);
                                    return 1;
                                case 111:
                                    chars[charIndex] = ((char) 0x1E4F);
                                    return 1;
                            }
                            break;
                        case 229:
                            switch (marcByte1)
                            {
                                case 85:
                                    chars[charIndex] = ((char) 0x1E7A);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x1E7B);
                                    return 1;
                            }
                            break;
                    }
                }
                if (marcByte3 == 233)
                {
                    switch (marcByte2)
                    {
                        case 232:
                            switch (marcByte1)
                            {
                                case 85:
                                    chars[charIndex] = ((char) 0x01D9);
                                    return 1;
                                case 117:
                                    chars[charIndex] = ((char) 0x01DA);
                                    return 1;
                            }
                            break;
                    }
                }

				//Since this is a three byte combination, just need to handle it SOMEHOW before the third byte is lost, or any introduced problem gets compounded. So, default this to just Unicode encoding.
                string fallbackEncoding = UTF8.GetString(new [] { (byte) marcByte3, (byte) marcByte2, (byte) marcByte1 });
				foreach (char c in fallbackEncoding)
				{
					chars[charIndex] = c;
					charIndex++;
					//This should only happen if we're doing a count instead of a full write
					if (charIndex == chars.Length)
						charIndex--;
				}
                return fallbackEncoding.Length;
            }
        }

		/// <summary>
		/// Converts a character to MARC8 bytes.
		/// </summary>
		/// <param name="c">The character</param>
		/// <returns>The converted MARC8 bytes</returns>
		private static byte?[] ConvertToMARC8Bytes(char c)
		{
			byte?[] bytes = new byte?[] { null, null, null };

			if (c < 127)
			{
				bytes[0] = (byte)c;
			}
			else if (c < 224)
			{
				switch (c)
				{
					case '\u0098':
						bytes[0] = 136;
						break;
					case '\u009c':
						bytes[0] = 137;
						break;
					case '\u00d8':
						bytes[0] = 162;
						break;
					case '\u00de':
						bytes[0] = 164;
						break;
					case '\u00c6':
						bytes[0] = 165;
						break;
					case '\u00b7':
						bytes[0] = 168;
						break;
					case '\u00ae':
						bytes[0] = 170;
						break;
					case '\u00b1':
						bytes[0] = 171;
						break;
					case '\u00a3':
						bytes[0] = 185;
						break;
					case '\u00b0':
						bytes[0] = 192;
						break;
					case '\u00a9':
						bytes[0] = 195;
						break;
					case '\u00bf':
						bytes[0] = 197;
						break;
					case '\u00a1':
						bytes[0] = 198;
						break;
					case '\u00df':
						bytes[0] = 199;
						break;
				}
			}
			else
			{
				switch (c)
				{
					case '\u1ea2':
						bytes[0] = 224;
						bytes[1] = 65;
						break;
					case '\u0100':
						bytes[0] = 229;
						bytes[1] = 65;
						break;
					case '\u0102':
						bytes[0] = 230;
						bytes[1] = 65;
						break;
					case '\u0226':
						bytes[0] = 231;
						bytes[1] = 65;
						break;
					case '\u01cd':
						bytes[0] = 233;
						bytes[1] = 65;
						break;
					case '\u0104':
						bytes[0] = 241;
						bytes[1] = 65;
						break;
					case '\u1ea0':
						bytes[0] = 242;
						bytes[1] = 65;
						break;
					case '\u1e00':
						bytes[0] = 244;
						bytes[1] = 65;
						break;
					case '\u1e02':
						bytes[0] = 231;
						bytes[1] = 66;
						break;
					case '\u1e04':
						bytes[0] = 242;
						bytes[1] = 66;
						break;
					case '\u0106':
						bytes[0] = 226;
						bytes[1] = 67;
						break;
					case '\u0108':
						bytes[0] = 227;
						bytes[1] = 67;
						break;
					case '\u010a':
						bytes[0] = 231;
						bytes[1] = 67;
						break;
					case '\u010c':
						bytes[0] = 233;
						bytes[1] = 67;
						break;
					case '\u1e0a':
						bytes[0] = 231;
						bytes[1] = 68;
						break;
					case '\u010e':
						bytes[0] = 233;
						bytes[1] = 68;
						break;
					case '\u1e10':
						bytes[0] = 240;
						bytes[1] = 68;
						break;
					case '\u1e0c':
						bytes[0] = 242;
						bytes[1] = 68;
						break;
					case '\u1eba':
						bytes[0] = 224;
						bytes[1] = 69;
						break;
					case '\u1ebc':
						bytes[0] = 228;
						bytes[1] = 69;
						break;
					case '\u0112':
						bytes[0] = 229;
						bytes[1] = 69;
						break;
					case '\u0114':
						bytes[0] = 230;
						bytes[1] = 69;
						break;
					case '\u0116':
						bytes[0] = 231;
						bytes[1] = 69;
						break;
					case '\u011a':
						bytes[0] = 233;
						bytes[1] = 69;
						break;
					case '\u0228':
						bytes[0] = 240;
						bytes[1] = 69;
						break;
					case '\u0118':
						bytes[0] = 241;
						bytes[1] = 69;
						break;
					case '\u1eb8':
						bytes[0] = 242;
						bytes[1] = 69;
						break;
					case '\u1e1e':
						bytes[0] = 231;
						bytes[1] = 70;
						break;
					case '\u01f4':
						bytes[0] = 226;
						bytes[1] = 71;
						break;
					case '\u011c':
						bytes[0] = 227;
						bytes[1] = 71;
						break;
					case '\u1e20':
						bytes[0] = 229;
						bytes[1] = 71;
						break;
					case '\u011e':
						bytes[0] = 230;
						bytes[1] = 71;
						break;
					case '\u0120':
						bytes[0] = 231;
						bytes[1] = 71;
						break;
					case '\u01e6':
						bytes[0] = 233;
						bytes[1] = 71;
						break;
					case '\u0122':
						bytes[0] = 240;
						bytes[1] = 71;
						break;
					case '\u0124':
						bytes[0] = 227;
						bytes[1] = 72;
						break;
					case '\u1e22':
						bytes[0] = 231;
						bytes[1] = 72;
						break;
					case '\u1e26':
						bytes[0] = 232;
						bytes[1] = 72;
						break;
					case '\u021e':
						bytes[0] = 233;
						bytes[1] = 72;
						break;
					case '\u1e28':
						bytes[0] = 240;
						bytes[1] = 72;
						break;
					case '\u1e24':
						bytes[0] = 242;
						bytes[1] = 72;
						break;
					case '\u1e2a':
						bytes[0] = 249;
						bytes[1] = 72;
						break;
					case '\u1ec8':
						bytes[0] = 224;
						bytes[1] = 73;
						break;
					case '\u0128':
						bytes[0] = 228;
						bytes[1] = 73;
						break;
					case '\u012a':
						bytes[0] = 229;
						bytes[1] = 73;
						break;
					case '\u012c':
						bytes[0] = 230;
						bytes[1] = 73;
						break;
					case '\u0130':
						bytes[0] = 231;
						bytes[1] = 73;
						break;
					case '\u01cf':
						bytes[0] = 233;
						bytes[1] = 73;
						break;
					case '\u012e':
						bytes[0] = 241;
						bytes[1] = 73;
						break;
					case '\u1eca':
						bytes[0] = 242;
						bytes[1] = 73;
						break;
					case '\u0134':
						bytes[0] = 227;
						bytes[1] = 74;
						break;
					case '\u1e30':
						bytes[0] = 226;
						bytes[1] = 75;
						break;
					case '\u01e8':
						bytes[0] = 233;
						bytes[1] = 75;
						break;
					case '\u0136':
						bytes[0] = 240;
						bytes[1] = 75;
						break;
					case '\u1e32':
						bytes[0] = 242;
						bytes[1] = 75;
						break;
					case '\u0139':
						bytes[0] = 226;
						bytes[1] = 76;
						break;
					case '\u013d':
						bytes[0] = 233;
						bytes[1] = 76;
						break;
					case '\u013b':
						bytes[0] = 240;
						bytes[1] = 76;
						break;
					case '\u1e36':
						bytes[0] = 242;
						bytes[1] = 76;
						break;
					case '\u1e3e':
						bytes[0] = 226;
						bytes[1] = 77;
						break;
					case '\u1e40':
						bytes[0] = 231;
						bytes[1] = 77;
						break;
					case '\u1e42':
						bytes[0] = 242;
						bytes[1] = 77;
						break;
					case '\u01f8':
						bytes[0] = 225;
						bytes[1] = 78;
						break;
					case '\u0143':
						bytes[0] = 226;
						bytes[1] = 78;
						break;
					case '\u1e44':
						bytes[0] = 231;
						bytes[1] = 78;
						break;
					case '\u0147':
						bytes[0] = 233;
						bytes[1] = 78;
						break;
					case '\u0145':
						bytes[0] = 240;
						bytes[1] = 78;
						break;
					case '\u1e46':
						bytes[0] = 242;
						bytes[1] = 78;
						break;
					case '\u1ece':
						bytes[0] = 224;
						bytes[1] = 79;
						break;
					case '\u014c':
						bytes[0] = 229;
						bytes[1] = 79;
						break;
					case '\u014e':
						bytes[0] = 230;
						bytes[1] = 79;
						break;
					case '\u022e':
						bytes[0] = 231;
						bytes[1] = 79;
						break;
					case '\u01d1':
						bytes[0] = 233;
						bytes[1] = 79;
						break;
					case '\u0150':
						bytes[0] = 238;
						bytes[1] = 79;
						break;
					case '\u01ea':
						bytes[0] = 241;
						bytes[1] = 79;
						break;
					case '\u1ecc':
						bytes[0] = 242;
						bytes[1] = 79;
						break;
					case '\u1e54':
						bytes[0] = 226;
						bytes[1] = 80;
						break;
					case '\u1e56':
						bytes[0] = 231;
						bytes[1] = 80;
						break;
					case '\u0154':
						bytes[0] = 226;
						bytes[1] = 82;
						break;
					case '\u1e58':
						bytes[0] = 231;
						bytes[1] = 82;
						break;
					case '\u0158':
						bytes[0] = 233;
						bytes[1] = 82;
						break;
					case '\u0156':
						bytes[0] = 240;
						bytes[1] = 82;
						break;
					case '\u1e5a':
						bytes[0] = 242;
						bytes[1] = 82;
						break;
					case '\u015a':
						bytes[0] = 226;
						bytes[1] = 83;
						break;
					case '\u015c':
						bytes[0] = 227;
						bytes[1] = 83;
						break;
					case '\u1e60':
						bytes[0] = 231;
						bytes[1] = 83;
						break;
					case '\u0160':
						bytes[0] = 233;
						bytes[1] = 83;
						break;
					case '\u015e':
						bytes[0] = 240;
						bytes[1] = 83;
						break;
					case '\u1e62':
						bytes[0] = 242;
						bytes[1] = 83;
						break;
					case '\u0218':
						bytes[0] = 247;
						bytes[1] = 83;
						break;
					case '\u1e6a':
						bytes[0] = 231;
						bytes[1] = 84;
						break;
					case '\u0164':
						bytes[0] = 233;
						bytes[1] = 84;
						break;
					case '\u0162':
						bytes[0] = 240;
						bytes[1] = 84;
						break;
					case '\u1e6c':
						bytes[0] = 242;
						bytes[1] = 84;
						break;
					case '\u021a':
						bytes[0] = 247;
						bytes[1] = 84;
						break;
					case '\u1ee6':
						bytes[0] = 224;
						bytes[1] = 85;
						break;
					case '\u0168':
						bytes[0] = 228;
						bytes[1] = 85;
						break;
					case '\u016a':
						bytes[0] = 229;
						bytes[1] = 85;
						break;
					case '\u016c':
						bytes[0] = 230;
						bytes[1] = 85;
						break;
					case '\u01d3':
						bytes[0] = 233;
						bytes[1] = 85;
						break;
					case '\u016e':
						bytes[0] = 234;
						bytes[1] = 85;
						break;
					case '\u0170':
						bytes[0] = 238;
						bytes[1] = 85;
						break;
					case '\u0172':
						bytes[0] = 241;
						bytes[1] = 85;
						break;
					case '\u1ee4':
						bytes[0] = 242;
						bytes[1] = 85;
						break;
					case '\u1e72':
						bytes[0] = 243;
						bytes[1] = 85;
						break;
					case '\u1e7c':
						bytes[0] = 228;
						bytes[1] = 86;
						break;
					case '\u1e7e':
						bytes[0] = 242;
						bytes[1] = 86;
						break;
					case '\u1e80':
						bytes[0] = 225;
						bytes[1] = 87;
						break;
					case '\u1e82':
						bytes[0] = 226;
						bytes[1] = 87;
						break;
					case '\u0174':
						bytes[0] = 227;
						bytes[1] = 87;
						break;
					case '\u1e86':
						bytes[0] = 231;
						bytes[1] = 87;
						break;
					case '\u1e84':
						bytes[0] = 232;
						bytes[1] = 87;
						break;
					case '\u1e88':
						bytes[0] = 242;
						bytes[1] = 87;
						break;
					case '\u1e8a':
						bytes[0] = 231;
						bytes[1] = 88;
						break;
					case '\u1e8c':
						bytes[0] = 232;
						bytes[1] = 88;
						break;
					case '\u1ef6':
						bytes[0] = 224;
						bytes[1] = 89;
						break;
					case '\u1ef2':
						bytes[0] = 225;
						bytes[1] = 89;
						break;
					case '\u0176':
						bytes[0] = 227;
						bytes[1] = 89;
						break;
					case '\u1ef8':
						bytes[0] = 228;
						bytes[1] = 89;
						break;
					case '\u0232':
						bytes[0] = 229;
						bytes[1] = 89;
						break;
					case '\u1e8e':
						bytes[0] = 231;
						bytes[1] = 89;
						break;
					case '\u0178':
						bytes[0] = 232;
						bytes[1] = 89;
						break;
					case '\u1ef4':
						bytes[0] = 242;
						bytes[1] = 89;
						break;
					case '\u0179':
						bytes[0] = 226;
						bytes[1] = 90;
						break;
					case '\u1e90':
						bytes[0] = 227;
						bytes[1] = 90;
						break;
					case '\u017b':
						bytes[0] = 231;
						bytes[1] = 90;
						break;
					case '\u017d':
						bytes[0] = 233;
						bytes[1] = 90;
						break;
					case '\u1e92':
						bytes[0] = 242;
						bytes[1] = 90;
						break;
					case '\u1ea3':
						bytes[0] = 224;
						bytes[1] = 97;
						break;
					case '\u00e0':
						bytes[0] = 225;
						bytes[1] = 97;
						break;
					case '\u00e1':
						bytes[0] = 226;
						bytes[1] = 97;
						break;
					case '\u00e2':
						bytes[0] = 227;
						bytes[1] = 97;
						break;
					case '\u00e3':
						bytes[0] = 228;
						bytes[1] = 97;
						break;
					case '\u0101':
						bytes[0] = 229;
						bytes[1] = 97;
						break;
					case '\u0103':
						bytes[0] = 230;
						bytes[1] = 97;
						break;
					case '\u0227':
						bytes[0] = 231;
						bytes[1] = 97;
						break;
					case '\u00e4':
						bytes[0] = 232;
						bytes[1] = 97;
						break;
					case '\u01ce':
						bytes[0] = 233;
						bytes[1] = 97;
						break;
					case '\u00e5':
						bytes[0] = 234;
						bytes[1] = 97;
						break;
					case '\u0105':
						bytes[0] = 241;
						bytes[1] = 97;
						break;
					case '\u1ea1':
						bytes[0] = 242;
						bytes[1] = 97;
						break;
					case '\u1e01':
						bytes[0] = 244;
						bytes[1] = 97;
						break;
					case '\u1e03':
						bytes[0] = 231;
						bytes[1] = 98;
						break;
					case '\u1e05':
						bytes[0] = 242;
						bytes[1] = 98;
						break;
					case '\u0107':
						bytes[0] = 226;
						bytes[1] = 99;
						break;
					case '\u0109':
						bytes[0] = 227;
						bytes[1] = 99;
						break;
					case '\u010b':
						bytes[0] = 231;
						bytes[1] = 99;
						break;
					case '\u010d':
						bytes[0] = 233;
						bytes[1] = 99;
						break;
					case '\u00e7':
						bytes[0] = 240;
						bytes[1] = 99;
						break;
					case '\u1e0b':
						bytes[0] = 231;
						bytes[1] = 100;
						break;
					case '\u010f':
						bytes[0] = 233;
						bytes[1] = 100;
						break;
					case '\u1e11':
						bytes[0] = 240;
						bytes[1] = 100;
						break;
					case '\u1e0d':
						bytes[0] = 242;
						bytes[1] = 100;
						break;
					case '\u1ebb':
						bytes[0] = 224;
						bytes[1] = 101;
						break;
					case '\u00e8':
						bytes[0] = 225;
						bytes[1] = 101;
						break;
					case '\u00e9':
						bytes[0] = 226;
						bytes[1] = 101;
						break;
					case '\u00ea':
						bytes[0] = 227;
						bytes[1] = 101;
						break;
					case '\u1ebd':
						bytes[0] = 228;
						bytes[1] = 101;
						break;
					case '\u0113':
						bytes[0] = 229;
						bytes[1] = 101;
						break;
					case '\u0115':
						bytes[0] = 230;
						bytes[1] = 101;
						break;
					case '\u0117':
						bytes[0] = 231;
						bytes[1] = 101;
						break;
					case '\u00eb':
						bytes[0] = 232;
						bytes[1] = 101;
						break;
					case '\u011b':
						bytes[0] = 233;
						bytes[1] = 101;
						break;
					case '\u0229':
						bytes[0] = 240;
						bytes[1] = 101;
						break;
					case '\u0119':
						bytes[0] = 241;
						bytes[1] = 101;
						break;
					case '\u1eb9':
						bytes[0] = 242;
						bytes[1] = 101;
						break;
					case '\u1e1f':
						bytes[0] = 231;
						bytes[1] = 102;
						break;
					case '\u01f5':
						bytes[0] = 226;
						bytes[1] = 103;
						break;
					case '\u011d':
						bytes[0] = 227;
						bytes[1] = 103;
						break;
					case '\u1e21':
						bytes[0] = 229;
						bytes[1] = 103;
						break;
					case '\u011f':
						bytes[0] = 230;
						bytes[1] = 103;
						break;
					case '\u0121':
						bytes[0] = 231;
						bytes[1] = 103;
						break;
					case '\u01e7':
						bytes[0] = 233;
						bytes[1] = 103;
						break;
					case '\u0123':
						bytes[0] = 240;
						bytes[1] = 103;
						break;
					case '\u0125':
						bytes[0] = 227;
						bytes[1] = 104;
						break;
					case '\u1e23':
						bytes[0] = 231;
						bytes[1] = 104;
						break;
					case '\u1e27':
						bytes[0] = 232;
						bytes[1] = 104;
						break;
					case '\u021f':
						bytes[0] = 233;
						bytes[1] = 104;
						break;
					case '\u1e29':
						bytes[0] = 240;
						bytes[1] = 104;
						break;
					case '\u1e25':
						bytes[0] = 242;
						bytes[1] = 104;
						break;
					case '\u1e2b':
						bytes[0] = 249;
						bytes[1] = 104;
						break;
					case '\u1ec9':
						bytes[0] = 224;
						bytes[1] = 105;
						break;
					case '\u00ec':
						bytes[0] = 225;
						bytes[1] = 105;
						break;
					case '\u00ed':
						bytes[0] = 226;
						bytes[1] = 105;
						break;
					case '\u00ee':
						bytes[0] = 227;
						bytes[1] = 105;
						break;
					case '\u0129':
						bytes[0] = 228;
						bytes[1] = 105;
						break;
					case '\u012b':
						bytes[0] = 229;
						bytes[1] = 105;
						break;
					case '\u012d':
						bytes[0] = 230;
						bytes[1] = 105;
						break;
					case '\u00ef':
						bytes[0] = 232;
						bytes[1] = 105;
						break;
					case '\u01d0':
						bytes[0] = 233;
						bytes[1] = 105;
						break;
					case '\u012f':
						bytes[0] = 241;
						bytes[1] = 105;
						break;
					case '\u1ecb':
						bytes[0] = 242;
						bytes[1] = 105;
						break;
					case '\u0135':
						bytes[0] = 227;
						bytes[1] = 106;
						break;
					case '\u01f0':
						bytes[0] = 233;
						bytes[1] = 106;
						break;
					case '\u1e31':
						bytes[0] = 226;
						bytes[1] = 107;
						break;
					case '\u01e9':
						bytes[0] = 233;
						bytes[1] = 107;
						break;
					case '\u0137':
						bytes[0] = 240;
						bytes[1] = 107;
						break;
					case '\u1e33':
						bytes[0] = 242;
						bytes[1] = 107;
						break;
					case '\u013a':
						bytes[0] = 226;
						bytes[1] = 108;
						break;
					case '\u013e':
						bytes[0] = 233;
						bytes[1] = 108;
						break;
					case '\u013c':
						bytes[0] = 240;
						bytes[1] = 108;
						break;
					case '\u1e37':
						bytes[0] = 242;
						bytes[1] = 108;
						break;
					case '\u1e3f':
						bytes[0] = 226;
						bytes[1] = 109;
						break;
					case '\u1e41':
						bytes[0] = 231;
						bytes[1] = 109;
						break;
					case '\u1e43':
						bytes[0] = 242;
						bytes[1] = 109;
						break;
					case '\u01f9':
						bytes[0] = 225;
						bytes[1] = 110;
						break;
					case '\u0144':
						bytes[0] = 226;
						bytes[1] = 110;
						break;
					case '\u00f1':
						bytes[0] = 228;
						bytes[1] = 110;
						break;
					case '\u1e45':
						bytes[0] = 231;
						bytes[1] = 110;
						break;
					case '\u0148':
						bytes[0] = 233;
						bytes[1] = 110;
						break;
					case '\u0146':
						bytes[0] = 240;
						bytes[1] = 110;
						break;
					case '\u1e47':
						bytes[0] = 242;
						bytes[1] = 110;
						break;
					case '\u1ecf':
						bytes[0] = 224;
						bytes[1] = 111;
						break;
					case '\u00f2':
						bytes[0] = 225;
						bytes[1] = 111;
						break;
					case '\u00f3':
						bytes[0] = 226;
						bytes[1] = 111;
						break;
					case '\u00f4':
						bytes[0] = 227;
						bytes[1] = 111;
						break;
					case '\u00f5':
						bytes[0] = 228;
						bytes[1] = 111;
						break;
					case '\u014d':
						bytes[0] = 229;
						bytes[1] = 111;
						break;
					case '\u014f':
						bytes[0] = 230;
						bytes[1] = 111;
						break;
					case '\u022f':
						bytes[0] = 231;
						bytes[1] = 111;
						break;
					case '\u00f6':
						bytes[0] = 232;
						bytes[1] = 111;
						break;
					case '\u01d2':
						bytes[0] = 233;
						bytes[1] = 111;
						break;
					case '\u0151':
						bytes[0] = 238;
						bytes[1] = 111;
						break;
					case '\u01eb':
						bytes[0] = 241;
						bytes[1] = 111;
						break;
					case '\u1ecd':
						bytes[0] = 242;
						bytes[1] = 111;
						break;
					case '\u1e55':
						bytes[0] = 226;
						bytes[1] = 112;
						break;
					case '\u1e57':
						bytes[0] = 231;
						bytes[1] = 112;
						break;
					case '\u0155':
						bytes[0] = 226;
						bytes[1] = 114;
						break;
					case '\u1e59':
						bytes[0] = 231;
						bytes[1] = 114;
						break;
					case '\u0159':
						bytes[0] = 233;
						bytes[1] = 114;
						break;
					case '\u0157':
						bytes[0] = 240;
						bytes[1] = 114;
						break;
					case '\u1e5b':
						bytes[0] = 242;
						bytes[1] = 114;
						break;
					case '\u015b':
						bytes[0] = 226;
						bytes[1] = 115;
						break;
					case '\u015d':
						bytes[0] = 227;
						bytes[1] = 115;
						break;
					case '\u1e61':
						bytes[0] = 231;
						bytes[1] = 115;
						break;
					case '\u0161':
						bytes[0] = 233;
						bytes[1] = 115;
						break;
					case '\u015f':
						bytes[0] = 240;
						bytes[1] = 115;
						break;
					case '\u1e63':
						bytes[0] = 242;
						bytes[1] = 115;
						break;
					case '\u0219':
						bytes[0] = 247;
						bytes[1] = 115;
						break;
					case '\u1e6b':
						bytes[0] = 231;
						bytes[1] = 116;
						break;
					case '\u1e97':
						bytes[0] = 232;
						bytes[1] = 116;
						break;
					case '\u0165':
						bytes[0] = 233;
						bytes[1] = 116;
						break;
					case '\u0163':
						bytes[0] = 240;
						bytes[1] = 116;
						break;
					case '\u1e6d':
						bytes[0] = 242;
						bytes[1] = 116;
						break;
					case '\u021b':
						bytes[0] = 247;
						bytes[1] = 116;
						break;
					case '\u1ee7':
						bytes[0] = 224;
						bytes[1] = 117;
						break;
					case '\u00f9':
						bytes[0] = 225;
						bytes[1] = 117;
						break;
					case '\u00fa':
						bytes[0] = 226;
						bytes[1] = 117;
						break;
					case '\u00fb':
						bytes[0] = 227;
						bytes[1] = 117;
						break;
					case '\u0169':
						bytes[0] = 228;
						bytes[1] = 117;
						break;
					case '\u016b':
						bytes[0] = 229;
						bytes[1] = 117;
						break;
					case '\u016d':
						bytes[0] = 230;
						bytes[1] = 117;
						break;
					case '\u00fc':
						bytes[0] = 232;
						bytes[1] = 117;
						break;
					case '\u01d4':
						bytes[0] = 233;
						bytes[1] = 117;
						break;
					case '\u016f':
						bytes[0] = 234;
						bytes[1] = 117;
						break;
					case '\u0171':
						bytes[0] = 238;
						bytes[1] = 117;
						break;
					case '\u0173':
						bytes[0] = 241;
						bytes[1] = 117;
						break;
					case '\u1ee5':
						bytes[0] = 242;
						bytes[1] = 117;
						break;
					case '\u1e73':
						bytes[0] = 243;
						bytes[1] = 117;
						break;
					case '\u1e7d':
						bytes[0] = 228;
						bytes[1] = 118;
						break;
					case '\u1e7f':
						bytes[0] = 242;
						bytes[1] = 118;
						break;
					case '\u1e81':
						bytes[0] = 225;
						bytes[1] = 119;
						break;
					case '\u1e83':
						bytes[0] = 226;
						bytes[1] = 119;
						break;
					case '\u0175':
						bytes[0] = 227;
						bytes[1] = 119;
						break;
					case '\u1e87':
						bytes[0] = 231;
						bytes[1] = 119;
						break;
					case '\u1e85':
						bytes[0] = 232;
						bytes[1] = 119;
						break;
					case '\u1e98':
						bytes[0] = 234;
						bytes[1] = 119;
						break;
					case '\u1e89':
						bytes[0] = 242;
						bytes[1] = 119;
						break;
					case '\u1e8b':
						bytes[0] = 231;
						bytes[1] = 120;
						break;
					case '\u1e8d':
						bytes[0] = 232;
						bytes[1] = 120;
						break;
					case '\u1ef7':
						bytes[0] = 224;
						bytes[1] = 121;
						break;
					case '\u1ef3':
						bytes[0] = 225;
						bytes[1] = 121;
						break;
					case '\u00fd':
						bytes[0] = 226;
						bytes[1] = 121;
						break;
					case '\u0177':
						bytes[0] = 227;
						bytes[1] = 121;
						break;
					case '\u1ef9':
						bytes[0] = 228;
						bytes[1] = 121;
						break;
					case '\u0233':
						bytes[0] = 229;
						bytes[1] = 121;
						break;
					case '\u1e8f':
						bytes[0] = 231;
						bytes[1] = 121;
						break;
					case '\u00ff':
						bytes[0] = 232;
						bytes[1] = 121;
						break;
					case '\u1e99':
						bytes[0] = 234;
						bytes[1] = 121;
						break;
					case '\u1ef5':
						bytes[0] = 242;
						bytes[1] = 121;
						break;
					case '\u017a':
						bytes[0] = 226;
						bytes[1] = 122;
						break;
					case '\u1e91':
						bytes[0] = 227;
						bytes[1] = 122;
						break;
					case '\u017c':
						bytes[0] = 231;
						bytes[1] = 122;
						break;
					case '\u017e':
						bytes[0] = 233;
						bytes[1] = 122;
						break;
					case '\u1e93':
						bytes[0] = 242;
						bytes[1] = 122;
						break;
					case '\u01fe':
						bytes[0] = 226;
						bytes[1] = 162;
						break;
					case '\u01fc':
						bytes[0] = 226;
						bytes[1] = 165;
						break;
					case '\u01e2':
						bytes[0] = 229;
						bytes[1] = 165;
						break;
					case '\u1ede':
						bytes[0] = 224;
						bytes[1] = 172;
						break;
					case '\u1edc':
						bytes[0] = 225;
						bytes[1] = 172;
						break;
					case '\u1eda':
						bytes[0] = 226;
						bytes[1] = 172;
						break;
					case '\u1ee0':
						bytes[0] = 228;
						bytes[1] = 172;
						break;
					case '\u1ee2':
						bytes[0] = 242;
						bytes[1] = 172;
						break;
					case '\u1eec':
						bytes[0] = 224;
						bytes[1] = 173;
						break;
					case '\u1eea':
						bytes[0] = 225;
						bytes[1] = 173;
						break;
					case '\u1ee8':
						bytes[0] = 226;
						bytes[1] = 173;
						break;
					case '\u1eee':
						bytes[0] = 228;
						bytes[1] = 173;
						break;
					case '\u1ef0':
						bytes[0] = 242;
						bytes[1] = 173;
						break;
					case '\u01ff':
						bytes[0] = 226;
						bytes[1] = 178;
						break;
					case '\u01fd':
						bytes[0] = 226;
						bytes[1] = 181;
						break;
					case '\u01e3':
						bytes[0] = 229;
						bytes[1] = 181;
						break;
					case '\u1edf':
						bytes[0] = 224;
						bytes[1] = 188;
						break;
					case '\u1edd':
						bytes[0] = 225;
						bytes[1] = 188;
						break;
					case '\u1edb':
						bytes[0] = 226;
						bytes[1] = 188;
						break;
					case '\u1ee1':
						bytes[0] = 228;
						bytes[1] = 188;
						break;
					case '\u1ee3':
						bytes[0] = 242;
						bytes[1] = 188;
						break;
					case '\u1eed':
						bytes[0] = 224;
						bytes[1] = 189;
						break;
					case '\u1eeb':
						bytes[0] = 225;
						bytes[1] = 189;
						break;
					case '\u1ee9':
						bytes[0] = 226;
						bytes[1] = 189;
						break;
					case '\u1eef':
						bytes[0] = 228;
						bytes[1] = 189;
						break;
					case '\u1ef1':
						bytes[0] = 242;
						bytes[1] = 189;
						break;
					case '\u1ea8':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 65;
						break;
					case '\u1ea6':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 65;
						break;
					case '\u1ea4':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 65;
						break;
					case '\u1eaa':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 65;
						break;
					case '\u1eb2':
						bytes[0] = 224;
						bytes[1] = 230;
						bytes[2] = 65;
						break;
					case '\u1eb0':
						bytes[0] = 225;
						bytes[1] = 230;
						bytes[2] = 65;
						break;
					case '\u1eae':
						bytes[0] = 226;
						bytes[1] = 230;
						bytes[2] = 65;
						break;
					case '\u1eb4':
						bytes[0] = 228;
						bytes[1] = 230;
						bytes[2] = 65;
						break;
					case '\u01e0':
						bytes[0] = 229;
						bytes[1] = 231;
						bytes[2] = 65;
						break;
					case '\u01de':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 65;
						break;
					case '\u01fa':
						bytes[0] = 226;
						bytes[1] = 234;
						bytes[2] = 65;
						break;
					case '\u1eac':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 65;
						break;
					case '\u1eb6':
						bytes[0] = 230;
						bytes[1] = 242;
						bytes[2] = 65;
						break;
					case '\u1e08':
						bytes[0] = 226;
						bytes[1] = 240;
						bytes[2] = 67;
						break;
					case '\u1ec2':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 69;
						break;
					case '\u1ec0':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 69;
						break;
					case '\u1ebe':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 69;
						break;
					case '\u1ec4':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 69;
						break;
					case '\u1e14':
						bytes[0] = 225;
						bytes[1] = 229;
						bytes[2] = 69;
						break;
					case '\u1e16':
						bytes[0] = 226;
						bytes[1] = 229;
						bytes[2] = 69;
						break;
					case '\u1e1c':
						bytes[0] = 230;
						bytes[1] = 240;
						bytes[2] = 69;
						break;
					case '\u1ec6':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 69;
						break;
					case '\u1e2e':
						bytes[0] = 226;
						bytes[1] = 232;
						bytes[2] = 73;
						break;
					case '\u1e38':
						bytes[0] = 229;
						bytes[1] = 242;
						bytes[2] = 76;
						break;
					case '\u1ed4':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 79;
						break;
					case '\u1ed2':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 79;
						break;
					case '\u1ed0':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 79;
						break;
					case '\u1ed6':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 79;
						break;
					case '\u1e4c':
						bytes[0] = 226;
						bytes[1] = 228;
						bytes[2] = 79;
						break;
					case '\u022c':
						bytes[0] = 229;
						bytes[1] = 228;
						bytes[2] = 79;
						break;
					case '\u1e4e':
						bytes[0] = 232;
						bytes[1] = 228;
						bytes[2] = 79;
						break;
					case '\u1e50':
						bytes[0] = 225;
						bytes[1] = 229;
						bytes[2] = 79;
						break;
					case '\u1e52':
						bytes[0] = 226;
						bytes[1] = 229;
						bytes[2] = 79;
						break;
					case '\u0230':
						bytes[0] = 229;
						bytes[1] = 231;
						bytes[2] = 79;
						break;
					case '\u022a':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 79;
						break;
					case '\u01ec':
						bytes[0] = 229;
						bytes[1] = 241;
						bytes[2] = 79;
						break;
					case '\u1ed8':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 79;
						break;
					case '\u1e5c':
						bytes[0] = 229;
						bytes[1] = 242;
						bytes[2] = 82;
						break;
					case '\u1e64':
						bytes[0] = 231;
						bytes[1] = 226;
						bytes[2] = 83;
						break;
					case '\u1e66':
						bytes[0] = 231;
						bytes[1] = 233;
						bytes[2] = 83;
						break;
					case '\u1e68':
						bytes[0] = 231;
						bytes[1] = 242;
						bytes[2] = 83;
						break;
					case '\u1e78':
						bytes[0] = 226;
						bytes[1] = 228;
						bytes[2] = 85;
						break;
					case '\u1e7a':
						bytes[0] = 232;
						bytes[1] = 229;
						bytes[2] = 85;
						break;
					case '\u01db':
						bytes[0] = 225;
						bytes[1] = 232;
						bytes[2] = 85;
						break;
					case '\u01d7':
						bytes[0] = 226;
						bytes[1] = 232;
						bytes[2] = 85;
						break;
					case '\u01d5':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 85;
						break;
					case '\u01d9':
						bytes[0] = 233;
						bytes[1] = 232;
						bytes[2] = 85;
						break;
					case '\u1ea9':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 97;
						break;
					case '\u1ea7':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 97;
						break;
					case '\u1ea5':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 97;
						break;
					case '\u1eab':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 97;
						break;
					case '\u1eb3':
						bytes[0] = 224;
						bytes[1] = 230;
						bytes[2] = 97;
						break;
					case '\u1eb1':
						bytes[0] = 225;
						bytes[1] = 230;
						bytes[2] = 97;
						break;
					case '\u1eaf':
						bytes[0] = 226;
						bytes[1] = 230;
						bytes[2] = 97;
						break;
					case '\u1eb5':
						bytes[0] = 228;
						bytes[1] = 230;
						bytes[2] = 97;
						break;
					case '\u01e1':
						bytes[0] = 229;
						bytes[1] = 231;
						bytes[2] = 97;
						break;
					case '\u01df':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 97;
						break;
					case '\u01fb':
						bytes[0] = 226;
						bytes[1] = 234;
						bytes[2] = 97;
						break;
					case '\u1ead':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 97;
						break;
					case '\u1eb7':
						bytes[0] = 230;
						bytes[1] = 242;
						bytes[2] = 97;
						break;
					case '\u1e09':
						bytes[0] = 226;
						bytes[1] = 240;
						bytes[2] = 99;
						break;
					case '\u1ec3':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 101;
						break;
					case '\u1ec1':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 101;
						break;
					case '\u1ebf':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 101;
						break;
					case '\u1ec5':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 101;
						break;
					case '\u1e15':
						bytes[0] = 225;
						bytes[1] = 229;
						bytes[2] = 101;
						break;
					case '\u1e17':
						bytes[0] = 226;
						bytes[1] = 229;
						bytes[2] = 101;
						break;
					case '\u1e1d':
						bytes[0] = 230;
						bytes[1] = 240;
						bytes[2] = 101;
						break;
					case '\u1ec7':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 101;
						break;
					case '\u1e2f':
						bytes[0] = 226;
						bytes[1] = 232;
						bytes[2] = 105;
						break;
					case '\u1e39':
						bytes[0] = 229;
						bytes[1] = 242;
						bytes[2] = 108;
						break;
					case '\u1ed5':
						bytes[0] = 224;
						bytes[1] = 227;
						bytes[2] = 111;
						break;
					case '\u1ed3':
						bytes[0] = 225;
						bytes[1] = 227;
						bytes[2] = 111;
						break;
					case '\u1ed1':
						bytes[0] = 226;
						bytes[1] = 227;
						bytes[2] = 111;
						break;
					case '\u1ed7':
						bytes[0] = 228;
						bytes[1] = 227;
						bytes[2] = 111;
						break;
					case '\u1e4d':
						bytes[0] = 226;
						bytes[1] = 228;
						bytes[2] = 111;
						break;
					case '\u022d':
						bytes[0] = 229;
						bytes[1] = 228;
						bytes[2] = 111;
						break;
					case '\u1e4f':
						bytes[0] = 232;
						bytes[1] = 228;
						bytes[2] = 111;
						break;
					case '\u1e51':
						bytes[0] = 225;
						bytes[1] = 229;
						bytes[2] = 111;
						break;
					case '\u1e53':
						bytes[0] = 226;
						bytes[1] = 229;
						bytes[2] = 111;
						break;
					case '\u0231':
						bytes[0] = 229;
						bytes[1] = 231;
						bytes[2] = 111;
						break;
					case '\u022b':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 111;
						break;
					case '\u01ed':
						bytes[0] = 229;
						bytes[1] = 241;
						bytes[2] = 111;
						break;
					case '\u1ed9':
						bytes[0] = 227;
						bytes[1] = 242;
						bytes[2] = 111;
						break;
					case '\u1e5d':
						bytes[0] = 229;
						bytes[1] = 242;
						bytes[2] = 114;
						break;
					case '\u1e65':
						bytes[0] = 231;
						bytes[1] = 226;
						bytes[2] = 115;
						break;
					case '\u1e67':
						bytes[0] = 231;
						bytes[1] = 233;
						bytes[2] = 115;
						break;
					case '\u1e69':
						bytes[0] = 231;
						bytes[1] = 242;
						bytes[2] = 115;
						break;
					case '\u1e79':
						bytes[0] = 226;
						bytes[1] = 228;
						bytes[2] = 117;
						break;
					case '\u1e7b':
						bytes[0] = 232;
						bytes[1] = 229;
						bytes[2] = 117;
						break;
					case '\u01dc':
						bytes[0] = 225;
						bytes[1] = 232;
						bytes[2] = 117;
						break;
					case '\u01d8':
						bytes[0] = 226;
						bytes[1] = 232;
						bytes[2] = 117;
						break;
					case '\u01d6':
						bytes[0] = 229;
						bytes[1] = 232;
						bytes[2] = 117;
						break;
					case '\u01da':
						bytes[0] = 233;
						bytes[1] = 232;
						bytes[2] = 117;
						break;
				}
			}

			return bytes;
		}
	}
}