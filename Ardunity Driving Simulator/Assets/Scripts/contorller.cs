using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using System.Text;

public class contorller : MonoBehaviour
{

    // Use this for initialization
    BluetoothHelper bluetoothHelper;
    string deviceName;
    string received_message;
    private string tmp;

    void Start()
    {
        deviceName = "HC-06"; //bluetooth should be turned ON;
        try
        {
            BluetoothHelper.BLE = false;
            bluetoothHelper = BluetoothHelper.GetInstance(deviceName);
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.setTerminatorBasedStream("\n");

            if (!bluetoothHelper.ScanNearbyDevices())
            {
                //scan didnt start (on windows desktop (not UWP))
                //try to connect
                bluetoothHelper.Connect();//this will work only for bluetooth classic.
                //scanning is mandatory before connecting for BLE.

            }
            LinkedList<BluetoothDevice> ds = bluetoothHelper.getPairedDevicesList();
            OnScanEnded(ds);
            Debug.Log(ds);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);
        }
    }

    private void write(string msg)
    {
        tmp += ">" + msg + "\n";
    }

    private void reset(string msg)
    {
        tmp = "";
    }

    void OnMessageReceived()
    {
        received_message = bluetoothHelper.Read();
        Debug.Log(received_message);
        write("Received : " + received_message);
    }

    void OnConnected()
    {
        try
        {
            bluetoothHelper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            write(ex.Message);

        }
    }
    void OnScanEnded(LinkedList<BluetoothDevice> devices)
    {
        Debug.Log(devices.Count);
    }

    void OnConnectionFailed()
    {
        write("Connection Failed");
        Debug.Log("Connection Failed");
    }

    void Update()
    {

        if (bluetoothHelper.isConnected())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                bluetoothHelper.SendData("f");
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bluetoothHelper.SendData("s");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                bluetoothHelper.SendData("b");
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                bluetoothHelper.SendData("l");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                bluetoothHelper.SendData("r");
            }
        }
    }
    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}