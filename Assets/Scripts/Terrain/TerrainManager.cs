using UnityEngine;

public enum Compass { North, South, East, West, None }

public class TerrainManager : MonoBehaviour
{
	public int ChunckSize = 7;
	public int InitialChunks = 4;
	public int TileSize = 1;
	public GameObject[] Streets, CityTerrain;

	private Vector3 TerrainPosition;
	private Vector3 IncreaseTileSizeX, IncreaseTileSizeZ;

	private Vector3 NorthLimit, SouthLimit, WestLimit, EastLimit;

	private void Start()
	{
		IncreaseTileSizeZ = new Vector3(0, 0, TileSize);
		IncreaseTileSizeX = new Vector3(TileSize, 0, 0);

		NorthLimit = SouthLimit = WestLimit = EastLimit = TerrainPosition = Vector3.zero;

		CreateNewChunck(TerrainPosition, Compass.None);

	}

	public void CreateNewChunck(Vector3 OriginalPosition, Compass comingFrom)
	{
		Vector3 auxOriginalPosition = OriginalPosition;

		for (int i = 0; i < ChunckSize; i++)
		{
			for (int j = 0; j < ChunckSize; j++)
			{
				Instantiate(CityTerrain[0], OriginalPosition, Quaternion.identity);
				OriginalPosition += IncreaseTileSizeX;
			}

			OriginalPosition -= IncreaseTileSizeX * ChunckSize;
			OriginalPosition += IncreaseTileSizeZ;

			NorthLimit = new Vector3(auxOriginalPosition.x + ((ChunckSize - 1) / 2), 0, auxOriginalPosition.z + ChunckSize - 1);
			SouthLimit = new Vector3(auxOriginalPosition.x + ((ChunckSize - 1) / 2), 0, auxOriginalPosition.z);

			EastLimit = new Vector3(auxOriginalPosition.x + ChunckSize - 1, 0, auxOriginalPosition.z + ((ChunckSize - 1) / 2));
			WestLimit = new Vector3(auxOriginalPosition.x, 0, auxOriginalPosition.z + ((ChunckSize - 1) / 2));

		}

		GameObject g;

		if (comingFrom != Compass.South)
		{
			g = Instantiate(Streets[0], SouthLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.South;
			g.name = "South";
		}

		if (comingFrom != Compass.North)
		{
			g = Instantiate(Streets[0], NorthLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.North;
			g.name = "North";
		}

		if (comingFrom != Compass.East)
		{
			g = Instantiate(Streets[0], EastLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.East;
			g.transform.Rotate(new Vector3(0, 90, 0));
			g.name = "East";
		}

		if (comingFrom != Compass.West)
		{
			g = Instantiate(Streets[0], WestLimit, Quaternion.identity);
			g.GetComponent<CreationCheckPoint>().mySign = Compass.West;
			g.transform.Rotate(new Vector3(0, 90, 0));
			g.name = "West";
		}
	}

	private void showTest()
	{
		
	}

}
