using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chip8_emu
{
    public class Chip8
    {
        byte[] memory = new byte[0x1000];
        byte[] v = new byte[16];
        ushort[] stack = new ushort[16];
        ushort i = 0x200; //index
        ushort pc = 0x200; //program counter
                           //instructions are two bytes long, jump by two

        byte sp; //stack pointer
        byte delay;

        private const int SCREEN_H = 32;
        private const int SCREEN_W = 64;
        private bool[,] _screen = new bool[SCREEN_W, SCREEN_H];
        private bool[,] _clearScreen = new bool[SCREEN_W, SCREEN_H];

        bool toBeCleared = true;

        Dictionary<byte, Action<OpCode>> opCodes;
        Dictionary<byte, Action<OpCode>> otherOpCodes;

        HashSet<byte> keyPressed = new HashSet<byte>();

        Action<bool[,]> draw;
        Action<int> sound;

        public struct OpCode
        {
            public ushort opCode;
            public ushort NNN;
            public byte NN;
            public byte N;
            public byte X;
            public byte Y;

            public override string ToString()
            {
                return $"{opCode:X4} (X: {X:X}, Y: {Y:X}, N: {N:X}, NN: {NN:X2}, NNN: {NNN:X3})";
            }

        }

        public Chip8(Action<bool[,]> draw, Action<int> sound)
        {
            this.draw = draw;
            this.sound = sound;

            WriteFont();

            opCodes = new Dictionary<byte, Action<OpCode>>
            {
                { 0x0, Clear }, //DRAW
                { 0x1, Jump },
                { 0x2, Subroutine },
                { 0x3, SkipXNotEqual },
                { 0x4, SkipXEqual },
                { 0x5, SkipXEqualY },
                { 0x6, SetX },
                { 0x7, AddX },
                { 0x8, ModifyXByY },
                { 0x9, SkipXNotEqualY },
                { 0xA, SetI },
                { 0xB, JumpToAddress },
                { 0xC, RandomNumber },
                { 0xD, SpriteDraw }, //DRAW
                { 0xE, KeyPressSkip },
                { 0xF, OtherOps },
            };

            otherOpCodes = new Dictionary<byte, Action<OpCode>>
            {
                { 0x07, XDelay },
                { 0x0A, KeyPressWait },
                { 0x15, SetDelay },
                { 0x18, SetSound },
                { 0x1E, XtoI },
                { 0x29, SetIToHex },
                { 0x33, BinaryDecimal },
                { 0x55, SaveX },
                { 0x65, LoadX }
            };

        }
        void Clear(OpCode opCode) //0x0
        {
            if (opCode.NN == 0xE0)
            {
                for (int x = 0; x < SCREEN_W; x++)
                    for (int y = 0; y < SCREEN_H; y++)
                        _screen[x, y] = false;

            }
            else if (opCode.NN == 0xEE)
            {
                pc = pcReturn();
            }

        }

        ushort pcReturn()
        {
            return stack[--sp];
        }

        void Jump(OpCode opCode) //0x1
        {
            pc = opCode.NNN;
        }

        void Subroutine(OpCode opCode) //0x2
        {
            stack[sp++] = opCode.NNN;
        }

        void SkipXNotEqual(OpCode opCode) //0x3
        {
            if (v[opCode.X] != opCode.NN)
            {
                pc += 2;
            }
        }

        void SkipXEqual(OpCode opCode) //0x4
        {
            if (v[opCode.X] == opCode.NN)
            {
                pc += 2;
            }
        }

        void SkipXEqualY(OpCode opCode) //0x5
        {
            if (v[opCode.X] == v[opCode.Y])
            {
                pc += 2;
            }
        }

        void SetX(OpCode opCode) //0x6
        {
            v[opCode.X] = opCode.NN;
        }

        void AddX(OpCode opCode) //0x7
        {
            v[opCode.X] += opCode.NN;
        }

        void ModifyXByY(OpCode opCode) //0x8
        {
            switch (opCode.N)
            {
                case 0x0:
                    v[opCode.X] = v[opCode.Y]; break;

                case 0x1:
                    v[opCode.X] |= v[opCode.Y]; break;

                case 0x2:
                    v[opCode.X] &= v[opCode.Y]; break;

                case 0x3:
                    v[opCode.X] ^= v[opCode.Y]; break;

                case 0x4:
                    v[opCode.X] += v[opCode.Y]; break; //TODO overflow

                case 0x5:
                    v[opCode.X] -= v[opCode.Y]; break; //TODO underflow

                case 0x6:
                    v[opCode.X] /= 2; break; //TODO check shift

                case 0x7:
                    v[opCode.Y] -= v[opCode.X]; break; //TODO underflow

                case 0xE:
                    v[opCode.X] *= 2; break; //TODO check shift

                default:
                    Console.WriteLine("Unknown opcode: 0x%X\n", opCode); break;

            }
        }

        void SkipXNotEqualY(OpCode opCode) //0x9
        {
            if (v[opCode.X] != v[opCode.Y])
            {
                pc += 2;
            }
        }

        void SetI(OpCode opCode) //0xA
        {
            i = opCode.NNN;
        }

        void JumpToAddress(OpCode opCode) //0xB
        {
            pc = (ushort)(opCode.NNN + v[0]);
        }

        void RandomNumber(OpCode opCode) //0xC
        {
            Random rnd = new Random();
            v[opCode.X] = (byte)(rnd.Next(0, 256) & opCode.NN);
        }

        void SpriteDraw(OpCode opCode) //0xD
        {
            var firstX = v[opCode.X];
            var firstY = v[opCode.Y];

            for (var x = 0; x < SCREEN_W; x++)
            {
                //Debug.Write(x);
                for (var y = 0; y < SCREEN_H; y++)
                {
                    //Debug.Write(y);

/*                    if (x > SCREEN_H)
                    {
                        x = SCREEN_H;
                    }

                    if (y > SCREEN_W)
                    { 
                        y = SCREEN_W; 
                    }*/

                    //Debug.Write(_clearScreen[x, y]);

                    if (_clearScreen[x, y]) 
                    {
                        if (_screen[x, y])
                        {
                            toBeCleared = true;
                        }
                        _clearScreen[x, y] = false;
                        _screen[x, y] = false;
                    }
                }
            }

            v[0xF] = 0;
            for (var n = 0; n < opCode.N; n++)
            {
                var line = memory[i + n];

                for (var bit = 0; bit < 8; bit++)
                {
                    var xPos = (firstX + bit) % SCREEN_W;
                    var yPos = (firstY + bit) % SCREEN_H;

                    var sprBit = ((line >> (7 - bit)) & 1);
                    var tBit = _screen[xPos, yPos] ? 1 : 0;

                    if (tBit != sprBit)
                    {
                        toBeCleared = true;
                    }

                    var nBit = tBit ^ bit;

                    if (tBit != 0 && nBit == 0)
                    {
                        v[0xF] = 1;
                    }

                    if (nBit != 0)
                    {
                        _screen[xPos, yPos] = true;
                    }
                    else
                    {
                        _clearScreen[xPos, yPos] = true;
                    }
                }
            }
        }
        void KeyPressSkip(OpCode opCode) //0xE
        {
            if ((opCode.NN == 0x9E && keyPressed.Contains(v[opCode.X])) || (opCode.NN == 0xA1 && keyPressed.Contains(v[opCode.X])))
            {
                pc += 2;
            }
        }

        void OtherOps(OpCode opCode)
        {
            if (otherOpCodes.ContainsKey(opCode.NN))
            {
                otherOpCodes[opCode.NN](opCode);
            }
        }

        void XDelay(OpCode opCode)
        {
            v[opCode.X] = delay;
        }

        void KeyPressWait(OpCode opCode)
        {
            if (keyPressed.Count != 0)
            {
                v[opCode.X] = keyPressed.First();
            }

            else
            {
                pc -= 2;
            }
        }

        void SetDelay(OpCode opCode)
        {
            delay = v[opCode.X];
        }

        void SetSound(OpCode opCode)
        {
            //TODO
        }

        void XtoI(OpCode opCode)
        {
            i += v[opCode.X];
        }

        void SetIToHex(OpCode opCode)
        {
            i = (ushort)(v[opCode.X] * 5);
        }

        void BinaryDecimal(OpCode opCode)
        {
            memory[i] = (byte)((v[opCode.X] / 100) % 10);
            memory[i + 1] = (byte)((v[opCode.X] / 10) % 10);
            memory[i + 2] = (byte)((v[opCode.X] / 10));
        }

        void SaveX(OpCode opCode)
        {
            for (var x = 0; x <= opCode.X; x++)
            {
                memory[i + x] = v[x];
            }
        }

        void LoadX(OpCode opCode)
        {
            for (var x = 0; x <= opCode.X; x++)
            {
                v[x] = memory[i + x];
            }
        }

        public void Tick()
        {
            var opCodeData = (ushort)(memory[pc++] << 8 | memory[pc++]);

            var opCodeSplit = new OpCode()
            {
                opCode = opCodeData,
                NNN = (ushort)(opCodeData & 0x0FFF),
                NN = (byte)(opCodeData & 0x00FF),
                N = (byte)(opCodeData & 0x000F),
                X = (byte)((opCodeData & 0x0F00) >> 8),
                Y = (byte)((opCodeData & 0x00F0) >> 4),

            };

            opCodes[(byte)(opCodeData >> 12)](opCodeSplit);
        }

        public void Tick60()
        {
            if (delay > 0)
            {
                delay--;
            }
            if (toBeCleared)
            {
                toBeCleared = false;
                draw(_screen);
            }
        }

        void Misc(OpCode data)
        {
            if (otherOpCodes.ContainsKey(data.NN))
                otherOpCodes[data.NN](data);
        }

        public void PressedKey(byte key)
        {
            keyPressed.Add(key);
        }

        public void ReleasedKey(byte key)
        {
            keyPressed.Remove(key);
        }

        void WriteFont()
        {
            var offset = 0x0;

            WriteFont(5 * offset++, FontCharacters.D0);
            WriteFont(5 * offset++, FontCharacters.D1);
            WriteFont(5 * offset++, FontCharacters.D2);
            WriteFont(5 * offset++, FontCharacters.D3);
            WriteFont(5 * offset++, FontCharacters.D4);
            WriteFont(5 * offset++, FontCharacters.D5);
            WriteFont(5 * offset++, FontCharacters.D6);
            WriteFont(5 * offset++, FontCharacters.D7);
            WriteFont(5 * offset++, FontCharacters.D8);
            WriteFont(5 * offset++, FontCharacters.D9);
            WriteFont(5 * offset++, FontCharacters.DA);
            WriteFont(5 * offset++, FontCharacters.DB);
            WriteFont(5 * offset++, FontCharacters.DC);
            WriteFont(5 * offset++, FontCharacters.DD);
            WriteFont(5 * offset++, FontCharacters.DE);
            WriteFont(5 * offset++, FontCharacters.DF);
        }

        void WriteFont(int address, long fontData)
        {
            memory[address + 0] = (byte)((fontData & 0xF000000000) >> (8 * 4));
            memory[address + 1] = (byte)((fontData & 0x00F0000000) >> (8 * 3));
            memory[address + 2] = (byte)((fontData & 0x0000F00000) >> (8 * 2));
            memory[address + 3] = (byte)((fontData & 0x000000F000) >> (8 * 1));
            memory[address + 4] = (byte)((fontData & 0x00000000F0) >> (8 * 0));
        }

        public void Load(byte[] data)
        {
            Array.Copy(data, 0, memory, 0x200, data.Length);
        }
    }
}

