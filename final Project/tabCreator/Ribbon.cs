using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using final_Project.tabCreator.ButtonClass;
using System.Reflection;
using System;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using final_Project;

namespace Addin
{
    public static class BitmapExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
    }
    public class Ribbon
    {
        #region create new tab

        public const string tabTitle = "InTake-43";
        public const string tabId = "10 10";

        [CommandMethod("TabCreator")]
        public void TabCreator()
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon != null)
            {
                RibbonTab ribbonTab = ribbon.FindTab(tabId);
                if (ribbonTab != null)
                {
                    ribbon.Tabs.Remove(ribbonTab);
                }

                ribbonTab = new RibbonTab();
                ribbonTab.Title = tabTitle;
                ribbonTab.Id = tabId;
                ribbon.Tabs.Add(ribbonTab);
                AddContentToTab(ribbonTab);
            }
        }
        #endregion

        #region create one panel

        private void AddContentToTab(RibbonTab ribbonTab)
        {
            ribbonTab.Panels.Add(AddPanelOne());
        }
        private static RibbonPanel AddPanelOne()
        {
            RibbonPanelSource rps = new RibbonPanelSource();
            rps.Title = "InTake-43";
            RibbonPanel rp = new RibbonPanel();
            rp.Source = rps;
            RibbonButton rci = new RibbonButton();
            rci.Name = "ITI Addin";
            rps.DialogLauncher = rci;

            #region create Panel Content

            #region Big Button
            //create Big button
            var addinAssembly = typeof(Ribbon).Assembly;
            RibbonButton BigButton = new RibbonButton
            {
                ShowImage = true,
                //LargeImage = GetEmbeddedPng(addinAssembly, "Resouces/ITI.png"),
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Text = "Big Button",
                Name = "ITI",
                Description = "ITI Plugin",
                ShowText = true,
                //CommandHandler = new RelayCommand();
            };
            #endregion

            #region Horizontal Buttons

            #region Horizontal button (1)
            // Horizontal button (1)
            var addinAssembly1 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton1 = new RibbonButton
            {
                ShowImage = true,
                Image = Resource1.beam2.ToBitmapImage(),
                AllowInStatusBar = true,
                Orientation = Orientation.Horizontal,
                Size = RibbonItemSize.Standard,
                Text = "  Beam Tag               ",
                ShowText = true,
                Name = "Beam Tag",
                Description = "this button will help you to generate all the beam tags at once",
                IsToolTipEnabled = true,
                //ToolTip = "Big button vertical",
                //CommandParameter = "BigButtonCommand ",
                MinWidth = 120,
                CommandHandler = new RelayCommand(new BeamTag().Execute)
            };
            #endregion

            #region Horizontal button (2)
            // Horizontal button (2)
            var addinAssembly2 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton2 = new RibbonButton
            {
                IsToolTipEnabled = true,
                //ToolTip = "Big button vertical",
                //CommandParameter = "BigButtonCommand ",
                Image = Resource1.redAxes.ToBitmapImage(),
                Orientation = Orientation.Horizontal,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Standard,
                Text = "  Axes Dimension     ",
                Name = "Axes Dimension",
                Description = "generate the dimensions between all the axes at once",
                ShowText = true,
                MinWidth = 120,
                CommandHandler = new RelayCommand(new AxesDim().Execute)
            };
            #endregion

            #region Horizontal button (3)
            // Horizontal button (3)
            var addinAssembly3 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton3 = new RibbonButton
            {
                //IsToolTipEnabled = true,
                //ToolTip = "Big button vertical",
                //CommandParameter = "BigButtonCommand ",
                Image = Resource1.frame.ToBitmapImage(),
                Orientation = Orientation.Horizontal,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Standard,
                Text = "  Column Dimension",
                Name = "Column Dimension",
                Description = "generate all the column dimensions at once",
                ShowText = true,
                MinWidth = 120,
                CommandHandler = new RelayCommand(new ColumnDim().Execute)
            };
            #endregion

            #region Horizontal button (4)
            // Horizontal button (4)
            var addinAssembly4 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton4 = new RibbonButton
            {
                IsToolTipEnabled = true,
                //ToolTip = "Big button vertical",
                //CommandParameter = "BigButtonCommand ",
                //Image = LoadImage(My.Resources.question),
                ShowImage=true,
                LargeImage= Resource1.safwat.ToBitmapImage(),
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Text = "Column Tag",
                Name = "Column Tag",
                Description = "generate all the column tags at once",
                ShowText = true,
                CommandHandler = new RelayCommand(new ColumnTag().Execute)
            };
            #endregion

            #region Horizontal button (5)
            // Horizontal button (5)
            var addinAssembly5 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton5 = new RibbonButton
            {
                IsToolTipEnabled = true,
                //ToolTip = "Big button vertical",
                //CommandParameter = "BigButtonCommand ",
                //Image = LoadImage(My.Resources.question),
                ShowImage=true,
                LargeImage=Resource1.safwat.ToBitmapImage(),
                Orientation = Orientation.Vertical,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Large,
                Text = "quantity Surveyor",
                Name = "quantity Surveyor",
                Description = "quantity Survey for footings, columns, beams & walls",
                ShowText = true,
                CommandHandler = new RelayCommand(new QuantitySurvey().Execute)
            };
            #endregion

            #region Horizontal button (6)
            // Horizontal button (6)
            var addinAssembly6 = typeof(Ribbon).Assembly;
            RibbonButton HorizontalButton6 = new RibbonButton
            {
                IsToolTipEnabled = true,
                ToolTip = "Big button vertical",
                CommandParameter = "BigButtonCommand ",
                //Image = LoadImage(My.Resources.question),
                Orientation = Orientation.Horizontal,
                AllowInStatusBar = true,
                Size = RibbonItemSize.Standard,
                Text = "Horizontal button (6)",
                Name = "ITI",
                Description = "ITI Plugin",
                ShowText = true,
                //CommandHandler = new RelayCommand(new SelectOnScreen().Execute)
            };
            #endregion

            #endregion

            #region SubPanels
            // SubPanel (1)
            RibbonRowPanel subPanel1 = new RibbonRowPanel();
            subPanel1.Items.Add(HorizontalButton1);
            subPanel1.Items.Add(new RibbonRowBreak());
            subPanel1.Items.Add(HorizontalButton2);
            subPanel1.Items.Add(new RibbonRowBreak());
            subPanel1.Items.Add(HorizontalButton3);
            subPanel1.Items.Add(new RibbonRowBreak());

            // SubPanel (2)
            RibbonRowPanel subPanel2 = new RibbonRowPanel();
            //subPanel2.Items.Add(HorizontalButton4);
            //subPanel2.Items.Add(new RibbonRowBreak());
            //subPanel2.Items.Add(HorizontalButton5);
            //subPanel2.Items.Add(new RibbonRowBreak());
            //subPanel2.Items.Add(HorizontalButton6);
            //subPanel2.Items.Add(new RibbonRowBreak());
            #endregion

            #region add the big button and the subPanels
            //rps.Items.Add(BigButton);
            rps.Items.Add(subPanel1);
            rps.Items.Add(HorizontalButton4);
            rps.Items.Add(HorizontalButton5);
            #endregion

            #endregion

            return rp;
        }
        #endregion

        public static System.Windows.Media.ImageSource GetEmbeddedPng(System.Reflection.Assembly app, string imageName)
        {
            var file = app.GetManifestResourceStream(imageName);
            BitmapDecoder source = PngBitmapDecoder.Create(file, BitmapCreateOptions.None, BitmapCacheOption.None);
            return source.Frames[0];
        }


        public BitmapImage GetBitmap(string fileName)

        {
            BitmapImage bmp = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.             
            bmp.BeginInit();
            bmp.UriSource = new Uri(string.Format ("pack://application:,,,/{0};component/{1}",Assembly.GetExecutingAssembly().GetName().Name,fileName));
            bmp.EndInit();
            return bmp;
        }

    }
}
