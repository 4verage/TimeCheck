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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.MessageBox;

// Final Project for CS50
// (C)opyright 2023 Jonathan E. Styles

namespace TimeCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Point moveMouse = new Point(0,0);    // Initial variable for mouse click location.
        private int screenCount = 0;        // Multi Monitor Support - Stores Number of Connected Monitors.
        private bool posLocked = false;     // Position Lock Value.
        FileInteractions FileIO = new FileInteractions();
        APICalls nfo = new APICalls();
        Results resWindow = new Results();

        public MainWindow()
        {
            InitializeComponent();

            // If a Last Position exists, start window there.
            string[] strPoints = FileIO.read_setting("Last Location").Split(',');
            Point startHere = new Point(Convert.ToDouble(strPoints[0]), Convert.ToDouble(strPoints[1]));
            this.Left = startHere.X;
            this.Top = startHere.Y;

            // Is locked?
            if (FileIO.read_setting("Position Lock") == "1")
            {
                posLocked = true;
                btnLock.Content = "\uF023;";
            }

            resWindow.setPosition(new Point(this.Left, this.Top + this.Height), this.Height);

        }

        private void btnDrag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(btnDrag);         // Locks Mouse to object to prevent movement glitches.
            if (!posLocked)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    windowBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF767171"));   // Change border to show user something is happening.
                    moveMouse = e.GetPosition(this);                        // Store original mouse position for calculation.
                    screenCount = Screen.AllScreens.Length;                 // Multi Monitor Support - Get number of available screens.
                }
            }

        }

        private void btnDrag_MouseUp(object sender, MouseButtonEventArgs e)
        {
            btnDrag.ReleaseMouseCapture();      // Unlock the mouse.
            if (!posLocked)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    windowBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3C3C3C"));   // Change border back to original color.
                    FileIO.write_setting("Last Location", this.Left.ToString() + "," + this.Top.ToString());
                }
            }
        }

        // Main Window Application Movement on Screen
        private void btnDrag_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!posLocked)
            {
                // Basic Move Structure
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {

                    // Store current screen Width / Height as a Point.
                    Point scr = new Point(System.Windows.SystemParameters.WorkArea.Width, System.Windows.SystemParameters.WorkArea.Height);

                    // Calculate current mouse position relative to original click.
                    double xDiff = moveMouse.X - e.GetPosition(this).X;
                    double yDiff = moveMouse.Y - e.GetPosition(this).Y;

                    // Change window position.
                    if ((!(this.Left - xDiff < 0)) && (!(this.Left + this.Width - xDiff > scr.X)))
                    {
                        this.Left = this.Left - xDiff;  // Change This Window.
                        resWindow.setPosition(new Point(this.Left, this.Top + this.Height), this.Height); //Update Results Window Position.
                    }
                    if ((!(this.Top - yDiff < 0)) && (!(this.Top + this.Height - yDiff > scr.Y)))
                    {
                        this.Top = this.Top - yDiff;
                        resWindow.setPosition(new Point(this.Left, this.Top + this.Height), this.Height); //Update Results Window Position.
                    }


                    // Lock to screen edges
                    int LOCKTOLLERANCE = 10;     // How far away the widget will be before it locks to edges.

                    // Check for left.
                    if (this.Left <= LOCKTOLLERANCE) { this.Left = 0; }

                    // Check for top.
                    if (this.Top <= LOCKTOLLERANCE) { this.Top = 0; }

                    // Check for bottom.
                    if (this.Top + this.Height >= scr.Y - LOCKTOLLERANCE) { this.Top = scr.Y - this.Height; }

                    // Check for right.
                    if (this.Left + this.Width >= scr.X - LOCKTOLLERANCE) { this.Left = scr.X - this.Width; }
                }
            }

        }

        // Lock Window Movement
        private void btnLock_Toggle(object sender, MouseButtonEventArgs e)
        {
            if (!posLocked)
            {
                posLocked = true;
                btnLock.Content = "\uF023;";
                FileIO.write_setting("Position Lock", "1");
            }
            else
            {
                posLocked = false;
                btnLock.Content = "\uF3C1;";
                FileIO.write_setting("Position Lock", "0");
            }
        }

        // Close the Application
        private void btnClose_Click(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }

        // Validate Integer-only Input
        private void txtZIP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        // Accept Enter Key (Regular or numpad) in text box.
        private void txtZIP_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Return))
            {
                txtZIP_Verify();
            }
        }

        // Validate proper input. (Length, ZIP exists.)
        private void txtZIP_Verify()
        {
            if (txtZIP.Text.Length == 5)
            {
                nfo.GetTimeData(txtZIP.Text);
                resWindow.Show();
                string[] results = nfo.GetTZInfo();
                resWindow.UpdateResults(results[0], results[1]);
            }
            else
            {
                MessageBox.Show("Not enough digits. ZIP must be five numbers long!");
            }
        }

        // Links-up Search Button to text control.
        private void btnSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtZIP_Verify();
        }
    }
}
