using UnityEngine;
using System.Collections;

/// This is attached to any object that can be manipulated by the user.
/// There are three other conditions that must be satisfied for this to all work correctly:
/// 1. The object this is attached to must have a RigidBody.  This body is set to kinematic
///    when the user is manipulating an object.
/// 2. The object this is attached to must be tagged in Unity with "Manipulate".  This
///    allows the ManipulateController to update it's associated highlight weight even
///    when it is not within the "reach" of the player.
/// 3. The Material/Shader applied to this object should be the Custom/Outline shader, or
///    at least support the "_OutlineColor" and "_OutlineWidth" attributes.
/// 
/// Author: Jason Rickwald

public class ManipulateObject : MonoBehaviour
{
	/// How far away from the camera to "hold" the object when viewing
	public float viewDistance = 1.25f;
	/// How far to drop the object down when not "inspecting" it
	public float dropDistance = 0.3f;
	/// Parameter for smoothing the movement of the object when "held"
	public float smooth = 5.0f;

	private float hweight;
	/// Getter and setter for the "weight" of the highlight.  This controls its thickness and color.
	public float HighlightWeight {
		get {
			return hweight;
		}
		set {
			hweight = value;
		}
	}

	// Initialization
	void Start()
	{
		hweight = -1.0f;
	}

	/// Turn manipulation on or off for this object.
	/// When true, the object's RigidBody is set to kinematic and collisions are turned off.
	/// When false, the object's RigidBody is no longer kinematic, and collision detection is enabled.
	public void manipulate(bool manip)
	{
		if (manip)
		{
			this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
		}
		else
		{
			this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			this.gameObject.GetComponent<Rigidbody>().detectCollisions = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (hweight > 0.1f)
		{
			this.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color (0.0f, hweight, hweight, hweight));
			this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", hweight * 0.005f);
		}
		else
		{
			this.GetComponent<Renderer>().material.SetColor("_OutlineColor", new Color (0.0f, 0.0f, 0.0f, 0.0f));
			this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.0f);
		}
	}
}
