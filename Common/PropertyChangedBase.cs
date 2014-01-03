using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NicCage
{
	/// <summary>
	/// A base class for INotifyPropertyChanged
	/// </summary>
	public class PropertyChangedBase : INotifyPropertyChanged
	{
		/// <summary>
		/// Property changed event
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="name">Property name</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

