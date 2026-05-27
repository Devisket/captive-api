namespace Captive.Model.Notifications
{
    public interface IOrderFileNotifier
    {
        /// <summary>Broadcasts a status detail to all processing/generating order files in a batch.</summary>
        Task NotifyBatchProgress(Guid batchId, string statusDetail, CancellationToken cancellationToken = default);

        /// <summary>Broadcasts a status detail to one specific order file.</summary>
        Task NotifyOrderFileProgress(Guid batchId, Guid orderFileId, string statusDetail, CancellationToken cancellationToken = default);
    }
}
