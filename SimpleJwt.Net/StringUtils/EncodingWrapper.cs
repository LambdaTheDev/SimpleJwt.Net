using System;
using System.Text;

namespace LambdaTheDev.SimpleJwt.Net.StringUtils
{
    // Wrapper class for encodings that allows them to write strings into
    //  reusable buffers
    public class EncodingWrapper
    {
        private const int InitialBufferLength = 64;
        
        private readonly Encoding _encoding;
        private byte[] _reusableByteBuffer = new byte[InitialBufferLength];
        private char[] _reusableCharBuffer = new char[InitialBufferLength / 2]; // by two, due to char is 2 bytes & byte 1
        private char[] _separator = new char[2];
        private int _appendedBytes;
        private int _appendedChars;

        public Encoding Encoding => _encoding; // Public encoding accessor
        

        public EncodingWrapper(Encoding encoding)
        {
            _encoding = encoding;
        }

        // Appends StringSegment with separator to this wrapper
        public void Append(StringSegment value, char separator)
        {
            // Separator as first element of separator buffer & append string
            _separator[0] = separator;
            InternalAppend(value, 1);
        }

        // Appends StringSegment with separator to this wrapper
        public void Append(StringSegment value, string separator = null)
        {
            // If separator null, just append
            if (separator == null)
            {
                InternalAppend(value, 0);
                return;
            }

            // Validate separator buffer length
            if(_separator.Length < separator.Length)
                _separator = new char[separator.Length + 1]; // Just in case length + 1

            // Append separator to array
            for (int i = 0; i < separator.Length; i++)
                _separator[i] = separator[i];
            
            // Append string
            InternalAppend(value, separator.Length);
        }

        private void InternalAppend(StringSegment value, int separatorLength)
        {
            // Validate char buffer size
            if(value.Count > _reusableCharBuffer.Length)
                _reusableCharBuffer = new char[GetNewBufferSize(value.Count, _reusableCharBuffer.Length)];

            // Put string chars in array
            _appendedChars = 0;
            for (int i = value.Offset; i < value.Offset + value.Count; i++)
                _reusableCharBuffer[_appendedChars++] = value.OriginalString[i];
            
            // Get separator bytes
            int separatorBytes = 0;
            if(separatorLength > 0 && _appendedBytes > 0)
                separatorBytes = _encoding.GetByteCount(_separator, 0, separatorLength);

            // Get bytes count & validate byte buffer size
            int neededBytes = _encoding.GetByteCount(_reusableCharBuffer, 0, _appendedBytes); // Note: in case of failures, try _appBytes - 1
            EnsureByteBufferCapacity(neededBytes + separatorBytes);

            // Append bytes with separator using unsafe code
            unsafe
            {
                fixed (byte* byteBufferPtr = &_reusableByteBuffer[_appendedBytes])
                fixed (char* charBufferPtr = _reusableCharBuffer)
                fixed (char* separatorBufferPtr = _separator)
                {
                    int appended = 0;
                    
                    if(separatorBytes > 0)
                        appended = _encoding.GetBytes(separatorBufferPtr, separatorLength, byteBufferPtr, _reusableByteBuffer.Length);
                    
                    appended += _encoding.GetBytes(charBufferPtr, _appendedChars, byteBufferPtr + appended, _reusableByteBuffer.Length);
                    _appendedBytes += appended;
                }
            }
            
            // And... It's ready?
        }

        // Returns string from ArraySegment
        public string GetString(ArraySegment<byte> value)
        {
            // Note: I haven't found a way to make it in less alloc way
            //  but I will try :P
            return _encoding.GetString(value.Array, value.Offset, value.Count);
        }

        // Returns string from byte array 
        public string GetString(byte[] value) => GetString(new ArraySegment<byte>(value));

        // Resets & clears memory of reusable buffers
        public void Clear()
        {
            _appendedBytes = 0;
            _appendedChars = 0;
        }

        // Wraps reusable buffer into ArraySegment
        public ArraySegment<byte> ToReusableBuffer()
        {
            return new ArraySegment<byte>(_reusableByteBuffer, 0, _appendedBytes);
        }

        // Allocates & returns new byte array based on reusable buffer content
        public byte[] ToAllocatedArray()
        {
            unsafe
            {
                byte[] newBuffer = new byte[_appendedBytes];
                
                fixed(byte* sourcePtr = _reusableByteBuffer)
                fixed (byte* destinationPtr = newBuffer)
                {
                    Buffer.MemoryCopy(sourcePtr, destinationPtr, _appendedBytes, _appendedBytes);
                }

                return newBuffer;
            }
        }

        // Ensures that byte buffer has enough space
        private void EnsureByteBufferCapacity(int requiredSize)
        {
            if (_reusableByteBuffer.Length < requiredSize)
            {
                byte[] newBuffer = new byte[GetNewBufferSize(requiredSize, _reusableByteBuffer.Length)];
                unsafe
                {
                    fixed(byte* sourcePtr = _reusableByteBuffer)
                    fixed (byte* destinationPtr = newBuffer)
                    {
                        Buffer.MemoryCopy(sourcePtr, destinationPtr, newBuffer.Length, _appendedBytes);
                    }
                }

                _reusableByteBuffer = newBuffer;
            }
        }
        
        // Gets new buffer size which is multiplied by 2
        private int GetNewBufferSize(int minimalRequiredSize, int checkedSize)
        {
            if (minimalRequiredSize > checkedSize)
                return GetNewBufferSize(minimalRequiredSize, checkedSize * 2);

            return checkedSize;
        }
    }
}