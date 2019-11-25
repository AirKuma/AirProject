using System;
using System.Collections.Generic;
using AirKuma.Chrono;

namespace AirKuma.Anim {

  public enum PlayState {
    Playing,
    Paused,
  }
  public interface IPlayable {
    PlayState PlayState { get; }
    void Start();
    void Stop();
  }
  public static class PlayableEx {

    public static void Toggle(this IPlayable playable) {
      if (playable.PlayState == PlayState.Paused)
        playable.Start();
      else
        playable.Stop();
    }
    public static void EnStart(this IPlayable playable) {
      if (playable.PlayState == PlayState.Paused)
        playable.Start();
    }
    public static void EnStop(this IPlayable playable) {
      if (playable.PlayState == PlayState.Playing)
        playable.Stop();
    }
  }

  public abstract class WellDisposable : IDisposable {

    private bool isDisposed = false;
    public bool Disposed => isDisposed;

    public void Dispose() {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    ~WellDisposable() {
      Dispose(false);
    }

    protected abstract void OnDispose();

    public void EnDispose() {
      if (!isDisposed)
        Dispose();
    }

    void Dispose(bool dispoing) {
      if (!isDisposed) {
        OnDispose();
        isDisposed = true;
      }
    }
  }

  public class CoroutineExecutor : IDisposable, IPlayable {

    public IEnumerator<float> coroutine;
    private float currentInterval;
    private float prevTime;

    //public PlayState PlayState { get; private set; }

    public CoroutineExecutor() { }

    public CoroutineExecutor(IEnumerator<float> coroutine) {
      DebugEx.Assert(coroutine != null);
      this.coroutine = coroutine;
      Start();
    }

    public void Dispose() {
      if (PlayState == PlayState.Playing)
        Stop();
    }

    public PlayState PlayState { get; private set; } = PlayState.Paused;

    // [Review(ReviewGoal.CheckApiSupport)]
    public void Start() {
      DebugEx.Assert(coroutine != null);
      //try {
      //  coroutine.Reset();
      //}
      //catch (NotSupportedException xpt) {
      //  DebugEx.LogError("Reset is not supported\n" + xpt.Message);
      //}
      currentInterval = 0.0f;
      prevTime = Chrono.TimeEx.Now;
      PlayState = PlayState.Playing;
      AirSystem.OnUpdate += OnUpdateCallback;
      //EditorApplication.update += EditorAppUpdateCallback;
    }

    public void Stop() {
      DebugEx.Assert(coroutine != null);
      AirSystem.OnUpdate -= OnUpdateCallback;
      //EditorApplication.update -= EditorAppUpdateCallback;
      PlayState = PlayState.Paused;
      coroutine = null;
    }

    public CoroutineExecutor Restart(IEnumerator<float> newCoroutine) {
      if (PlayState == PlayState.Playing)
        Stop();
      coroutine = newCoroutine;
      Start();
      return this;
    }

    private void OnUpdateCallback() {
      if (coroutine != null) {
        float now = (float)TimeEx.Now;
        if ((now - prevTime) > currentInterval) {
          if (coroutine.MoveNext()) {
            prevTime = now;
            currentInterval = coroutine.Current;
          }
          else
            Dispose();
        }
      }
    }
  }
}
