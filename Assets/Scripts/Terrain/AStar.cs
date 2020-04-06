using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Gemini;

// script que contiene el algoritmo principal de la practica
// calcula el camino por el cual se movera nuestro tanque basandose en 
// el algoritmo A estrella

public struct coords // lo uiliizaremos para facilitar la comprobacion de coordenadas
{
    public int x_, z_;

    public coords(int p1, int p2)
    {
        x_ = p1;
        z_ = p2;
    }
}

public struct dir // lo utilizaremos para facilitar la creacion y ejecucion de direcciones
{
    public int x_, z_;

    public dir(int p1, int p2)
    {
        x_ = p1;
        z_ = p2;
    }
}

public class AStar : MonoBehaviour
{
    GameObject GM; // necesitaremos acceder a la informacion del GameManager
    
    Node[,] nodeMatrix; // matriz de nodos que utilizaremos para ir creando el camino a seguir
    List<Node> RESULT; // aqui se almacenara la secuencia de nodos que debera seguir el tanque como resultado final

    // necesitaremos dos contenedores sobre los que nos apoyaremos en la ejecucion
    // y que serviran para controlar los nodos
    IndexedPriorityQueue<Node> abiertos; // aqui guardaremos los adyacentes a un nodo concreto y el nodo inicial
    List<Node> cerrados; // aqui guardaremos los nodos ya visitados y las rocas (imposible estar en ella)

	float timeTaken = 0f; // tiempo que ha tardado el algoritmo en finalizar el calculo

    bool FOUND; // flag, booleano de control, se activa si encontramos un camino
    int HEURISTIC = 0; // controla que heuristica se utilizara

    // lo primero que haremos sera inicializar y crear las variables necesarias
    private void Setup()
    {
        int W, H;
        W = 7;
        H = 7;

        nodeMatrix = new Node[W, H];
        RESULT = new List<Node>();

        FOUND = false;
    }

    // metodo al que llamaremos desde fuera para realizar el calculo
    // se encargara de empezar la logica y de llamar al metodo principal
    public bool Begin(int origX, int origZ, int destX, int destZ)
    {
        Setup();

        // inicializamos heuristica a utilizar
        HEURISTIC = 0;

        // inicializamos contenedores
        abiertos = new IndexedPriorityQueue<Node>(7 * 7);
        cerrados = new List<Node>();

        // iniciamos y creamos el nodo origen (los nodos se van creando en ejecucion)
        Node origen = new Node();
        origen.setPosition(origX, origZ);

        // metemos el nodo origen a la cola de prioridad
        abiertos.Insert(origX + origZ * 7, origen);

        // actualizamos matriz de nodos
        nodeMatrix[origen.i_, origen.j_] = origen;

        // llamamos al algoritmo principal
        Algorithm(destX, destZ, origX, origZ);

        return FOUND;
    }

    private void Algorithm(int destX, int destZ, int origX, int origZ)
    {
        Node minimo = null;

		//timeTaken = Time.time;

		DateTime d = DateTime.Now;
		timeTaken = d.Millisecond;

        // bucle principal a partir del cual se ejecutara la logica del algoritmo
		while (abiertos.Count > 0 && !FOUND)
        {
            // sacamos el nodo minimo de la cola
            minimo = abiertos.Top(); abiertos.Pop();

            // lo guardamos en la cola de cerrados, ya esta comprobado
            cerrados.Add(minimo);
            
            // este metodo devuelve las coordenadas de las casillas adyacentes a 
            // la que estamos comprobando (cada nodo tiene la informacion de en
            // que posicion esta su casilla)
            List<coords> adyacentes = calcAdyacentes(minimo.i_, minimo.j_);

            foreach (coords c in adyacentes)
            {
                // si el nodo a comprobar esta en cerrados no hacemos nada
                if (cerrados.Contains(nodeMatrix[c.x_, c.z_]))
                {
                    continue;
                }

                // si en esa posicion no hay un nodo asociado lo creamos,
                // le ponemos los valores en funcion de su coste y la heuristica 
                // a utilizar y le asignamos el padre
                if (nodeMatrix[c.x_, c.z_] == null)
                {
                    Node ady = new Node();
                    ady.setPosition(c.x_, c.z_);
                    ady.Parent_ = minimo;
                    ady.h_ = calculateH(HEURISTIC, ady.i_, ady.j_, origX, origZ);
                    ady.g_ = calculateG(ady);
                    ady.f_ = ady.h_ + ady.g_;

					abiertos.Insert(ady.i_ + 7 * ady.j_, ady);

                    // actualizamos matriz de nodos
                    nodeMatrix[ady.i_, ady.j_] = ady;
                }

                // si el nodo ya esta creado lo que haremos sera 
                // ver si el nuevo camino que va a el es mejor
                else
                {
                    Node ady = nodeMatrix[c.x_, c.z_];

                    int weight = 0;

                    int g = minimo.g_ + weight;
                    
                    // si el nuevo camino es mejor, actualizamos el nodo
                    if (g + ady.h_ < ady.f_)
                    {
                        ady.Parent_ = minimo;
                        ady.g_ = g;
                        ady.f_ = ady.g_ + ady.h_;
                    }
                }

            }

            // si las coordenadas asociadas al nodo coinciden con el destino,
            // hemos llegado, salimos del bucle
            if (minimo.i_ == destX && minimo.j_ == destZ)
            {
                FOUND = true;
            }
        }

        if (FOUND)
        {
            // para saber el tiempo que ha tardado en finalizar el algoritmo
			DateTime da = DateTime.Now;
			float auxTime = da.Millisecond - timeTaken;

            // ahora calculamos el camino a seguir recorriendo los nodos que nos han llevado a la solucion
			calculatePath(minimo, origX, origZ);
		}
        else
        {
			Debug.Log("No hay solución");
		}
    }

    // calcula las coordenadas de las casillas adyacentes de una concreta
    // tiene en cuenta no salirse del margen del mapa
    private List<coords> calcAdyacentes(int posX, int posZ)
    {
        dir up = new dir(0, 1);
        dir down = new dir(0, -1);
        dir left = new dir(-1, 0);
        dir right = new dir(1, 0);

        dir[] directions = new dir[4];
        directions[0] = up;
        directions[1] = down;
        directions[2] = left;
        directions[3] = right;

        List<coords> adyacentes = new List<coords>();

        for (int i = 0; i < 4; i++)
        {
            coords aux;
            int nX = posX + directions[i].x_;
            int nZ = posZ + directions[i].z_;

            if (nX >= 0 && nZ >= 0 && nX < 7 && nZ < 7)
            {
                aux = new coords(nX, nZ);
                adyacentes.Add(aux);
            }
        }

        return adyacentes;
    }

    // heuristica Manhattan, tiene en cuenta la distancia entre origen y destino
    // en nuestro caso es en casillas, restamos coordenadas
    public int calculateH_Manhattan(int destX, int destZ, int origX, int origZ)
    {
        return Math.Abs(destX - origX) + Math.Abs(destZ - origZ);
    }

    // heuristica Chebyshev, menos optima que la Manhattan pues esta pensada
    // para 8 direcciones pero igualmente es funcional
    public int calculateH_Chebyshev(int destX, int destZ, int origX, int origZ)
    {
        return Math.Max(Math.Abs(destX - origX), Math.Abs(destZ - origZ));
    }

    // heuristica Mala, un ejemplo de una heuristica menos eficaz
    // siempre devuelve el valor h = 0
    public int calculateH_Bad()
    {
        return 0;
    }

    // funcion auxiliar para llamar a alguna de las tres heuristicas
    public int calculateH(int num, int destX, int destZ, int origX, int origZ)
    {
        int h = 0;
        switch (num)
        {
            case 0:
                h = calculateH_Manhattan(destX, destZ, origX, origZ);
                break;
            case 1:
                h = calculateH_Chebyshev(destX, destZ, origX, origZ);
                break;
            case 2:
                h = calculateH_Bad();
                break;
            default:
                h = calculateH_Manhattan(destX, destZ, origX, origZ);
                break;
        }
        return h;
    }
    
    // funcion auxiliar que calcula el valor g de un nodo
    // g es igual a el peso de la casilla que tiene asociada el nodo
    // + el valor g del padre del nodo
    public int calculateG(Node n)
    {
        int weight = 0;

        int aux = weight + n.Parent_.g_;
        return aux;
    }

    // funcion que almacena en RESULT la lista de nodos que hay
    // que seguir para llegar a la solucion
    // para ello lo que hacemos es partir del nodo destino e ir 
    // recorriendo los siguientes a traves de los padres
    // luego invertimos este resultado para que la solucion 
    // vaya de origen a destino y no de destino a origen
    public void calculatePath(Node n, int origX, int origZ)
    {
        while (!(n.i_ == origX && n.j_ == origZ))
        {
            RESULT.Add(n);
            n = n.Parent_;
        }

        RESULT.Add(n);

        RESULT.Reverse();

	}

    // para acceder al resultado desde el manager
    public List<Node> getPath()
    {
        return RESULT;
    }
}
