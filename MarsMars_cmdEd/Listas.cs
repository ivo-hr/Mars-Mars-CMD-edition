//Enrique Juan Gamboa

using System;

namespace Listas{
	// listas enlazadas de ENTEROS (fácilmente adaptable a cualquier otro tipo)
	public class Listas{

		// CLASE NODO (clase privada para los nodos de la lista)
		private class Nodo{
			public int dato;   // información del nodo (podría ser de cualquier tipo)
			public Nodo sig;   // referencia al siguiente

			// la constructora por defecto sería:
			// public Nodo() {} // por defecto

			// implementamos nuestra propia constructora para nodos
			public Nodo(int _dato=0, Nodo _sig=null) {  // valores por defecto dato=0; y sig=null
				dato = _dato;
				sig = _sig;
			}
		}
		// FIN CLASE NODO

		// CAMPOS Y MÉTODOS DE LA CLASE Lista

		// campo pri: referencia al primer nodo de la lista
		Nodo pri;  


		// constructora de la clase Lista
		public Listas(){  
			pri = null;   //  lista vacia
		}


		// insertar elto e al ppio de la lista
		public void InsertaPpio(int e){  
			Nodo aux = new Nodo(e,pri);
			pri = aux;
		}


		// añadir elto e al final de la lista
		public void InsertaFinal(int e){  
			// distinguimos dos casos
			
			// lista vacia
			if (pri == null) {     
				pri = new Nodo(e,null); // creamos nodo en pri

			// lista no vacia				
			} else { 
				Nodo aux = pri;   // recorremos la lista hasta el ultimo nodo
				while (aux.sig != null) aux = aux.sig;
				// aux apunta al último nodo
				aux.sig = new Nodo(e,null); // creamos el nuevo a continuación
			}
		}



		// buscar elto e
		public bool BuscaDato(int e){
			Nodo aux = pri; // referencia al primero para buscar de ppio a fin
			  // búsqueda de nodo con elto e
			while (aux!=null && aux.dato!=e) aux = aux.sig;

			// termina con aux==null (elto no encontrado)
			// o bien con aux apuntando al primer nodo con elto e
			return aux!=null;
		}


		// Conversion a string
        // método ToString que se invoca implícitamente cuando se hace Console.Write
        public override string ToString() { 
			string salida = "\nLista: ";						
			Nodo aux = pri;
			while (aux!=null) {
				salida += aux.dato + " ";
				aux = aux.sig;
			}
			salida += "\n\n";
			return salida;
        }

		// elimina elto e (la primera aparición) de la lista, si está, y devuelve true
		// no hace nada en otro caso y devuelve false
		public bool EliminaElto(int e){						
			if (pri==null) return false; // si la lista es vacia no hay nada que eliminar
			else { 
				// si es el primero puenteamos pri al siguiente
				if (e == pri.dato) { 
					pri = pri.sig; 
					return true; 
				} else { // eliminar otro distino al primero				
					// busqueda desde el ppio
					Nodo aux = pri;
					// recorremos lista buscando el ANTERIOR al que hay que eliminar (para poder luego enlazar)
					while (aux.sig != null && e!=aux.sig.dato) aux = aux.sig;
					// si lo encontramos
					if (aux.sig != null) { 
						aux.sig = aux.sig.sig; // puenteamos al siguiente
						return true;
					} else return false;
				}
			}
		}

		// devuelve el num de eltos de la lista
		public int NumElems(){
			int n=0;
			Nodo aux=pri;
			while (aux!=null) {
				aux = aux.sig;
				n++;
			}
			return n;
		}

		private Nodo nEsimoNode(int n) {

			if (n < 0) throw new Exception("Non-existent");

			else
			{
				Nodo aux = pri;
				while (aux != null && n > 0)
				{ 
					aux = aux.sig;

					n--;
				}

				if (aux == null) throw new Exception("Non-existent");
				else return aux;
			}
		}

		public int nEsimo(int n) {

			try
			{
				Nodo aux = nEsimoNode(n);
				if (aux == null) throw new Exception("Not Found");
				else return aux.dato;
			}
			catch { throw new Exception("Negative n not possible"); }
		}

		public void InsertaNEsimo(int pos, int e)
        {
			if (pos > 0) pri = new Nodo(e, pri);
            else
            {
                try
                {
					Nodo aux = nEsimoNode(pos - 1);
					if (aux == null) throw new Exception("No position in list");
                }
                catch { throw new Exception(); }
            }
        }

		public void BorraENesimo(int n)
        {
            try
            {
				Nodo aux = nEsimoNode(n - 1);
				aux.sig = aux.sig.sig;
            }
			catch { throw new Exception();}
        }
	}
}

