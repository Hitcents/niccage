using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using NicCage.Reflection;

namespace NicCage
{
	/// <summary>
	/// Class for observing INotifyPropertyChanged and setting up bindings
	/// </summary>
	public class PropertyObserver
	{
		private readonly Dictionary<string, List<Action<object>>> _actions = new Dictionary<string, List<Action<object>>>();
		private WeakReference<INotifyPropertyChanged> _target;

		public PropertyObserver(INotifyPropertyChanged target)
		{
			target.SetWeakHandler(
				h => target.PropertyChanged += h, 
				h => target.PropertyChanged -= h, 
				OnPropertyChanged);

			_target = new WeakReference<INotifyPropertyChanged>(target);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			List<Action<object>> actions;
			if (_actions.TryGetValue(e.PropertyName, out actions))
			{
				INotifyPropertyChanged obj;
				if (_target.TryGetTarget(out obj))
				{
					var value = obj.GetProperty(e.PropertyName);
					foreach (var action in actions)
					{
						action(value);
					}
				}
			}
		}

		/// <summary>
		/// Adds a binding by a property name and a callback
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="action">Action for setting up the binding</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public void Add<T>(string propertyName, Action<T> action)
		{
			Add(propertyName, value => action((T)value));
		}

		/// <summary>
		/// Adds a binding by a property name and a callback
		/// </summary>
		/// <param name="propertyName">Property name</param>
		/// <param name="action">Action for setting up the binding</param>
		public void Add(string propertyName, Action<object> action)
		{
			List<Action<object>> actions;
			if (!_actions.TryGetValue(propertyName, out actions))
			{
				actions =
					_actions [propertyName] = new List<Action<object>>();
			}
			actions.Add(action);

			//Fire initial event
			INotifyPropertyChanged target;
			if (_target.TryGetTarget(out target))
			{
				action(target.GetProperty(propertyName));
			}
		}

		/// <summary>
		/// Invokes a method on the bound object
		/// </summary>
		/// <param name="methodName">Method name</param>
		public void InvokeMethod(string methodName)
		{
			INotifyPropertyChanged target;
			if (_target.TryGetTarget(out target))
			{
				target.Invoke(methodName);
			}
		}
	}
}
