using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShakeWave
{

	public class demo : MonoBehaviour
	{

		public List<GameObject> mWaveList = new List<GameObject> ();
		public UnityEngine.UI.Text mLable;
	
		public int mColorIndex = 0;
		public void OnClickColor( int colorIndex )
		{
			mColorIndex = colorIndex;
		}

		public int mWaveIndex = 0;
		public void OnClickNext()
		{
			mWaveIndex++;
			if ( mWaveIndex > 19 )
				mWaveIndex = 0;

			mLable.text = (mWaveIndex+1).ToString() + "/20";
		}

		public void OnClickPre()
		{
			mWaveIndex--;
			if ( mWaveIndex < 0 )
				mWaveIndex = 19;

			mLable.text = (mWaveIndex+1).ToString() + "/20";
		}

		// Update is called once per frame
		void Update ()
		{
	
			if ( Input.GetMouseButtonDown (0) )
			{
				Ray mRay=Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit mHit;
				if ( Physics.Raycast (mRay, out mHit) )
				{
					playWave (mHit.point);
				}
			}
	
			if ( Input.GetKeyDown (KeyCode.A) )
				OnClickPre ();
			if ( Input.GetKeyDown (KeyCode.D) )
				OnClickNext ();

			if ( Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
				OnClickColor (0);
			if ( Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2) )
				OnClickColor (1);
			if ( Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3) )
				OnClickColor (2);
			if ( Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4) )
				OnClickColor (3);
			if ( Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5) )
				OnClickColor (4);
			if ( Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6) )
				OnClickColor (5);

		}


		public List<Color> mColorList = new List<Color> ();
		void playWave( Vector3 pos )
		{
			Vector3 playPos = pos;

			GameObject wave = Instantiate (mWaveList [mWaveIndex]) as GameObject;
			wave.transform.position = playPos;
			wave.SetActive (true);

			ParticleSystem ps = wave.GetComponent<ParticleSystem> ();
			ParticleSystem.MainModule mm = ps.main;
			ParticleSystem.MinMaxGradient color = ps.main.startColor;
			mm.startColor = mColorList[mColorIndex];
		}

	}

}