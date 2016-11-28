/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 21.11.2016
 * Time: 16:28
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
	/// Interaction logic for Hysteresys_cfg.xaml
	/// </summary>
	public partial class Hysteresys_cfg : Window
	{
		
		public float hystValue;
		
		public Hysteresys_cfg()
		{
			InitializeComponent();
			slHyst.Value = (this.hystValue);
			lblHyst.Content = String.Format("{0,4:N1}", this.hystValue);
		}
		
		public Hysteresys_cfg(float aVal)
		{
			InitializeComponent();
			slHyst.Value = (aVal);
			lblHyst.Content = String.Format("{0,4:N1}", aVal);
		}
		
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			hystValue = (float)(slHyst.Value);
			Close();
		}
		//==============================================================
		void slValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
						
			lblHyst.Content = String.Format("{0,4:N1}", slHyst.Value);
		}
	}
}