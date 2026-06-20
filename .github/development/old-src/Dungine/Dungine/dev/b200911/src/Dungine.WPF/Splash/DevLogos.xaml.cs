/* PROJECT: Dungine.WPF (https://github.com/calistadalanegames/Dungine.WPF)
 *    FILE: Dungine.WPF.Splash.DevLogos.xaml.cs
 * UPDATED: 11-10-2020-9:05 AM
 * LICENSE: Apache v2 (https://apache.org/licenses/LICENSE-2.0)
 *          Copyright 2020 A Pretty Cool Program & Calistadalane Games
 *          All rights reserved
 */

/* Displays all development logos when a Dungine game is launched.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Dungine.WPF.Splash
{
    /// <include file='DevLogos.comment.xml' path='class/member[@name="DevLogos"]/*'/>
    public partial class DevLogos : Window
    {
        /// <include file='DevLogos.comment.xml' path='class/member[@name="DevLogos"]/*'/>
        public DevLogos(List<string> gameLogoFileNames)
        {
            InitializeComponent();

            var devLogos = GetAllDevLogos(gameLogoFileNames);

            NewDisplay(gameLogoFileNames);

            //DisplayDevLogos(devLogos);

            var test = 0;
        }

        /// <include file='DevLogos.comment.xml' path='class/member[@name="GetAllDevLogos"]/*'/>
        private List<BitmapImage> GetAllDevLogos(List<string> gameLogoFileNames)
        {
            List<BitmapImage> standardDevLogos = GetStandardDevLogos();
            List<BitmapImage> gameDevLogos     = GetGameDevLogos(gameLogoFileNames);

            return Du.WPF.WithBitmap.MergeLists(standardDevLogos, gameDevLogos);
        }

        /// <include file='DevLogos.comment.xml' path='class/member[@name="GetStandardDevLogos"]/*'/>
        private List<BitmapImage> GetStandardDevLogos()
        {
            var standardDevLogos    = new List<BitmapImage>();
            var standardDevLogoRoot = AppDomain.CurrentDomain.BaseDirectory + @"application-data\dungine\asset\image\logo\";

            standardDevLogos.Add(Du.WPF.WithBitmap.LoadFromFile($"{standardDevLogoRoot}dungine-logo-250x250.png"));

            return standardDevLogos;
        }

        /// <include file='DevLogos.comment.xml' path='class/member[@name="GetGameDevLogos"]/*'/>
        private List<BitmapImage> GetGameDevLogos(List<string> gameLogoFileNames)
        {
            var gameDevLogos = new List<BitmapImage>();
            var gameDevLogoRoot  = AppDomain.CurrentDomain.BaseDirectory + @"application-data\asset\image\included\logo\";

            foreach(var gameLogoFileName in gameLogoFileNames)
            {
                gameDevLogos.Add(Du.WPF.WithBitmap.LoadFromFile($"{gameDevLogoRoot}{gameLogoFileName}"));
            }

            return gameDevLogos;
        }

        private void NewDisplay(List<string> gameLogoFileNames)
        {
            var devLogo = new BitmapImage();


            foreach(var gameLogoFileName in gameLogoFileNames)
            {
                devLogo = Du.WPF.WithBitmap.LoadFromFile(AppDomain.CurrentDomain.BaseDirectory + @"application-data\asset\image\included\logo\" + gameLogoFileName);

                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 1.0;
                myDoubleAnimation.To = 0.0;



                //imgDevLogo.Opacity = 0.50;
                //imgDevLogo.Source = devLogo;

                //Thread.Sleep(1000);

                //imgDevLogo.Opacity = 1.0;

                //while(imgDevLogo.Opacity <= 1.0)
                //{
                //    Thread.Sleep(1000);
                //    imgDevLogo.Opacity += 0.1;
                //    //imgDevLogo.Source = devLogo;
                //}

            }


        }


        /// <include file='DevLogos.comment.xml' path='class/member[@name="DisplayDevLogos"]/*'/>
        private void DisplayDevLogos(List<BitmapImage> devLogos)
        {





                //imgDevLogo.Opacity = .2;
                //imgDevLogo.Source = devLogos[1];

            foreach(var devLogo in devLogos)
            {
                imgDevLogo.Opacity = 0.00;
                imgDevLogo.Source = devLogo;

                while(imgDevLogo.Opacity <= 1.0)
                {
                    Thread.Sleep(1000);
                    imgDevLogo.Opacity += 0.1;
                    imgDevLogo.Source = devLogo;
                }


                //for(double i = 0.00; i < 1.00; i + 0.10)
                //{
                //    Thread.Sleep(1000);
                //    imgDevLogo.Opacity = i;
                //}


            }
        }

        /// <include file='DevLogos.comment.xml' path='class/member[@name="CloseSplashScreen"]/*'/>
        private void CloseSplashScreen()
        {
            Close();
        }
    }
}
