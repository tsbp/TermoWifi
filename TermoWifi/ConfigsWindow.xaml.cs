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
		
		const int MODE_GET_WEEK  = 0;
		const int MODE_GET_WORK  = 1;
		const int MODE_GET_HOLLY = 2;
		int currentMode = MODE_GET_WEEK;
		
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
				lvConfigs.ItemsSource = items;
		}					
	
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
		//==============================================================
		void Button_Click(object sender, RoutedEventArgs e)
		{
			currentMode = MODE_GET_WORK;
			items.Add(new User() {
			          	PicTime = "/TermoWifi;component/drawable/timeicon.png",
			          	Time = "12:24", 
			          	PicTemp = "/TermoWifi;component/drawable/tempicon.png", 
			          	Temp = "28,2"});
		}
		//==============================================================
		string [] weekDays = {"Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье"};
		byte dayType	= 0x60;
		//==============================================================
		void Button_Click_week(object sender, RoutedEventArgs e)
		{
			currentMode = MODE_GET_WEEK;
			
			for(int i = 0; i < 7; i++)
			{
				string path = "/TermoWifi;component/drawable/";
				if((dayType & (1 << i)) != 0) 	path += "beer.png";
				else 							path += "shovel.png";
				items.Add(new User() {PicTime = path, Time = weekDays[i]});
			}
			
		}
		//==============================================================
		int currentItem;
		//==============================================================
		void lvConfigs_SelectionChanged(object sender, MouseButtonEventArgs e)
		{			
			ListView fe = (ListView)sender;
			currentItem = fe.SelectedIndex;
			string sTemp = null, sTime = null;
			
			switch(currentMode)
			{
				case MODE_GET_WEEK:
					dayType ^= (byte)(1 << currentItem);
					
					string path = "/TermoWifi;component/drawable/";
					if((dayType & (1 << currentItem)) != 0) 	items[currentItem].PicTime = path + "beer.png";
					else 										items[currentItem].PicTime = path + "shovel.png";
					
					lvConfigs.Items.Refresh();
					break;
					
				case MODE_GET_WORK:
					sTemp = items[currentItem].Temp;
			        sTime = items[currentItem].Time;	      
					
					PartConfig cWin = new PartConfig();		
					cWin.lblTemp.Content = sTemp;
					cWin.lblTime.Content = sTime;
					cWin.Closed += cWinClosed;
					cWin.Show();
					break;
					
				case MODE_GET_HOLLY:
					break;
			}
	       
		}
		//==============================================================
		public void cWinClosed(object sender, System.EventArgs e)
        {           
            items[currentItem].Temp = tempReturn;
            items[currentItem].Time = timeReturn;
            lvConfigs.Items.Refresh();
        }
		//===========================================================================================================================
		private void rectangle2_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
	}
}