using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeFaceController : MonoBehaviour 
{
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;

	[Tooltip("Transform of the joint, used to move and rotate the head.")]
	public Transform headTransform;

	[Tooltip("Whether the model's head is facing the player or not.")]
	public bool mirroredHeadMovement = true;

	[Tooltip("Camera used to estimate the overlay position of the head over the background.")]
	public Camera foregroundCamera;

	[Tooltip("Vertical offset of the model above the head position.")]
	public float verticalOffset = 0f;

	[Tooltip("Scale factor for the head model.")]
	[Range(0.1f, 2.0f)]
	public float modelScaleFactor = 1f;

	[Tooltip("Smooth factor used for head movement.")]
	public float moveSmoothFactor = 10f;

	[Tooltip("Skinned mesh renderer, containing the blend shapes.")]
	public SkinnedMeshRenderer skinnedMeshRenderer;

	[Tooltip("List of the tracked face animations.")]
	public List<KinectInterop.FaceShapeAnimations> faceAnimUnits = new List<KinectInterop.FaceShapeAnimations>();

	[Tooltip("List of the respective blend shape names. Must match the FaceAnimUnits-list.")]
	public List<string> faceBlendShapes = new List<string>();

	[Tooltip("Multiplier used for converting the face animations to blend shapes.")]
	public float valueMultiplier = 100f;

	[Tooltip("Smooth factor used for animation blending.")]
	public float blendSmoothFactor = 10f;


	// shared mesh reference
	private Mesh skinnedMesh;

	// previous blend shape values
	private float[] blendShapeValues = null;

	private KinectManager kinectManager;
	private FacetrackingManager faceManager;
	private Dictionary<KinectInterop.FaceShapeAnimations, float> dictAnimUnits = new Dictionary<KinectInterop.FaceShapeAnimations, float>();

	// initial head transform position & rotation
	private Vector3 headInitialPosition;
	private Quaternion headInitialRotation;


	void Awake()
	{
		if (skinnedMeshRenderer == null) 
		{
			skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
		}

		if (skinnedMeshRenderer != null) 
		{
			skinnedMesh = skinnedMeshRenderer.sharedMesh;
		}

		if(headTransform != null)
		{
			headInitialPosition = headTransform.position;
			headInitialRotation = headTransform.rotation;
		}
	}


	void Start () 
	{
		// reference to KinectManager
		kinectManager = KinectManager.Instance;

		// check if the lists match in size
		if (faceAnimUnits.Count != faceBlendShapes.Count) 
		{
			Debug.LogError("The FaceAnimUnits- and FaceBlendShapes-lists must have the same size!");
		}

		if (skinnedMesh != null) 
		{
//			// print out the found blend shape names
//			for (int i = 0; i < skinnedMesh.blendShapeCount; i++) 
//			{
//				Debug.Log(i + " - " + skinnedMesh.GetBlendShapeName(i));
//			}
		}
	}
	
	void Update () 
	{
		if (skinnedMeshRenderer == null)
			return;
		if (faceAnimUnits.Count != faceBlendShapes.Count)
			return;

		if (blendShapeValues == null || blendShapeValues.Length != faceAnimUnits.Count) 
		{
			blendShapeValues = new float[faceAnimUnits.Count];
		}
		
		// reference to face manager
		if (!faceManager) 
		{
			faceManager = FacetrackingManager.Instance;
		}

		if (kinectManager && kinectManager.IsInitialized() && faceManager && faceManager.IsFaceTrackingInitialized()) 
		{
			// check for tracked user
			long userId = kinectManager.GetUserIdByIndex(playerIndex);

			if (userId != 0 && kinectManager.IsUserTracked(userId)) 
			{
				// move the model transform according to user's head
				if (headTransform != null) 
				{
					MoveHeadTransform(userId);
				}

				// animate blend shapes
				if (faceManager.GetUserAnimUnits(userId, ref dictAnimUnits)) 
				{
					for (int i = 0; i < faceAnimUnits.Count; i++) 
					{
						KinectInterop.FaceShapeAnimations faceAnim = faceAnimUnits[i];

						if(dictAnimUnits.ContainsKey(faceAnim))
						{
							float animValue = dictAnimUnits[faceAnim];
							string blendShapeName = faceBlendShapes[i];

							int blendShapeIndex = !string.IsNullOrEmpty(blendShapeName) ? skinnedMesh.GetBlendShapeIndex(blendShapeName) : -1;
							if (blendShapeIndex >= 0) 
							{
								if (animValue < 0f) 
								{
									//Debug.Log(faceAnim + ": " + animValue);
									animValue = -animValue;
								}
								
								// lerp to the new value
								blendShapeValues[i] = Mathf.Lerp(blendShapeValues[i], animValue * valueMultiplier, blendSmoothFactor * Time.deltaTime);
								skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, blendShapeValues[i]);
							}
							else
							{
								Debug.LogError("Blend shape '" + blendShapeName + "' not found!");
							}
						}
					}
				}
			}
		}

	}


	// moves the transform according to the user's head position & rotation
	private void MoveHeadTransform(long userId)
	{
		// head position
		Vector3 newPosition = faceManager.GetHeadPosition(userId, mirroredHeadMovement);

		// head rotation
		Quaternion newRotation = headInitialRotation * faceManager.GetHeadRotation(userId, mirroredHeadMovement);

		// rotational fix, provided by Richard Borys:
		// The added rotation fixes rotational error that occurs when person is not centered in the middle of the kinect
		Vector3 addedRotation = newPosition.z != 0f ? new Vector3(Mathf.Rad2Deg * (Mathf.Tan(newPosition.y) / newPosition.z),
			Mathf.Rad2Deg * (Mathf.Tan(newPosition.x) / newPosition.z), 0) : Vector3.zero;

		addedRotation.x = newRotation.eulerAngles.x + addedRotation.x;
		addedRotation.y = newRotation.eulerAngles.y + addedRotation.y;
		addedRotation.z = newRotation.eulerAngles.z + addedRotation.z;

		newRotation = Quaternion.Euler(addedRotation.x, addedRotation.y, addedRotation.z);
		// end of rotational fix

		if(moveSmoothFactor != 0f)
			headTransform.rotation = Quaternion.Slerp(headTransform.rotation, newRotation, moveSmoothFactor * Time.deltaTime);
		else
			headTransform.rotation = newRotation;

		// check for head pos overlay
		if(foregroundCamera)
		{
			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;

			if(portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			KinectManager kinectManager = KinectManager.Instance;

			if(kinectManager)
			{
				Vector3 posColorOverlay = kinectManager.GetJointPosColorOverlay(userId, (int)KinectInterop.JointType.Head, foregroundCamera, backgroundRect);

				if(posColorOverlay != Vector3.zero)
				{
					newPosition = posColorOverlay;
				}
			}
		}
		else
		{
			// move around the initial position
			newPosition += headInitialPosition;
		}

		// vertical offet
		if(verticalOffset != 0f)
		{
			// add the vertical offset
			Vector3 dirHead = new Vector3(0, verticalOffset, 0);
			dirHead = headTransform.InverseTransformDirection(dirHead);
			newPosition += dirHead;
		}

		// set the position
//		if(moveSmoothFactor != 0f)
//			headTransform.position = Vector3.Lerp(headTransform.position, newPosition, moveSmoothFactor * Time.deltaTime);
//		else
			headTransform.position = newPosition;

		// scale factor
		if(headTransform.localScale.x != modelScaleFactor)
		{
			headTransform.localScale = new Vector3(modelScaleFactor, modelScaleFactor, modelScaleFactor);
		}
	}


}
