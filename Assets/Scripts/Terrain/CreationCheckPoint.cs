using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationCheckPoint : MonoBehaviour
{
	public Compass mySign;
	private Compass myOppositeSign;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Car"))
		{
			TerrainManager TM = FindObjectOfType<TerrainManager>();

			Vector3 newPosition = Vector3.zero;

			if (mySign == Compass.North)
			{
				newPosition = new Vector3(transform.position.x - (TM.ChunckSize / 2), 0, transform.position.z + 1);
				myOppositeSign = Compass.South;
			}

			else if (mySign == Compass.South)
			{
				newPosition = new Vector3(transform.position.x - (TM.ChunckSize / 2), 0, transform.position.z - (TM.ChunckSize));
				myOppositeSign = Compass.North;
			}

			else if (mySign == Compass.East)
			{
				newPosition = new Vector3(transform.position.x  + 1, 0, transform.position.z - (TM.ChunckSize / 2));
				myOppositeSign = Compass.West;
			}

			else if (mySign == Compass.West)
			{
				newPosition = new Vector3(transform.position.x - (TM.ChunckSize), 0, transform.position.z - (TM.ChunckSize / 2));
				myOppositeSign = Compass.East;
			}


			TM.CreateNewChunck(newPosition, myOppositeSign);

			Destroy(gameObject);
		}

		else if (other.CompareTag("CreationCheckPoint"))
		{
			Destroy(gameObject);
		}
	}
}
