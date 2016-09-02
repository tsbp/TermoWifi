/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 02.09.2016
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TermoWifi
{
	/// <summary>
	/// Interaction logic for ConfigsWindow.xaml
	/// </summary>
	public partial class ConfigsWindow : Window
	{
		public ConfigsWindow()
		{
			InitializeComponent();
		}
		//==============================================================
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
						
		}	
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		//==============================================================
	}
}