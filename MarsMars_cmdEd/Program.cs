using System;
using PhySick_engine;
using System.Threading;
using Listas;
using RandWrld_generator;
namespace MainPr
{
    internal class Program
    {

        static bool inAir = false;

        static void Main(string[] args)
        {
            //WORLD DATA
            int WIDTH = 30, 
                RENDERSIZE = 4, 
                MAXHEIGHT = 25, 
                HEIGHT = 30,
                SEED = 1, 
                ROUGHNESS = 2, 
                FEATURES = 3;

            //PHYSICS DATA
            float GRAVITY = 5,
                  RESISTANCE = 1,
                  MASS = 2,
                  MAXSPEED = 10,
                  REFRESH = 300;

            float[] startingPoint;


            Console.WindowWidth = WIDTH * RENDERSIZE;

            RandWrld mars = new RandWrld(WIDTH, MAXHEIGHT, SEED);

            mars.StartWorld(FEATURES, ROUGHNESS);

            mars.RenderWorld(HEIGHT, RENDERSIZE);

            PhySick player = new PhySick(GRAVITY, RESISTANCE, MASS, MAXSPEED, REFRESH);

            startingPoint = new float[2] {mars.GetWorld()[WIDTH/2], WIDTH/2};

            player.objPos = startingPoint;

            RenderPlayer(player.objPos, RENDERSIZE);

            char ch = ' ';
            bool play = true;
            while (play)
            {
                Console.Clear();

                ch = LeeInput();
                play = ch != 'q';
                ManageInput(ref player, ch);

                player.MainPhysics();

                player.TimeSinceFloor(inAir);

                RenderPlayer(player.objPos, RENDERSIZE);
                mars.RenderWorld(HEIGHT, RENDERSIZE);

                Thread.Sleep((int)REFRESH);
            }

        }
        
        static void RenderPlayer(float[] pos, int renderSz)
        {
            for(int i = 0; i < renderSz; i++)
            {
                for(int j = 0; j < renderSz; j++)
                {
                    int x = (int)pos[1] * renderSz + i;
                    int y = (int)pos[0] - renderSz + j;
                    
                    if (y > 0)
                    {

                        Console.SetCursorPosition(x, y);
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("A");
                    }
                }
            }
        }

        static void ManageInput(ref PhySick pl, char im)
        {
            switch (im)
            {
                case 'l':
                    pl.ApplyForce(-6f, 3f);
                    inAir = true;
                    break;
                case 'r':
                    pl.ApplyForce(-6f, -3f);
                    inAir = true;
                    break;
                case 'u':
                case 'x':
                    pl.ApplyForce(-9f, 0f);
                    inAir = true;
                    break;
            }
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