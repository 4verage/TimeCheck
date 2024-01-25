using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.ComponentModel;

// Final Project for CS50
// (C)opyright 2023 Jonathan E. Styles

namespace TimeCheck
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Window
    {

        private Point startPosition = new Point(0, 0);
        private string strTZ = "";
        private string strTime = "";
        bool _shown;
        private Color prevColor;
        private bool isAbove = false;

        public Results()
        {
            InitializeComponent();
            this.Height = 2;
            this.Top = startPosition.Y - 2;
            this.Left = startPosition.X;
        }

        public void setPosition(Point pos, double winHeight)
        {
            startPosition = pos;

            // If position is higher than result height + window height...
            if (this.Height + startPosition.Y < SystemParameters.WorkArea.Height)
            {
                this.Top = startPosition.Y;
                this.Left = startPosition.X + 7;
                isAbove = false;
            }

            // If position is greater than the working area, render above.
            else
            {
                this.Top = startPosition.Y - winHeight - this.Height;
                this.Left = startPosition.X + 7;
                isAbove = true;
            }
        }

        public Point getPosition()
        {
            return new Point(this.Left, this.Top);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            PopOut();
        }

        private void PopOut()
        {
            if (_shown)
                return;

            _shown = true;

            this.Height = 2;

            if (this.Top + 50 <= SystemParameters.WorkArea.Height)
            {
                DoubleAnimation dblAnim = new DoubleAnimation();
                dblAnim.To = 50;
                dblAnim.Duration = TimeSpan.FromSeconds(1);
                dblAnim.FillBehavior = FillBehavior.Stop;
                this.BeginAnimation(HeightProperty, dblAnim);
            }
            else
            {
                DoubleAnimation dblAnim = new DoubleAnimation();
                dblAnim.To = 50;
                dblAnim.Duration = TimeSpan.FromSeconds(1);
                dblAnim.FillBehavior = FillBehavior.Stop;
                this.BeginAnimation(HeightProperty, dblAnim);

                DoubleAnimation risAnim = new DoubleAnimation();
                risAnim.To = this.Top - 50;
                risAnim.Duration = TimeSpan.FromSeconds(1);
                risAnim.FillBehavior = FillBehavior.Stop;
                this.BeginAnimation(TopProperty, risAnim);

                isAbove = true;

            }

        }      

        public void UpdateResults(string tz, string time)
        {
            this.lblTZResult.Content = tz;
            this.lblTimeResult.Content = time;
        }

        private void btnClose_Over(object sender, MouseEventArgs e)
        {
            prevColor = ((SolidColorBrush)btnClose.Foreground).Color;
            btnClose.Foreground = new SolidColorBrush(Colors.White);
            btnClose.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void btnClose_Leave(object sender, MouseEventArgs e)
        {
            btnClose.Foreground = new SolidColorBrush(prevColor);
        }

        private void btnClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _shown = false;

            if (isAbove)
            {
               this.Top = this.Top + this.Height + 2;
            }

            this.Height = 2;
            this.Hide();
        }

    }
}
