using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Socket.Server
{
    public class BufferManager
    {
        private readonly int _bufferSize;
        private readonly Stack<int> _freeReceiveIndexPool;
        private readonly Stack<int> _freeSendIndexPool;
        private readonly int _maxConnectionsAllowed;
        private readonly int _totalBytes;

        /// <summary>
        /// Main Buffer array split into two sections, first is used for receiving, and the second is for sending
        /// </summary>
        private byte[] _buffer;

        private int _currentReceiveIndex;
        private int _currentSendIndex;

        public BufferManager(int maxConnectionsAllowed, int bufferSize)
        {
            _maxConnectionsAllowed = maxConnectionsAllowed;
            _bufferSize = bufferSize;
            _totalBytes = _maxConnectionsAllowed * bufferSize * 2;
            _currentSendIndex = _maxConnectionsAllowed * bufferSize;

            _currentReceiveIndex = 0;
            _freeReceiveIndexPool = new Stack<int>();
            _freeSendIndexPool = new Stack<int>();
        }

        public void InitBuffer()
        {
            _buffer = new byte[_totalBytes];
        }

        private bool SetReceiveBuffer(SocketAsyncEventArgs args)
        {
            if (_freeReceiveIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeReceiveIndexPool.Pop(), _bufferSize);
            }
            else
            {
                if (_maxConnectionsAllowed * _bufferSize - _currentReceiveIndex < _bufferSize)
                    return false;
                args.SetBuffer(_buffer, _currentReceiveIndex, _bufferSize);
                _currentReceiveIndex += _bufferSize;
            }

            return true;
        }

        private bool SetSendBuffer(SocketAsyncEventArgs args)
        {
            if (_freeSendIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeSendIndexPool.Pop(), _bufferSize);
            }
            else
            {
                if (_totalBytes - _currentSendIndex < _bufferSize)
                    return false;
                args.SetBuffer(_buffer, _currentSendIndex, _bufferSize);
                _currentSendIndex += _bufferSize;
            }

            return true;
        }

        public bool SetBuffer(DualModeSocketAsyncEventArgs args)
        {
            return SetReceiveBuffer(args.ReceiveArgs) &&
                   SetSendBuffer(args.SendArgs);
        }

        public void AdjustSendBuffer(SocketAsyncEventArgs args, int bufferSize)
        {
            if (bufferSize > _bufferSize)
                throw new Exception("buffersize is too big to send, default size is :" + _bufferSize);

            args.SetBuffer(args.Offset, bufferSize);
        }
    }
}