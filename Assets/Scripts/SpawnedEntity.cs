using System;
using UnityEngine;

public class SpawnedEntity : MonoBehaviour
{
	private Action miAction;

	public void EjecutarAlDestruirse(Action miActionCallback)
	{
        miAction = miActionCallback;
	}

	private void OnDestroy()
	{
		// Ejecutar la accion enviada como callback
        miAction?.Invoke();
	}
}