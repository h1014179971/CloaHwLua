using UnityEngine;
using System.Collections;

/// A CoroutineTask object represents a coroutine.  CoroutineTask can be started, paused, and stopped.
/// It is an error to attempt to start a task that has been stopped or which has
/// naturally terminated.
/// /// Example usage:
///
/// ----------------------------------------------------------------------------
/// IEnumerator MyCoroutineTask()
/// {
///     while(true) {
///         Debug.Log("I'm a coroutine task.");
///         yield return null;
////    }
/// }
///
/// IEnumerator CoroutineTaskKiller(float delay, Task t)
/// {
///     yield return new WaitForSeconds(delay);
///     t.Stop();
/// }
///
/// void SomeCodeThatCouldBeAnywhereInTheUniverse()
/// {
///     CoroutineTask spam = new CoroutineTask(MyAwesomeTask());
///     new CoroutineTask(CoroutineTaskKiller(5, spam));
/// }
/// ----------------------------------------------------------------------------
public class CoroutineTask
{
	/// Returns true if and only if the coroutine is running.  Paused tasks
	/// are considered to be running.
	public bool Running
	{
		get
		{
			return _task.Running;
		}
	}

	/// Returns true if and only if the coroutine is currently paused.
	public bool Paused
	{
		get
		{
			return _task.Paused;
		}
	}

	/// Delegate for termination subscribers.  manual is true if and only if
	/// the coroutine was stopped with an explicit call to Stop().
	public delegate void FinishedHandler(bool manual);

	/// Termination event.  Triggered when the coroutine completes execution.
	public event FinishedHandler Finished;

	/// Creates a new Task object for the given coroutine.
	///
	/// If autoStart is true (default) the task is automatically started
	/// upon construction.
	public CoroutineTask(IEnumerator c, bool autoStart = true)
	{
		_task = CoroutineTaskManager.CreateTask(c);
		_task.Finished += TaskFinished;
		if (autoStart)
			Start();
	}

	/// Begins execution of the coroutine
	public void Start()
	{
		_task.Start();
	}

	/// Discontinues execution of the coroutine at its next yield.
	public void Stop()
	{
		_task.Stop();
	}

	public void Pause()
	{
		_task.Pause();
	}

    public void Resume()
	{
        _task.Resume();
	}

	void TaskFinished(bool manual)
	{
		FinishedHandler handler = Finished;
		if (handler != null)
			handler(manual);
	}

    CoroutineTaskManager.Task _task;
}

class CoroutineTaskManager : MonoBehaviour
{
	public class Task
	{
		public bool Running
		{
			get
			{
				return _running;
			}
		}

		public bool Paused
		{
			get
			{
				return _paused;
			}
		}

		public delegate void FinishedHandler(bool manual);
		public event FinishedHandler Finished;

        IEnumerator _coroutine;
        bool _running;
        bool _paused;
        bool _stopped;

		public Task(IEnumerator c)
		{
			_coroutine = c;
		}

		public void Pause()
		{
			_paused = true;
		}

		public void Resume()
		{
			_paused = false;
		}

		public void Start()
		{
            if (!_running)
            {
                _running = true;
                _singleton.StartCoroutine(CallWrapper());
            }
		}

		public void Stop()
		{
			_stopped = true;
			_running = false;
		}

		IEnumerator CallWrapper()
		{
			yield return null;
			IEnumerator e = _coroutine;
			while (_running)
			{
				if (_paused)
					yield return null;
				else
				{
					if (e != null && e.MoveNext())
					{
						yield return e.Current;
					}
					else
					{
						_running = false;
					}
				}
			}

			if (Finished != null)
				Finished(_stopped);
		}
	}

    static CoroutineTaskManager _singleton;

	public static Task CreateTask(IEnumerator coroutine)
	{
		if (_singleton == null)
		{
			GameObject go = new GameObject("CoroutineTaskManager");
			_singleton = go.AddComponent<CoroutineTaskManager>();
		}
		return new Task(coroutine);
	}
}