﻿using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Tester {
	public partial class AutocompleteSample3 : Form {
		readonly AutocompleteMenu popupMenu;

		public AutocompleteSample3() {
			InitializeComponent();

			//create autocomplete popup menu
			popupMenu = new AutocompleteMenu(fctb) {
				ForeColor = Color.White,
				BackColor = Color.Gray,
				SelectedColor = Color.Purple,
				SearchPattern = @"[\w\.]",
				AllowTabKey = true,
				AlwaysShowTooltip = true
			};
			//assign DynamicCollection as items source
			popupMenu.Items.SetAutocompleteItems(new DynamicCollection(popupMenu));
		}
	}

	/// <summary>
	/// Builds list of methods and properties for current class name was typed in the textbox
	/// </summary>
	internal class DynamicCollection : IEnumerable<AutocompleteItem> {
		private readonly AutocompleteMenu menu;

		public DynamicCollection(AutocompleteMenu menu) {
			this.menu = menu;
		}

		public IEnumerator<AutocompleteItem> GetEnumerator() {
			//get current fragment of the text
			var text = menu.Fragment.Text;

			//extract class name (part before dot)
			var parts = text.Split('.');
			if (parts.Length < 2)
				yield break;
			var className = parts[^2];

			//find type for given className
			var type = FindTypeByName(className);

			if (type == null)
				yield break;

			//return static methods of the class
			foreach (var methodName in type.GetMethods().AsEnumerable().Select(mi => mi.Name).Distinct())
				yield return new MethodAutocompleteItem(methodName + "()") {
					ToolTipTitle = methodName,
					ToolTipText = "Description of method " + methodName + " goes here.",
				};

			//return static properties of the class
			foreach (var pi in type.GetProperties())
				yield return new MethodAutocompleteItem(pi.Name) {
					ToolTipTitle = pi.Name,
					ToolTipText = "Description of property " + pi.Name + " goes here.",
				};
		}

		static Type FindTypeByName(string name) {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var a in assemblies) {
				foreach (var t in a.GetTypes())
					if (t.Name == name) {
						return t;
					}
			}

			return null;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
