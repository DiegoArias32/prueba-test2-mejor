namespace ElectroHuila.Domain.Enums;

public enum DocumentType
{
    CC = 1,
    TI = 2,
    RC = 3,
    CE = 4,
    NIT = 5,
    Pasaporte = 6,
}

 public static class DocumentTypeExtensions
{
    /// <summary>
    /// Devuelve el nombre completo del tipo de documento.
    /// </summary>
    public static string ToDisplayName(this DocumentType type)
        => type switch
        {
            DocumentType.CC => "Cédula de Ciudadanía",
            DocumentType.TI => "Tarjeta de Identidad",
            DocumentType.CE => "Cédula de Extranjería",
            DocumentType.RC => "Registro Civil",
            DocumentType.NIT => "NIT",
            DocumentType.Pasaporte => "Pasaporte",
            _ => type.ToString()
        };

    /// <summary>
    /// Devuelve la longitud mínima esperada para el número de documento.
    /// </summary>
    public static int GetMinLength(this DocumentType type)
        => type switch
        {
            DocumentType.CC => 6,
            DocumentType.TI => 8,
            DocumentType.CE => 6,
            DocumentType.RC => 10,
            DocumentType.NIT => 9,
            DocumentType.Pasaporte => 6,
            _ => 0
        };

    /// <summary>
    /// Devuelve la longitud máxima esperada para el número de documento.
    /// </summary>
    public static int GetMaxLength(this DocumentType type)
        => type switch
        {
            DocumentType.CC => 10,
            DocumentType.TI => 10,
            DocumentType.CE => 10,
            DocumentType.RC => 15,
            DocumentType.NIT => 10,
            DocumentType.Pasaporte => 20,
            _ => 20
        };
}