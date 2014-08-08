using System;
using System.IO.Ports;
using System.Threading; 
namespace Engine {
public class KheperaSerialPortCommunicator
{
	public static void Main(string[] args)
	{
		KheperaSerialPortCommunicator myTest = new KheperaSerialPortCommunicator("/dev/rfcomm0");
	}
 
	private SerialPort mySerial;
 
	// Constructor
	public KheperaSerialPortCommunicator(string port)
	{
		Init(port);
		//demo();
	}
	
	public void demo() {
		foreach (float i in GetSensors()) {
		 Console.WriteLine(i);
		}
		
		Console.WriteLine("Turn left");
		SendMotor(0.5f,0.0f);
		System.Threading.Thread.Sleep(1000);
		
		Console.WriteLine("Stop");
		SendMotor(0.0f,0.0f);
		System.Threading.Thread.Sleep(1000);
		
		Console.WriteLine("Turn right");
		SendMotor(0.0f,0.5f);
		System.Threading.Thread.Sleep(1000);
		
		Console.WriteLine("Stop");
		SendMotor(0.0f,0.0f);
	}
	
	public void Init(string port)
	{
		if (mySerial != null)
			if (mySerial.IsOpen)
				mySerial.Close();
 
		mySerial = new SerialPort(port, 115200,Parity.None,8,StopBits.One);
		mySerial.Open();
		mySerial.ReadTimeout = 400;
		 //initialize the motors
		SendData("M"+mySerial.NewLine);
        // Should write 'm'
		Console.WriteLine(port+":"+ReadData());  
	}
	
    public void SendMotor(float left, float right) {
	// int lval = (int)(1023.0*left);
	// int rval = (int)(1023.0*right);
	 //string tosend = "L,l"+lval+",l"+rval+mySerial.NewLine;
	int lval = (int)(43000.0*left);
	int rval = (int)(43000.0*right);
    string tosend = "D,l" +lval + ",l"+rval+mySerial.NewLine; 	 
	Console.WriteLine(tosend);
	 SendData(tosend);
	  
	 //Should write 'l'
	 Console.WriteLine(ReadData());
	}
	public float[] GetSensors() {
	 SendData("N"+mySerial.NewLine);
	 float[] sensors = new float[6];
	 string ret = ReadData();
	 string[] splits = ret.Split(',');
	 for(int i=0;i<6;i++) {
	  int ind=i;
	  if(splits.Length==1) 
		return new float[6];
					
	 float val=(float)(Convert.ToDouble(splits[2+i]));
	  Console.Write(val + " ");
	  sensors[ind]=Math.Min(1.0f, (float)val / 4000.0f);
	  sensors[ind]=1.0f-sensors[ind];
	 }
	 Console.WriteLine();
	 return sensors;
	}
	
	public string ReadData()
	{
		byte tmpByte;
		string rxString = "";
		return mySerial.ReadLine();
	}
 
	public bool SendData(string Data)
	{
		mySerial.Write(Data);
		return true;		
	}
}
}