﻿#pragma checksum "..\..\PartConfig.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "857AD7E53E512429FB63F5AD2F41A5EB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace TermoWifi {
    
    
    /// <summary>
    /// PartConfig
    /// </summary>
    public partial class PartConfig : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\PartConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slTemp;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\PartConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTemp;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\PartConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTime;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\PartConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slTimeH;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\PartConfig.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider slTimeM;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/TermoWifi;component/partconfig.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PartConfig.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 7 "..\..\PartConfig.xaml"
            ((TermoWifi.PartConfig)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 20 "..\..\PartConfig.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.slTemp = ((System.Windows.Controls.Slider)(target));
            
            #line 24 "..\..\PartConfig.xaml"
            this.slTemp.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slValueChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lblTemp = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblTime = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.slTimeH = ((System.Windows.Controls.Slider)(target));
            
            #line 50 "..\..\PartConfig.xaml"
            this.slTimeH.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slTimeHourChanged);
            
            #line default
            #line hidden
            return;
            case 7:
            this.slTimeM = ((System.Windows.Controls.Slider)(target));
            
            #line 55 "..\..\PartConfig.xaml"
            this.slTimeM.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.slTimeMinChanged);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

