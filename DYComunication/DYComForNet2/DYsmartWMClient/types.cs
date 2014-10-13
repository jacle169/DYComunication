
namespace DYsmartWMClient
{
    public class IOs
    {
        public int[] analogs { get; set; }
        public byte[] digitals { get; set; }
    }

    public enum operations
    {
        ResetAll,
        SetPwd,
        BeginSerial,
        SerialSend,
        PinMode,
        SetDigital,
        SetPWM,
        SetServo,
        SetSteper,
        Get18B20,
        GetDigital,
        GetAnalog,
        GetAllDigitalAndAnalog,
        runIRReader,
        IRReadOnData,
        SendIRRemote,
        SerialOnData
    }

    public enum dMode
    {
        LOW,
        HIGH
    }

    public enum pMode
    {
        INPUT,
        OUTPUT
    }
}
