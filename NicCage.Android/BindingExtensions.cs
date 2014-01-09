using System;
using System.Linq;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using NicCage.Reflection;
using Praeclarum.Bind;

namespace NicCage
{
	public static class BindingExtensions
	{
        public static void Bind(this object bindable, INotifyPropertyChanged viewModel, View view)
		{
			var context = view.Context;

			var viewGroup = view as ViewGroup;
			if (viewGroup != null)
			{
				BindChildren(bindable, viewModel, viewGroup);
			}

			int id = view.Id;
			if (id == -1)
				return;

			string name = context.Resources.GetResourceName(id);
			name = name.Split('/').Last();

			//TODO: These are our conventions, which need to be configurable

			var button = view as Button;
			if (button != null)
			{
                Binding.Create(() => button.Enabled == ((bool)viewModel.GetProperty("Can" + name)));

                button.Click += (sender, e) => viewModel.Invoke(name);

				return;
			}

			var editText = view as EditText;
			if (editText != null)
			{
                Binding.Create(() => editText.Text == viewModel.GetProperty(name));
                return;
			}

			var textView = view as TextView;
			if (textView != null)
			{
                Binding.Create(() => textView.Text == viewModel.GetProperty(name));

				return;
			}

			//No conventions hit
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

