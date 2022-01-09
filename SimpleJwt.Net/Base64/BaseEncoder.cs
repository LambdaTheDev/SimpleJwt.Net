/*
 * BSD 2-Clause License

Copyright (c) 2017, Ramon Smits
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using LambdaTheDev.SimpleJwt.Net.StringUtils;

namespace Exyll
{
	/// <remarks>Based on http://www.csharp411.com/convert-binary-to-base64-string/</remarks>
	/// MINE MODIFICATIONS: I reduced allocations by using reusable buffers!
	public class BaseEncoder
	{
		private const char PaddingChar = '=';
		private static readonly byte[] EmptyArray = new byte[0];

		public readonly char[] CharacterSet;
		public readonly bool PaddingEnabled;
		private readonly byte[] Map;

		private char[] _reusableS = new char[32];
		private char[] _reusableString = new char[32];
		private byte[] _reusableData = new byte[64];


		public BaseEncoder(char[] characterSet, bool paddingEnabled)
		{
			PaddingEnabled = paddingEnabled;
			CharacterSet = characterSet;

			Map = new byte[128];
			for (byte i = 0; i < characterSet.Length; i++)
				Map[(byte)characterSet[i]] = i;
		}

		public string ToBase(ArraySegment<byte> data)
		{
			int length;
			if (data == null || 0 == (length = data.Count))
				return string.Empty;

			unsafe
			{
				fixed (byte* _d = &data.Array[data.Offset])
				fixed (char* _cs = CharacterSet)
				{
					byte* d = _d;

					int padding = length % 3;
					if (padding > 0)
						padding = 3 - padding;
					int blocks = (length - 1) / 3 + 1;

					int l = blocks * 4;

					EnsureSCapacity(l);

					fixed (char* _sp = _reusableS)
					{
						char* sp = _sp;
						byte b1, b2, b3;

						for (int i = 1; i < blocks; i++)
						{
							b1 = *d++;
							b2 = *d++;
							b3 = *d++;

							*sp++ = _cs[(b1 & 0xFC) >> 2];
							*sp++ = _cs[(b2 & 0xF0) >> 4 | (b1 & 0x03) << 4];
							*sp++ = _cs[(b3 & 0xC0) >> 6 | (b2 & 0x0F) << 2];
							*sp++ = _cs[b3 & 0x3F];
						}

						bool pad2 = padding == 2;
						bool pad1 = padding > 0;

						b1 = *d++;
						b2 = pad2 ? (byte)0 : *d++;
						b3 = pad1 ? (byte)0 : *d++;

						*sp++ = _cs[(b1 & 0xFC) >> 2];
						*sp++ = _cs[(b2 & 0xF0) >> 4 | (b1 & 0x03) << 4];
						*sp++ = pad2 ? '=' : _cs[(b3 & 0xC0) >> 6 | (b2 & 0x0F) << 2];
						*sp++ = pad1 ? '=' : _cs[b3 & 0x3F];

						if (!PaddingEnabled)
						{
							if (pad2) l--;
							if (pad1) l--;
						}
					}

					return new string(_reusableS, 0, l);
				}
			}
		}

		private void EnsureSCapacity(int required)
		{
			if(_reusableS.Length < required)
				_reusableS = new char[(int) (1.5f * required)];
		}

		public ArraySegment<byte> FromBase(string data) => FromBase(new StringSegment(data));

		public ArraySegment<byte> FromBase(StringSegment data)
		{
			int length = data.IsNull ? 0 : data.Count;

			if (length == 0)
				return new ArraySegment<byte>(EmptyArray);

			unsafe
			{
				PutStringInReusableBuffer(data);
				fixed (char* _p = _reusableString)
				{
					char* p2 = _p;

					int blocks = (length - 1) / 4 + 1;
					int bytes = blocks * 3;

					int padding = blocks * 4 - length;

					if (length > 2 && p2[length - 2] == PaddingChar)
						padding = 2;
					else if (length > 1 && p2[length - 1] == PaddingChar)
						padding = 1;

					EnsureDataBufferCapacity(bytes - padding);

					byte temp1, temp2;
					byte* dp;

					fixed (byte* _d = _reusableData)
					{
						dp = _d;

						for (int i = 1; i < blocks; i++)
						{
							temp1 = Map[*p2++];
							temp2 = Map[*p2++];

							*dp++ = (byte) ((temp1 << 2) | ((temp2 & 0x30) >> 4));
							temp1 = Map[*p2++];
							*dp++ = (byte) (((temp1 & 0x3C) >> 2) | ((temp2 & 0x0F) << 4));
							temp2 = Map[*p2++];
							*dp++ = (byte) (((temp1 & 0x03) << 6) | temp2);
						}

						temp1 = Map[*p2++];
						temp2 = Map[*p2++];

						*dp++ = (byte) ((temp1 << 2) | ((temp2 & 0x30) >> 4));

						temp1 = Map[*p2++];

						if (padding != 2)
							*dp++ = (byte) (((temp1 & 0x3C) >> 2) | ((temp2 & 0x0F) << 4));


						temp2 = Map[*p2++];
						if (padding == 0)
							*dp++ = (byte) (((temp1 & 0x03) << 6) | temp2);


					}

					return new ArraySegment<byte>(_reusableData, 0, bytes - padding);
				}
			}
		}
		
		
		private void PutStringInReusableBuffer(StringSegment value)
		{
			if (value.Count > _reusableString.Length) 
				_reusableString = new char[(int) (1.5f * value.Count)];
			
			for (int i = 0; i < value.Count; i++)
				_reusableString[i] = value.OriginalString[value.Offset + i];
		}

		private void EnsureDataBufferCapacity(int required)
		{
			if(_reusableData.Length < required)
				_reusableData = new byte[(int) (1.5f * required)];
		}
	}
}