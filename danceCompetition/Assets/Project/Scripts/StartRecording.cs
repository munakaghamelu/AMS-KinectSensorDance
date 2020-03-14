using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class StartRecording : MonoBehaviour
{
    private SpeechManager speechManager;
    private KinectRecorderPlayer saverPlayer;

    /*
    void Update()
    {
        if (speechManager == null)
        {
            speechManager = SpeechManager.Instance;
        }

        if (speechManager != null && speechManager.IsSapiInitialized())
        {
            if (speechManager.IsPhraseRecognized())
            {
                string sPhraseTag = speechManager.GetPhraseTagRecognized();

                switch (sPhraseTag)
                {
                    case "RECORD":
                        if (saverPlayer)
                        {
                            saverPlayer.StartRecording();
                        }
                        break;

                    case "STOP":
                        if (saverPlayer)
                        {
                            saverPlayer.StopRecordingOrPlaying();
                        }
                        break;

                }

                speechManager.ClearPhraseRecognized();
            }
        }

        // alternatively, use the keyboard
        /* if (Input.GetButtonDown("Jump"))  // start or stop recording
         {
             startRecord();
         } 

    } */

    public void startRecord()
    {
        saverPlayer = KinectRecorderPlayer.Instance;
        if (saverPlayer)
        {
           saverPlayer.StartRecording();
        }
    }

    public void stopRecord()
    {
        saverPlayer = KinectRecorderPlayer.Instance;
        if (saverPlayer)
        {
            saverPlayer.StopRecordingOrPlaying();
        }

    }

}
