using UnityEngine;
using System.Collections;

/// This controller handles most of the logic around how a user picks up and manipulates objects.
/// There should only be one in the scene, and it should be attached to the First Person Controller object
/// so that it can be queried by MouseLook.
/// 
/// Author: Jason Rickwald

public class ManipulateController : MonoBehaviour
{
	/// The farthest object that can be "reached" by the user.
	/// Objects that are closer to the user are given a better "weight".
	/// In other words, closer objects are more likely to be picked up when E is pressed.
	public float reachDistance = 5.0f;
	/// In normalized screen space (0,0) to (1,1), this number represents how far from the
	/// center of screen, (0.5, 0.5), an object can be in X to be picked up.
	/// Objects closer to the center of screen are given a better "weight".
	/// In other words, objects that aren't in the periphery are more likely to be picked up when E is pressed.
	public float selectionLowExtentX = 0.25f;
	/// In normalized screen space (0,0) to (1,1), this number represents how far from the
	/// center of screen, (0.5, 0.5), an object can be in Y to be picked up.
	/// Objects closer to the center of screen are given a better "weight".
	/// In other words, objects that aren't in the periphery are more likely to be picked up when E is pressed.
	public float selectionLowExtentY = 0.15f;

	private ManipulateObject manipulateObject;
	private GameObject mainCamera;

	private bool inspect;
	/// Used to check if the user is currently manipulating an object.
	public bool IsManipulating {
		get {
			return manipulateObject != null;
		}
	}

	// Initialization
	void Start()
	{
		manipulateObject = null;
		inspect = false;
		mainCamera = GameObject.FindWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update()
	{
		// First, reset the weight of all objects in the scene.
		// We need to do this to all objects, because it's possible for a fast moving
		// player to move away from an object and still leave it highlighted otherwise.
		// For this to work correctly, all manipulatable objects must be tagged with "Manipulate".
		GameObject[] allManips = GameObject.FindGameObjectsWithTag("Manipulate");
		foreach (GameObject man in allManips)
		{
			ManipulateObject mo = man.GetComponent<ManipulateObject>();
			if (mo != null)
				mo.HighlightWeight = -1.0f;
		}

		// In order to choose the best object to pick up, we're going to weight objects by their
		// proximity to the player and their relative positoin to the middle of the screen.
		Vector3 screenMiddle = new Vector3(0.5f, 0.5f, 0.0f);
		Collider[] inside = Physics.OverlapSphere(mainCamera.transform.position, (float) reachDistance);
		SortedList bestPick = new SortedList(inside.Length);

		if (manipulateObject == null)
		{
			foreach (Collider c in inside)
			{
				ManipulateObject mo = c.GetComponent<ManipulateObject> ();
				if (mo != null)
				{
					Vector3 viewPos = mainCamera.GetComponent<Camera>().WorldToViewportPoint(c.transform.position);
					viewPos.z = 0.0f;
					if (viewPos.x >= selectionLowExtentX && viewPos.x <= 1.0 - selectionLowExtentX && viewPos.y >= selectionLowExtentY && viewPos.y <= 1.0 - selectionLowExtentY)
					{
						Vector3 viewVec = viewPos - screenMiddle;
						viewVec.x /= (float) (0.5 - selectionLowExtentX);
						viewVec.y /= (float) (0.5 - selectionLowExtentY);
						Vector3 groundVec = c.transform.position - mainCamera.transform.position;
						groundVec.y = 0;
						float screenWeight = Mathf.Sqrt(viewVec.x * viewVec.x + viewVec.y * viewVec.y);
						float distWeight = groundVec.magnitude / reachDistance;
						mo.HighlightWeight = (1.0f - screenWeight) * (1.0f - distWeight);
						bestPick.Add(screenWeight * distWeight, mo);
					}
					else
					{
						mo.HighlightWeight = -1.0f;
					}
				}
			}
		}

		// We use the "E" key to pick up and put down objects.
		// I should probably be using Unity's support for setting up platform-agnostic input bindings,
		// but I got lazy.
		// Pressing E once picks up the object and holds it directly in front of the camera. Pressing
		// it again drops the object down a bit, making it easier to carry it around and still see
		// the world in front of you. Pressing it a final time "drops" the object.
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (manipulateObject == null)
			{
				if (bestPick.Count > 0)
				{
					manipulateObject = (ManipulateObject) bestPick.GetByIndex(0);
					manipulateObject.manipulate(true);
					inspect = true;

					foreach (Collider c in inside)
					{
						ManipulateObject mo = c.GetComponent<ManipulateObject> ();
						if (mo != null)
						{
							mo.HighlightWeight = -1.0f;
						}
					}
				}
			}
			else
			{
				if (inspect)
				{
					inspect = false;
				}
				else
				{
					manipulateObject.manipulate(false);
					manipulateObject = null;
				}
			}
		}

		// Finally, do the actual work of placing the object in front of the camera.
		if (manipulateObject != null)
		{
			Vector3 holdPosition = mainCamera.transform.position + (manipulateObject.viewDistance * mainCamera.transform.forward);
			if (!inspect)
				holdPosition += new Vector3(0.0f, -manipulateObject.dropDistance, 0.0f);
			manipulateObject.GetComponent<Rigidbody>().transform.position = Vector3.Lerp(manipulateObject.GetComponent<Rigidbody>().transform.position, holdPosition, Time.deltaTime * manipulateObject.smooth);
		}
	}

	/// Used by the added code in MouseLook to rotate the object.
	/// rotateX - angle to rotate around a camera-local "x axis" (cross product of up and camera forward)
	/// rotateY - angle to rotate around a camera-local "y axis" (cross product of local x axis and camera forward).
	public void RotateManipulated(float rotateX, float rotateY)
	{
		if (manipulateObject != null)
		{
			Vector3 camLocalX = Vector3.Cross(mainCamera.transform.forward, Vector3.up).normalized;
			Vector3 camLocalY = Vector3.Cross(camLocalX, mainCamera.transform.forward).normalized;
			manipulateObject.GetComponent<Rigidbody>().transform.Rotate(camLocalX, rotateX, Space.World);
			manipulateObject.GetComponent<Rigidbody>().transform.Rotate(camLocalY, rotateY, Space.World);
		}
	}
}
