using System;
using System.Collections.Generic;
using System.Linq;

namespace COMChat
{
    class PackageManager
    {
        private const byte Sd = 0xA7;
        private const byte Ed = 0xA7;
        private const byte Token = 0x00;
        private const byte Package = 0x10;

        public byte[] GetToken()
        {
            return new[] {Sd, Token, Ed};
        }

        public bool IsToken(byte[] package)
        {
            return (package[1] & Package).Equals(Token);
        }

        public bool IsData(byte[] package)
        {
            return (package[1] & Package).Equals(Package);
        }

        public byte GetSource(byte[] package)
        {
            return package[3];
        }

        public byte GetDest(byte[] package)
        {
            return package[2];
        }

        public byte[] GetData(byte[] package)
        {
            return package.ToList().GetRange(4, package.Length - 5).ToArray();
        }

        public byte[] CreatePackage(byte[] data, byte source, byte dest)
        {
            var temp = new List<byte>{ Sd, Package, dest, source };
            temp.AddRange(data);
            temp.Add(Ed);
            return temp.ToArray();
        }
    }
}
