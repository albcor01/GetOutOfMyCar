using System;

// utilizaremos esta clase para poder ayudarnos de ella en la implementacion de AStar
// en AStar se utilizara una matriz de nodos para efectuar los calculos
// a cada casilla se le ira asociando un nodo mientras se esta efectuando el calculo del camino

public class Node : IComparable<Node>
{
    public int h_ = 0; //heuristica
    public int g_ = 0; //coste de la casilla (peso)
    public int f_ = 0; //coste + heuristica

    public int i_ = 0; //fila
    public int j_ = 0; //columna
    
    public Node Parent_ = null; //nodo padre, para saber de donde se viene posteriormente

    // metodo de IComparable que, cuando compara dos nodos
    // devuelve el que tenga mejor valor (f menor)
    public int CompareTo(Node other)
    {
        return f_.CompareTo(other.f_);
    }

    public void setPosition(int x, int z)
    {
        i_ = x;
        j_ = z;
    }

	public void setAttributes(int h, int g, int i, int j, Node p)
	{
        h_ = h;
		g_ = g;
        f_ = g_ + h_;

		i_ = i;
		j_ = j;

		Parent_ = p;
	}
}
