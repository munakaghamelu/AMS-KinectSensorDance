using UnityEngine;
//using Windows.Kinect;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text; 


/// <summary>
/// Pose model helper matches joints to model transforms.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PoseModelHelperClassic : PoseModelHelper
{

    // Public variables that will get matched to bones. If empty, the Kinect will simply not track it.
    public Transform HipCenter;
    public Transform Spine;
    public Transform ShoulderCenter;
    public Transform Neck;
//	public Transform Head;

    //public Transform ClavicleLeft;
    public Transform ShoulderLeft;
    public Transform ElbowLeft;
    public Transform HandLeft;
//    public Transform FingersLeft;
////	private Transform FingerTipsLeft = null;
//    public Transform ThumbLeft;

    //public Transform ClavicleRight;
    public Transform ShoulderRight;
    public Transform ElbowRight;
    public Transform HandRight;
//    public Transform FingersRight;
////	private Transform FingerTipsRight = null;
//    public Transform ThumbRight;

    public Transform HipLeft;
    public Transform KneeLeft;
    public Transform FootLeft;
//	private Transform ToesLeft = null;

    public Transform HipRight;
    public Transform KneeRight;
    public Transform FootRight;
//	private Transform ToesRight = null;


    // If the bones to be mapped have been declared, map that bone to the model.
    protected override void MapBones()
	{
        bones[0] = HipCenter;
        bones[1] = Spine;
        bones[2] = ShoulderCenter;
        bones[3] = Neck;
//		bones[4] = Head;

        bones[5] = ShoulderLeft;
        bones[6] = ElbowLeft;
        bones[7] = HandLeft;
//		bones[8] = FingersLeft;
//		bones[9] = FingerTipsLeft;
//		bones[10] = ThumbLeft;

        bones[11] = ShoulderRight;
        bones[12] = ElbowRight;
        bones[13] = HandRight;
//		bones[14] = FingersRight;
//		bones[15] = FingerTipsRight;
//		bones[16] = ThumbRight;

        bones[17] = HipLeft;
        bones[18] = KneeLeft;
        bones[19] = FootLeft;
//		bones[20] = ToesLeft;

        bones[21] = HipRight;
        bones[22] = KneeRight;
        bones[23] = FootRight;
//		bones[24] = ToesRight;

        //// special bones
        //bones[25] = ClavicleLeft;
        //bones[26] = ClavicleRight;

        //bones[27] = FingersLeft;
        //bones[28] = FingersRight;
        //bones[29] = ThumbLeft;
        //bones[30] = ThumbRight;
    }

}

