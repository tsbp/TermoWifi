/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 09/02/2016
 * Time: 15:52
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
	/// Interaction logic for Window2.xaml
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