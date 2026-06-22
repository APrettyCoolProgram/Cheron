// 260620_code
// 260620_documentation

namespace Tekst.Catalog;

public static class msg_Cartridge
{
    public static string LookingFor(string cartPath) => $"\n[CHERON] Looking for game cartridge:\n  {cartPath}\n";

    public static string NotFound(string cartPath) => $"[CHERON] Cartridge not found!\n";

    public static string DeserializationFailed(string cartPath) => $"[CHERON] Failed to deserialize cartridge!\n";

}

