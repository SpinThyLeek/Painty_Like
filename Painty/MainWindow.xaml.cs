using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading; //added for timer functionality

namespace Painty_Like
{
    //Initialize the backend
    public partial class MainWindow : Window
    {
        //initialize our global variables
        private bool isMouseButtonDown = false; //Bool to check if left mouse is being held
        
        private int paintRadius = 10; //determines area of spray
        private int eraserRadius = 10; //determins area of erase
        private double paintDensity = 100; //determines size of each paint 'particle'

        private Random random = new Random(); //Random to determin location of paint 'particle'
        private List<Ellipse> ellipses = new List<Ellipse>(); // Create a new paint 'particle'

        private DispatcherTimer paintTimer; //timer to ensure paint is continuously sprayed (while loops did not work)
        public MainWindow()
        {
            InitializeComponent();

            //Set up our time so that we can continously "spray" the paint onto the canvas
            paintTimer = new DispatcherTimer();
            paintTimer.Interval = TimeSpan.FromMilliseconds(0.3); //Number set will determine how quickly paint is sprayed, not interactable by the user
            paintTimer.Tick += PaintTimer_Tick;
            paintTimer.Start();

           

        }

        //======================================================================================
        //                       Import and Save Button Methods - Start
        //======================================================================================

        //Import Button Functionality
        private void Import_Image_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog(); //Opens file explorer

            // Nullable<int> x is the same as int? x. Need this due to OpenFileDialog being a null return type
            bool? response = openFileDialog.ShowDialog();

            //Check if user has picked a file
            if (response == true)
            {
                //collect file path
                string filepath = openFileDialog.FileName;

                //collect file type
                string type = filepath.Substring(filepath.LastIndexOf('.') + 1);

                //create list of possible image types
                List<string> imageTypes = new List<string>();
                imageTypes.Add("jpeg");
                imageTypes.Add("png");
                imageTypes.Add("jpg");
                imageTypes.Add("bmp");
                imageTypes.Add("webp");

                if (imageTypes.Contains(type)) //Not a fast approach nor user benefical, however thought it was interesting to keep in comparison to the save function
                {
                    // If we are importing a new image we need to clear the canvas of our previous edits and image
                    Draw_On_Image.Children.Clear();

                    //MessageBox.Show(filepath); //testing

                    //Show image on the canvas
                    DisplayCanvas(filepath);
                }
                else //user entered a file  format which is not a supported image
                {
                    MessageBox.Show("File type you entered is NOT supported!");
                }

            }
        }

        // Saves the canvas as a new image
        private void Save_New_Image_Click(object sender, RoutedEventArgs e)
        {
            //begin the saving process
            SaveFileDialog saveCanvas = new SaveFileDialog();

            //Learned this later in the process about filtering types:
            saveCanvas.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png";
            saveCanvas.Title = "Save an Image File";

            //show file explorer
            bool? result = saveCanvas.ShowDialog();

            if (result == true)
            {

                using (FileStream fs = new FileStream(saveCanvas.FileName, FileMode.Create))
                {
                    // Need to use DrawingVisual to actually capture the correct space the image is in (not just the outskirts of the canvas)
                    DrawingVisual drawingVisual = new DrawingVisual();

                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        // Render the content of the canvas onto the DrawingVisual
                        drawingContext.DrawRectangle(new VisualBrush(Draw_On_Image), null, new Rect(0, 0, Draw_On_Image.ActualWidth, Draw_On_Image.ActualHeight));
                    }

                    // Get the actual content size
                    Rect contentBounds = VisualTreeHelper.GetDescendantBounds(drawingVisual);

                    // Render the image
                    RenderTargetBitmap saveUserWork = new RenderTargetBitmap((int)contentBounds.Width, (int)contentBounds.Height, 96d, 96d, PixelFormats.Default);
                    saveUserWork.Render(drawingVisual);

                    // Save the image to specified type
                    switch (saveCanvas.FilterIndex)
                    {
                        case 1: // Jpeg?
                            JpegBitmapEncoder encoder1 = new JpegBitmapEncoder();
                            encoder1.Frames.Add(BitmapFrame.Create(saveUserWork));
                            encoder1.Save(fs);
                            break;
                        case 2: // Bitmap?
                            BmpBitmapEncoder encoder2 = new BmpBitmapEncoder();
                            encoder2.Frames.Add(BitmapFrame.Create(saveUserWork));
                            encoder2.Save(fs);
                            break;
                        case 3: // PNG?
                            PngBitmapEncoder encoder3 = new PngBitmapEncoder();
                            encoder3.Frames.Add(BitmapFrame.Create(saveUserWork));
                            encoder3.Save(fs);
                            break;
                    }
                }
            }
        }

        //======================================================================================
        //                        Import and Save Button Methods - End
        //======================================================================================


        //======================================================================================
        //                            Canvas and Spray Methods - Start
        //======================================================================================


        // Once an image is imported, set up the canvas so that the user may draw on their image
        private void DisplayCanvas(string filePath)
        {
            // Create our image
            BitmapImage image = new BitmapImage();
            
            // Set up the path where Bitmap will capture the user image
            image.BeginInit();
            image.UriSource = new Uri(filePath);
            image.EndInit();

            // Create the "brush" or copiable version of the image
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = image;
            imageBrush.Stretch = Stretch.Uniform;

            // Paste image into the background of the canvas
            Draw_On_Image.Background = imageBrush;

            // Once the image is collected, initialize our work space which the border highlights
            Draw_On_Image.Width = image.PixelWidth;
            Draw_On_Image.Height = image.PixelHeight;

            // Set border around the scroll view
            CanvasBorder.Width = viewer.Width;
            CanvasBorder.Height = viewer.Height;
        }

        // Handles the timer to trigger painting. Essentially checks if left click is still being held, continue to paint
        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            if (isMouseButtonDown) // If left mouse click is STILL being held
            {
                PaintRandomPixel(Mouse.GetPosition(Draw_On_Image)); // Paint random pixel, needed Mouse instead of e, since e isn't related to the mouse itself
            }
        }

        // Handles the left click event (Start spray)
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseButtonDown = true; // Left click is being pushed
            PaintRandomPixel(e.GetPosition(Draw_On_Image)); // 
            paintTimer.Start(); // Start the timer
        }


        // Handles when left click is not being pressed (End spray)
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseButtonDown = false; // No longer left clicking
            paintTimer.Stop(); // Stop the timer
        }

        // Handles when the mouse moved. Realized that it would spray a lot less when moving so included this method. Originally was not here. Still I'm not sure if I need it here.
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseButtonDown)
            {
                PaintRandomPixel(e.GetPosition(Draw_On_Image));
            }
        }

        //Handles the actual spray functionality
        private void PaintRandomPixel(Point position)
        {
            if (Spray.IsChecked == true) // Is the spray enabled?
            {
                //Create random offset from the mouse's current position
                double offsetX = random.Next(-paintRadius, paintRadius + 1);
                double offsetY = random.Next(-paintRadius, paintRadius + 1);
                double x = position.X + offsetX;
                double y = position.Y + offsetY;

                // Ensure that the generated coordinates are within the image boundaries. Originally a coninous while loop but was rudimentary and really slow
                x = Math.Max(0, Math.Min(x, Draw_On_Image.ActualWidth - paintDensity));
                y = Math.Max(0, Math.Min(y, Draw_On_Image.ActualHeight - paintDensity));

                // Create a new 'particle'
                Ellipse ellipse = new Ellipse
                {
                    Width = paintDensity,
                    Height = paintDensity,
                    Fill = new SolidColorBrush(EllipseColor.Color) //figure out color of particle
                };

                // Create the Ellipse / Particle
                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                // Add ellipse to the canvas
                Draw_On_Image.Children.Add(ellipse);

                // Add particle to an array so we may edit it for later
                ellipses.Add(ellipse);
            }
            else // Are we using the eraser? 
            {
                // Go through the paricles
                foreach (Ellipse ellipse in ellipses)
                {
                    //get particle position
                    double ellipseX = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
                    double ellipseY = Canvas.GetTop(ellipse) + ellipse.Height / 2;

                    // Find position in comparion to the mouse
                    double distance = Math.Sqrt(Math.Pow(position.X - ellipseX, 2) + Math.Pow(position.Y - ellipseY, 2));

                    //Within the erasers radius?
                    if (distance <= eraserRadius)
                    {
                        Draw_On_Image.Children.Remove(ellipse); //remove
                    }
                }
            }
        }

        //======================================================================================
        //                             Canvas and Drawing - End
        //======================================================================================

        //======================================================================================
        //                              Spray + Eraser Affects - Start
        //======================================================================================

        //Will take the sliders input and change the spray size
        private void Spray_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            paintRadius = (int)e.NewValue;
        }

        //Changes spray density (larger particles)
        private void Spray_Density_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            paintDensity = e.NewValue;
        }

        //Changes eraser size
        private void Eraser_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            eraserRadius = (int)e.NewValue;
        }

        //Makes certain sliders / buttons visible or invisible. Does not affect color picker only because of how much time it would take
        private void Eraser_Checked(object sender, RoutedEventArgs e)
        {
            Spray_Density.Visibility = Visibility.Collapsed;
            Spray_Size.Visibility = Visibility.Collapsed;
            SprayD.Visibility = Visibility.Collapsed;
            SprayS.Visibility = Visibility.Collapsed;

            Eraser_Size.Visibility = Visibility.Visible;
            EraseS.Visibility = Visibility.Visible;
        }

        private void Spray_Checked(object sender, RoutedEventArgs e)
        {
            Spray_Density.Visibility = Visibility.Visible;
            Spray_Size.Visibility = Visibility.Visible;
            SprayD.Visibility = Visibility.Visible;
            SprayS.Visibility = Visibility.Visible;

            Eraser_Size.Visibility = Visibility.Collapsed;
            EraseS.Visibility = Visibility.Collapsed;
        }


        //======================================================================================
        //                               Spray Affects - End
        //======================================================================================

        //======================================================================================
        //                                Colors - Start
        //======================================================================================

        public SolidColorBrush EllipseColor { get; set; } = new SolidColorBrush(Colors.Black);
        private void set_blue(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Blue;
        }
        private void set_purple(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Purple;
        }
        private void set_red(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Red;
        }
        private void set_yellow(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Yellow;
        }
        private void set_green(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Green;
        }
        private void set_brown(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.SaddleBrown;
        }
        private void set_orange(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Orange;
        }
        private void set_black(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.Black;
        }
        private void set_white(object sender, RoutedEventArgs e)
        {
            EllipseColor.Color = Colors.White;
        }

        //======================================================================================
        //                                Colors - End
        //======================================================================================


    }
}
