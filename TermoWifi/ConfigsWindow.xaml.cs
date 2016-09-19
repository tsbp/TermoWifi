/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 02.09.2016
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TermoWifi
{
	/// <summary>
	/// Interaction logic for ConfigsWindow.xaml
	/// </summary>
	public partial class ConfigsWindow : Window
	{
		public ObservableCollection<User> items = new ObservableCollection<User>();
		 
		public static string  tempReturn;
		public static string  timeReturn;
		
		public class User
        {
                public string PicTime { get; set; }
                public string Time { get; set; }
                public string PicTemp { get; set; }
                public string Temp { get; set; }
//                public Button btn;
        }
		//==============================================================
		public ConfigsWindow()
		{
			InitializeComponent();
		}
		//==============================================================
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
					
//				List<User> items = new List<User>();
				lvConfigs.ItemsSource = items;
//				items.Add(new User() {  Time = "18:44",   Temp = "25.4"});
//				items.Add(new User() {  Time = "22:27",   Temp = "21.8"});
				//items.Add(new User() { PicTime = new Image { Width = 16, Height = 16, Source = new BitmapImage(new Uri("pack://application:,,,/TermoWifi;component/drawable/timeicon.png")) }, Time = "12:24",   Temp = "23.4"});
				
//				
//				ListBoxItem lbItem = new ListBoxItem();
//				lbItem.Content = "123";
//				lbConfigs.Items.Add(lbItem);
		}					
//		//==============================================================
//		private void lvButton_Click(object sender, RoutedEventArgs e)
//		{            			
//			FrameworkElement fe = (FrameworkElement)sender;
//			object it = fe.DataContext;
//			int aa = lvConfigs.Items.IndexOf(it);
//			
//	        string sTemp = null, sTime = null;
//	        
//			ConfigsWindow.User a = it as ConfigsWindow.User;
//			if (a!=null)
//			{
//				sTemp = a.Temp;
//				sTime = a.Time;
//			}
//			
//			PartConfig cWin = new PartConfig();		
//			cWin.lblTemp.Content = sTemp;
//			cWin.lblTime.Content = sTime;
//			cWin.Show();
//
//		}
		
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		//==============================================================
		void Button_Click(object sender, RoutedEventArgs e)
		{
			items.Add(new User() {
			          	PicTime = "/TermoWifi;component/drawable/timeicon.png",
			          	Time = "12:24", 
			          	PicTemp = "/TermoWifi;component/drawable/tempicon.png", 
			          	Temp = "28,2"});
		}
		//==============================================================
		int currentItem;
		//==============================================================
		void lvConfigs_SelectionChanged(object sender, MouseButtonEventArgs e)
		{
			ListView fe = (ListView)sender;
			
	        string sTemp = null, sTime = null;
	        currentItem = fe.SelectedIndex;
	        
	        sTemp = items[fe.SelectedIndex].Temp;
	        sTime = items[fe.SelectedIndex].Time;	      
			
			PartConfig cWin = new PartConfig();		
			cWin.lblTemp.Content = sTemp;
			cWin.lblTime.Content = sTime;
			cWin.Closed += cWinClosed;
			cWin.Show();
		}
		//==============================================================
		public void cWinClosed(object sender, System.EventArgs e)
        {           
            items[currentItem].Temp = tempReturn;
            items[currentItem].Time = timeReturn;
            lvConfigs.Items.Refresh();
        }
	}
}