# Image Processing Library
This image processing library is a lightweight open source library targeting **.NET Standard v2.0**.  

Note: **.NET Standard** libraries can be used in **.NET Full Framework** and **.NET Core** (now **.NET 5.0**) projects.

This library may be used as an educational tool on how such image processing methods can be implemented, or used within your own projects that require some form of image processing.  

See the appropriate section for details on each image processing class and their methods available.  

You are welcome to use/update this software under the terms of the **MIT license**.  
<br />

## Release History
The published package is available for download on [NuGet.org](https://www.nuget.org/packages/Freedom35.ImageProcessing).  
|Date|Version|Release Notes|
|:---|:---:|-----|
|2021/07/28|[1.3.0](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.3.0)|Added support for applying sepia filter to images.|
|20201/07/08|[1.2.0](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.2.0)|Fixed issue with stride padded images causing an *index out of bounds* exception when enhancing contrast.<br />Fixed issue with RGB color filters not working correctly for images with stride padding and alpha bytes.<br />Fixed issue with processing images with stride padding and alpha bytes.<br />Fixed issue where max threshold value was not correctly applied to the red (RGB) byte for color images.<br />Revision to combine images using bitwise OR.|
|2021/03/24|[1.1.0](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.1.0)|Added support for applying EXIF orientation data to images.|
|2021/01/26|[1.0.2](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.0.2)|Revision to return rounded value for ImageBytes.GetAverageValue.|
|2021/01/25|[1.0.1](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.0.1)|Revision to add enum description attribute for 'Mexican Hat' smoothing filter.|
|2020/10/02|[1.0.0](https://www.nuget.org/packages/Freedom35.ImageProcessing/1.0.0)|Initial release.|

<br />
<br />

# Sample Solutions
The repository contains some sample [Visual Studio](https://visualstudio.microsoft.com) solutions described below.

|Name|Description|
|-----|-----|
|Freedom35.ImageProcessing.sln|Portable solution (for both Windows/Mac) containing image processing library and unit tests.|
|Freedom35.ImageProcessing.WindowsDesktop.sln|Solution containing image processing library, unit tests, plus an Image Viewer app for Windows desktop.|  

<br />
<br />

# Usage of Library
Below are the steps involved in using the Image Processing Library in your own projects.  
The repository also contains an **ImageViewerApp** (WPF) project to demonstrate usage of the Image Processing Library.  

Methods support image types found in the **System.Drawing** namespace.

The **Image** class used is **System.Drawing.Image** (base class).  
The **Bitmap** class used is **System.Drawing.Bitmap**.  
<br />

## Usage in Projects
Note: Examples are in C#, but the library may also be used in other .NET language projects.

1. Add the **Image Processing Library** NuGet package to your .NET project.  

2. Include the namespace for the Image Processing Library at the top of your code file.  
```csharp
using Freedom35.ImageProcessing;
```

3. You may also want to include the System.Drawing namespace at the top of your code file for the *System.Drawing.Image* and *System.Drawing.Bitmap* classes.  
```csharp
using System.Drawing;
```

4. Load image into memory as a *System.Drawing.Image* object. Here is an example if you're simply loading an existing image from file:
```csharp
Image currentImage = Image.FromFile(@"C:\Images\Example-Image.bmp");
```

5. Most methods within the library are static, they should be called directly - they do not require instantiating a class first. Make the appropriate call to the method associated with the image processing function you wish to perform.  
For example:
```csharp
Image newImage = ImageColor.ToNegative(currentImage);
```  
<br />

## Notes on Usage
If you're only making a single call and do not necessarily want to include the namespace at the top of your code file, methods may also be called by explicitly including the namespace before the method name.  
Example:
```csharp
// Full namespace
Image newImage = Freedom35.ImageProcessing.ImageColor.ToNegative(currentImage);
```  
Most methods have a standard method that will return a new image (leaving the original intact), and also a direct method which will alter the original image directly. The **direct methods require using a bitmap** encoded image.  

The standard methods will automatically convert the image to a Bitmap (if required) for processing, and return the processed image in the original format.  
Example:
```csharp
// Will apply threshold and return a new image
Image newImage = ImageThreshold.ApplyOtsuMethod(currentImage);
```  
```csharp
// Will apply threshold directly to current image
ImageThreshold.ApplyOtsuMethodDirect(ref currentImage);
```  

If you wish to display images in WPF projects where controls (such as the Image control) typically use the **System.Windows.Media.Imaging** namespace, **System.Drawing** images can be converted using the **Freedom35.ImageProcessing.ImageConverter** class.  
Example:  

```csharp
// Convert to WPF image
ImageSource wpfImage = ImageConverter.ConvertImageToBitmapSource(image);
```

<br />
<br />

# Image Processing Classes
The section below summarizes the classes available within the image processing library.  
<br />

## Image Binary Class
Class for converting an image to binary/monochrome (0's and 1's).  
<br />

## Image Bytes Class
Class for returning the bytes of an image, or information on them, such as determining the type of image that bytes are encoded with (**bitmap**, **JPEG** etc.).  
<br />

## Image Color Class
Class for manipulating and filtering image colors.  
<br />

## Image Combine Class
Class for combining multiple images into a single image.  
<br />

## Image Contrast Class
Class for improving the contrast of images.  
<br />

### Contrast Stretch
Contrast stretching improves the contrast of an image by utilizing unused areas of the upper and lower pixel values. However, if an image already contains pixels at the extreme upper and lower values, then contrast stretching will not provide any improvement as there is no room to 'stretch' into.  

For example, if an image is predominantly dark whereby all the values are in the lower pixel range (possibly due to poor lighting), the values can be 'stretched' into the upper range to use the full pixel range, thereby increasing the value difference between each pixel value and therefore improve the image contrast.  

When an image is contrast stretched, the proportions between each pixel value of the original image are maintained.  
I.e. In the original image, if a pixel (p1) is twice as bright as another pixel (p2), the first pixel (p1) may become brighter or darker than the original image (depending on the stretch direction) but will still end up twice as bright as the value of the second pixel (p2).  

<br />

### Histogram Equalization
Histogram equalization improves the general contrast of an image by re-distributing the pixel value levels within an image so that the difference in brightness between each pixel value is even.  
This can be useful when the image contains both light and dark areas but is not necessarily using all the available range.  

For example, there may be no pixel values used in the mid-range or extreme light/dark ranges. Pixel values will be re-distributed to use the full range, widening the value difference and thereby improving the contrast.  

When an image is equalized, the proportions between each pixel value of the original image are **not** necessarily maintained.  
I.e. In the original image, if a pixel (p1) is twice as bright as another pixel (p2), which is only slightly brighter than another pixel (p3), the brightness of all three pixels may change, but the delta brightness between p1 and p2 will become the same as the delta between p2 and p3 (after equalization).  
<br />


## Image Convert Class
Methods for converting images to different formats/types.  
<br />


## Image Convolution Class
An image can be manipulated to enhance or isolate features etc. This is a fundamental of image processing and often used as a pre-processing step in machine vision applications.  
<br />

### Convolution Filters
The tables below list the types of filters that can be applied to an image using this library.  
<br />

#### *Edge Filters:*

|Name|Description|
|-----|-----|
|Edge|Default filter to detect the edges within an image, both horizontally and vertically.|
|Sharpen|Sharpens edges/lines of an image.|
|Edge Laplacian (With Peak 4)|Laplacian A edge/high-pass filter.  (Convolution matrix with peak of 4)|
|Edge Laplacian (With Peak 8)|Laplacian B edge/high-pass filter.  (Convolution matrix with diagonals and peak of 8)|
|Edge Laplacian of Gaussian|Applies Gaussian smoothing and Laplacian edge. Less sensitive to noise than Laplacian with peak, Gaussian σ = 1.4|
|Edge Sobel Vertical|Sobel vertical edge/high-pass filter.|
|Edge Sobel Horizontal|Sobel horizontal edge/high-pass filter.|

<br />

#### *Smoothing Filters:*

|Name|Description|
|-----|-----|
|Smoothing|Default smoothing/blur (low-pass) filter, smooths out sudden changes. Can be useful for a 'snowy' image.|
|Smoothing With High Peak|Alternate smoothing (high-pass) filter. Aims to remove gradual changes and enhance sudden changes.|
|Noise Reduction|Reduces image noise (by smoothing).|
|Smoothing Mexican Hat|Smoothing/Low-pass filter.  (Less blurring than normal smoothing.)|

<br />

#### *Special Effect Filters:*

|Name|Description|
|-----|-----|
|Emboss|Creates an embossing effect.|  
<br />
<br />

## Image Copy Class
Class for copying image bytes from one **bitmap** to another.  
<br />

## Image Crop Class
Class for cropping an image based on a region.  
<br />

## Image Edit Class
Class to begin/end the editing of **bitmap** image bytes.  
<br />

## Image EXIF Class
Class for adjusting the orientation of an image based on **exchangeable image file format** (EXIF) metadata.  
<br />

## Image Formatting Class
Class for changing the format/type of image, such as from **JPEG** to **bitmap** etc.  
<br />

## Image Histogram Class
Class for determining the histogram values for an image (as an array). Also supports creating a histogram image for a source image.   
<br />

## Image Resize Class
Class for resizing an image.   
<br />

## Image Thresholding Class
When applying a threshold to an image, values below the threshold are changed to black (0x00), and values above (or equal to) the threshold are changed to white (0xFF, 255 decimal).  
<br />

### Basic Threshold
For basic thresholding, a pre-determined value is used for the threshold, typically in the middle of the pixel value range such as 0x7F (127 decimal).  Pixel values are simply determined to be either above/below the threshold.  
<br />

### Otsu's Method
Otsu's method is more complex, but ultimately finds a better threshold value. This method searches for the threshold that minimizes the metric known as intra-class variance by using a histogram of the image to split the values into two groups (ideally foreground and background) with the smallest total variance.  
<br />

### Chow & Kaneko Method
The Chow & Kaneko method builds on Otsu's method in that it first divides an image into individual regions (typically 9), applies Otsu's method to each individual region, and then uses a weighted threshold based on the nearest 4 regions to each pixel. Regions closer to a pixel will carry more weight.  
Obviously, this method is more processing intensive than strict Otsu's method (so may not be ideal for real-time applications), but will produce a better overall result when light intensity varies across an image.  
The Chow & Kaneko method is also known as local or adaptive thresholding.  
<br />

## Image Thumbnail Class
Class for creating a thumbnail size image for a source image.   
<br />