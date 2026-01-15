using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Mobile.Data;

/// <summary>
/// Data access service for MAUI using device file system storage.
/// </summary>
public class DataAccessService(
    ICompressionService compressionService,
    IStorageService storageService,
    IEncryptionKeyProvider encryptionKeyProvider,
    IEncryptionService encryptionService,
    ILogger<DataAccessService> logger)
    : DataAccessServiceBase(compressionService, storageService, encryptionKeyProvider, encryptionService, logger)
{
    private const string ProjectName = "KidsIdKitData";

    protected override string StorageKey => ProjectName + ".zip";
    protected override string BackupKey => ProjectName + ".bak.zip";
    protected override string EntryName => ProjectName + ".dat";
    protected override bool SupportsBackup => true;
}
