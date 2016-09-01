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
	    
	    short [] aBuf = new short[]{102,334,223,123,256,278,267,345,456,234,345,234,
                222,222,222,222,222,222,222,222,222,222,222,222};
	    //==============================================================
		public Window1()
		{
			InitializeComponent();			
			
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
             pnt.Y = aY + TOP_OFFSET+7;

             DrawText(can, "123", pnt, 13, HorizontalAlignment.Left, VerticalAlignment.Center);
             
             pnt = new Point();
             pnt.X = 0;
             pnt.Y = aY + AREA_HEIGH-10;

             DrawText(can, "123", pnt, 13, HorizontalAlignment.Left, VerticalAlignment.Center);
	         
	         for(int i = 0; i < POINTS_CNT-1; i++)	         
	         	addLine (i*HGRID_SPACING + LEFT_OFFSET,       aY + PLOT_HEIGH + TOP_OFFSET - (int)((aBuf[i]   - tmin)*cena),
	         	                 (i+1) *HGRID_SPACING + LEFT_OFFSET, aY + PLOT_HEIGH + TOP_OFFSET - (int)((aBuf[i+1] - tmin)*cena),
	         	                Brushes.Red, 5, can);	        
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
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			plot(Can1);	
			plot(Can2);	
		}
		
		private void CloseButton_Click(object sender, RoutedEventArgs e)
			{
				Close();
			}
	}
}