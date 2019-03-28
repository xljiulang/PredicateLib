using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供Uri扩展
    /// </summary>
    static class UriExtensions
    {
        /// <summary>
        /// 获取Query参数值
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="encoding">query字符串的编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> GetQueryValues(this Uri uri, Encoding encoding)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            if (string.IsNullOrEmpty(uri.Query))
            {
                yield break;
            }

            var query = uri.Query.TrimStart('?').Split('&');
            foreach (var q in query)
            {
                var kv = q.Split('=');
                if (kv.Length == 2)
                {
                    var key = UrlDecode(kv[0], encoding);
                    var value = UrlDecode(kv[1], encoding);
                    yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }


        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        private static string UrlDecode(string str, Encoding encoding)
        {
            if (str == null)
            {
                return null;
            }

            var count = str.Length;
            var helper = new UrlDecoder(count, encoding);

            for (var pos = 0; pos < count; pos++)
            {
                var ch = str[pos];
                if (ch == '+')
                {
                    ch = ' ';
                }
                else if (ch == '%' && pos < count - 2)
                {
                    if (str[pos + 1] == 'u' && pos < count - 5)
                    {
                        var h1 = HexToInt(str[pos + 2]);
                        var h2 = HexToInt(str[pos + 3]);
                        var h3 = HexToInt(str[pos + 4]);
                        var h4 = HexToInt(str[pos + 5]);

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
                        var h1 = HexToInt(str[pos + 1]);
                        var h2 = HexToInt(str[pos + 2]);

                        if (h1 >= 0 && h2 >= 0)
                        {     // valid 2 hex chars
                            var b = (byte)((h1 << 4) | h2);
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
            private readonly int _bufferSize;

            // Accumulate characters in a special array
            private int _numChars;
            private readonly char[] _charBuffer;

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
            }

            public void AddChar(char ch)
            {
                if (_numBytes > 0)
                {
                    FlushBytes();
                }
                _charBuffer[_numChars++] = ch;
            }

            public void AddByte(byte b)
            {
                if (_byteBuffer == null)
                {
                    _byteBuffer = new byte[_bufferSize];
                }
                _byteBuffer[_numBytes++] = b;
            }

            public override string ToString()
            {
                if (_numBytes > 0)
                {
                    FlushBytes();
                }

                if (_numChars > 0)
                {
                    return new string(_charBuffer, 0, _numChars);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
