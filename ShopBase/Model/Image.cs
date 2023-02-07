namespace ShopBase.Model;

public class Image : ModelBase<Image>
{
    public byte[]? ImageData { get; set; }

    public string DataType { get; set; } = "image/png";

    public DbImageType Type { get; set; }

}

public enum DbImageType 
{
    Unclassified = 0,
    ArticleImage = 1,
}