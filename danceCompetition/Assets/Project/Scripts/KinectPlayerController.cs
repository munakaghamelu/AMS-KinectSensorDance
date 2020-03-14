using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KinectPlayerController : MonoBehaviour 
{
	private SpeechManager speechManager;
	private KinectRecorderPlayer saverPlayer;


	void Start()
	{
		saverPlayer = KinectRecorderPlayer.Instance;
	}

	void Update () 
	{
		if(speechManager == null)
		{
			speechManager = SpeechManager.Instance;
		}
		
		if(speechManager != null && speechManager.IsSapiInitialized())
		{
			if(speechManager.IsPhraseRecognized())
			{
				string sPhraseTag = speechManager.GetPhraseTagRecognized();
				
				switch(sPhraseTag)
				{
						
					case "PLAY":
						if(saverPlayer)
						{
							saverPlayer.StartPlaying();
						}
						break;

					case "STOP":
						if(saverPlayer)
						{
							saverPlayer.StopRecordingOrPlaying();
						}
						break;

				}
				
				speechManager.ClearPhraseRecognized();
			}
		}


		if(Input.GetButtonDown("Jump"))  // start or stop playing
		{
            startPlaying();
		}


	}

    public void startPlaying()
    {
        if (saverPlayer)
        {
            if (!saverPlayer.IsPlaying())
            {
                saverPlayer.StartPlaying();
            }
        }
    }

}
