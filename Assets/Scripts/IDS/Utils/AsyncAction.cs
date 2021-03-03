using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

public class AsyncAction : DDOLSingleton<AsyncAction>
{
	public struct DelayedQueueItem
	{
		public int time;//ms
		public Action action;
	}

	public int maxThreads = 8;
	int numThreads;

	List<Action> _actions = new List<Action>();
	List<Action> _currentActions = new List<Action>();

    List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
	List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

	public void QueueOnMainThread(Action action)
	{
		QueueOnMainThread(action, 0);
	}

    /// <summary>
    /// Queues the action on main thread.
    /// </summary>
    /// <param name="action">Action.</param>
    /// <param name="time">Time(ms)</param>
	public void QueueOnMainThread(Action action, int time)
	{
        if (Math.Abs(time) > float.Epsilon)
		{
            lock (Instance._delayed)
			{
                Instance._delayed.Add(new DelayedQueueItem { time = DateTime.Now.Millisecond + time, action = action });
			}
		}
		else
		{
			lock (Instance._actions)
			{
				Instance._actions.Add(action);
			}
		}
	}

	public Thread RunAsync(Action a)
	{
		while (numThreads >= maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
		return null;
	}

    void RunAction(object action)
	{
		try
		{
			((Action)action)();
		}
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
		{
			Interlocked.Decrement(ref numThreads);
		}

	}

	// Update is called once per frame  
	void Update()
	{
        if (_actions.Count > 0)
        {
			lock (_actions)
			{
				_currentActions.Clear();
				_currentActions.AddRange(_actions);
				_actions.Clear();
			}
			for (int i = 0; i < _currentActions.Count; ++i)
			{
				_currentActions[i]();
			}

		}

        if (_delayed.Count > 0)
		{
			lock (_delayed)
			{
				_currentDelayed.Clear();
				_currentDelayed.AddRange(_delayed.Where(d => d.time <= DateTime.Now.Millisecond));
				foreach (var item in _currentDelayed)
					_delayed.Remove(item);
			}
			for (int i = 0; i < _currentDelayed.Count; ++i)
			{
				_currentDelayed[i].action();
			}
        }
	}
}