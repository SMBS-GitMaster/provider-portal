namespace PortalProveedor.Helpers;

public class AppSettings
{
    public string Secret { get; set; }
    public Azure Azure { get; set; }
}

public class Azure
{
    public BlobStorage BlobStorage { get; set; }
}

public class BlobStorage
{
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
}