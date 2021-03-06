﻿/*
 * Created by SharpDevelop.
 * User: Voodoo
 * Date: 31.08.2016
 * Time: 15:26
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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;

namespace TermoWifi
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{				
		const int LEFT_OFFSET = 30;
        const int RIGHT_OFFSET  =  (3);
	    const int X_OFFSET     =    (LEFT_OFFSET + RIGHT_OFFSET);
	    const int TOP_OFFSET   =    (5);
	    const int BOTTOM_OFFSET  =  (5);
	    const int Y_OFFSET       =  (TOP_OFFSET  + BOTTOM_OFFSET);
	
	    int AREA_WIDTH;
	    int AREA_HEIGH;
	    
	    const int aY = 0;
	    
	    Line line;
	    
	    short [] aBuf = new short[]{102,334,223,123,256,278,267,345,456,234,345,234,222,222,222,222,222,222,222,222,222,222,222,222};
	    
	    private static IPAddress masterIPAddress = null;        
		DispatcherTimer nRespTimer;

        BackgroundWorker bgWorker;
        UdpClient udpClient;
        
        bool stop;
        //==============================================================
        struct packStruct
	    {
        	public byte [] data;
			public IPAddress ipAddress;			
	    };
	    //==============================================================
		public Window1()
		{
			InitializeComponent();	
			
			bgWorker = new BackgroundWorker();
			bgWorker.DoWork += backgroundWorker_DoWork;
			bgWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
			bgWorker.WorkerReportsProgress = true;
			bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorkerReport);
			bgWorker.WorkerSupportsCancellation = true;
			
			nRespTimer = new DispatcherTimer();
			nRespTimer.Interval = TimeSpan.FromMilliseconds(2000);
			nRespTimer.Tick += nRespTimer_tick;
			
			udpClient = new UdpClient(7777);			
		}
		//==============================================================
		void nRespTimer_tick(object sender, EventArgs e)
        {
            if(masterIPAddress != null)
			{
				UdpClient udpClient = new UdpClient();
				udpClient.Connect(masterIPAddress.ToString(), 7777);
	            nRespTimer.Start();
				udpClient.Send(udp_send_buf, udp_send_buf.Length);
			}            
        }
		//==============================================================
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{			
			bgWorker.RunWorkerAsync();	
			
//			stop = false;
//			send_udp(8);
			
			UdpClient udpClient = new UdpClient();
			udpClient.Connect("192.168.10.122", 7777);
			byte [] udp_send_buf = Encoding.ASCII.GetBytes("123");
			udpClient.Send(udp_send_buf, udp_send_buf.Length);
		}
	    //==============================================================================================
	    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	    {
			while(!bgWorker.CancellationPending)
			{
				byte [] receiveBytes = null;
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
				
				if (udpClient.Available > 0)
				{
					receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
					packStruct pack;
					pack.data = receiveBytes;
					pack.ipAddress = RemoteIpEndPoint.Address;
					(sender as BackgroundWorker).ReportProgress(50, pack);
				}			
				       
		        Thread.Sleep(100);
			}
			
			if (bgWorker.CancellationPending) e.Cancel = true;
	    }	   
		//==============================================================
		private void btnConfigsClick(object sender, RoutedEventArgs e)
		{
				bgWorker.CancelAsync();
				nRespTimer.Stop();
				
		}		
		//===========================================================================================================================	
		void cWin_Closed(object sender, EventArgs e)
        {
			udpClient = new UdpClient(7777);
			bgWorker.RunWorkerAsync();
        }
	    //===========================================================================================================================	
		bool extTemp = true;		
		//===========================================================================================================================		    
	    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	    {
	    	if(e.Cancelled)
	    	{
	    		if(masterIPAddress != null)
	    		{
	    			udpClient.Close();
	    			ConfigsWindow cWin = new ConfigsWindow();		    	
		    		cWin.masterIP = masterIPAddress;
		    		cWin.Closed +=  new EventHandler(cWin_Closed);
		    		cWin.ShowDialog();	
	    		}
	    	}
	    	else bgWorker.RunWorkerAsync();
	    }
	    //===========================================================================================================================
	    byte [] udp_send_buf;
	    //===========================================================================================================================
		void send_udp( )
		{			
			if(masterIPAddress != null)
			{
				UdpClient udpClient = new UdpClient();
				udpClient.Connect(masterIPAddress.ToString(), 7777);
				
				udp_send_buf = new byte[8];
	            udp_send_buf[0] = (byte) 0x20;
	            
				if(extTemp) udp_send_buf[1] = (byte) 0x80;
				else        udp_send_buf[1] = (byte) 0x00;
	            
	            string str =  DateTime.Now.ToString("yyMMddHHmmss");
	            for(int  i = 0; i < 6; i++)
	                  udp_send_buf[i + 2] = (byte)Convert.ToByte(str.Substring(i * 2, 2));
				
	            nRespTimer.Start();
				udpClient.Send(udp_send_buf, udp_send_buf.Length);
			}
			
		}
		//===========================================================================================================================
		void btnUpdateClick(object sender, RoutedEventArgs e)
		{
			stop = false;
			send_udp();
		}
		//===========================================================================================================================
		private void bgWorkerReport(object sender, ProgressChangedEventArgs e)
	    {
			packStruct pack = (packStruct) e.UserState;
			
			byte [] receiveBytes = pack.data;
			string str = Encoding.ASCII.GetString(receiveBytes);
	    	switch(receiveBytes[0])
		        {
		            case (byte)0x10: // BROADCAST_DATA
						
	    				nRespTimer.Stop();
	    				
	    				str = str.Substring(1,8) + "    ";
			    			
			    			if(str[0] != '0')
			    			{
			    				string s = str.Substring(0,3) + "." + str.Substring(3,1);			    				
			    				if(s[1] == '0') s = s.Substring(0, 1) + "" + s.Substring(2, 3);
			    				inTemp.Content = s;
			    			}
			    			if(str[4] != '0')
			    			{
			    				string s = str.Substring(4,3) + "." + str.Substring(7,1);
			    				if(s[1] == '0') s = s.Substring(0, 1) + "" + s.Substring(2, 3);
			    				outTemp.Content = s;
			    			}
			    			
		    			if(receiveBytes.Length > 9 ) 
		    			{
		    				if(masterIPAddress == null)
		    				{
			    				btnConfigs.Visibility = System.Windows.Visibility.Visible;
			    				masterIPAddress =  pack.ipAddress;
			    				stop = false;
			    				send_udp();
		    				}		    			
					                
					        if(receiveBytes[9] != 0) str = (receiveBytes[11] + ":" + 
			    			                                receiveBytes[10] + ":"+
			    			                                receiveBytes[9] + "\r\n" +
			    			                                receiveBytes[12] + "." +
			    			                                (receiveBytes[13]+1) + "."+
			    			                                receiveBytes[14]);
					               
					        lblTime.Content = str; 
					        timeLbl.Content = "";
		    			}
		                break;
		
		            case (byte) 0x21:// PLOT_DATA_ANS
		                
		                nRespTimer.Stop();
		                
		                timeLbl.Content = "plot data";
	    	
				    	for(int i = 0; i< 24; i++) aBuf[i] = 0;
				    	for(int i = 0; i< 24; i++)
				    		aBuf[i] = BitConverter.ToInt16(new byte[2] { receiveBytes[i*2+1], receiveBytes[i*2+2] }, 0);
				    	
				    	string sign = "";
				    	if(aBuf[aBuf.Length-1] > 0) sign = "+"; 
				    		
				    	 string tmp = sign + String.Format("{0,4:N1}",((float)aBuf[aBuf.Length-1]/10)); 
				    	 if(!stop)
				    	 {
					    	if(extTemp) 
					    	{ 
					    		outTemp.Content = tmp;
					    		plot(Can2); 
					    		extTemp = false;
					    		send_udp();
					    	}
					    	else 	
					    	{ 
					    		inTemp.Content  = tmp;
					    		plot(Can1); 
					    		extTemp = true;
					    		stop = true;
					    	}
				    	 }
		                
		                break;

		
		        }
	    }		
		//==============================================================
		private void addLine (int x, int y, int x2, int y2, Brush col, int thik, Canvas Can)
		{
			line = new Line();
			line.Stroke = col;			
			line.StrokeThickness = thik;	
			
			line.X1 = x;
			line.X2 = x2;
			line.Y1 = y;
			line.Y2 = y2;
			Can.Children.Add(line);
		}
		
		//==============================================================
		private void plot(Canvas can)
		{
			can.Children.Clear();
			AREA_HEIGH = (int) Can1.ActualHeight;
			AREA_WIDTH = (int) Can1.ActualWidth;
			
			int PLOT_WIDTH    =   (AREA_WIDTH - X_OFFSET);
	        int PLOT_HEIGH    =   (AREA_HEIGH - Y_OFFSET);
	        const int POINTS_CNT    =   (24);
	        int HGRID_SPACING =   (PLOT_WIDTH / (POINTS_CNT-1));   // dots between dividings
	        const int VGRID_CNT      =  (10);
	        int VGRID_SPACING  =  (PLOT_HEIGH / VGRID_CNT);			
			
	        addLine(LEFT_OFFSET,  aY + TOP_OFFSET, LEFT_OFFSET +PLOT_WIDTH, aY + TOP_OFFSET, Brushes.Gray, 1, can);
			
			 for(int i = 0; i < VGRID_CNT+1;  i++)			        
			 	addLine(LEFT_OFFSET,  aY + PLOT_HEIGH - i*VGRID_SPACING + TOP_OFFSET, 
			 	        LEFT_OFFSET + PLOT_WIDTH, aY + PLOT_HEIGH - i*VGRID_SPACING + TOP_OFFSET,
			 	       Brushes.Gray, 1, can);

			 for(int i = 0; i < POINTS_CNT; i++)
			 	addLine(i*HGRID_SPACING + LEFT_OFFSET, aY + TOP_OFFSET, 
			 	        i*HGRID_SPACING + LEFT_OFFSET, aY + TOP_OFFSET + PLOT_HEIGH,
			 	       Brushes.Gray, 1, can);
			 
			 short tmax = aBuf[0]; 
			 short tmin = aBuf[0];
	         for(int i = 1; i < POINTS_CNT; i++) if (tmax < aBuf[i]) tmax = aBuf[i]; // tmax
	         for(int i = 1; i < POINTS_CNT; i++) if (tmin > aBuf[i]) tmin = aBuf[i]; // tmin
	
	         tmax /= 10; tmax *= 10; tmax += 10;
	         tmin /= 10; tmin *= 10; tmin -= 10;
	
	         float delta = tmax - tmin;
	
	         float cena = PLOT_HEIGH / delta;
             
             Point pnt = new Point();
             pnt.X = 0;
             pnt.Y = aY + TOP_OFFSET + 7;

             DrawText(can, Convert.ToString(tmax / 10), pnt, 13, HorizontalAlignment.Left, VerticalAlignment.Center);
             
             pnt = new Point();
             pnt.X = 0;
             pnt.Y = aY + AREA_HEIGH-10;

             DrawText(can, Convert.ToString(tmin / 10), pnt, 13, HorizontalAlignment.Left, VerticalAlignment.Center);
	         
	         for(int i = 0; i < POINTS_CNT-1; i++)	         
	         	addLine (i*HGRID_SPACING + LEFT_OFFSET,       aY + PLOT_HEIGH + TOP_OFFSET - (int)((aBuf[i]   - tmin)*cena),
	         	                 (i+1) *HGRID_SPACING + LEFT_OFFSET, aY + PLOT_HEIGH + TOP_OFFSET - (int)((aBuf[i+1] - tmin)*cena),
	         	                Brushes.Red, 3, can);	        
		}
		//==============================================================
		// Position a label at the indicated point.
		private void DrawText(Canvas can, string text, Point location,
		    double font_size,
		    HorizontalAlignment halign, VerticalAlignment valign)
		{
		    // Make the label.
		    Label label = new Label();
		    label.Content = text;
		    label.FontSize = font_size;
		    can.Children.Add(label);
		
		    // Position the label.
		    label.Measure(new Size(double.MaxValue, double.MaxValue));
		
		    double x = location.X;
		    if (halign == HorizontalAlignment.Center)
		        x -= label.DesiredSize.Width / 2;
		    else if (halign == HorizontalAlignment.Right)
		        x -= label.DesiredSize.Width;
		    Canvas.SetLeft(label, x);
		
		    double y = location.Y;
		    if (valign == VerticalAlignment.Center)
		        y -= label.DesiredSize.Height / 2;
		    else if (valign == VerticalAlignment.Bottom)
		        y -= label.DesiredSize.Height;
		    Canvas.SetTop(label, y);
		}		
		//==============================================================
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{			
			//bgWorker.CancelAsync();
			Close();
		}
		
		//===========================================================================================================================
		private void rectangle2_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
		
	}
	
}