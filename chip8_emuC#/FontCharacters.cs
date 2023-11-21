﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace chip8_emu
{

    public static class FontCharacters
    {
        /* readonly public byte[] fontset =
            {
                   0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                  0x20, 0x60, 0x20, 0x20, 0x70, // 1
                  0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                  0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                  0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                  0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                  0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                  0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                  0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                  0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                  0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                  0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                  0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                  0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                  0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                  0xF0, 0x80, 0xF0, 0x80, 0x80  // F 

             }; */
        public const long D0 = 0xF0909090F0;
        public const long D1 = 0x2060202070;
        public const long D2 = 0xF010F080F0;
        public const long D3 = 0xF010F010F0;
        public const long D4 = 0x9090F01010;
        public const long D5 = 0xF080F010F0;
        public const long D6 = 0xF080F090F0;
        public const long D7 = 0xF010204040;
        public const long D8 = 0xF090F090F0;
        public const long D9 = 0xF090F010F0;
        public const long DA = 0xF090F09090;
        public const long DB = 0xE090E090E0;
        public const long DC = 0xF0808080F0;
        public const long DD = 0xE0909090E0;
        public const long DE = 0xF080F080F0;
        public const long DF = 0xF080F08080;

    }
}
