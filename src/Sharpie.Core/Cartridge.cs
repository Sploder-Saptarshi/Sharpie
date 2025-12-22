public struct Cartridge
{
    public string Title { get; private set; }
    public ushort MusicAddress { get; private set; }
    public byte[] RomData { get; private set; }
    public byte[] HeaderPalette { get; private set; }

    public static Cartridge? Load(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);

        if (
            fileBytes[0] != 'S'
            || fileBytes[1] != 'H'
            || fileBytes[2] != 'R'
            || fileBytes[3] != 'P'
        )
            return null;

        var cart = new Cartridge();
        cart.Title = System.Text.Encoding.ASCII.GetString(fileBytes, 0x09, 20).TrimEnd('\0');
        cart.HeaderPalette = new byte[16];
        Array.Copy(fileBytes, 0x20, cart.HeaderPalette, 0, 16);

        cart.MusicAddress = BitConverter.ToUInt16(fileBytes, 0x30);
        cart.RomData = new byte[fileBytes.Length - 50];
        Array.Copy(fileBytes, 50, cart.RomData, 0, cart.RomData.Length);

        return cart;
    }
}
