using UnityEngine;
using System.Collections;

public class DrawGizmo : MonoBehaviour
{
	public Color color = Color.blue;

	void OnDrawGizmos() 
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(transform.position, 1);
	}
}

