using KidsIdKit.Core.Services;
using Microsoft.Extensions.Logging;

namespace KidsIdKit.Web.Data;

/// <summary>
/// Data access service for Blazor WebAssembly using browser LocalStorage.
/// </summary>
public class DataAccessService(
    ICompressionService compressionService,
    IStorageService storageService,
    IEncryptionKeyProvider encryptionKeyProvider,
    IEncryptionService encryptionService,
    ILogger<DataAccessService> logger)
    : DataAccessServiceBase(compressionService, storageService, encryptionKeyProvider, encryptionService, logger)
{
    protected override string StorageKey => "FamilyZip";
    protected override string BackupKey => string.Empty; // Not supported on web
    protected override string EntryName => "Family.json";
    protected override bool SupportsBackup => false;
}
