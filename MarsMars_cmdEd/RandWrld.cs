using System;

namespace RandWrld_generator
{
    class RandWrld
    {
        private int ancho, maxAlt;

        private int[] wrld;

        Random rnd = new Random();
        public RandWrld(int ANCHO, int ALTOMAX, int oldWrld)
        {
            ancho = ANCHO;

            maxAlt = ALTOMAX;

            wrld = new int[ancho];

            if (oldWrld != 0)
            {
                rnd = new Random(oldWrld);
            }
        }

        public int[] GetWorld()
        {
            return wrld;
        }

         public void WorldGen(int features, int roughness)
        {
            for (int i = 0; i < ancho - 1; i++)
            { // desplazamiento de eltos a la izda
                wrld[i] = wrld[i + 1];
            }

            int s; // ultimas posiciones de suelo y techo
            s = wrld[ancho - 1];

            // generamos nuevos valores para esas ultimas posiciones
            int opt = rnd.Next(features); //si varia
            int var = rnd.Next(1, roughness); //Cuánto varia
            if (opt == 0 && s < maxAlt)
                s +=var;
            else if (opt == 1 && s > 3)
                s -= var;


            // asignamos ultimas posiciones en el array
            wrld[ancho - 1] = s;
        }

        public void StartWorld(int feat, int roug )
        {
            //Se esconde el cursor para rederizado limpio
            Console.CursorVisible = false;
            //Primeros valores del techo y suelo

            wrld[ancho - 1] = maxAlt - 1;
            //Generación del túnel en la primera pantalla
            for (int i = 0; i < ancho - 1; i++) WorldGen(feat, roug);
        }

        public void RenderWorld(int floor, int renderSize)
        {
            //Cada columna
            for (int i = 0; i < ancho; i++)
            {
                //La fila de cada columna
                for (int j = 0; j < floor; j++)
                {
                    //Colocación del cursor
                    Console.SetCursorPosition(renderSize * i, j);
                    //Si es un muro

                    if (j >= wrld[i])
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    //Si no es nada
                    else Console.BackgroundColor = ConsoleColor.Black;
                    //Escritura dela posición
                    for (int k = 0; k < renderSize; k++) Console.Write(" ");
                }
            }
        }
    }
}
