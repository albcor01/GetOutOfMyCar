using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Compass { North, South, East, West, None }

public class TerrainManager : MonoBehaviour
{
	public int ChunckSize = 7;
	public int InitialChunks = 4;
	public int TileSize = 1;
	public GameObject[] Streets, CityTerrain;
	public GameObject TerrainCheckPoint, Car;

	private Vector3 TerrainPosition;
	private Vector3 IncreaseTileSizeX, IncreaseTileSizeZ;

	private Vector3 NorthLimit, SouthLimit, WestLimit, EastLimit;

	public GameObject cube;

	private void Start()
	{
		IncreaseTileSizeZ = new Vector3(0, 0, TileSize);
		IncreaseTileSizeX = new Vector3(TileSize, 0, 0);

		NorthLimit = SouthLimit = WestLimit = EastLimit = TerrainPosition = Vector3.zero;

		CreateNewChunck(TerrainPosition, Compass.None);
	}

	private void createStreets(Compass comingFrom, ref bool[,] matrix)
	{

		int RandomXStreet, RandomYStreet;

		int rnd = Random.Range(2, 6);

		int[,] values = new int[rnd + 4, 2];

		for (int i = 0; i < rnd; i++)
		{
			RandomXStreet = Random.Range(1, ChunckSize - 1);
			RandomYStreet = Random.Range(1, ChunckSize - 1);

			matrix[RandomXStreet, RandomYStreet] = true;
			values[i, 0] = RandomXStreet; values[i, 1] = RandomYStreet;

		}

		//Bordes para conectar con otras zonas

		int rand = Random.Range(0, ChunckSize);
		matrix[0, rand] = true;
		values[rnd, 0] = 0; values[rnd, 1] = rand;

		rand = Random.Range(0, ChunckSize);
		matrix[ChunckSize - 1, rand] = true;
		values[rnd + 1, 0] = ChunckSize - 1; values[rnd + 1, 1] = rand;

		rand = Random.Range(0, ChunckSize);
		matrix[rand, 0] = true;
		values[rnd + 2, 0] = rand; values[rnd + 2, 1] = 0;

		rand = Random.Range(0, ChunckSize);
		matrix[rand, ChunckSize - 1] = true;
		values[rnd + 3, 0] = rand; values[rnd + 3, 1] = ChunckSize - 1;


		AStar algorithm = new AStar();

		for (int i = 0; i < rnd + 4; i++)
		{
			float closestOne = float.PositiveInfinity, secondClosestOne = float.PositiveInfinity;
			int closestOneIndex = 0, secondClosestOneIndex = 0;

			for (int j = 0; j < rnd + 4; j++)
			{
				float distance = Vector2.Distance(new Vector2(values[i, 0], values[i, 1]), new Vector2(values[j, 0], values[j, 1]));

				if (i != j && distance < closestOne)
				{
					closestOne = distance;
					closestOneIndex = j;
				}

				if(i != j && distance > secondClosestOne)
				{
					secondClosestOne = distance;
					secondClosestOneIndex = j;

				}

			}

			algorithm.Begin(values[i, 0], values[i, 1], values[closestOneIndex, 0], values[closestOneIndex, 1]);
			List<Node> path = algorithm.getPath();

			foreach (Node n in path)
			{
				if (numberOfAdyacentStreets(matrix, n.i_, n.j_) < 2)
				{
					matrix[n.i_, n.j_] = true;
				}
			}

			algorithm.Begin(values[i, 0], values[i, 1], values[secondClosestOneIndex, 0], values[secondClosestOneIndex, 1]);
			path = algorithm.getPath();

			foreach (Node n in path)
			{
				if (numberOfAdyacentStreets(matrix, n.i_, n.j_) < 2)
				{
					matrix[n.i_, n.j_] = true;
				}
			}
		}
	}


	private int numberOfAdyacentStreets(bool[,] matrix, int i, int j)
	{
		int cont = 0;

		if (i + 1 < ChunckSize && matrix[i + 1, j])
			cont++;
		else if (i - 1 >= 0 && matrix[i - 1, j])
			cont++;
		else if (j + 1 < ChunckSize && matrix[i, j + 1])
			cont++;
		else if (j - 1 >= 0 && matrix[i, j - 1])
			cont++;

		return cont;
	}

	public void CreateNewChunck(Vector3 OriginalPosition, Compass comingFrom)
	{
		Vector3 auxOriginalPosition = OriginalPosition;
		bool[,] matrix = new bool[ChunckSize, ChunckSize];

		RaycastScenary(OriginalPosition, ref matrix);

		createStreets(comingFrom, ref matrix);

		for (int i = 0; i < ChunckSize; i++)
		{
			for (int j = 0; j < ChunckSize; j++)
			{
				if (matrix[i, j])
				{
					GameObject ga = Instantiate(Streets[0], OriginalPosition, Quaternion.identity);

					if (comingFrom == Compass.None)
					{
						Car.transform.position = new Vector3(ga.transform.position.x, 2, ga.transform.position.z);
					}
				}

				else
					Instantiate(CityTerrain[Random.Range(0, CityTerrain.Length)], OriginalPosition, Quaternion.identity);

				OriginalPosition += IncreaseTileSizeX;
			}

			OriginalPosition -= IncreaseTileSizeX * ChunckSize;
			OriginalPosition += IncreaseTileSizeZ;

			NorthLimit = new Vector3(auxOriginalPosition.x + ((ChunckSize - 1) / 2) * TileSize, 0, auxOriginalPosition.z + (TileSize * ChunckSize - TileSize / 2));
			SouthLimit = new Vector3(auxOriginalPosition.x + ((ChunckSize - 1) / 2) * TileSize, 0, auxOriginalPosition.z - TileSize / 2);

			EastLimit = new Vector3(auxOriginalPosition.x + (TileSize * ChunckSize - TileSize / 2), 0, auxOriginalPosition.z + ((ChunckSize - 1) / 2) * TileSize);
			WestLimit = new Vector3(auxOriginalPosition.x - TileSize / 2, 0, auxOriginalPosition.z + ((ChunckSize - 1) / 2) * TileSize);

		}

		GameObject g;

		if (comingFrom != Compass.South)
		{
			g = Instantiate(TerrainCheckPoint, SouthLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.South;
			g.name = "South";
		}

		if (comingFrom != Compass.North)
		{
			g = Instantiate(TerrainCheckPoint, NorthLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.North;
			g.name = "North";
		}

		if (comingFrom != Compass.East)
		{
			g = Instantiate(TerrainCheckPoint, EastLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.East;
			g.transform.Rotate(new Vector3(0, 90, 0));
			g.name = "East";
		}

		if (comingFrom != Compass.West)
		{
			g = Instantiate(TerrainCheckPoint, WestLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.West;
			g.transform.Rotate(new Vector3(0, 90, 0));
			g.name = "West";
		}
	}

	private void RaycastScenary(Vector3 OriginalPosition, ref bool[,] matrix)
	{
		//SUR
		for (int i = 0; i < ChunckSize; i++)
		{
			RayPosition(OriginalPosition + new Vector3(i * TileSize, 0, -TileSize), ref matrix, 0, i);
		}

		//NORTE
		for (int i = 0; i < ChunckSize; i++)
		{
			RayPosition(OriginalPosition + new Vector3(i * TileSize, 0, (ChunckSize * TileSize)), ref matrix, ChunckSize - 1, i);
		}

		//ESTE
		for (int i = 0; i < ChunckSize; i++)
		{
			RayPosition(OriginalPosition + new Vector3(-TileSize, 0, i * TileSize), ref matrix, i, 0);
		}

		//OESTE
		for (int i = 0; i < ChunckSize; i++)
		{
			RayPosition(OriginalPosition + new Vector3((ChunckSize * TileSize), 0, i * TileSize), ref matrix, i, ChunckSize - 1);
		}


	}

	private void RayPosition(Vector3 OriginalPosition, ref bool[,] matrix, int i, int j)
	{

		RaycastHit hit;

		GameObject g = Instantiate(cube, OriginalPosition + new Vector3(0, 0, 0), Quaternion.identity);

		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(OriginalPosition + new Vector3(0, 10, 0), new Vector3(0, -20, 0), out hit, Mathf.Infinity))
		{
			if (hit.collider.gameObject.CompareTag("Street"))
			{
				matrix[i, j] = true;
			}
		}

		Destroy(g);

	}

}
