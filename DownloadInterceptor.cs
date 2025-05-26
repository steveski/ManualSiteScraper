using CefSharp;

namespace ManualWebScraper;

public class DownloadInterceptor : IDownloadHandler
{
    /// <summary>
    /// When true we only capture URL+filename; when false we let the download go through.
    /// </summary>
    public bool CaptureOnly { get; set; }

    public string? LastDownloadUrl { get; set; }
    public string? LastDownloadFilename { get; set; }

    // 1) Let CEF know we want to handle every download
    public bool CanDownload(
        IWebBrowser chromiumWebBrowser,
        IBrowser browser,
        string url,
        string requestMethod)
    {
        return true;
    }

    // 1) A TCS that we'll await from the VM
    private TaskCompletionSource<bool>? _tcs;

    /// <summary>
    /// Call this _before_ EvaluateScriptAsync to reset state.
    /// </summary>
    public void PrepareCapture()
    {
        CaptureOnly = true;
        LastDownloadUrl = null;
        LastDownloadFilename = null;
        _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>
    /// VM can await this for up to <paramref name="timeout"/>.
    /// </summary>
    public Task<bool> WaitForDownloadAsync(TimeSpan timeout)
    {
        if (_tcs == null)
            return Task.FromResult(false);
        var completed = Task.WhenAny(_tcs.Task, Task.Delay(timeout));
        return completed.ContinueWith(t =>
            t.Result == _tcs.Task && _tcs.Task.Result,
            TaskScheduler.Default);
    }

    public bool OnBeforeDownload(
        IWebBrowser chromiumWebBrowser,
        IBrowser browser,
        DownloadItem downloadItem,
        IBeforeDownloadCallback callback)
    {
        // capture the URL/filename
        LastDownloadUrl = downloadItem.Url;
        LastDownloadFilename = downloadItem.SuggestedFileName;

        // signal the VM that we saw it
        _tcs?.TrySetResult(true);

        if (CaptureOnly)
            return true;      // swallow (cancel) the download

        // otherwise actually continue
        if (!callback.IsDisposed)
            callback.Continue(LastDownloadFilename, showDialog: false);
        return true;
    }

    // 3) Optional: progress updates
    public void OnDownloadUpdated(
        IWebBrowser chromiumWebBrowser,
        IBrowser browser,
        DownloadItem downloadItem,
        IDownloadItemCallback callback)
    {
        // no-op
    }
}
