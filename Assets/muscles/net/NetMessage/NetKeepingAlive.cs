using Unity.Networking.Transport;

public class NetKeepingAlive : NetMessage
{
    public NetKeepingAlive()
    {
        Code = OpCode.KEEP_ALIVE;
    }
    public NetKeepingAlive(DataStreamReader reader)
    {
        Code= OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
