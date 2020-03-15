using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class SkeletonCollider : MonoBehaviour 
{
//	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
//	public GUITexture backgroundImage;

	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Game object used to represent the body joints.")]
	public GameObject jointPrefab;

	[Tooltip("Line object used to represent the bones between joints.")]
	//public LineRenderer linePrefab;
	public GameObject linePrefab;

	[Tooltip("Camera that will be used to represent the Kinect-sensor's point of view in the scene.")]
	public Camera kinectCamera;

	[Tooltip("Body scale factors in X,Y,Z directions.")]
	public Vector3 scaleFactors = Vector3.one;


	//public UnityEngine.UI.Text debugText;
	
	private GameObject[] joints = null;
	//private LineRenderer[] lines = null;
	private GameObject[] lines = null;

	//private Quaternion initialRotation = Quaternion.identity;


	void Start()
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			int jointsCount = manager.GetJointCount();

			if(jointPrefab)
			{
				// array holding the skeleton joints
				joints = new GameObject[jointsCount];
				
				for(int i = 0; i < joints.Length; i++)
				{
					joints[i] = Instantiate(jointPrefab) as GameObject;
					joints[i].transform.parent = transform;
					joints[i].name = ((KinectInterop.JointType)i).ToString();
					joints[i].SetActive(false);
				}
			}

			// array holding the skeleton lines
			//lines = new LineRenderer[jointsCount];
			lines = new GameObject[jointsCount];
		}

		// always mirrored
		//initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
	}
	
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized())
		{
//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
//			if(backgroundImage && (backgroundImage.texture == null))
//			{
//				backgroundImage.texture = manager.GetUsersClrTex();
//			}

			// overlay all joints in the skeleton
			if(manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for(int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if(manager.IsJointTracked(userId, joint))
					{
						Vector3 posJoint = !kinectCamera ? manager.GetJointPosition(userId, joint) : manager.GetJointKinectPosition(userId, joint);
						posJoint = new Vector3(posJoint.x * scaleFactors.x, posJoint.y * scaleFactors.y, posJoint.z * scaleFactors.z);

						if (kinectCamera) 
						{
							posJoint = kinectCamera.transform.TransformPoint (posJoint);
						}

						if(joints != null)
						{
							// overlay the joint
							if(posJoint != Vector3.zero)
							{
								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

//								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
//								rotJoint = initialRotation * rotJoint;
//								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}

						if(lines[i] == null && linePrefab != null)
						{
							lines[i] = Instantiate(linePrefab);  // as LineRenderer;
							lines[i].transform.parent = transform;
							lines[i].name = ((KinectInterop.JointType)i).ToString() + "_Line";
							lines[i].gameObject.SetActive(false);
						}

						if(lines[i] != null)
						{
							// overlay the line to the parent joint
							int jointParent = (int)manager.GetParentJoint((KinectInterop.JointType)joint);
							Vector3 posParent = Vector3.zero;

							if (manager.IsJointTracked(userId, jointParent)) 
							{
								posParent = !kinectCamera ? manager.GetJointPosition(userId, jointParent) : manager.GetJointKinectPosition(userId, jointParent);
								posJoint = new Vector3(posJoint.x * scaleFactors.x, posJoint.y * scaleFactors.y, posJoint.z * scaleFactors.z);

								if (kinectCamera) 
								{
									posParent = kinectCamera.transform.TransformPoint(posParent);
								}
							}

							if(posJoint != Vector3.zero && posParent != Vector3.zero)
							{
								lines[i].gameObject.SetActive(true);
								
//								//lines[i].SetVertexCount(2);
//								lines[i].SetPosition(0, posParent);
//								lines[i].SetPosition(1, posJoint);

								Vector3 dirFromParent = posJoint - posParent;

								lines [i].transform.localPosition = posParent + dirFromParent / 2f;
								lines [i].transform.up = transform.rotation * dirFromParent.normalized;

								Vector3 lineScale = lines [i].transform.localScale;
								lines [i].transform.localScale = new Vector3 (lineScale.x, dirFromParent.magnitude / 2f, lineScale.z);
							}
							else
							{
								lines[i].gameObject.SetActive(false);
							}
						}
						
					}
					else
					{
						if(joints != null)
						{
							joints[i].SetActive(false);
						}
						
						if(lines[i] != null)
						{
							lines[i].gameObject.SetActive(false);
						}
					}
				}

			}
		}
	}

}
