﻿using GameServer.Networks.Messages;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using Utility;

namespace GameServer.Networks.Protocols
{
    public class GameProtocol : IScsWireProtocol
    {
        protected MemoryStream Stream = new MemoryStream();

        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes)
        {
            Log.Debug("receivedBytes");
            Console.WriteLine(receivedBytes.FormatHex());

            byte[] unpackBytes = new byte[receivedBytes.Length - 4];
            Buffer.BlockCopy(receivedBytes, 2, unpackBytes, 0, unpackBytes.Length);

            Stream.Write(unpackBytes, 0, unpackBytes.Length);
            List<IScsMessage> messages = new List<IScsMessage>();
            while (ReadMessage(messages)) ;
            return messages;
        }

        private bool ReadMessage(List<IScsMessage> messages)
        {
            Stream.Position = 0;

            if (Stream.Length < 8)
                return false;

            byte[] headerBytes = new byte[8];
            Stream.Read(headerBytes, 0, 8);

            ushort opcode = BitConverter.ToUInt16(headerBytes, 4);
            int length = BitConverter.ToUInt16(headerBytes, 6);

            if (Stream.Length < length)
                return false;

            GameMessage message = new GameMessage
            {
                OpCode = (short)opcode,
                Data = new byte[length]
            };

            Stream.Read(message.Data, 0, message.Data.Length);

            messages.Add(message);

            TrimStream();

            return true;
        }

        private void TrimStream()
        {
            if (Stream.Position == Stream.Length)
            {
                Stream = new MemoryStream();
                return;
            }

            byte[] remaining = new byte[Stream.Length - Stream.Position];
            Stream.Read(remaining, 0, remaining.Length);
            Stream = new MemoryStream();
            Stream.Write(remaining, 0, remaining.Length);
        }

        public byte[] GetBytes(IScsMessage message)
        {
            return ((GameMessage)message).Data;
        }

        public void Reset()
        {
        }
    }
}
