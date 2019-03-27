using System.Text;

namespace System
{
    /// <summary>
    /// 提供URL的解码功能
    /// </summary>
    static class HttpUtility
    {         
        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string UrlDecode(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            int count = str.Length;
            UrlDecoder helper = new UrlDecoder(count, encoding);

            // go through the string's chars collapsing %XX and %uXXXX and
            // appending each char as char, with exception of %XX constructs
            // that are appended as bytes

            for (int pos = 0; pos < count; pos++)
            {
                char ch = str[pos];

                if (ch == '+')
                {
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    if (str[pos + 1] == 'u' && pos < count - 5)
                    {
                        int h1 = HttpUtility.HexToInt(str[pos + 2]);
                        int h2 = HttpUtility.HexToInt(str[pos + 3]);
                        int h3 = HttpUtility.HexToInt(str[pos + 4]);
                        int h4 = HttpUtility.HexToInt(str[pos + 5]);

                        if (h1 >= 0 && h2 >= 0 && h3 >= 0 && h4 >= 0)
                        {   // valid 4 hex chars
                            ch = (char)((h1 << 12) | (h2 << 8) | (h3 << 4) | h4);
                            pos += 5;

                            // only add as char
                            helper.AddChar(ch);
                            continue;
                        }
                    }
                    else
                    {
                        int h1 = HttpUtility.HexToInt(str[pos + 1]);
                        int h2 = HttpUtility.HexToInt(str[pos + 2]);

                        if (h1 >= 0 && h2 >= 0)
                        {     // valid 2 hex chars
                            byte b = (byte)((h1 << 4) | h2);
                            pos += 2;

                            // don't add as char
                            helper.AddByte(b);
                            continue;
                        }
                    }
                }

                if ((ch & 0xFF80) == 0)
                    helper.AddByte((byte)ch); // 7 bit have to go as bytes because of Unicode
                else
                    helper.AddChar(ch);
            }

            return helper.ToString();
        }

        /// <summary>
        /// hex转为int
        /// </summary>
        /// <param name="h">hex</param>
        /// <returns></returns>
        private static int HexToInt(char h)
        {
            return (h >= '0' && h <= '9') ? h - '0' :
                (h >= 'a' && h <= 'f') ? h - 'a' + 10 :
                (h >= 'A' && h <= 'F') ? h - 'A' + 10 :
                -1;
        }    

        private class UrlDecoder
        {
            private int _bufferSize;

            // Accumulate characters in a special array
            private int _numChars;
            private char[] _charBuffer;

            // Accumulate bytes for decoding into characters in a special array
            private int _numBytes;
            private byte[] _byteBuffer;

            // Encoding to convert chars to bytes
            private Encoding _encoding;

            private void FlushBytes()
            {
                if (_numBytes > 0)
                {
                    _numChars += _encoding.GetChars(_byteBuffer, 0, _numBytes, _charBuffer, _numChars);
                    _numBytes = 0;
                }
            }

            public UrlDecoder(int bufferSize, Encoding encoding)
            {
                _bufferSize = bufferSize;
                _encoding = encoding;

                _charBuffer = new char[bufferSize];
                // byte buffer created on demand
            }

            public void AddChar(char ch)
            {
                if (_numBytes > 0)
                    FlushBytes();

                _charBuffer[_numChars++] = ch;
            }

            public void AddByte(byte b)
            {
                // if there are no pending bytes treat 7 bit bytes as characters
                // this optimization is temp disable as it doesn't work for some encodings
                /*
                                if (_numBytes == 0 && ((b & 0x80) == 0)) {
                                    AddChar((char)b);
                                }
                                else
                */
                {
                    if (_byteBuffer == null)
                        _byteBuffer = new byte[_bufferSize];

                    _byteBuffer[_numBytes++] = b;
                }
            }

            public override string ToString()
            {
                if (_numBytes > 0)
                {
                    FlushBytes();
                }

                if (_numChars > 0)
                {
                    return new String(_charBuffer, 0, _numChars);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
    }
}