//ENRIQUE JUAN GAMBOA
//Proyecto final FPII

using System;
using PhySick_engine;
using System.Threading;
//using Listas;     Las listas al final no han tenido uso debido a que no hay arrays de tamaño variable ni elementos de  valores concisos:
//todos los arrays son fijos y las variables tienden a ser "análogas" y sobre una sola entidad
using System.IO;
using RandWrld_generator;
namespace MainPr
{
    internal class Program
    {
        //Si el astronauta está en el aire
        static bool inAir = false;
        //Modo debug
        static bool DEBUG = false;
        //Si ha muerto el astronauta
        static bool DED = false;

        //DATOS DEL MUNDO
        static int WIDTH = 30,
            RENDERSIZE = 4,
            MAXHEIGHT = 27,
            HEIGHT = 30,
            SEED = 0,
            ROUGHNESS = 2,
            FEATURES = 3;

        //ELEMENTOS DEL MUNDO
        static float GRAVITY = 20,
              RESISTANCE = 1,
              MASS = 2,
              DEATHSPEED = 15,
              MAXSPEED = 15,
              REFRESH = 100;

        //DATOS  DEL JUGADOR
        static string USER = "default";
        static int FUEL = 100,
                    POINTS = 0,
                    GUZZLE = 2;
        static float[] startingPoint;


        static void Main(string[] args)
        {
            //Introducción
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄\n" +
                                "██ ▄▀▄ █ ▄▄▀██ ▄▄▀██ ▄▄▄ █▀████ ▄▀▄ █ ▄▄▀██ ▄▄▀██ ▄▄▄ █████▀▄▀█ ▄▀▄ █ ▄▀██\n" +
                                "██ █ █ █ ▀▀ ██ ▀▀▄██▄▄▄▀▀██████ █ █ █ ▀▀ ██ ▀▀▄██▄▄▄▀▀█████ █▀█ █▄█ █ █ ██\n" +
                                "██ ███ █ ██ ██ ██ ██ ▀▀▀ █▄████ ███ █ ██ ██ ██ ██ ▀▀▀ ██████▄██▄███▄█▄▄███\n" +
                                "▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
            Console.WriteLine("");
            Console.WriteLine("Enter your username: ");

            //Asignación del usuario e inicio/continuación de la partida
            USER = Console.ReadLine();
            StartGame(ref USER, ref SEED, ref ROUGHNESS, ref FEATURES, ref POINTS);

            //Refresco de la pantalla
            Console.Clear();

            //CREACIÓN DEL MUNDO
            RandWrld mars = new RandWrld(WIDTH, MAXHEIGHT, SEED);
            mars.StartWorld(FEATURES, ROUGHNESS);
            for (int i = 0; i < POINTS + (3*WIDTH/4); i++) mars.WorldGen(FEATURES, ROUGHNESS);
            mars.RenderWorld(HEIGHT, RENDERSIZE);

            //CREACIÓN DEL JUGADOR
            PhySick player = new PhySick(GRAVITY, RESISTANCE, MASS, DEATHSPEED, MAXSPEED, REFRESH);
            startingPoint = new float[2] {mars.GetWorld()[WIDTH/4], WIDTH/4};
            player.objPos = startingPoint;

            //Elementos para el bucle
            char ch = ' ';
            bool play = true;
            bool danger = player.FastEntry();

            //B U C L E  P R I N C I P A L
            while (play)
            {
                //Juego si está vivo
                if (!DED) PlayGame(ref play, ref player, ref mars, ref ch);

                //Si jugador muere
                else
                {
                    //Guardado de partida
                    SaveData(USER, SEED, ROUGHNESS, FEATURES, POINTS);

                    //Pantalla de muerte y continuación de la partida
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    //Console.Clear();
                    Console.SetCursorPosition(0, HEIGHT/4 - 3);
                    Console.Write("▄▄▄▄·             • ▌ ▄ ·. \n" +
                                    "▐█ ▀█▪ ▄█▀▄  ▄█▀▄ ·██ ▐███▪\n" +
                                    "▐█▀▀█▄▐█▌.▐▌▐█▌.▐▌▐█ ▌▐▌▐█·\n" +
                                    "██▄▪▐█▐█▌.▐▌▐█▌.▐▌██ ██▌▐█▌\n" +
                                    "·▀▀▀▀  ▀█▄▀▪ ▀█▄▀▪▀▀  █▪▀▀▀");
                    Console.WriteLine("");
                    Console.WriteLine(USER + " scored " + POINTS + " in the seed " + SEED + "\n" +
                        "Wanna play again? [y/n]");
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
            //Guardado final
            SaveData(USER, SEED, ROUGHNESS, FEATURES, POINTS);
        }
        
        //Renderizado del jugador
        static void RenderPlayer(float[] pos, int renderSz, int fuel, bool danger)
        {
            //Si jugador no ha muerto
            if (!DED)
            {
                //Bucles de renderizado
                for (int i = 0; i < renderSz; i++)
                {
                    for (int j = 0; j < renderSz; j++)
                    {
                        //Posición del jugador
                        int x = (int)pos[1] * renderSz + i;
                        int y = (int)pos[0] - renderSz + j;

                        if (y > 0)
                        {
                            Console.SetCursorPosition(x, y);

                            //Aviso de peligro para el jugador (si  va a impactar de muerte)
                            if (!danger) Console.BackgroundColor = ConsoleColor.Black;
                            else Console.BackgroundColor = ConsoleColor.DarkRed;

                            //Aviso de nivel de gasolina
                            if (fuel > 66) Console.ForegroundColor = ConsoleColor.Green;
                            else if (fuel > 33) Console.ForegroundColor = ConsoleColor.DarkYellow;
                            else if (fuel > 0) Console.ForegroundColor = ConsoleColor.Red;
                            //Si se queda sin gasolina
                            else Console.ForegroundColor = ConsoleColor.DarkGray;

                            //Dibujado del  astronauta
                            if (i < renderSz / 2) Console.Write("(");
                            else if (i == renderSz / 2) Console.Write("|");
                            else Console.Write(")");
                        }
                    }
                }
            }
        }

        //Detección de  colisiones
        static void Collisions(ref PhySick pl, int[] wrld, ref int fuel, ref int pt)
        {
            //Posición del jugador
            int y = (int)pl.objPos[0];
            int x = (int)pl.objPos[1];

            //Detección  de límites izq. y dcho
            if (x * RENDERSIZE < 0 || x * RENDERSIZE + RENDERSIZE > Console.WindowWidth) DED = true;

            //Si el jugador toca el suelo
            if  (!DED && wrld[x] < y && inAir)
            {
                //Si ha impactado de muerte
                DED = pl.FastEntry();
                
                //Si no,
                if (!DED)
                {
                    //Ya no está en el aire
                    inAir = false;
                    
                    //Recolocamos el jugador por si clipea
                    pl.objPos = new float[2] { wrld[x], x };

                    //Cancelamos fuerzas del jugador
                    pl.CancelForces();
                    //Reiniciamos gasolina
                    fuel = 100;
                    //Subimos puntuación
                    pt = x;
                }
            }
        }

        //Gestión de input
        static void ManageInput(ref PhySick pl, char im, ref int fuel)
        {
            //Si aún queda gasolina
            if (fuel > 0)
            //Direcciones:
            switch (im)
            {
                //Motor izq (vuelo hacia la dcha)
                case 'l':
                        //Aplicamos fuerza
                    pl.ApplyForce(5f, -3f * 10);
                        //Quemamos gasolina
                    fuel-= GUZZLE;
                        //Estamos en el aire
                    inAir = true;
                    break;
                //Motor dcho (vuelo hacia la  izq)
                case 'r':
                    pl.ApplyForce(-5f, -3f * 10);
                    inAir = true;
                    fuel-= GUZZLE;
                    break;
                //Ambos motores (vuelo vertical)
                case 'u':
                case 'x':
                    pl.ApplyForce(0f, -6f * 10);
                    inAir = true;
                    fuel-= 2*GUZZLE;
                    break;
            }
        }

        //Inicio de  partida (Carga/creación de datos)
        static void StartGame(ref string user, ref int seed, ref int roug, ref int feat, ref int pt)
        {
            //Si el input es nulo, predeterminamos
            if (user == null) user = "default";

            //Semilla aleatoria por si  no cargamos
            Random sd = new Random();

            //Asganción de semilla y relieve del mundo
            seed = sd.Next();
            roug = 2;
            feat = 3;
            //Puntuación inicial
            pt = 0;

            //Por si no encontramos el jugador
            bool found = false;
            //Si existe el archivo
            if (File.Exists("saveDat"))
            {
                //Lectura
                StreamReader reader = new StreamReader("saveDat");

                //Linea inicial
                string line = " ";
                //Para dividir la línea
                string[] words = new string[] {};
                //Lectura hasta la última línea (el último nivel completado)
                while (!reader.EndOfStream && !found)
                {
                    //Leemos línea
                    line = reader.ReadLine();
                    //Dividimos línea
                    words = line.Split(' ');
                    //Si se encuentra el jugador
                    if (words[0] == user) found = true;
                }
                //Si lo hemos encontrado
                if (found)
                {
                    Console.WriteLine("Found player! Reading file...");

                    //Intentamos cargarlo
                    try
                    {
                        seed = int.Parse(words[1]);
                        roug = int.Parse(words[2]);
                        feat = int.Parse(words[3]);
                        pt = int.Parse(words[4]);
                    }
                    //Si no crearemos un guardado nuevo
                    catch
                    {
                        found = false;

                        Console.WriteLine("ERROR: Data was corrupted! Press [ENTER] to create a new save");
                    }
                    
                }

                reader.Close();
            }
            //Creación de nuevo archivo si no existe
            else
            {
                StreamWriter writer = new StreamWriter("saveDat", false);        //creación del archivo de usuario
                writer.Close();
            }
            //Guardado de la nueva partida
            if (!found)
            {
                SaveData(user, seed, roug, feat, pt);
            }
        }

        //Guardado de partidas
        static void SaveData(string user, int seed, int  roug, int feat, int pt)
        {
            //Buscamos el usuario que  quiere guardar
            StreamReader reader = new StreamReader("saveDat");

            string line = " ";
            //Número de línea en el que escribiremos
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

            //Si existe el usuario lo sobreeescribimos
            if (found)
            {
                StreamWriter writer = new StreamWriter("saveDat", false);

                //Guardamos todos los parámetros por si el código se edita
                writer.Write(user + " " + seed + " " + roug + " " + feat + " " + pt, lineNo);

                writer.Close();
            }
            //Si no,  creamos una nueva línea
            else
            {
                StreamWriter writer = new StreamWriter("saveDat", true);

                //Guardamos todos los parámetros por si el código se edita
                writer.WriteLine(user + " " + seed + " " + roug + " " + feat + " " + pt);

                writer.Close();
            }
           
        }

        //J U E G O!!
        static void PlayGame(ref bool play, ref PhySick player, ref RandWrld mars, ref char oldIn)
        {
            //debug
            if (DEBUG)
            {
                Console.SetCursorPosition(1, 1);
                Console.WriteLine("Forces: " + player.GetForce(1) + "x  " + player.GetForce(0) + "y");
                Console.WriteLine("Pos: " + player.objPos[1] + "x  " + player.objPos[0] + "y");
            }
            //Contador  de tiempo desde en el aire (véase PhySick)
            player.TimeSinceFloor(inAir);
            //Para avance del mundo
            bool oldAir = inAir;

            //Lectura de input
            char ch = LeeInput();

            //Procesdo del input
            ManageInput(ref player, ch, ref FUEL);
            if (ch == 'q') play = false;

            //Físicas generales (véase PhySick)
            player.MainPhysics();
            //Peligro de impacto de muerte
            bool danger = player.FastEntry();

            //Colisiones
            Collisions(ref player, mars.GetWorld(), ref FUEL, ref POINTS);

            //Renderizado del mundo y jugador
            mars.RenderWorld(HEIGHT, RENDERSIZE);
            RenderPlayer(player.objPos, RENDERSIZE, FUEL, danger);

            //Avance del nivel si jugador aterriza bien
            if (!inAir && oldAir && !DED)
            {
                //Generación del mundo hasta un punto (véase RandWrld)
                for (int i = POINTS % WIDTH; i >  WIDTH/4; i--) mars.WorldGen(FEATURES, ROUGHNESS);

                //Posicionamiento del jugador
                startingPoint = new float[2] { mars.GetWorld()[WIDTH/4], WIDTH / 4 };
                player.objPos = startingPoint;

                //Renderizado del mundo y del jugador
                mars.RenderWorld(HEIGHT, RENDERSIZE);
                RenderPlayer(player.objPos, RENDERSIZE, FUEL, danger);

            }

            //Tasa de refresco
            Thread.Sleep((int)REFRESH);
        }

        //Mítico leeinput, MODIFICADO!
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

                //MODIFICACIÓN: permite leer dos inputs  del teclado. Habilita el thrust vertical pulsando izq. y dcha. a la vez
                if (Console.KeyAvailable)
                {
                    string dir2 = Console.ReadKey(true).Key.ToString();
                    if (((dir2 == "A" || dir2 == "LeftArrow") && ch == 'r') ||
                        ((dir2 == "D" || dir2 == "RightArrow") && ch == 'l'))ch = 'u';

                    while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
                }

                while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
            }

            return ch;
        }
    }
}