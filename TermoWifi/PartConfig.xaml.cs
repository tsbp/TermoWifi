/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 16.09.2016
 * Time: 14:11
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
	/// Interaction logic for PartConfig.xaml
	/// </summary>
	public partial class PartConfig : Window
	{
		
		    
		public float tempValue;
		
		public PartConfig()
		{
			InitializeComponent();
		}
		//==============================================================
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			float a = float.Parse(lblTemp.Content.ToString());
			slTemp.Value = a - 19;
			
			string b = lblTime.Content.ToString();
			slTimeH.Value = 0.5 + double.Parse(b.Substring(0, b.IndexOf(":"))) / 2.4;			
			slTimeM.Value = double.Parse(b.Substring(b.IndexOf(":") + 1)) / 5.9;
		}
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			ConfigsWindow.tempReturn = lblTemp.Content.ToString();
			ConfigsWindow.timeReturn = lblTime.Content.ToString();
			Close();
		}
		//==============================================================
		void slValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
						
			lblTemp.Content = String.Format("{0,4:N1}", 19 + slTemp.Value);
		}
		//==============================================================
		void slTimeHourChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			string a = lblTime.Content.ToString();
			string min =  a.Substring(a.IndexOf(":"));
			double hour = slTimeH.Value * 2.3;		
			string s = "";		
			if(hour < 10) 				s += "0";			
			lblTime.Content = s + (int)hour + min;
		}
		//==============================================================
		void slTimeMinChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			string a = lblTime.Content.ToString();
			string hour =  a.Substring(0, a.IndexOf(":")+1);
			double min = slTimeM.Value * 5.9;
			
			string s = "";
			if(min < 10) 				s += "0";
			lblTime.Content = hour + s + (int)min;
		}
	}
}