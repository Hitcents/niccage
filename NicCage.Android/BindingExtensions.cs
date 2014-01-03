using System;
using System.Linq;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using NicCage.Reflection;

namespace NicCage
{
	public static class BindingExtensions
	{
		public static PropertyObserver Bind(this object bindable, INotifyPropertyChanged viewModel, View view)
		{
			var context = view.Context;
			var observer = new PropertyObserver(viewModel);

			var viewGroup = view as ViewGroup;
			if (viewGroup != null)
			{
				BindChildren(bindable, viewModel, viewGroup);
			}

			int id = view.Id;
			if (id == -1)
				return observer;

			string name = context.Resources.GetResourceName(id);
			name = name.Split('/').Last();

			//TODO: These are our conventions, which need to be configurable

			var button = view as Button;
			if (button != null)
			{
				observer.Add<bool>("Can" + name, enabled => button.Enabled = enabled);

				button.Click += (sender, e) => observer.InvokeMethod(name);
				return observer;
			}

			var editText = view as EditText;
			if (editText != null)
			{
				observer.Add<string>(name, text => editText.Text = text);

				editText.TextChanged += (sender, e) => viewModel.SetProperty(name, editText.Text);
				return observer;
			}

			var textView = view as TextView;
			if (textView != null)
			{
				observer.Add<string>(name, text => textView.Text = text);
				return observer;
			}

			//No conventions hit
			return observer;
		}

		private static void BindChildren(object bindable, INotifyPropertyChanged viewModel, ViewGroup parent)
		{
			for (int i = 0; i < parent.ChildCount; i++)
			{
				var view = parent.GetChildAt(i);
				Bind(bindable, viewModel, view);

				var viewGroup = view as ViewGroup;
				if (viewGroup != null)
				{
					BindChildren(bindable, viewModel, viewGroup);
				}
			}
		}
	}
}

