using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;
using System.Windows;

namespace CreatureKingdom
{
    class BarrettDylanCreature : Creature
    {
        Image foxImage;
        BitmapImage rightBitmap;
        BitmapImage leftBitmap;
        double foxWidth = 356;
        double changePos =5.0;
        double kingdomWidth = 0;
        private Thread positionThread = null;
        private Boolean goingRight = true;

        public BarrettDylanCreature(Canvas kingdom, Dispatcher dispatcher, Int32 waitTime = 100) : base(kingdom, dispatcher, waitTime) {
            foxImage = new Image();
            leftBitmap = LoadBitmap(@"BarrettDylan\foxLeft.png", foxWidth);
            rightBitmap = LoadBitmap(@"BarrettDylan\foxRight.png", foxWidth);
        }

        public override void Place(double x, double y) {
            foxImage.Source = rightBitmap;
            goingRight = true;

            this.x = x;
            this.y = y;

            kingdom.Children.Add(foxImage);
            foxImage.SetValue(Canvas.LeftProperty, this.x);
            foxImage.SetValue(Canvas.TopProperty, this.y);

            positionThread = new Thread(Position);
            positionThread.Start();
        }

        void Position() {
            while (true) {
                if (goingRight && !Paused) {
                    x += changePos;
                    if (x > kingdomWidth) {
                        goingRight = false;
                        SwitchBitmap(leftBitmap);
                    }
                }
                else if (!Paused) {
                    x -= changePos;
                    if (x < 0) {
                        goingRight = true;
                        SwitchBitmap(rightBitmap);
                    }
                }
                if (kingdomWidth != kingdom.RenderSize.Width - foxWidth) {
                    kingdomWidth = kingdom.RenderSize.Width - foxWidth;
                }
                UpdatePosition();
                Thread.Sleep(WaitTime);
            }
        }

        void UpdatePosition() {
            Action action = () => { foxImage.SetValue(Canvas.LeftProperty, x); foxImage.SetValue(Canvas.TopProperty, y); };
            dispatcher.BeginInvoke(action);
        }

        void SwitchBitmap(BitmapImage bitmap) {
            Action action = () => { foxImage.Source = bitmap; };
            dispatcher.BeginInvoke(action);
        }
        public override void Shutdown()
        {
            if (positionThread != null)
            {
                positionThread.Abort();
            }
        }
    }
}