using System;
using PhySick_engine;
using Listas;
namespace MarsCMD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
        
        static char LeeInput()
        {
            char ch = ' ';
            if (Console.KeyAvailable)
            {
                string dir = Console.ReadKey(true).Key.ToString();
                if (dir == "A" || dir == "LeftArrow") ch = 'l';
                else if (dir == "D" || dir == "RightArrow") ch = 'r';
                else if (dir == "W" || dir == "UpArrow") ch = 'u';
                else if (dir == "S" || dir == "DownArrow") ch = 'd';
                else if (dir == "X" || dir == "Spacebar") ch = 'x'; // bomba                   
                else if (dir == "P") ch = 'p'; // pausa					
                else if (dir == "Q" || dir == "Escape") ch = 'q'; // salir

                if (Console.KeyAvailable)
                {
                    string dir2 = Console.ReadKey(true).Key.ToString();
                    if (((dir2 == "A" || dir2 == "LeftArrow") && ch == 'r') ||
                        ((dir2 == "D" || dir2 == "RightArrow") && ch == 'l'))ch = 'u';
                }

                while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
            }

            return ch;
        }
    }
}