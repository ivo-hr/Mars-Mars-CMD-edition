//Enrique Juan Gamboa

//Añadido elementos para mejorar Quality of Life (QoL):
//-Leyenda de controles
//-Indicador de pausa
//-Indicador al Game Over
//-Indicador al Quit
//-Cambio de sprite al morir la nave


using System;
using System.Threading;

namespace PRAC2
{
    class Program
    {
        //Booleano para activar el debug
        static bool DEBUG = false;
        //Ancho y alto del juego
        const int ANCHO = 30, ALTO = 15;
        //Número "aleatorio" utilizado para el juego (cueva y enemigo)
        static Random rnd = new Random();

        //M E T O D O  P P A L
        static void Main()
        {
            int[] suelo = new int[ANCHO], // límites del tunel
                   techo = new int[ANCHO];
            
            //Inicialización de posiciones del jugador, enemigo, bala y colisión
            int naveC = ANCHO/2, naveF,
                balaC = -1, balaF = 0,
                enemigoC = -1, enemigoF = 0,
                colC = -1, colF = -1;
            //Booleanos para acabar el juego, pausar y perder
            bool quit = false, pause = false,
                gameOver = false;

            //Inicialización del túnel
            IniciaTunel(ref suelo, ref techo);
            //Asignación del lugar inicial de la nave una vez creado el túnel
            naveF = techo[naveC] + (suelo[naveC] - techo[naveC]) / 2;
            //Renderizado inicial
            Render(suelo, techo, enemigoC, enemigoF, naveC, naveF, balaC, balaF, colC, colF, gameOver);
            //(QoL) Leyenda de controles
            Console.SetCursorPosition(0, ALTO);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("Move: WASD/arrow keys  Shoot: X/space  Pause: P  Quit: Q/esc");

            //BUCLE PRINCIPAL
            while (!quit && !gameOver)
            {
                //Input del usuario
                char chIn = LeeInput();
                //Función de pausa y salida del juego
                if (chIn == 'p') pause = !pause;
                else if (chIn == 'q') quit = true;

                //Juego! (descripciones de los métodos en cada método)
                if (!pause)
                {
                    AvanzaTunel(suelo, techo);
                    
                    GeneraAvanzaEnemigo(ref enemigoC, ref enemigoF, suelo, techo);

                    AvanzaNave(chIn, ref naveC, ref naveF, enemigoC, enemigoF, suelo, techo);

                    GeneraAvanzaBala(chIn, ref balaC, ref balaF, naveC, naveF, enemigoC, enemigoF, suelo, techo);
                    
                    gameOver = ColisionaNave(naveC, naveF, suelo, techo, enemigoC, enemigoF);

                    ColisionaBala(ref balaC, balaF, ref enemigoC, enemigoF, ref suelo, ref techo, out colC, out colF);

                    Render(suelo, techo, enemigoC, enemigoF, naveC, naveF, balaC, balaF, colC, colF, gameOver);

                    //Retardo
                    Thread.Sleep(100);

                }
                //(QoL) Indicación de que el juego está pausado
                else
                {
                    Console.SetCursorPosition(ANCHO - 6, ALTO/2);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(" P A U S E D ");
                }
            }
            //(QoL) Al acabar el juego
            Console.SetCursorPosition(0, ALTO);
            //Si se ha perdido
            if (gameOver)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("YOU LOST");
            }
            //Si se ha salido del juego
            else Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("press any key to exit...");
            //Esconde el texto final
            Console.ForegroundColor = ConsoleColor.Black;
        }


        //Renderizado del túnel
        static void RenderTunel(int[] suelo, int[] techo)
        {
            //Cada columna
            for (int i = 0; i < ANCHO; i++)
            {
                //La fila de cada columna
                for (int j = 0; j < ALTO; j++)
                {
                    //Colocación del cursor
                    Console.SetCursorPosition(2 * i, j);
                    //Si es un muro
                    if (j >= suelo[i] || j <= techo[i]) Console.BackgroundColor = ConsoleColor.Blue;
                    //Si no es nada
                    else Console.BackgroundColor = ConsoleColor.Black;
                    //Escritura dela posición
                    Console.Write("  ");
                } 
            }
        }

        //Inicialización del túnel
        static void IniciaTunel(ref int[] suelo, ref int[] techo)
        {
            //Se esconde el cursor para rederizado limpio
            Console.CursorVisible = false;
            //Primeros valores del techo y suelo
            techo[ANCHO - 1] = 0;
            suelo[ANCHO - 1] = ALTO - 1;
            //Generación del túnel en la primera pantalla
            for (int i = 0; i < ANCHO - 1; i++) AvanzaTunel(suelo, techo);
        }

        //Generación progresiva del túnel
        static void AvanzaTunel(int[] suelo, int[] techo)
        {
            for (int i = 0; i < ANCHO - 1; i++)
            { // desplazamiento de eltos a la izda
                techo[i] = techo[i + 1];
                suelo[i] = suelo[i + 1];
            }

            int s, t; // ultimas posiciones de suelo y techo
            s = suelo[ANCHO - 1];
            t = techo[ANCHO - 1];

            // generamos nuevos valores para esas ultimas posiciones
            int opt = rnd.Next(5); // 5 alternativas
            if (opt == 0 && s < ALTO - 1) { s++; t++; }   // tunel baja           
            else if (opt == 1 && t > 0) { s--; t--; }   // sube
            else if (opt == 2 && s - t > 7) { s--; t++; } // estrecha
            else if (opt == 3)
            {                    // ensancha
                if (s < ALTO - 1) s++;
                if (t > 0) t--;
            } // con 4 se deja igual, no se hace nada

            // asignamos ultimas posiciones en el array
            suelo[ANCHO - 1] = s;
            techo[ANCHO - 1] = t;
        }

        //Generación y movimiento del enemigo
        static void GeneraAvanzaEnemigo(ref int enemigoC, ref int enemigoF, int[] suelo, int[] techo)
        {
            //1/4 probabailidad de generar un enemigo cada llamada si no existe ya
            if (enemigoC < 0 && rnd.Next(0, 4) < 1)
            {
                //Posición vertical aleatoria del enemigo
                enemigoF = rnd.Next(techo[ANCHO - 1] + 1, suelo[ANCHO - 1]);
                
                enemigoC = ANCHO - 1;
            }
            //Si existe ya, se mueve
            else if (enemigoC >= 0) enemigoC--;
        }

        //Comportamiento de la nave
        static void AvanzaNave (char chIn, ref int naveC, ref int naveF, int enemigoC, int enemigoF, int[] suelo, int[] techo)
        {
            //Movimiento de la nave a menos que esté en la misma posición que un muro/enemigo
            if ((naveC != enemigoC && naveF != enemigoF) ||
                (naveF > techo[naveC] || naveF < suelo[naveC]))
            {
                //Movimiento de la nave horizontalmente sólo si está dentro del juego
                if (naveC > 0 && naveC < ANCHO - 1)
                {
                    if (chIn == 'l') naveC--;
                    else if (chIn == 'r') naveC++;
                }
                //Movimiento de la nave vertical
                if (chIn == 'u') naveF--;
                else if (chIn == 'd') naveF++;
            }
                

        }

        //Generación y movimiento de la bala
        static void GeneraAvanzaBala(char chIn, ref int balaC, ref int balaF, int naveC, int naveF, int enemigoC, int enemigoF, int[] suelo, int [] techo)
        {
            //Generación de la bala si no existe y lo pide el jugador
            if (chIn == 'x' && balaC < 0)
            {
                //Colocación de la bala en la pos del jugador
                balaC = naveC + 1;
                balaF = naveF;
            }
            //Movimiento de la bala a menos que esté en la misma posición que un muro/enemigo y dentro del juego
            else if ((balaC >= 0 && balaC < ANCHO) &&
                (balaC != enemigoC || balaF != enemigoF) &&
                (balaF > techo[balaC] && balaF < suelo[balaC])
                )
                balaC++;
        }

        //Renderizado del juego
        static void Render(int[] suelo, int[] techo, int enemigoC, int enemigoF, int naveC, int naveF, int balaC, int balaF, int colC, int colF, bool gameOver)
        { 
            //Renderizado del túnel
            RenderTunel(suelo, techo);
            //Cambio de fondo
            Console.BackgroundColor = ConsoleColor.Black;
            //Renderizado de la bala (si existe)
            if (balaC >= 0)
            {
                Console.SetCursorPosition(2 * balaC, balaF);
                Console.ForegroundColor = ConsoleColor.Magenta;
                
                Console.Write("--");
            }
            //Renderizado de la colisión (si existe)
            else if (colC >= 0)
            {
                Console.SetCursorPosition(2 * colC, colF);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("**");
            }
            //Renderizado del enemigo (si existe)
            if (enemigoC >= 0)
            {
                Console.SetCursorPosition(2 * enemigoC, enemigoF);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("<>");
            }
            //Renderizado del jugador
            Console.SetCursorPosition(2 * naveC, naveF);
            //Si no ha colisionado, renderizado normal
            if (!gameOver) 
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("->");
            } 
            //(QoL) Si ha colisionado
            else 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(":(");
            }
            

            //Información del debug
            if (DEBUG)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(0, ALTO + 2);
                //Pos del jugador
                Console.Write("Player position: " + naveC + ", " + naveF + "   ");
                //Pos de la bala
                Console.Write("Bullet position: " + balaC + ", " + balaF + "   ");
                //Pos del enemigo
                Console.Write("Enemy position: " + enemigoC + ", " + enemigoF + "   ");
                //Pos de la colisión
                Console.Write("Collision position: " + colC + ", " + colF + "   ");
                //Techos y suelos de la cueva actual
                Console.SetCursorPosition(0, ALTO + 3);
                Console.Write("Cavern info: ");
                for (int i = 0; i < ANCHO; i++)
                {
                    Console.Write("col" + i + " " + techo[i] + ", " + suelo[i] + " | ");
                }
            }
        }

        //Input del usuario
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
                while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
            }

            return ch;
        }

        //Colisión de la nave
        static bool ColisionaNave(int naveC, int naveF, int[] suelo, int[] techo, int enemigoC, int enemigoF)
        {
            //Booleano para devolver al método
            bool crashed = false;
            //Comprobación de colisión
            if ((naveC == enemigoC && naveF == enemigoF) ||
                (naveF <= techo[naveC] || naveF >= suelo[naveC]))
                crashed = true;

            //Devolución del booleano al método
            return crashed;
        }

        //Colisión de la bala
        static void ColisionaBala(ref int balaC, int balaF, ref int enemigoC, int enemigoF, ref int[] suelo, ref int[] techo, out int colC, out int colF)
        {
            //Booleano para asignar la pos de colisión
            bool isThere = false;
            //Inicialización de la colisión
            colC = colF = -1;

            if (balaC > 0)
            {
                //Eliminación de la bala al llegar al final de la pantalla
                if (balaC > ANCHO - 1) balaC = -1;
                //Colisión con el enemigo
                else if (balaC >= enemigoC && balaF == enemigoF)
                {
                    //Destrucción del enemigo
                    enemigoC = -1;
                    //Hubo colisión
                    isThere = true;
                }

                else if (balaF <= techo[balaC])
                {
                    //Destrucción del techo desde la posición de colisión
                    techo[balaC] = balaF - 1;
                    //Hubo colisión
                    isThere = true;
                }

                else if (balaF >= suelo[balaC])
                {
                    //Destrucción del suelo hasta la posición de colisión
                    suelo[balaC] = balaF + 1;
                    //Hubo colisión
                    isThere = true;
                }
                //Si hay colisión
                if (isThere)
                {
                    //Se asigna los valores de colisión
                    colC = balaC;
                    colF = balaF;
                    //Destrucción de la bala
                    balaC = -1;
                }
            }
        }


    }
}


