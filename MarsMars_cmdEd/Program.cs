using System;
using PhySick_engine;
using System.Threading;
using Listas;
using System.IO;
using RandWrld_generator;
namespace MainPr
{
    internal class Program
    {

        static bool inAir = false;
        static bool DEBUG = false;
        static bool DED = false;

        //WORLD DATA
        static int WIDTH = 30,
            RENDERSIZE = 4,
            MAXHEIGHT = 27,
            HEIGHT = 30,
            SEED = 1,
            ROUGHNESS = 2,
            FEATURES = 3;

        //PHYSICS DATA
        static float GRAVITY = 20,
              RESISTANCE = 1,
              MASS = 2,
              DEATHSPEED = 12,
              MAXSPEED = 15,
              REFRESH = 120;

        //PLAYER DATA
        static string USER = "default";
        static int FUEL = 100,
            POINTS = 0;

        static float[] startingPoint;
        static void Main(string[] args)
        {
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello!");
            Console.WriteLine("Enter your username: ");

            USER = Console.ReadLine();

            StartGame(ref USER, ref SEED, ref ROUGHNESS, ref FEATURES, ref POINTS);

            Console.Clear();

            RandWrld mars = new RandWrld(WIDTH, MAXHEIGHT, SEED);

            mars.StartWorld(FEATURES, ROUGHNESS);

            for (int i = 0; i < POINTS + (3*WIDTH/4); i++) mars.WorldGen(FEATURES, ROUGHNESS);

            mars.RenderWorld(HEIGHT, RENDERSIZE);

            PhySick player = new PhySick(GRAVITY, RESISTANCE, MASS, DEATHSPEED, MAXSPEED, REFRESH);

            startingPoint = new float[2] {mars.GetWorld()[WIDTH/4], WIDTH/4};

            player.objPos = startingPoint;

            //RenderPlayer(player.objPos, RENDERSIZE, FUEL,   danger);

            char ch = ' ';
            bool play = true;
            bool danger = player.FastEntry();
            while (play)
            {
                if (!DED) PlayGame(ref play, ref player, ref mars, ref ch);
                else
                {
                    SaveData(USER, SEED, ROUGHNESS, FEATURES, POINTS);

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                    Console.WriteLine(USER + " scored " + POINTS + " in the seed " + SEED);
                    Console.WriteLine("Wanna play again? [y/n]");
                    string resp = Console.ReadLine();

                    if (resp == "y" || resp == "Y")
                    {
                        StartGame(ref USER, ref SEED, ref ROUGHNESS, ref FEATURES, ref POINTS);
                        startingPoint = new float[2] { mars.GetWorld()[WIDTH / 4], WIDTH / 4 };
                        player.objPos = startingPoint;
                        player.CancelForces();

                        DED = false;
                    }
                    else play = false;
                }
            }

            SaveData(USER, SEED, ROUGHNESS, FEATURES, POINTS);
        }
        
        static void RenderPlayer(float[] pos, int renderSz, int fuel, bool danger)
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
                        if (!danger) Console.BackgroundColor = ConsoleColor.Black;
                        else Console.BackgroundColor = ConsoleColor.DarkRed;
                        if (fuel > 75) Console.ForegroundColor = ConsoleColor.Green;
                        else if (fuel > 50) Console.ForegroundColor = ConsoleColor.Yellow;
                        else if (fuel > 25) Console.ForegroundColor = ConsoleColor.DarkYellow;
                        else if (fuel > 0)  Console.ForegroundColor = ConsoleColor.Red;
                        else Console.ForegroundColor = ConsoleColor.Gray;
                        if (i < renderSz/2) Console.Write("(");
                        else if (i == renderSz/2) Console.Write("|");
                        else Console.Write(")");
                    }

                    if (x < 0 || x > Console.WindowWidth) DED = true;
                }
            }


        }

        static void Collisions(ref PhySick pl, int[] wrld, ref int fuel, ref int pt)
        {
                int y = (int)pl.objPos[0];
                int x = (int)pl.objPos[1];
                
                if  (wrld[x] < y && inAir)
                {
                    DED = pl.FastEntry();
                    
                    if (!DED)
                    {
                        inAir = false;

                        pl.objPos = new float[2] { wrld[x], x };


                        pl.CancelForces();
                        fuel = 100;
                        pt = x;
                    }
                }
        }

        static void ManageInput(ref PhySick pl, char im, ref int fuel)
        {
            if (fuel > 0)
            switch (im)
            {
                case 'l':
                    pl.ApplyForce(5f, -3f * 10);
                    fuel-=4;
                    inAir = true;
                    break;
                case 'r':
                    pl.ApplyForce(-5f, -3f * 10);
                    inAir = true;
                    fuel-=4;
                    break;
                case 'u':
                case 'x':
                    pl.ApplyForce(0f, -6f * 10);
                    inAir = true;
                    fuel-=8;
                    break;
                default: pl.ApplyForce(0, 0);
                    break;
            }
        }

        static void StartGame(ref string user, ref int seed, ref int roug, ref int feat, ref int pt)
        {
            if (user == null) user = "default";

            Random sd = new Random();

            seed = sd.Next();

            roug = 2;
            feat = 3;
            pt = 0;

            bool found = false;

            if (File.Exists("saveDat"))
            {
                StreamReader reader = new StreamReader("saveDat");

                string line = " ";

                string[] words = new string[] {};
                //Lectura hasta la última línea (el último nivel completado)
                while (!reader.EndOfStream && !found)
                {
                    line = reader.ReadLine();
                    words = line.Split(' ');
                    if (words[0] == user) found = true;
                }

                if (found)
                {
                    try
                    {
                        seed = int.Parse(words[1]);
                        roug = int.Parse(words[2]);
                        feat = int.Parse(words[3]);
                        pt = int.Parse(words[4]);
                    }

                    catch
                    {
                        found = false;

                        Console.WriteLine("ERROR: Data was corrupted! Press [ENTER] to create a new save");
                    }
                    
                }

                reader.Close();
            }
            else
            {
                StreamWriter writer = new StreamWriter("saveDat", false);        //creación del archivo de usuario
                writer.Close();
            }
            if (!found)
            {
                SaveData(user, seed, roug, feat, pt);
            }
        }

        static void SaveData(string user, int seed, int  roug, int feat, int pt)
        {
            StreamReader reader = new StreamReader("saveDat");

            string line = " ";
            int lineNo = 0;
            bool found = false;
            string[] words = new string[] { };
            //Lectura hasta la última línea (el último nivel completado)
            while (!reader.EndOfStream && !found)
            {
                line = reader.ReadLine();
                lineNo++;
                words = line.Split(' ');
                if (words[0] == user) found = true;
            }
            reader.Close();
            if (found)
            {
                StreamWriter writer = new StreamWriter("saveDat", false);        //creación del archivo de usuario

                writer.Write(user + " " + seed + " " + roug + " " + feat + " " + pt, lineNo);

                writer.Close();
            }
            else
            {
                StreamWriter writer = new StreamWriter("saveDat", true);        //creación del archivo de usuario

                writer.WriteLine(user + " " + seed + " " + roug + " " + feat + " " + pt);
                writer.Close();
            }
           
        }

        static void PlayGame(ref bool play, ref PhySick player, ref RandWrld mars, ref char oldIn)
        {
            //Console.Clear();
            if (DEBUG)
            {
                Console.SetCursorPosition(1, 1);
                Console.WriteLine("Forces: " + player.GetForce(1) + "x  " + player.GetForce(0) + "y");
                Console.WriteLine("Pos: " + player.objPos[1] + "x  " + player.objPos[0] + "y");
            }
            player.TimeSinceFloor(inAir);
            bool oldAir = inAir;
            char ch = LeeInput();
            if (ch != oldIn)
            {
                ManageInput(ref player, ch, ref FUEL);
                if (ch == 'q') play = false;

            }

            player.MainPhysics();
            bool danger = player.FastEntry();

            Collisions(ref player, mars.GetWorld(), ref FUEL, ref POINTS);

            mars.RenderWorld(HEIGHT, RENDERSIZE);
            RenderPlayer(player.objPos, RENDERSIZE, FUEL, danger);

            if (!inAir && oldAir && !DED)
            {
                for (int i = 0; i < POINTS; i++) mars.WorldGen(FEATURES, ROUGHNESS);
                startingPoint = new float[2] { mars.GetWorld()[POINTS], WIDTH / 4 };

                player.objPos = startingPoint;

                oldAir = inAir;

                mars.RenderWorld(HEIGHT, RENDERSIZE);
                RenderPlayer(player.objPos, RENDERSIZE, FUEL, danger);

            }


            Thread.Sleep((int)REFRESH);
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