# Install Azure IoT Edge runtime on your Edge device.


# https://docs.microsoft.com/en-us/azure/iot-edge/quickstart?view=iotedge-2018-06#install-and-start-the-iot-edge-runtime

<#
our Windows virtual machine starts with Windows version 1809 (build 17763), which is the latest Windows long-term support build.
 Windows automatically checks for updates every 22 hours by default. After a check on your virtual machine, Windows pushes 
 a version update that is incompatible with IoT Edge for Windows, which prevents further use of IoT Edge for Windows features. 
 We recommend limiting use of your virtual machine to within 22 hours or temporarily pausing Windows updates.

This quickstart uses a Windows desktop virtual machine for simplicity. For information about which Windows operating systems 
are generally available for production scenarios, see Azure IoT Edge supported systems.

If you want to configure your own Windows device for IoT Edge, including devices running IoT Core, follow the steps 
in Install the Azure IoT Edge runtime.
#>


# Use an AMD64 session of PowerShell
(Get-Process -Id $PID).StartInfo.EnvironmentVariables["PROCESSOR_ARCHITECTURE"]
 
# Install Azure IoT Edge runtime on the device
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; `
Deploy-IoTEdge -ContainerOs Windows

# Configure the IoT Edge runtime on your machine
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; `
Initialize-IoTEdge -ContainerOs Windows

# Check the status of the IoT Edge service
Get-Service iotedge

# retrieve the service logs
. {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Get-IoTEdgeLog

# View all the modules running on your IoT Edge device.
iotedge list

# Monitor the simulated temrature data
iotedge logs SimulatedTemperatureSensor -f