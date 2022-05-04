//Enrique Juan Gamboa

using System;
using System.IO;

namespace prac1
{
    class Program
    {
        const int DIM = 12;                                             //Dimensiones del tablero

        enum Direccion { Arriba, Abajo, Izquierda, Derecha, None };     //Struct que marca dirección

        struct Coordenada { public int x, y; };                         //Struct para las posiciones de los elementos

        struct Bloque                                                   //Bloques del juego con posición, dirección y color
        {
            public Coordenada pos;
            public int color;
            public Direccion dir;
        }

        struct Estado                                                   //Struct que marca el estado del juego
        {
            public int[,] huecos;
            public Direccion[,] giros;
            public Bloque[] bloques;
            public Coordenada cursor;
        }


        static void Main()
        {
            Estado est;                                                                     //Creacion de la variable estado

            int[] oldWinSize = new int[2] {Console.WindowHeight, Console.WindowWidth};      //(Opcional) Para detectar si cambia el tamaño de pantalla

            int lvl;                                                                        //Nivel a jugar

            string user;                                                                    //Usuario para la selección de partida

            Console.WriteLine("Hello!");
            Console.WriteLine("Enter your username: ");
            user = Console.ReadLine();
            lvl = CargaProgreso(user);

            //Si existe progreso, pregunta si quiere continuar o reempezar
            if (lvl != 1)
            {
                Console.WriteLine("Want to continue? (y/n): ");
                if (Console.ReadLine() == "n")
                {
                    Console.WriteLine("Level Nº: ");
                    lvl = int.Parse(Console.ReadLine());
                }
            }
            //Si no, se empieza directamente
            else
            {
                Console.WriteLine("Level Nº: ");
                lvl = int.Parse(Console.ReadLine());
            }

            //Lectura del nivel
            string lvlN = LeeNivel(lvl, out est);

            //Si el nivel no existe, se vuelve a pedir
            while (lvlN == "error")
            {
                Console.WriteLine("[ERROR]: That level does not exist!");
                Console.WriteLine("Please enter a valid level number: ");
                lvl = int.Parse(Console.ReadLine());

                lvlN = LeeNivel(lvl, out est);
            }
            //Renderizado inicial
            Console.Clear();
            Render(est);

            char oldInput = ' ';        //Input anterior
            char input = ' ';           //Input actual
            int numMov = 0;             //Número de movimientos
            bool quit = false;          //Marcador de fin de partida
            


            //B U C L E   P P A L
            while (!quit)
            {
                //(Opcional) Si se cambia el tamaño de la pantalla, se re-renderiza
                if (Console.WindowHeight != oldWinSize[0] || Console.WindowWidth != oldWinSize[1])
                {
                    oldWinSize[0] = Console.WindowHeight;
                    oldWinSize[1] = Console.WindowWidth;

                    Console.Clear();
                    Render(est);
                }

                input = LeeInput();                                                                                 //Entrada de input

                //Si se finaliza el nivel
                if (FinNivel(est))
                {
                    GuardaProgreso(user, lvl, numMov);                                                              //Guardado del progreso

                    //Mensaje de fin y opción a pasar al siguiente nivel
                    Console.Clear();
                    Console.WriteLine("Yay!  You did " + numMov + " movements. next level? y/n");
                    string resp = Console.ReadLine();

                    //Inicialización del siguiente nivel
                    if (resp == "y")
                    {
                        Console.Clear();
                        numMov = 0;
                        lvl++;
                        lvlN = LeeNivel(lvl, out est);
                        Render(est);

                    }
                    else quit = true;                                                                               //Fin del juego
                }


                //Si el input es diferente al anterior
                if (input != oldInput)
                {
                    if (input == 'X') lvlN = LeeNivel(lvl, out est);                                               //Reseteo del nivel
                    else if (input == 'q') quit = true;                                                            //Fin del juego

                    //Procesado de input
                    else
                    {
                        ProcesaInput(input, ref est);

                        if (input == 'e') numMov++;                                                                 //Al clicar una casilla se considera movimiento
                    }

                    Render(est);
                }
            }
            
        }

        //Lectura del nivel
        static string LeeNivel(int n, out Estado est)
        {
            //Inicialización del estado
            est.huecos = new int[DIM, DIM];
            est.giros = new Direccion[DIM, DIM];
            est.bloques = new Bloque[DIM + 1];
            est.cursor.x = 0;
            est.cursor.y = 0;

            //Rellenado inicial del tablero vacío
            for (int i = 0; i < DIM; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    est.huecos[i, j] = -1;
                    est.giros[i, j] = Direccion.None;
                }
            }

            //LECTURA DEL NIVEL
            StreamReader file = new StreamReader("levels");

            bool found = false;                                                     //Para encontar el nivel pedido
            string lvlName = "error";                                               //Se inicia 
            string line;

            //Búsqueda del nivel
            while (!file.EndOfStream && !found)
            {
                line = file.ReadLine();                                             //Lectura de la línea

                string[]words = line.Split(" ");                                    //División para buscar el nivel

                //Si es el inicio de un nivel
                if (words[0] == "level")
                {
                    int lvlNo = int.Parse(words[1]);                                //Número del nivel

                    //Si el número del nivel es el pedido
                    if (lvlNo == n)
                    {
                        lvlName = words[2];                                         //Nombre del nivel
                        found = true;                                               //Nivel encontrado!
                    }
                }
            }

            //Si se ha encontrado el nivel
            if (lvlName != "error")
            {
                line = file.ReadLine();

                int blockNo = 0;                                                                        //Número de bloques

                //Hasta que no se acabe el nivel
                while (line != "")
                {
                    string[] words = line.Split(" ");

                    //Si se lee un hueco
                    if (words[0] == "hueco")
                    {
                        est.huecos[int.Parse(words[1]), int.Parse(words[2])] = int.Parse(words[4]);     //Creación del hueco en el estado
                    }
                    //Si se lee un giro
                    else if (words[0] == "giro")
                    {
                        est.giros[int.Parse(words[1]), int.Parse(words[2])] = DirParser(words[3]);      //Creación del giro en el estado (método de transformación de la dirección)
                    }

                    //Si se lee un bloque
                    else if (words[0] == "bloque")
                    {
                        //Posición del bloque en el estado
                        est.bloques[blockNo].pos.x = int.Parse(words[1]);
                        est.bloques[blockNo].pos.y = int.Parse(words[2]);

                        est.bloques[blockNo].color = int.Parse(words[4]);                               //Color del bloque
                        est.bloques[blockNo].dir = DirParser(words[5]);                                 //Dirección del bloque (método de transformación de la dirección)

                        AplicaGiro(est.giros, ref est.bloques[blockNo]);                                //Si el bloque se encuentra en una casilla de giro, se gira

                        blockNo++;
                    }

                    line = file.ReadLine();
                }

                //Creación del último bloque vacío en el estado
                est.bloques[blockNo].color = -1;
                est.bloques[blockNo].dir = Direccion.None;

                file.Close();
            }


            return lvlName;                                     //Devolución del nombre ("error" si no se encuentra)
        }

        //Traducción de la dirección dada
        static Direccion DirParser(string s)
        {
            Direccion toRet = Direccion.None;                           //A devolver
            
            //Traducción del string al struct Direccion
            switch (s)
            {
                case "up": toRet = Direccion.Arriba; break;
                case "down": toRet = Direccion.Abajo; break;
                case "left": toRet = Direccion.Izquierda; break;
                case "right": toRet = Direccion.Derecha; break;
            }

            return toRet;                                              //Dirección traducida (si no hay dirección, Direccion.None)
        }

        //Renderizado
        static void Render(Estado est)
        {
            Console.CursorVisible = false;                      //Se esconde el cursor para el dibujo

            //(Opcional) Centrado del juego
            int startDrawH = (Console.WindowHeight - DIM) / 2;
            int startDrawW = (Console.WindowWidth - DIM) / 2;
            
            //Bucles de dibujo
            for (int i = 0; i < DIM; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    
                    Console.SetCursorPosition(startDrawW + 2 * i,startDrawH + j);        //Posición del cursor en el punto a dibujar
                    Console.BackgroundColor = ConsoleColor.Black;                        //Color de fondo inicial
                    bool thisHuecoColor = false;                                         //Ayuda para coloración de la posición del cubo sobre una meta/giro

                    //Si hay una meta/giro
                    if (est.huecos[i, j] != -1 || est.giros[i, j] != Direccion.None)
                    {
                        thisHuecoColor = true;

                        if (est.giros[i, j] == Direccion.None) Console.ForegroundColor = cols[est.huecos[i, j]];    //Si es una meta
                        else Console.ForegroundColor = cols.Last();

                        //Dibujo de la meta/giro
                        switch (est.giros[i, j])
                        {
                            case Direccion.Abajo: Console.Write(downG); break;
                            case Direccion.Arriba: Console.Write(upG); break;
                            case Direccion.Izquierda: Console.Write(leftG); break;
                            case Direccion.Derecha: Console.Write(rightG); break;
                            case Direccion.None: Console.Write(goal); break;
                        }
                    }
                    //Si es un hueco vacío
                    else
                    {
                        Console.ForegroundColor = cols.Last();
                        Console.Write(free);
                    }


                    Console.SetCursorPosition(startDrawW + 2 * i, startDrawH + j);        //Se recoloca el cursor para el dibujo de las cajas

                    int k = 0;                                                            //Ayuda para el while
                    bool allBlocksR = false;                                              //Si se ha llegado al último bloque

                    //Búsqueda de un bloque en la posición (si hay más bloques)
                    while (!allBlocksR && k <= est.bloques.Length)
                    {
                        //Si no es el bloque vacío
                        if (est.bloques[k].color != -1 && est.bloques[k].dir != Direccion.None)
                        {
                            //Si el bloque está en esa posición
                            if (est.bloques[k].pos.x == i && est.bloques[k].pos.y == j)
                            {
                                //Si está en la meta
                                if (thisHuecoColor)
                                {
                                    Console.BackgroundColor = Console.ForegroundColor;
                                }

                                Console.ForegroundColor = cols[est.bloques[k].color];           //Color del bloque

                                //Dibujo del bloque
                                switch (est.bloques[k].dir)
                                {
                                    case Direccion.Abajo: Console.Write(downA); break;
                                    case Direccion.Arriba: Console.Write(upA); break;
                                    case Direccion.Izquierda: Console.Write(leftA); break;
                                    case Direccion.Derecha: Console.Write(rightA); break;
                                }
                            }
                        }
                        else allBlocksR = true;                                                 //Si es el bloque vacío se acaba la búsqueda 
                        k++;
                    }
                }
            }

            //Se muestra el cursor y se sitúa en la posición dada
            Console.CursorVisible = true;
            Console.SetCursorPosition(startDrawW + 2 * est.cursor.x, startDrawH + est.cursor.y);
        }
        
        //Iconos del juego
        const char
            upA = '▲', downA = '▼', leftA = '◄', rightA = '►',
            goal = 'o', free = '■',
            upG = '^', downG = 'v', leftG = '<', rightG = '>';

        //Colores del juego
        static ConsoleColor[] cols = {
            ConsoleColor.Red,ConsoleColor.Blue,ConsoleColor.Green,ConsoleColor.Magenta,
            ConsoleColor.Yellow,ConsoleColor.Cyan,ConsoleColor.Gray};

        //Colores oscurecidos del juego
        static ConsoleColor[] colsDark = {
            ConsoleColor.DarkRed,ConsoleColor.DarkBlue,ConsoleColor.DarkGreen,
            ConsoleColor.DarkMagenta,ConsoleColor.DarkYellow,ConsoleColor.DarkCyan,
            ConsoleColor.DarkGray};

        //Lectura del input
        static char LeeInput()
        {
            char d = ' ';
            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey().Key.ToString().ToUpper();
                switch (tecla)
                {
                    case "LEFTARROW": d = 'l'; break;
                    case "UPARROW": d = 'u'; break;
                    case "RIGHTARROW": d = 'r'; break;
                    case "DOWNARROW": d = 'd'; break;
                    case "ENTER": case "SPACEBAR": d = 'e'; break;
                    case "Escape": case "Q": d = 'q'; break;
                    case "R": d = 'R'; break;
                    case "X": d = 'X'; break;
                }
            }
            return d;
        }

        //Procesado del input
        static void ProcesaInput(char c, ref Estado est)
        {
            //Casos del input
            switch (c)
            {
                //Movimiento del cursor siempre que esté en los límites del juego
                case 'l': if (est.cursor.x - 1 >= 0)
                        est.cursor.x--; break;
                case 'r': if (est.cursor.x + 1 < DIM)
                        est.cursor.x++; break;
                case 'd': if (est.cursor.y  + 1< DIM)
                        est.cursor.y++; break;
                case 'u': if (est.cursor.y - 1 >= 0)
                        est.cursor.y--; break;

                case 'e': ClickCasilla(ref est); break;     //Al clicarse la casilla
            }
        }

        //Comprobación si una coordenada está dentro del tablero
        static bool Dentro(Coordenada c)
        {
            bool toRet = (c.x >= 0 && c.x < DIM && c.y >= 0 && c.y < DIM);

            return toRet;
        }

        //Cambio de dirección de un bloque si se encuentra en una casilla de giro
        static void AplicaGiro(Direccion[,] giros, ref Bloque b)
        {
            //Si hay giro en la casilla
            if (giros[b.pos.x, b.pos.y] != Direccion.None)
            b.dir = giros[b.pos.x, b.pos.y];
        }

        //Búsqueda de un bloque sobre la coordenada dada
        static int BuscaBloque(Coordenada c, Bloque[] bloques)
        {
            bool found = false;                                                                         //Si se ha encontrado
            bool endOfBlocks = false;                                                                   //Si se ha llegado al bloque vacío
            int currBlock = 0;                                                                          //Bloque actual a devolver

            //Búsqueda del bloque
            while (!found &&  !endOfBlocks)
            {
                if (bloques[currBlock].color == -1) endOfBlocks = true;                                 //Si es el bloque vacío se deja de buscar

                else 
                {
                    found = bloques[currBlock].pos.x == c.x && bloques[currBlock].pos.y == c.y;
                    currBlock++;
                } 
            }

            if (found) currBlock--;                                                                    //Si lo ha encontrado devuelve la posición en el array del bloque
            else currBlock = -1;
            return currBlock;
        }

        //Búsqueda de la primera casilla libre en la dirección dada
        static Coordenada BuscaCasillaLibre(Coordenada pos, Coordenada dir, Bloque[] bloques)
        {
            //Coordeandas a devolver
            Coordenada freeSp;      
            freeSp.x = pos.x;
            freeSp.y = pos.y;

            bool thisFree = false;                                  //Si la casilla está libre

            //Búsqueda de la casilla
            while (!thisFree)
            {
                freeSp.x += dir.x;
                freeSp.y += dir.y;

                thisFree = BuscaBloque(freeSp, bloques) == -1;      //Devuelve la casilla libre encontrada
            }

            return freeSp;
        }

        //Evento de clicar una casilla
        static void ClickCasilla(ref Estado est)
        {
            int clickedBlockNo = BuscaBloque(est.cursor, est.bloques);                               //Búsqueda de un bloque en la casilla

            //Si hay un bloque
            if (clickedBlockNo != -1)
            {   
                Bloque clickedBlock = est.bloques[clickedBlockNo];                                    //Bloque clicado

                //Dirección del bloque
                Coordenada clickedBlockDir;
                clickedBlockDir.x = 0;
                clickedBlockDir.y = 0;
                switch (clickedBlock.dir)
                {
                    case Direccion.Arriba: clickedBlockDir.y = -1; break;
                    case Direccion.Izquierda: clickedBlockDir.x = -1; break;
                    case Direccion.Abajo: clickedBlockDir.y = +1; break;
                    case Direccion.Derecha: clickedBlockDir.x = +1; break;
                }

                Coordenada freeSp = BuscaCasillaLibre(est.cursor, clickedBlockDir, est.bloques);      //Coordenadas de la casilla libre más cercana

                //Si la casilla libre está dentro del tablero
                if (Dentro(freeSp))
                {
                    bool lastBlock = false;                                                             //Si es el último bloque que hay que mover
                   
                    //Hasta que no se muevan todoas los bloques
                    while (!lastBlock)
                    {
                       
                        //Se desplaza la casilla libre hacia el bloque clicado
                        freeSp.x -= clickedBlockDir.x;
                        freeSp.y -= clickedBlockDir.y;

                        int currBlockNo = BuscaBloque(freeSp, est.bloques);                             //Número del bloque actual a mover

                        //Movimiento y giro del bloque actual
                        est.bloques[currBlockNo].pos.x = clickedBlockDir.x + freeSp.x;
                        est.bloques[currBlockNo].pos.y = clickedBlockDir.y + freeSp.y;
                        AplicaGiro(est.giros, ref est.bloques[currBlockNo]);


                        lastBlock = freeSp.x == clickedBlock.pos.x && freeSp.y == clickedBlock.pos.y;   //Comprobación si el último bloque



                    }

                    est.cursor = est.bloques[clickedBlockNo].pos;                                      //Se posiciona el cursor sobre la nueva posición del bloque
                }
            }
        }

        //Comprobación del final del nivel
        static bool FinNivel(Estado est)
        {
            bool toReturn = true;                           //Variable a devolver


            int blockNum = 0;                               //Número del bloque en comprobación
            bool finSearch = false;                         //Si se ha llegado al bloque vacío
            while (!finSearch && toReturn)
            {

                //Si se encuentra un bloque que no está en la meta se deja de buscar
                if (est.bloques[blockNum].color != est.huecos[est.bloques[blockNum].pos.x, est.bloques[blockNum].pos.y])        
                    toReturn = false;

                
                blockNum++;
                finSearch = est.bloques[blockNum].color == -1;  //Deja de buscar si es el último bloque
            }

            return toReturn;
        }

        //Guardado del progreso
        static void GuardaProgreso(string u, int lvl, int moves)
        {
            
            
            StreamWriter writer = new StreamWriter(u, true);        //Lectura/creación del archivo de usuario

            writer.WriteLine(lvl + " " + moves);                    //Guardado del último nivel hecho y de los movimientos

            writer.Close();


        }

        //Cargado del progreso
        static int CargaProgreso(string u)
        {
            int lvl = 1;        //Nivel por defecto

            //Si existe el usuario, devuelve el nivel siguiente al último completado
            if (File.Exists(u))
            {
                StreamReader reader = new StreamReader(u);

                string line = " ";
                //Lectura hasta la última línea (el último nivel completado)
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                }
                
                //Lectura del número del último nivel
                string[] words = line.Split(' ');
                lvl = int.Parse(words[0]) + 1;          //Nivel siguiente al completado
                
                reader.Close();
            }


            return lvl;
        }
    }
}
