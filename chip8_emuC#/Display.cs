using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing;

namespace chip8_emu
{
    public partial class Display : Form
    {
        readonly Chip8 chip8;
        readonly Bitmap bmp;

        readonly string ROM = "../../../roms/Breakout (Brix hack) [David Winter, 1997].ch8";

        readonly Stopwatch stopwatch = Stopwatch.StartNew();
        readonly TimeSpan elapsed60hz = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        readonly TimeSpan elapsed = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 1000);
        TimeSpan lastTimespan;

        public Display()
        {
            InitializeComponent();

            bmp = new Bitmap(64, 32);
            pbScreen.Image = bmp;

            chip8 = new Chip8(ScreenDraw, Sound);
            chip8.Load(File.ReadAllBytes(ROM));

            KeyDown += SetKeyDown;
            KeyUp += SetKeyUp;
        }
        protected override void OnLoad(EventArgs e)
        {
            LoopGame();
        }

        void ScreenDraw(bool[,] buffer)
        {
            var bits = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* pointer = (byte*)bits.Scan0;
                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        pointer[0] = 0; // Blue
                        pointer[1] = buffer[x, y] ? (byte)0x64 : (byte)0; // Green
                        pointer[2] = 0; // Red
                        pointer[3] = 255; // Alpha

                        pointer += 4; // 4 bytes per pixel
                    }
                }
            }

            bmp.UnlockBits(bits);
        }

        void Sound(int ms)
        {
            Console.Beep(500, ms);
        }

        Dictionary<Keys, byte> keyMapping = new Dictionary<Keys, byte>
        {
            { Keys.D1, 0x1 },
            { Keys.D2, 0x2 },
            { Keys.D3, 0x3 },
            { Keys.D4, 0xC },
            { Keys.Q, 0x4 },
            { Keys.W, 0x5 },
            { Keys.E, 0x6 },
            { Keys.R, 0xD },
            { Keys.A, 0x7 },
            { Keys.S, 0x8 },
            { Keys.D, 0x9 },
            { Keys.F, 0xE },
            { Keys.Z, 0xA },
            { Keys.X, 0x0 },
            { Keys.C, 0xB },
            { Keys.V, 0xF },
        };

        void SetKeyDown(object sender, KeyEventArgs e)
        {
            if (keyMapping.ContainsKey(e.KeyCode))
            {
                chip8.PressedKey(keyMapping[e.KeyCode]);
            }
        }

        void SetKeyUp(object sender, KeyEventArgs e)
        {
            if (keyMapping.ContainsKey(e.KeyCode))
            {
                chip8.ReleasedKey(keyMapping[e.KeyCode]);
            }
        }
        void Tick() => chip8.Tick();

        void Tick60hz()
        {
            chip8.Tick60();
            pbScreen.Refresh();
        }

        private void pbScreen_Click(object sender, EventArgs e)
        {

        }
        void LoopGame()
        {
            Task.Run(GameLoop);
        }

        Task GameLoop()
        {
            while (true)
            {
                var currentWatch = stopwatch.Elapsed;
                var usedWatch = currentWatch - lastTimespan;

                while (usedWatch >= elapsed60hz)
                {
                    this.Invoke((Action)Tick60hz);
                    usedWatch -= elapsed60hz;
                    lastTimespan += elapsed60hz;
                }

                this.Invoke((Action)Tick);

                Thread.Sleep(elapsed);
            }
        }
    }
}