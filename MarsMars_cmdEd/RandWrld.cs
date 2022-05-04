//ENRIQUE JUAN GAMBOA
//Proyecto final FPII

using System;

//GENERADOR DE MUNDOS ALEATORIOS
namespace RandWrld_generator
{
    class RandWrld
    {
        //Ancho y altitud máxima del mundo
        private int ancho, maxAlt;

        //El mundo
        private int[] wrld;
        //Randomizador del terreno
        Random rnd = new Random();

        //Inicialización del mundo: ancho, alto y semilla para generar
        public RandWrld(int ANCHO, int ALTOMAX, int oldWrld)
        {
            ancho = ANCHO;

            maxAlt = ALTOMAX;

            wrld = new int[ancho];

            rnd = new Random(oldWrld);
        }

        //Devolver el mundo (readonly)
        public int[] GetWorld()
        {
            return wrld;
        }

        //Generación del mundo 1 a 1
         public void WorldGen(int features, int roughness)
        {
            for (int i = 0; i < ancho - 1; i++)
            { // desplazamiento de eltos a la izda
                wrld[i] = wrld[i + 1];
            }

            int s; // ultima posicion de suelo
            s = wrld[ancho - 1];

            // generamos nuevos valores para esa posicion
            int opt = rnd.Next(features); //si varia
            int var = rnd.Next(1, roughness); //Cuánto varia

            //Límites de la variación
            if (opt == 0 && s < maxAlt)
                s +=var;
            else if (opt == 1 && s > 3)
                s -= var;


            // asignamos ultima posicion en el array
            wrld[ancho - 1] = s;
        }

        //Inicialización  del mundo
        public void StartWorld(int feat, int roug )
        {
            //Se esconde el cursor para rederizado limpio
            Console.CursorVisible = false;

            //Primeros valores del techo
            wrld[ancho - 1] = maxAlt - 1;

            //Generación del mundo en la primera pantalla
            for (int i = 0; i < ancho - 1; i++) WorldGen(feat, roug);
        }

        //Renderizado del mundo
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

                    //Si es el suelo
                    if (j >= wrld[i])
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    }
                    //Si no es nada
                    else Console.BackgroundColor = ConsoleColor.Black;

                    //Escritura de la posición
                    for (int k = 0; k < renderSize; k++) Console.Write(" ");
                }
            }
        }
    }
}
