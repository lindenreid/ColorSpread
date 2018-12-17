using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class ColorClick : MonoBehaviour {

	public PostProcessVolume volume;
	private ColorSpread colorSettings;
	private float distance = 50f;

	void Start () {
		PostProcessProfile profile = volume.sharedProfile;
		colorSettings = profile.GetSetting<ColorSpread>();
		// TODO: asserts
	}

	void Update () 
	{    
		if(Input.GetMouseButtonDown(0))
		{
			//create a ray cast and set it to the mouses cursor position in game
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, distance)) 
			{
				//Debug.Log(hit.point);
				colorSettings.clickLocation.Override( new Vector4Parameter { value = new Vector4(
					hit.point.x,
					hit.point.y,
					hit.point.z,
					0.0f
				)});
				colorSettings.startTime.Override( new FloatParameter{ value = Time.time} );				
			}    
		}    
	}

}
