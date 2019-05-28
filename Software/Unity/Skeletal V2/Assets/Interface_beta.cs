//C#
using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
//using quaterion_algo;

using AHRS;

public class Interface_beta : MonoBehaviour
{
    SerialPort sp;
		string[] stringDelimiters = new string[] {":"};
		public Transform target1;
		public Transform target2;
		public Transform target0;
		public Transform tracker;
		private float timeCount = 0.0f;
		public MadgwickAHRS algo1;
		public MadgwickAHRS algo2;
		public MadgwickAHRS algo0;
		private int flag;
		private float ax0,az0_to_ay0,ax1,az1_to_ay1,ax2,az2_to_ay2;
		private Quaternion rot0,rot1,rot2;
		private Vector3 dir0,dir1,dir2;
		private string writestr = "0000";
		//float pi= (float) Math.PI ;
		//private Thread read;
    void Start()
    {
				algo1 = new MadgwickAHRS(0.0002f,0.0041f);
				algo2 = new MadgwickAHRS(0.0002f,0.0041f);
				algo0 = new MadgwickAHRS(0.0002f,0.0041f);
        sp = new SerialPort("COM3", 115200, Parity.Even, 8, StopBits.One); //Replace "COM4" with whatever port your Arduino is on.
        sp.DtrEnable = false; //Prevent the Arduino from rebooting once we connect to it.
                              //A 10 uF cap across RST and GND will prevent this. Remove cap when programming.
        sp.ReadTimeout = 3; //Shortest possible read time out.
        sp.WriteTimeout = 1; //Shortest possible write time out.

        sp.Open();
    }

    void Update()
    {
        // string cmd = CheckForRecievedData();
				string test = get_pong();
				ParseAccelerometerData(test);
				float c_angle,z_angle;
				Vector3 temp; Quaternion tempQ;
				switch(flag){
					case 3:
					  dir0 = new Vector3(-1*ax0, 0, -1*az0_to_ay0);
						dir1 = new Vector3(-1*ax1, 0, -1*az1_to_ay1);
						dir2 = new Vector3(-1*ax2, 0, -1*az2_to_ay2);
						// rot1 = Quaternion.FromToRotation(dir1,Vector3.left) * Quaternion.Euler(270, 270, 90); //270 270
						// rot2 = Quaternion.FromToRotation(dir2,Vector3.left) * Quaternion.Euler(270, 270, 90); //right
						c_angle = Vector3.Angle(Vector3.right,tracker.transform.forward);
						z_angle = Vector3.Angle(Vector3.forward,tracker.transform.up);
						Debug.Log("sngle->|| " + z_angle);

						temp = tracker.transform.rotation.eulerAngles;
						tempQ = Quaternion.Euler(0, temp.y, temp.z);
						if(c_angle>90){
							if(z_angle>90){
								rot0 = Quaternion.FromToRotation(dir0,Vector3.right) * Quaternion.Euler(270, 270, 90);
								rot1 = Quaternion.FromToRotation(dir1,Vector3.right) * Quaternion.Euler(270, 270, 90); //270 270
								rot2 = Quaternion.FromToRotation(dir2,Vector3.right) * Quaternion.Euler(270, 270, 90); //right
							}
							else{
								rot0 = Quaternion.FromToRotation(dir0,Vector3.left) * Quaternion.Euler(270, 270, 90);
								rot1 = Quaternion.FromToRotation(dir1,Vector3.left) * Quaternion.Euler(270, 270, 90); //270 270
								rot2 = Quaternion.FromToRotation(dir2,Vector3.left) * Quaternion.Euler(270, 270, 90);
							}
						}
						else{
							if(z_angle>90){
								rot0 = Quaternion.FromToRotation(dir0,Vector3.left) * Quaternion.Euler(270, 270, 90);
								rot1 = Quaternion.FromToRotation(dir1,Vector3.left) * Quaternion.Euler(270, 270, 90); //270 270
								rot2 = Quaternion.FromToRotation(dir2,Vector3.left) * Quaternion.Euler(270, 270, 90);
							}
							else{
								rot0 = Quaternion.FromToRotation(dir0,Vector3.right) * Quaternion.Euler(270, 270, 90);
								rot1 = Quaternion.FromToRotation(dir1,Vector3.right) * Quaternion.Euler(270, 270, 90); //270 270
								rot2 = Quaternion.FromToRotation(dir2,Vector3.right) * Quaternion.Euler(270, 270, 90); //right
							}
						}

						target0.transform.rotation = tempQ* Quaternion.Slerp(target0.transform.rotation,rot0,timeCount);
						target1.transform.rotation = tempQ* Quaternion.Slerp(target1.transform.rotation,rot1,timeCount); //tracker.transform.rotation *
						target2.transform.rotation = tempQ* Quaternion.Slerp(target2.transform.rotation,rot2,timeCount); //tracker.transform.rotation *
						// target1.transform.rotation=rot1*tracker.transform.rotation;
						// target2.transform.rotation=rot2*tracker.transform.rotation;
						algo0.Quaternion[0]=target0.transform.rotation.x;
					  algo0.Quaternion[1]=target0.transform.rotation.y;
					  algo0.Quaternion[2]=target0.transform.rotation.z;
					  algo0.Quaternion[3]=target0.transform.rotation.w;
						algo1.Quaternion[0]=target1.transform.rotation.x;
					  algo1.Quaternion[1]=target1.transform.rotation.y;
					  algo1.Quaternion[2]=target1.transform.rotation.z;
					  algo1.Quaternion[3]=target1.transform.rotation.w;
						algo2.Quaternion[0]=target2.transform.rotation.x;
						algo2.Quaternion[1]=target2.transform.rotation.y;
						algo2.Quaternion[2]=target2.transform.rotation.z;
						algo2.Quaternion[3]=target2.transform.rotation.w;
						break;
					case 2:
						Quaternion TO21 = new Quaternion(algo1.Quaternion[0],algo1.Quaternion[1],algo1.Quaternion[2],algo1.Quaternion[3]);
					  dir2 = new Vector3(-1*ax2, 0, -1*az2_to_ay2);
						c_angle = Vector3.Angle(Vector3.right,tracker.transform.forward);
						z_angle = Vector3.Angle(Vector3.forward,tracker.transform.up);
						temp = tracker.transform.rotation.eulerAngles;
						tempQ = Quaternion.Euler(0, temp.y, temp.z);
						if(c_angle>90){
							if(z_angle>90){
								rot2 = Quaternion.FromToRotation(dir2,Vector3.right) * Quaternion.Euler(270, 270, 90); //right
							}
							else{
								rot2 = Quaternion.FromToRotation(dir2,Vector3.left) * Quaternion.Euler(270, 270, 90);
							}
						}
						else{
							if(z_angle>90){
								rot2 = Quaternion.FromToRotation(dir2,Vector3.left) * Quaternion.Euler(270, 270, 90);
							}
							else{
								rot2 = Quaternion.FromToRotation(dir2,Vector3.right) * Quaternion.Euler(270, 270, 90); //right
							}
						}

						target1.transform.rotation = tracker.transform.rotation * Quaternion.Slerp(target1.transform.rotation,TO21,timeCount) * Quaternion.Euler(90, 270, 90);;
						target2.transform.rotation = tempQ * Quaternion.Slerp(target2.transform.rotation,rot2,timeCount);
						algo2.Quaternion[0]=target2.transform.rotation.x;
						algo2.Quaternion[1]=target2.transform.rotation.y;
						algo2.Quaternion[2]=target2.transform.rotation.z;
						algo2.Quaternion[3]=target2.transform.rotation.w;
						break;

					case 1:
						dir1 = new Vector3(-1*ax1, 0, -1*az1_to_ay1);
						c_angle = Vector3.Angle(Vector3.right,tracker.transform.forward);
						z_angle = Vector3.Angle(Vector3.forward,tracker.transform.up);
						temp = tracker.transform.rotation.eulerAngles;
						tempQ = Quaternion.Euler(0, temp.y, temp.z);
						if(c_angle>90){
							if(z_angle>90){
								rot1 = Quaternion.FromToRotation(dir1,Vector3.right) * Quaternion.Euler(270, 270, 90); //270 270
							}
							else{
								rot1 = Quaternion.FromToRotation(dir1,Vector3.left) * Quaternion.Euler(270, 270, 90); //270 270
							}
						}
						else{
							if(z_angle>90){
								rot1 = Quaternion.FromToRotation(dir1,Vector3.left) * Quaternion.Euler(270, 270, 90); //270 270
							}
							else{
								rot1 = Quaternion.FromToRotation(dir1,Vector3.right) * Quaternion.Euler(270, 270, 90); //270 270
							}
						}
						Quaternion TO12 = new Quaternion(algo2.Quaternion[0],algo2.Quaternion[1],algo2.Quaternion[2],algo2.Quaternion[3]);

						target1.transform.rotation = tempQ * Quaternion.Slerp(target1.transform.rotation,rot1,timeCount);
						target2.transform.rotation = tracker.transform.rotation * Quaternion.Slerp(target2.transform.rotation,TO12,timeCount) * Quaternion.Euler(90, 270, 90); //madgwick
						algo1.Quaternion[0]=target1.transform.rotation.x;
					  algo1.Quaternion[1]=target1.transform.rotation.y;
					  algo1.Quaternion[2]=target1.transform.rotation.z;
					  algo1.Quaternion[3]=target1.transform.rotation.w;
						break;

					default:
					  Quaternion TO0 = new Quaternion(algo0.Quaternion[0],algo0.Quaternion[1],algo0.Quaternion[2],algo0.Quaternion[3]);
						Quaternion TO1 = new Quaternion(algo1.Quaternion[0],algo1.Quaternion[1],algo1.Quaternion[2],algo1.Quaternion[3]);
						Quaternion TO2 = new Quaternion(algo2.Quaternion[0],algo2.Quaternion[1],algo2.Quaternion[2],algo2.Quaternion[3]);
						target0.transform.rotation = Quaternion.Slerp(target0.transform.rotation,TO0,timeCount)* Quaternion.Euler(0, 0, 0);
						target1.transform.rotation = Quaternion.Slerp(target1.transform.rotation,TO1,timeCount)* Quaternion.Euler(0, 0, 0); //tracker * 90, 270, 90
						target2.transform.rotation = Quaternion.Slerp(target2.transform.rotation,TO2,timeCount)* Quaternion.Euler(0, 0, 0);
						break;
			}
			timeCount = timeCount + Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && sp.IsOpen)
            sp.Close();
    }


    void ParseAccelerometerData(string data) {
				//Debug.Log(data);
				string[] splitResult = data.Split(stringDelimiters, StringSplitOptions.RemoveEmptyEntries);
				//Debug.Log(splitResult.Length);
				if(splitResult.Length == 11) {
						float gx1 = float.Parse(splitResult[1]); //* pi/180f; //convert deg/s to rad/s
						ax1 = float.Parse(splitResult[2]);
						az1_to_ay1 = float.Parse(splitResult[3]);
						float gx2 = float.Parse(splitResult[4]); //* pi/180f; //convert deg/s to rad/s
						ax2 = float.Parse(splitResult[5]);
						az2_to_ay2 = float.Parse(splitResult[6]);
						float gx0 = float.Parse(splitResult[7]); //* pi/180f; //convert deg/s to rad/s
						ax0 = float.Parse(splitResult[8]);
						az0_to_ay0 = float.Parse(splitResult[9]);
						flag =int.Parse(splitResult[10]);
						algo0.Update(0.0f,-gx0,0.0f,0.0f,-ax0,0.0f);
						algo1.Update(0.0f,-gx1,0.0f,0.0f,-ax1,0.0f);
						algo2.Update(0.0f,-gx2,0.0f,0.0f,-ax2,0.0f);
				}
		}

		void OnCollisionEnter (Collision coll) {
			writestr = "1111";
		}

		void OnCollisionExit (Collision coll) {
			writestr = "0000";
		}

		public string get_pong(){
			Debug.Log(writestr);
			sp.Write(writestr);
			string inData = sp.ReadLine();
			int inSize = inData.Count();
			Debug.Log("NUCLEO->|| " + inData + " ||MSG SIZE:" + inSize.ToString());
			return inData;
		}

    public string CheckForRecievedData()
    {
			try //Sometimes malformed serial commands come through. We can ignore these with a try/catch.
			{
					string inData = sp.ReadLine();
					int inSize = inData.Count();
					if (inSize > 0)
					{
							Debug.Log("NUCLEO->|| " + inData + " ||MSG SIZE:" + inSize.ToString());
					}
					//Got the data. Flush the in-buffer to speed reads up.
					inSize = 0;
					sp.BaseStream.Flush();
					sp.DiscardInBuffer();
					return inData;
			}
			catch { return string.Empty; }
		}
}
