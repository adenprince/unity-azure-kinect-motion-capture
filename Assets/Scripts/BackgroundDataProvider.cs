﻿using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BackgroundDataProvider : IDisposable
{
    private BackgroundData m_frameBackgroundData = new BackgroundData();
    private bool m_latest = false;
    object m_lockObj = new object();
    public bool IsRunning { get; set; } = false;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _token;
    private int _id;
    private Task _backgroundThread;

    public BackgroundDataProvider(int id)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.quitting += OnEditorClose;
#endif
        _cancellationTokenSource = new CancellationTokenSource();
        _token = _cancellationTokenSource.Token;
        _id = id;
    }

    public void StartBackgroundThread()
    {
        _backgroundThread = Task.Run(() => RunBackgroundThreadAsync(_id, _token));
    }

    private void OnEditorClose()
    {
        Dispose();
    }

    protected abstract void RunBackgroundThreadAsync(int id, CancellationToken token);

    public void SetCurrentFrameData(ref BackgroundData currentFrameData)
    {
        lock (m_lockObj)
        {
            var temp = currentFrameData;
            currentFrameData = m_frameBackgroundData;
            m_frameBackgroundData = temp;
            m_latest = true;
        }
    }

    public bool GetCurrentFrameData(ref BackgroundData dataBuffer)
    {
        lock (m_lockObj)
        {
            var temp = dataBuffer;
            dataBuffer = m_frameBackgroundData;
            m_frameBackgroundData = temp;
            bool result = m_latest;
            m_latest = false;
            return result;
        }
    }

    public void Dispose()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.quitting -= OnEditorClose;
#endif
        _cancellationTokenSource?.Cancel();
        _backgroundThread.Wait();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}
