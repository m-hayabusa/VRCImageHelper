namespace VRCImageHelper.Core;

using System;

internal sealed class SemaphoreSlimWrapper
{
    private readonly SemaphoreSlim _semaphore;
    public SemaphoreSlimWrapper(int initialCount, int maxCount)
    {
        _semaphore = new SemaphoreSlim(initialCount, maxCount);
    }
    public SemaphoreSlimWrapper(int initialCount)
    {
        _semaphore = new SemaphoreSlim(initialCount);
    }

    public IDisposable Wait()
    {
        _semaphore.Wait();
        return new Releaser(this);
    }
    private sealed class Releaser : IDisposable
    {
        private readonly SemaphoreSlimWrapper _semaphoreWrapper;
        internal Releaser(SemaphoreSlimWrapper semaphoreWrapper)
        {
            _semaphoreWrapper = semaphoreWrapper;
        }
        public void Dispose()
        {
            _semaphoreWrapper._semaphore.Release();
        }
    }
}
