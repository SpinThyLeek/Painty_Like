# Painty

Operation:

[What images are supported]

Importing { jpeg, png, jpg, bmp, and webp}

Saving {jpeg, png, bmp}

The program works by first presenting an empty space in the center, which the user cannot interact with at first. The right-hand side includes a color picker, Spray tools (size and density), and an option set (radio buttons) to change into the eraser, which has a hidden slider for its size. The top left includes the save and import buttons for the images.

The user can rescale the window, however, there is a limit to how small they can make it. I did this when originally I did not have the ScrollViewer in the Xaml, and to combat times when the image would be large and not cover up the buttons/sliders, I simply made the minimum window size the image size plus an additional few pixels. This proved a problem when I uploaded an image larger than the monitor. I originally wanted just not to let the user upload large images, but instead of removing the feature, I found the scroll viewer, thankfully. 

The import button opens the file explorer and lets the user pick any file. In hindsight (especially after learning from the save button), I should have restricted the files I would support, but I simply made it so if they don’t put certain image types like PNG, it would create a pop-up box warning the user and not import the file. If the user successfully imports an image, it will show up in the white canvas within the scroll viewer. Scaling the window won’t affect operation but allows the user to see more of the image permitted if it is larger than the window.

The canvas receives this image from a file and “paints” it onto the canvas by using bitmapimage to capture the image contents and Imagebrush to copy the capture and paste it onto the canvas, changing the canvas size accordingly to the image itself. I originally had it be an inkcanvas which lets you draw by default. I figured out how to disable the drawing since it was a line drawing, and I couldn’t find a way to make a custom stroke type, so I opted to change to a normal canvas.

The user has the option of 9 different colors to pick from when spraying, which was done with simple buttons and no content, just changing the color of the background to match the operation. 

The spray itself works by painting the surface with one dot (or particle, as I call it in the code). Using a timer, it would continuously create new dots randomly within a specific range. I originally planned on using while loops to complete this task; however, I soon realized that since the program itself is essentially in a true loop already, the program doesn’t have time to check if the left click is being held, meaning it would freeze. Below the color picker, the user can change the density (size of each dot) or the size of the spray (range dots can spawn in). The spray won’t go off the image if the window size is too big, but it will collect additional particles on the edges in case the mouse is close to the image edge. It will spray this paint/particle in a square pattern. You can also change (within the code) how often a pixel gets applied on line 33 (TimeSpan.FromMilliseconds(0.3);) the 0.3 ensures that plenty of pain gets sprayed at once like the spray function should do.

On top of this, the user can select which tool to use, using the radio buttons and switching from spray or eraser.

The eraser has its own slider, which appears (and the rest disappears), where the user can select how much of a radius the eraser has. This is in a circle instead of a square, much like other typical painting software.

The save button allows the user ONLY to save jpeg, png, or Bitmap images using the .filter method (hence why earlier I said I could have changed the import function a bit). It can be expanded in the future to save even more types, but this is what has been implemented. The saving works by using DrawingContext to capture the actual image location (I had an early problem where it would capture above the canvas, so looking online, this apparently works to alleviate this problem). Capturing the image boundary was also done with VisualTreeHelper (again online), and finally, I used RenderTargetBitmap to paste the image onto a file which would later be converted to the correct type of file. 


Resources used:

How to get started with WPF Application: 
https://learn.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-wpf?view=vs-2022

Video demonstration on basic commands for WPF:
https://youtu.be/8snKUcvaMmc?si=6VAvQU_tcr3Xd8x8 

How to incorporate WPF Sliders
https://www.c-sharpcorner.com/UploadFile/mahesh/slider-control-in-wpf/ 

Allow user to open file explorer to pic file:
https://youtu.be/DKYssZ8JUx0?si=fNWchsaOz7a711tQ 

How to create and show images on WPF:
https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/how-to-use-a-bitmapimage?view=netframeworkdesktop-4.8 

How to use the canvas (draw on images):
https://www.youtube.com/watch?v=atrUkhpiPkI&ab_channel=WPFTutorials 

How to make inkcanvas background the user image:
https://stackoverflow.com/questions/52489444/wpf-inkcanvas-background-image 

How to save files (and filter different types):
https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-save-files-using-the-savefiledialog-component?view=netframeworkdesktop-4.8 

DrawingBrush:
https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.drawingbrush?view=windowsdesktop-8.0 

How to create mouseclick events:
https://stackoverflow.com/questions/41718317/how-can-i-check-if-mouse-button-is-left-or-right-in-wpf-c 

More on Slider control:
https://youtu.be/7GiLMeluaBc?si=vIZZ07C9UWkMK8a7 

Scroll Viewer Info:
https://stackoverflow.com/questions/14142865/scrollviewer-and-scroll-direction-vertical-vs-horizontal 

If I didn’t have to use Canvas and could use inkcanvas would have liked to included the following:
https://learn.microsoft.com/en-us/windows/apps/design/controls/inking-controls
To make it feel more professional and to also incorporate more aspects of MS paint

When creating the color buttons I REALLY wanted to use this:
https://stackoverflow.com/questions/16329584/dynamically-creating-buttons-in-wpf-xaml-file-xaml-cs-file-after-reading-an-xml 
It would have streamlined the process and stopped me from copying and pasting the same button 18 million times but I couldn’t figure out how to complete this while maintaining the buttons would be in the correct positions. 

Zoom functionality:
https://stackoverflow.com/questions/14729853/wpf-zooming-in-on-an-image-inside-a-scroll-viewer-and-having-the-scrollbars-a 

Color picker functionality, however the implementation they provided did not work:
https://stackoverflow.com/questions/17089382/wpf-color-picker-implementation 

Using bitmap render and saving images:
https://stackoverflow.com/questions/4560173/save-wpf-view-as-image-preferably-png 

How to use the timer functionality in C#:
https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatchertimer?view=windowsdesktop-8.0 
