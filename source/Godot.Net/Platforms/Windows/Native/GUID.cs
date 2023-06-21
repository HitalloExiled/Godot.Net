namespace Godot.Net.Platforms.Windows.Native;

public unsafe struct GUID
{
    public uint       Data1;
    public ushort     Data2;
    public ushort     Data3;
    public fixed byte Data4[8];

    public static implicit operator GUID(Guid value)
    {
        var bytes = value.ToByteArray();

        var output = new GUID
        {
            Data1 = BitConverter.ToUInt32(bytes, 0),
            Data2 = BitConverter.ToUInt16(bytes, 4),
            Data3 = BitConverter.ToUInt16(bytes, 6),
        };

        for (var i = 0; i < 8; i++)
        {
            output.Data4[i] = bytes[i + 8];
        }

        return output;
    }

    public static implicit operator Guid(GUID value)
    {
        var bytes = new byte[16];

        BitConverter.GetBytes(value.Data1).CopyTo(bytes, 0);
        BitConverter.GetBytes(value.Data2).CopyTo(bytes, 4);
        BitConverter.GetBytes(value.Data3).CopyTo(bytes, 6);

        for (var i = 0; i < 8; i++)
        {
            bytes[i + 8] = value.Data4[i];
        }

        return new Guid(bytes);
    }
}
