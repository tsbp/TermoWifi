/*
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
using System.Threading;
using System.ComponentModel;

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
	    
	    private static IPAddress remoteIPAddress;        
        private static int localPort;
        
        BackgroundWorker bgWorker;
        DispatcherTimer dispatcherTimer;
	    //==============================================================
		public Window1()
		{
			InitializeComponent();	
			
			
		}		
		//==============================================================
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			 dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
//            dispatcherTimer.Start();
			
            remoteIPAddress = IPAddress.Parse ("192.168.10.122");
			localPort = 7777;
			
			bgWorker = new BackgroundWorker();
			bgWorker.DoWork += backgroundWorker_DoWork;
			bgWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
			bgWorker.RunWorkerAsync();
			
//			Thread thdUDPServer = new Thread(new ThreadStart(serverThread));
//			thdUDPServer.Start();
			
            
		}	
		//==============================================================
		private void dispatcherTimer_Tick(object sender, EventArgs e)
		{
		   stop = false;
			if(extTemp) extTemp = false;
			else extTemp = true;
			send_udp(8);
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
			 
			 int tmax = aBuf[0]; 
			 int tmin = aBuf[0];
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
			Close();
		}
		//==============================================================
		private void btnConfigsClick(object sender, RoutedEventArgs e)
		{
			ConfigsWindow cWin = new ConfigsWindow();			
			cWin.ShowDialog();       
		}		
		//==============================================================================================		
	    string rTmp1 = "____";
	    string rTmp2 = "____";
	    Byte[] receiveBytes;
	    //==============================================================================================
	    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
	    {
	    	UdpClient udpClient = new UdpClient(localPort);
	    	
	    	bool packRec = false;
			while(!packRec)
			{
				receiveBytes = null;
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
				receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
				string str = Encoding.ASCII.GetString(receiveBytes);	
				
				//string str = "";
				
				
				switch(receiveBytes[0])
		        {
		            case (byte)0x10: // BROADCAST_DATA
						
						
						if(RemoteIpEndPoint.Address.ToString().Equals("192.168.10.122"))
						{
							ipString = RemoteIpEndPoint.Address.ToString();
							localPort = RemoteIpEndPoint.Port;
							str = str.Substring(1,9) + "    ";
		                
			                if(str[0] != '0')
			                	rTmp1 = str.Substring(0,4);
			                if(str[4] != '0')
			                	rTmp2 = str.Substring(4,4);
			                
			                if(receiveBytes[9] != 0) str = (receiveBytes[11] + ":" + 
			                                                receiveBytes[10] + ":"+ 
			                                                receiveBytes[9] + ", " + 
			                                                receiveBytes[12] + "." + 
			                                                (receiveBytes[13]+1) + "."+ 
			                                                receiveBytes[14]);
			                
			                timeLbl.Dispatcher.BeginInvoke((Action)(() => timeLbl.Content = rTmp1 + "  ...  " + rTmp2 + " \\" + str + " \\"));
						}
		                break;
		
		            case (byte) 0x21:// PLOT_DATA_ANS
		                //timeLbl.Dispatcher.BeginInvoke((Action)(() => timeLbl.Content = "plot data"));
//		                timeLbl.Content = "plot data";
//		                if(receiveBytes[9] != 0) str = receiveBytes[11] + "";
		                packRec = true;
		                break;
//		
//		            default: //case (byte) 0xAA: //OK_ANS
//		                tvTmp.setText("SAVED!!!");
//		                break;
		
		        }
		        if(receiveBytes.Length > 12)
		        {
		
		        }
		        Thread.Sleep(100);
			}
	    }
	    //===========================================================================================================================	
		bool extTemp = false;	  
		bool stop = false;		
		//===========================================================================================================================		    
	    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	    {
	    	bgWorker.RunWorkerAsync();
	    	timeLbl.Content = "plot data";
	    	
	    	for(int i = 0; i< 24; i++) aBuf[i] = 0;
	    	for(int i = 0; i< 24; i++)
	    		aBuf[i] = BitConverter.ToInt16(new byte[2] { receiveBytes[i*2+1], receiveBytes[i*2+2] }, 0);
	    	
	    	string sign = "+";
	    	if(aBuf[aBuf.Length-1] < 0) 
	    	{
	    		sign = "-";
	    		aBuf[aBuf.Length-1] *= -1; 
	    	}
	    		
	    	 string tmp = sign + String.Format("{0,4:N1}",((float)aBuf[aBuf.Length-1]/10));         
	    	if(extTemp & !stop) 
	    	{ 
	    		outTemp.Content = tmp;
	    		plot(Can2); 
//	    		extTemp = false;
//	    		stop = true;
	    	}
	    	else if(!stop)		
	    	{ 
	    		inTemp.Content  = tmp;
	    		plot(Can1); 
//	    		extTemp = true;
//	    		send_udp(8);
	    	}
		    
		 
	    	
	    } 
		//===========================================================================================================================
		byte [] udp_send_buf;
		string ipString;
		//===========================================================================================================================
		void send_udp( int aBufLng)
		{
			UdpClient udpClient = new UdpClient();
			udpClient.Connect(ipString, localPort);	
			
			udp_send_buf = new byte[aBufLng];
            udp_send_buf[0] = (byte) 0x20;
            
			if(extTemp) udp_send_buf[1] = (byte) 0x80;
			else        udp_send_buf[1] = (byte) 0x00;
            
            string str =  DateTime.Now.ToString("yyMMddHHmmss");
            for(int  i = 0; i < 6; i++)
                  udp_send_buf[i + 2] = (byte)Convert.ToByte(str.Substring(i * 2, 2));
			
			udpClient.Send(udp_send_buf, aBufLng);
		}
		//===========================================================================================================================
		void btnUpdateClick(object sender, RoutedEventArgs e)
		{
//			stop = false;
//			if(extTemp) extTemp = false;
//			else extTemp = true;
//			send_udp(8);
			 dispatcherTimer.Start();
		}
		//===========================================================================================================================
		private void rectangle2_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
		
	}
	
}