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
using System.Windows.Threading;
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
		
//		const int MODE_GET_WEEK  = 0;
//		const int MODE_GET_DAY_GFG  = 1;
//		const int MODE_GET_HYST  = 2;
//		//const int MODE_GET_HOLLY = 2;
		int currentMode;
		
		const byte  OK_ANS			=				(0xAA);
		const byte  BAD_ANS			=				(0xEE);
		
		
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
	    
	    
	    const int trysToReceiveCount = 5;
	    int trysToReceive = 0;
	    
	    private BackgroundWorker bg_Worker;
        
        private UdpClient udpClient;
        public IPAddress masterIP;
        
        DispatcherTimer nRespTimer;
        
	    		
		public class User
        {
			public string number { get; set; }
			public string PicTime { get; set; }
			public string Time { get; set; }
			public string PicTemp { get; set; }
			public string Temp { get; set; }
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
				
				RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
				
				bg_Worker = new BackgroundWorker();
				bg_Worker.DoWork += bgWorker_DoWork;
				bg_Worker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
				bg_Worker.WorkerSupportsCancellation  = true;
				bg_Worker.WorkerReportsProgress = true;
			    bg_Worker.ProgressChanged += new ProgressChangedEventHandler(bg_WorkerReport);
				bg_Worker.RunWorkerAsync();
				
				nRespTimer = new DispatcherTimer();
				nRespTimer.Interval = TimeSpan.FromMilliseconds(2000);
				nRespTimer.Tick += nRespTimer_tick;
				
				udpClient = new UdpClient(7777);				
		}
		//==============================================================
		void startTimeout()
		{
			pbWait.Visibility = Visibility.Visible;
			//trysToReceive = 0;
			nRespTimer.Start();
		}
		//==============================================================
		void nRespTimer_tick(object sender, EventArgs e)
        {
            if(masterIP != null)
			{
            	trysToReceive++;
            	if(trysToReceive > trysToReceiveCount)
            		MessageBox.Show("Could not get data!", "Error");
            	else
            	{
					UdpClient udpClient = new UdpClient();
					udpClient.Connect(masterIP.ToString(), 7777);				
		            //startTimeout();
					udpClient.Send(udp_send_buf, udp_send_buf.Length);					
            	}
			}            
        }
		//=============================================================================================
		bool hDayCfg = false;
		//=============================================================================================
		private void bg_WorkerReport(object sender, ProgressChangedEventArgs e)
	    {
			byte [] receiveBytes = e.UserState as byte[];
			if(receiveBytes != null)
			switch(receiveBytes[0])
	    	{
	    			
	    		case READ_WEEK_CONFIGS_ANS:
					pbWait.Visibility = Visibility.Hidden;
					nRespTimer.Stop();
	    			cfgTitle.Content = "Неделя";
	    			for(int i = 0; i < 7; i++)
	    			{
	    				string path = "/TermoWifi;component/drawable/";
	    				if((dayType & (1 << i)) != 0) 	path += "beer.png";		
	    				else 							path += "shovel.png";
	    				items.Add(new User() {PicTime = path, Time = weekDays[i]});
	    			}
	    			break;
	    			
	    		case READ_DAY_CONFIGS_ANS:
	    			
	    			nRespTimer.Stop();
	    			int pNumb  = receiveBytes[1] & 0x0f;
	    			int pCount = receiveBytes[2];
	    			byte param =  Convert.ToByte((receiveBytes[1] & 0x0f) + 1);
	    			
	    			pbWait.Value = (100 / pCount) * pNumb;
	    			
	    			if(hDayCfg) param |= (byte)0x80;
	    			
	    			if(hDayCfg)
	    				cfgTitle.Content = "Выходные";
	    			else
	    				cfgTitle.Content = "Будни";
	    			
	    			string h = "", m = "";
	    			if (receiveBytes[3] < 10) h = "0";
	    			if (receiveBytes[4] < 10) m = "0";
	    			
	    			
	    			items.Add(new User() {
	    			          	number = pNumb.ToString(),// + " of " + pCount,
	    			          	PicTime = "/TermoWifi;component/drawable/timeicon.png",
	    			          	Time = h + receiveBytes[3] + ":" + m + receiveBytes[4],
	    			          	PicTemp = "/TermoWifi;component/drawable/tempicon.png",
	    			          	Temp = (String.Format("{0,4:N1}", (float)BitConverter.ToInt16(new byte[2] { receiveBytes[5], receiveBytes[6] }, 0)/10))});
	    			
	    			if(pNumb < pCount)
	    				send_udp(READ_DAY_CONFIGS, param);
	    			else
	    				pbWait.Visibility = Visibility.Hidden;
	    			
	    			break;
	    			
	    		case READ_USTANOVKI_ANS:
	    			nRespTimer.Stop();	    			
	    			pbWait.Visibility = Visibility.Hidden;
	    			//hystWin.hystValue = (float)((float)receiveBytes[1] / 10);
	    			Hysteresys_cfg hystWin = new Hysteresys_cfg((float)((float)receiveBytes[1] / 10));
	    			hystWin.Closed += hystWinClosed;
	    			hystWin.ShowDialog();
	    			break;
	    			
	    		case OK_ANS:
	    			pbWait.Visibility = Visibility.Hidden;
	    			nRespTimer.Stop();
	    			break;
	    			
	    		case BAD_ANS:
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
			nRespTimer.Stop();
		}		
		//==============================================================
		private void Button_Click_GetHolly(object sender, RoutedEventArgs e)
		{
			btnAdd.Visibility = System.Windows.Visibility.Visible;
			trysToReceive = 0;
			items.Clear();
			pbWait.Value = 0;
			hDayCfg = true;
			currentMode = READ_DAY_CONFIGS;
			send_udp(READ_DAY_CONFIGS, 0x81);			
		}		
		//==============================================================
		private void Button_Click_GetWork(object sender, RoutedEventArgs e)
		{
			btnAdd.Visibility = System.Windows.Visibility.Visible;
			trysToReceive = 0;
			items.Clear();
			pbWait.Value = 0;
			hDayCfg = false;
			currentMode = READ_DAY_CONFIGS;
			send_udp(READ_DAY_CONFIGS, 0x01);			
		}
		//==============================================================
		private void Button_Click_hyst(object sender, RoutedEventArgs e)
		{
			btnAdd.Visibility = System.Windows.Visibility.Hidden;
			trysToReceive = 0;
			pbWait.Value = 0;
			currentMode = READ_USTANOVKI;
			send_udp(currentMode,0);
		}
		//==============================================================
		public void hystWinClosed(object sender, System.EventArgs e)
        {           
            currentMode = SAVE_USTANOVKI;
            Hysteresys_cfg s  = sender as Hysteresys_cfg;
            double a = s.hystValue;            
            send_udp(currentMode, (byte)(a * 10));
        }
		//==============================================================
		private void Button_Click_add(object sender, RoutedEventArgs e)
		{
			
			items.Add(new User() {
			          	number = (items.Count + 1).ToString(),// + " of " + pCount,
			          	PicTime = "/TermoWifi;component/drawable/timeicon.png",
			          	Time ="00:00",
			          	PicTemp = "/TermoWifi;component/drawable/tempicon.png",
			          	Temp = "19,0"});
			sortbytime();
		}		
		//==============================================================
		void sortbytime()
		{
			int [] time = new int[items.Count];
			string [] temp = new string[items.Count];
			
			for (int i = 0; i < time.Length; i++)
			{
				time[i] = int.Parse((items[i].Time).Replace(":", ""));
				temp[i] = items[i].Temp;
			}
			
			for (int i = 0; i < time.Length; i++)
				for (int j = i; j < time.Length; j++)
					if(time[i] > time[j])
					{
						int a = time[j];
						time[j] = time[i];
						time[i] = a;
						
						string s = temp[j];
						temp[j] = temp[i];
						temp[i] = s;
					}
			
			items.Clear();
			for (int i = 0; i < time.Length; i++)
			{
				string s;
				if(time[i] < 10) s = "00:0" + time[i];
				else if(time[i] < 60) s = "00:" + time[i];
				else if(time[i] < 1000)
				{
					s = "0" + (time[i] / 100) + ":" ;
					if(time[i] % 100 < 10) s += "0" + (time[i] % 100);
					   else  s += (time[i] % 100);
				}
				else 
				{
					s = (time[i] / 100) + ":";
					if(time[i] % 100 < 10) s += "0" + (time[i] % 100);
					   else  s += (time[i] % 100);
				}
					
				items.Add(new User() {
			          	number = (items.Count + 1).ToString(),// + " of " + pCount,
			          	PicTime = "/TermoWifi;component/drawable/timeicon.png",
			          	Time = s,
			          	PicTemp = "/TermoWifi;component/drawable/tempicon.png",
			          	Temp = temp[i]});
			}
		}
		//==============================================================
		string [] weekDays = {"Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье"};
		byte dayType	= 0x60;
		//==============================================================
		void Button_Click_week(object sender, RoutedEventArgs e)
		{
			btnAdd.Visibility = System.Windows.Visibility.Hidden;
			items.Clear();
			currentMode = READ_WEEK_CONFIGS;
			send_udp(READ_WEEK_CONFIGS, 0);
		}
		//==============================================================
		void lvConfigs_del_item(object sender, MouseButtonEventArgs e)
		{
			if(System.Windows.MessageBox.Show("Delete item number  ?", "Your Caption Here", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				//this.Close();
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
				case READ_WEEK_CONFIGS:
					dayType ^= (byte)(1 << currentItem);
					
					string path = "/TermoWifi;component/drawable/";
					if((dayType & (1 << currentItem)) != 0) 	items[currentItem].PicTime = path + "beer.png";
					else 										items[currentItem].PicTime = path + "shovel.png";					
					lvConfigs.Items.Refresh();
					break;
					
				case READ_DAY_CONFIGS:
					sTemp = items[currentItem].Temp;
			        sTime = items[currentItem].Time;	      
					
					PartConfig cWin = new PartConfig();		
					cWin.lblTemp.Content = sTemp;
					cWin.lblTime.Content = sTime;
					cWin.Closed += cWinClosed;
					cWin.Show();
					break;
			}	       
		}
		//==============================================================
		public void cWinClosed(object sender, System.EventArgs e)
        {           
            items[currentItem].Temp = tempReturn;
            items[currentItem].Time = timeReturn;
            sortbytime();
            lvConfigs.Items.Refresh();
        }
		//===========================================================================================================================
		private void rectangle2_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
		//===========================================================================================================================
		byte [] udp_send_buf;
		//===========================================================================================================================
		void send_udp( int aCmd, byte aParam)
		{
			startTimeout();
			UdpClient udpClient = new UdpClient();
			udpClient.Connect(masterIP.ToString(), 7777);
			
			if(!bg_Worker.IsBusy) bg_Worker.RunWorkerAsync();
			
			switch(aCmd)
			{
				case READ_WEEK_CONFIGS:
					
					udp_send_buf = new byte[1];
					udp_send_buf[0] = (byte) aCmd;					
					udpClient.Send(udp_send_buf, 1);
					break;
				
				case READ_DAY_CONFIGS:
					udp_send_buf = new byte[2];
					udp_send_buf[0] = (byte) aCmd;
					udp_send_buf[1] = aParam;
					udpClient.Send(udp_send_buf, 2);
					break;
					
				case READ_USTANOVKI:
					udp_send_buf = new byte[1];
					udp_send_buf[0] = (byte) aCmd;					
					udpClient.Send(udp_send_buf, 1);
					break;
					
				case SAVE_USTANOVKI:
					udp_send_buf = new byte[3];
					udp_send_buf[0] = (byte) aCmd;
					udp_send_buf[1] = (byte) aParam;
					udp_send_buf[2] = (byte) 0;
					udpClient.Send(udp_send_buf, 3);
					break;
			}			
		}
	}
}