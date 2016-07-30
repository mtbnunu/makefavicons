using System;

namespace MakeFavicons
{
    public static class ColoredConsole
    {
        public static void WriteLine(string msg, ConsoleColor color, ConsoleColor? bgcol = null)
        {
            setcolor(color,bgcol);
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Write(string msg, ConsoleColor color, ConsoleColor? bgcol = null)
        {
            setcolor(color, bgcol);
            Console.Write(msg);
            Console.ResetColor();
        }

        private static void setcolor(ConsoleColor col, ConsoleColor? bgcol = null)
        {
            if (bgcol != null)
            {
                Console.BackgroundColor = bgcol.Value;
            }
            Console.ForegroundColor = col;
        }
    }
}
