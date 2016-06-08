using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System.IO.Ports;

public class ControllerButton {

	private List<int> stateLog = new List<int>();
	private int currentState = 0;
	public ArduinoManager.CustomGamepadButton buttonName;

	public List<int> StateLog {
		get {
			return stateLog;
		}
		set {
			stateLog = value;
		}
	}

	public int CurrentState {
		get {
			return currentState;
		}
		set {
			currentState = value;
		}
	}

	public ControllerButton(ArduinoManager.CustomGamepadButton buttonName)
	{
		this.buttonName = buttonName;
	}

	void Start () {

	}

	public void Update () {

		if(stateLog.Count > 10)
		{
			if(CheckPressed())
			{
				OnButtonPressed();
			}
			else if(CheckHeld())
			{
				OnButtonHeld();
			}

			if (stateLog [0] == 1 && stateLog [1] == 0)
			{
				OnButtonDown();
			}

			if (stateLog [0] == 0 && stateLog [1] == 1)
			{
				OnButtonUp();
			}
		}

	}

	private bool CheckPressed()
	{
		if (stateLog [0] == 0 && stateLog [1] == 1)
		{
			for(int i=2; i<=7; i++)
			{
				if(stateLog [i] == 0)
				{
					return true;
				}
			}
		}

		return false;
	}

	private bool CheckHeld()
	{
		for(int i=0; i<8; i++)
		{
			if(stateLog [i] == 0)
			{
				return false;
			}
		}
		return true;
	}

	public void OnButtonDown()
	{
		//Debug.Log ("ON BUTTON DOWN");
	}

	public void OnButtonUp()
	{
		//Debug.Log ("ON BUTTON UP");
	}

	public void OnButtonPressed()
	{
		//Debug.Log ("ON BUTTON PRESSED");
	}

	public void OnButtonHeld()
	{
		//Debug.Log ("ON BUTTON HELD");
	}
}

public class ArduinoManager : MonoBehaviour {

	public static string serialName = "COM5";
	public SerialPort mySPort = new SerialPort(serialName, 9600);

	OneEuroFilter floatFilter;
	public float filterFrequency = 10.0f;

	public bool lerpMode = false;
	private float[] previousBendValues = new float[2];
	
	private bool calibrateComplete = false;
	private float bend1Av = 0f;
	private float bend2Av = 0f;
	private int currentCalibrationFrame = 0;
	private const int calibrationFrames = 10;

	public float rtValue = 0f;
	public float ltValue = 0f;
	public float stickValue = 0f;
	public float previousStickValue = 0f;

	private float maxBendValue1 = 960f;
	private float minBendValue1 = 340f;
	private float maxBendValue2 = 800f;
	private float minBendValue2 = 300f;

	private float sensitivityConstant = 100f;

	public enum CustomGamepadButton
	{
		UP,
		DOWN,
		LEFT,
		RIGHT,
		A,
		B
	}

	private List<ControllerButton> gamepadButtons = new List<ControllerButton>();

	void Start()
	{
		mySPort.Open();

		// initialize the controller dictionary
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.A));
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.B));
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.UP));
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.DOWN));
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.LEFT));
		gamepadButtons.Add (new ControllerButton(CustomGamepadButton.RIGHT));

		floatFilter = new OneEuroFilter(filterFrequency);
	}

	void Update()
	{
		if (mySPort.ReadLine () != null) {
			if (Input.GetKeyDown (KeyCode.C)) {
				if (calibrateComplete) {
					calibrateComplete = false;
					bend1Av = 0;
					bend2Av = 0;
					currentCalibrationFrame = 0;  
				}
			}

			string serialValue = mySPort.ReadLine ();
			mySPort.ReadTimeout = 25;
			string[] serialValues = serialValue.Split ('&');

			for (int i = 0; i < (gamepadButtons.Count); i++) {
				if (i >= serialValues [0].Length) {
					//no value for current button
				} else {
					gamepadButtons [i].StateLog.Insert (0, int.Parse (serialValues [0] [i].ToString ()));

					if (serialValues [0] [i] == '0') {
						//not pressed
					} else {
						//pressed
					}
				}
			}

			if (serialValues.Length > 1) {

				string[] bendValues = serialValues [1].Split (',');
				float[] floatBendValues = new float[bendValues.Length];
				for (int j = 0; j < (bendValues.Length); j++) {
					if (j == (bendValues.Length - 1)) {
						bendValues [j] = bendValues [j].Substring (0, bendValues [j].Length);
					}

					if (lerpMode) {
						float currentVal = float.Parse (bendValues [j]);
						previousBendValues [j] = Mathf.Lerp (previousBendValues [j], currentVal, 0.5f);
					} else {
						previousBendValues [j] = float.Parse (bendValues [j]);
					}
						
					floatBendValues [j] = previousBendValues [j];
				}

				if(calibrateComplete)
				{
					if (((floatBendValues [0] + floatBendValues [1]) / 2) >= (restValues[0] + restValues[1]) / 2) {
						rtValue = (((floatBendValues [0] - restValues[0]) / (maxBendValue1 - restValues[0])) + ((floatBendValues [1] - restValues[1]) / (maxBendValue2 - restValues[1]))) / 2f;
						if (rtValue > 1f)
							rtValue = 1f;
						ltValue = 0f;
					} else {
						rtValue = 0f;
						ltValue = (((restValues[0]-floatBendValues[0])/(restValues[0]-minBendValue1))+((restValues[1]-floatBendValues[1])/(restValues[1]-minBendValue2)))/2f;
						ltValue *= -1f;
						if (ltValue < -1f)
							ltValue = -1f;
					}

					rtValue *= (1f/0.83f);
					ltValue *= (1f/0.83f);

					GameObject.Find ("SliderRight").GetComponent<Slider> ().value = (int)floatBendValues [0];
					GameObject.Find ("SliderLeft").GetComponent<Slider> ().value = (int)floatBendValues [1];

					previousStickValue = stickValue;

					float bValue1 = floatBendValues [0];
					float sv1 = 0;
					if (bValue1 - restValues [0] > sensitivityConstant) {
						sv1 = (bValue1 - restValues [0]) / (maxBendValue1 - restValues [0]);
					} else if(bValue1 - restValues [0] < -sensitivityConstant){
						sv1 = (bValue1 - restValues [0]) / (restValues [0] - minBendValue1);
					}

					//sv1 = floatFilter.Filter(sv1);

					float bValue2 = floatBendValues [1];
					float sv2 = 0;
					if (bValue2 - restValues [1] > sensitivityConstant) {
						sv2 = (bValue2 - restValues [1]) / (maxBendValue2 - restValues [1]);
					} else if(bValue2 - restValues [1] < -sensitivityConstant){
						sv2 = (bValue2 - restValues [1]) / (restValues [1] - minBendValue2);
					}

					//sv2 = floatFilter.Filter(sv2);

					stickValue = (sv2 - sv1);
					stickValue = floatFilter.Filter(stickValue);

					//stickValue *= 20f;

					if (stickValue > 1f)
						stickValue = 1f;
					else if (stickValue < -1f)
						stickValue = -1f;

					//GameObject.Find ("RTValue").GetComponent<Text> ().text = GameObject.FindObjectOfType<TTSRacer> ().GetComponent<Rigidbody> ().velocity.magnitude.ToString();
					//GameObject.Find ("LTValue").GetComponent<Text> ().text = stickValue.ToString ();
					//GameObject.Find ("TriggerValue").GetComponent<Text> ().text = (rtValue + ltValue).ToString ();
				}
				else
				{
					CalibrateController (floatBendValues);
					
					//DEBUG STUFF
					float calibrationProgress = (float)currentCalibrationFrame / (float)calibrationFrames;
					calibrationProgress *= 100f;
					string progressTemp = "Calibrating...";
					progressTemp += calibrationProgress;
					progressTemp += "%";
					Debug.Log (progressTemp);
				}
			}
		}

		foreach (ControllerButton button in gamepadButtons)
		{
			button.Update();
		}
	}

	private float[] restValues = {0, 0};
	private const float stateCushionValue = 50f;

	public void CalibrateController(float[] floatBendValues)
	{
		currentCalibrationFrame += 1;

		//Debug.Log ("<color=blue>Sensor 1: " + floatBendValues [0] + ", Sensor 2: " + floatBendValues [1] + "</color>");

		bend1Av += floatBendValues [0];
		bend2Av += floatBendValues [1];
		
		if(currentCalibrationFrame >= calibrationFrames)
		{
			bend1Av /= currentCalibrationFrame;
			restValues[0] = bend1Av;
			bend2Av /= currentCalibrationFrame;
			restValues[1] = bend2Av;
			calibrateComplete = true;

			Debug.Log ("<color=green>Calibration Complete</color>");
			Debug.Log ("<color=green>Sensor 1: " + restValues[0] + ", Sensor 2: " + restValues[1] + "</color>");
		}
	}
	
}
