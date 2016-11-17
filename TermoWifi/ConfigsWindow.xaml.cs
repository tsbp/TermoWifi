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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
		
		
		const byte PLOT_DATA			=		(0x20);
	    const byte PLOT_DATA_ANS		=		(0x21);
	
	    const byte READ_WEEK_CONFIGS	=		(0x30);
	    const byte READ_WEEK_CONFIGS_ANS	=	(0x31);
	    const byte SAVE_WEEK_CONFIGS	=		(0x32);
	
	    const byte READ_DAY_CONFIGS	=		(0x33);
	    const byte READ_DAY_CONFIGS_ANS	=	(0x34);
	    const byte SAVE_DAY_CONFIGS	=		(0x35);
	
	    const byte READ_USTANOVKI	=		(0x36);
	    const byte READ_USTANOVKI_ANS=		(0x37);
	    const byte SAVE_USTANOVKI	=		(0x38);
	
	    
	    private BackgroundWorker bg_Worker;
	    
	    private static IPAddress remoteIPAddress;        
        private static int localPort;
        
        UdpClient udpClient;
        
        
	    		
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
				
				remoteIPAddress = IPAddress.Parse ("192.168.10.122");
				localPort = 7777;
				
				
				RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
				
				bg_Worker = new BackgroundWorker();
				bg_Worker.DoWork += bgWorker_DoWork;
				bg_Worker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
				bg_Worker.WorkerSupportsCancellation  = true;
				bg_Worker.WorkerReportsProgress = true;
			    bg_Worker.ProgressChanged += new ProgressChangedEventHandler(bg_WorkerReport);
				bg_Worker.RunWorkerAsync();
				
				udpClient = new UdpClient(7777);
				
		}
		//==============================================================
		private void bg_WorkerReport(object sender, ProgressChangedEventArgs e)
	    {
			byte [] receiveBytes = e.UserState as byte[];
			if(receiveBytes != null)
			switch(receiveBytes[0])
	    	{
	    			
	    		case READ_WEEK_CONFIGS_ANS:
	    			
	    			for(int i = 0; i < 7; i++)
	    			{
	    				string path = "/TermoWifi;component/drawable/";
	    				if((dayType & (1 << i)) != 0) 	path += "beer.png";		
	    				else 							path += "shovel.png";
	    				items.Add(new User() {PicTime = path, Time = weekDays[i]});
	    			}
	    			break;	    				    			
	    	}
		}
		//==============================================================================================		
		IPEndPoint RemoteIpEndPoint;
		//==============================================================================================
	    private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
	    {
	    	
			while(!bg_Worker.CancellationPending)
			{
				byte [] receiveBytes = null;
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
				
				if (udpClient.Available > 0)
				{
					receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
					(sender as BackgroundWorker).ReportProgress(50, receiveBytes);
				}		
				       
		        Thread.Sleep(100);
			}
			
			if(bg_Worker.CancellationPending) e.Cancel = true;
	    }	 
		//===========================================================================================================================		    
	    private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	    {	
	    	if(!e.Cancelled)
	    		bg_Worker.RunWorkerAsync();
	    	else
	    	{
	    		udpClient.Close();
	    		Close();
	    	}
	    } 	    
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			bg_Worker.CancelAsync();				
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
			send_udp(MODE_GET_WEEK);
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
		
		
		//===========================================================================================================================
		byte [] udp_send_buf;
		string ipString = "192.168.10.122";
		//===========================================================================================================================
		void send_udp( int aCmd)
		{
			UdpClient udpClient = new UdpClient();
			udpClient.Connect(ipString, localPort);
			
			if(!bg_Worker.IsBusy) bg_Worker.RunWorkerAsync();
			
			switch(aCmd)
			{
				case MODE_GET_WEEK:
					
					udp_send_buf = new byte[1];
					udp_send_buf[0] = (byte) READ_WEEK_CONFIGS;
					
					udpClient.Send(udp_send_buf, 1);
				break;
//				
//				case default:
//				break;
			}
			
			        
			
			
		}
	}
}