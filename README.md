# Image Processing Library
This image processing library is a lightweight open source library targeting [.NET Standard v2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0).  

A packaged build is available for download on [nuget.org](https://www.nuget.org/packages/Freedom35.ImageProcessing).  

This library may be used as an educational tool on how such image processing methods can be implemented, or used within your own projects that require some form of image processing.  

See the appropriate section for details on each image processing class and their methods available.  

You are welcome to use/update this software under the terms of the [MIT license](https://github.com/freedom35/image-processing?tab=MIT-1-ov-file).  


Notes: 
1. **.NET Standard v2.0** libraries can be used in **.NET Full Framework** and **.NET Core** (now known simply as **.NET**) projects.
2. The image processing library is only supported on **Windows OS** due to the dependency on Microsoft's [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common) NuGet package.

## Table of Contents
- [Release History](#release-history)
- [Usage of Library](#usage-of-library)
  - [Sample Code Solutions](#sample-code-solutions)
  - [Usage in Projects](#usage-in-projects)
  - [Notes on Usage](#notes-on-usage)
    - [Method Calls](#method-calls)
    - [Namespace](#namespace)
    - [Windows Presentation Foundation (WPF)](#windows-presentation-foundation-wpf)
- [Image Processing Classes](#image-processing-classes)
  - [Image Binary Class](#image-binary-class)
  - [Image Bytes Class](#image-bytes-class)
  - [Image Color Class](#image-color-class)
  - [Image Combine Class](#image-combine-class)
  - [Image Contrast Class](#image-contrast-class)
    - [Contrast Stretch](#contrast-stretch)
    - [Histogram Equalization](#histogram-equalization)
  - [Image Convolution Class](#image-convolution-class)
    - [Convolution Filters](#convolution-filters)
      - [Edge Filters](#edge-filters)
      - [Smoothing Filters](#smoothing-filters)
      - [Special Effect Filters](#special-effect-filters)
  - [Image Copy Class](#image-copy-class)
  - [Image Crop Class](#image-crop-class)
  - [Image EXIF Class](#image-exif-class)
  - [Image Formatting Class](#image-formatting-class)
  - [Image Histogram Class](#image-histogram-class)
  - [Image Resize Class](#image-resize-class)
  - [Image Thresholding Class](#image-thresholding-class)
    - [Basic Threshold](#basic-threshold)
    - [Otsu's Method](#otsus-method)
    - [Chow & Kaneko Method](#chow--kaneko-method)
  - [Image Thumbnail Class](#image-thumbnail-class)


## Release History

### v1.5.2 (2025-01-05)
* Revision to update package properties.

### v1.5.1 (2024-12-28)
* Revision for including readme in NuGet package.

### v1.5.0 (2024-12-28)
* Updated System.Drawing.Common package dependency to v9.0.0.

### v1.4.1 (2022-06-16)
* Updated System.Drawing.Common package dependency to v6.0.0.

### v1.4.0 (2021-07-30)
* Added overload methods for converting color images to grayscale.
* Added overload methods for converting images to black & white.

### v1.3.0 (2021-07-28)
* Added support for applying sepia filter to images.

### v1.2.0 (2021-07-08)
* Fixed issue with stride padded images causing an *'index out of bounds'* exception when enhancing contrast.
* Fixed issue with RGB color filters not working correctly for images with stride padding and alpha bytes.  
* Fixed issue with processing images with stride padding and alpha bytes.
* Fixed issue where max threshold value was not correctly applied to the red (RGB) byte for color images.
* Revision to combine images using bitwise OR.

### v1.1.0 (2021-03-24)
* Added support for applying EXIF orientation data to images.

### v1.0.2 (2021-01-26)
* Revision to return rounded value for ImageBytes.GetAverageValue.

### v1.0.1 (2021-01-25)
* Revision to add enum description attribute for 'Mexican Hat' smoothing filter.

### v1.0.0 (2020-10-02)
* Initial release.



## Usage of Library
Below are the steps involved in using the Image Processing Library in your own projects.  
The repository also contains an **ImageViewerApp** (WPF) project to demonstrate usage of the Image Processing Library functions.  

Libaray class methods support the image types found in the **System.Drawing** namespace.

The **Image** class used is **System.Drawing.Image** (base class).  
The **Bitmap** class used is **System.Drawing.Bitmap**.  
  

### Sample Code Solutions
The [GitHub repository](https://github.com/freedom35/image-processing) contains some [Visual Studio](https://visualstudio.microsoft.com) solutions described below.

|Name|Description|
|-----|-----|
|Freedom35.ImageProcessing.sln|Base solution containing image processing library and unit tests.|
|Freedom35.ImageProcessing.WindowsDesktop.sln|Extended solution containing projects from base solution, plus an **Image Viewer app** for Windows desktop.|  


### Usage in Projects
Note: Examples are in C#, but the library may also be used in other .NET language projects.

1. Add the [Image Processing Library](https://www.nuget.org/packages/Freedom35.ImageProcessing) NuGet package to your .NET project.  

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

5. Most methods within the library are static, they should be called directly - they do not require instantiating a class first. Make the appropriate call to the method associated with the image processing function you wish to perform. For example:
```csharp
Image newImage = ImageContrast.Enhance(currentImage);
```  


### Notes on Usage
Below are some additional notes on using the Image Processing Library in projects.

#### Method Calls
Most methods have a standard method that will return a new image (leaving the original intact), and also a direct method which will alter the original image directly. The **direct methods require using a bitmap** encoded image.  

The standard methods will automatically convert the image to a Bitmap (if required) for processing, and return the processed image in the original format.  

```csharp
// Example: Will apply threshold and return a new image
Image newImage = ImageThreshold.ApplyOtsuMethod(currentImage);
```  
```csharp
// Example: Will apply threshold directly to current image
ImageThreshold.ApplyOtsuMethodDirect(ref currentImage);
```  

#### Namespace
If you're only making a single call to the library and do not necessarily want to include the namespace at the top of your code file, methods may also be called by explicitly including the full namespace before the method name.  
```csharp
// Example using full namespace
Image newImage = Freedom35.ImageProcessing.ImageColor.ToNegative(currentImage);
```  

#### Windows Presentation Foundation (WPF)
If you wish to display images in WPF projects where user controls typically use the **System.Windows.Media.Imaging** namespace for images rather than **System.Drawing** namespace, the ***ImageConverter*** class in the **ImageViewerApp** provides an example of how to convert images for WPF usage.  
```csharp
// Example: Convert System.Drawing image to WPF image
BitmapSource wpfImage = ImageConverter.ConvertImageToBitmapSource(image);
```


# Image Processing Classes
The section below summarizes the classes available within the image processing library.  


## Image Binary Class
Class for converting an image to binary/monochrome (0's and 1's).  


## Image Bytes Class
Class for returning the bytes of an image, or information on them, such as determining the type of image that bytes are encoded with (**bitmap**, **JPEG** etc.).  
* Bytes from **image object**.
* Bytes from **image file**.
* Bytes from **image stream**.
* Bytes from an **embedded resource**.
* Min/Max/Average image bytes.
* Bytes to/from bits.
* Determine image format/type based on image bytes.


## Image Color Class
Class for manipulating and filtering image colors, such as:  
* Convert to **grayscale**.
* Convert to **black & white**.
* Convert to **negative**.
* Convert to **sepia**.
* Convert to **red**.
* Convert to **green**.
* Convert to **blue**.
* Apply specific **RGB** filter.


## Image Combine Class
Class for combining multiple images into a single image.  
  

## Image Contrast Class
Class for improving the contrast of images.  


### Contrast Stretch
Contrast stretching improves the contrast of an image by utilizing unused areas of the upper and lower pixel values. However, if an image already contains pixels at the extreme upper and lower values, then contrast stretching will not provide any improvement as there is no room to 'stretch' into.  

For example, if an image is predominantly dark whereby all the values are in the lower pixel range (possibly due to poor lighting), the values can be 'stretched' into the upper range to use the full pixel range, thereby increasing the value difference between each pixel value and therefore improve the image contrast.  

When an image is contrast stretched, the proportions between each pixel value of the original image are maintained.  
I.e. In the original image, if a pixel (p1) is twice as bright as another pixel (p2), the first pixel (p1) may become brighter or darker than the original image (depending on the stretch direction) but will still end up twice as bright as the value of the second pixel (p2).  


### Histogram Equalization
Histogram equalization improves the general contrast of an image by re-distributing the pixel value levels within an image so that the difference in brightness between each pixel value is even.  
This can be useful when the image contains both light and dark areas but is not necessarily using all the available range.  

For example, there may be no pixel values used in the mid-range or extreme light/dark ranges. Pixel values will be re-distributed to use the full range, widening the value difference and thereby improving the contrast.  

When an image is equalized, the proportions between each pixel value of the original image are **not** necessarily maintained.  
I.e. In the original image, if a pixel (p1) is twice as bright as another pixel (p2), which is only slightly brighter than another pixel (p3), the brightness of all three pixels may change, but the delta brightness between p1 and p2 will become the same as the delta between p2 and p3 (after equalization).  


## Image Convolution Class
An image can be manipulated to enhance or isolate features etc. This is a fundamental of image processing and often used as a pre-processing step in machine vision applications.  


### Convolution Filters
The tables below list the types of filters that can be applied to an image using this library.  


#### Edge Filters

|Name|Description|
|-----|-----|
|Edge|Default filter to detect the edges within an image, both horizontally and vertically.|
|Sharpen|Sharpens edges/lines of an image.|
|Edge Laplacian (With Peak 4)|Laplacian A edge/high-pass filter.  (Convolution matrix with peak of 4)|
|Edge Laplacian (With Peak 8)|Laplacian B edge/high-pass filter.  (Convolution matrix with diagonals and peak of 8)|
|Edge Laplacian of Gaussian|Applies Gaussian smoothing and Laplacian edge. Less sensitive to noise than Laplacian with peak, Gaussian σ = 1.4|
|Edge Sobel Vertical|Sobel vertical edge/high-pass filter.|
|Edge Sobel Horizontal|Sobel horizontal edge/high-pass filter.|


#### Smoothing Filters

|Name|Description|
|-----|-----|
|Smoothing|Default smoothing/blur (low-pass) filter, smooths out sudden changes. Can be useful for a 'snowy' image.|
|Smoothing With High Peak|Alternate smoothing (high-pass) filter. Aims to remove gradual changes and enhance sudden changes.|
|Noise Reduction|Reduces image noise (by smoothing).|
|Smoothing Mexican Hat|Smoothing/Low-pass filter.  (Less blurring than normal smoothing.)|


#### Special Effect Filters

|Name|Description|
|-----|-----|
|Emboss|Creates an embossing effect.|  


## Image Copy Class
Class for copying image bytes from one **bitmap** to another.  


## Image Crop Class
Class for cropping an image based on a region.  


## Image EXIF Class
Class to apply orientation information to image based on **exchangeable image file format** (EXIF) metadata.  


## Image Formatting Class
Class for changing the format/type of image, such as from **JPEG** to **bitmap** etc.  


## Image Histogram Class
Class for determining the [histogram](https://en.wikipedia.org/wiki/Histogram) values for an image (as an array). A histogram is a bar graph/chart representation of data distribution, such as the number of pixels at each gray level. A histogram can help identify any bias in image intensity, which can prove useful when enhancing contrast etc. The histogram class also contains methods for creating a histogram image representation of a source image.  


## Image Resize Class
Class for resizing an image.   


## Image Thresholding Class
When applying a threshold to an image, values below the threshold are changed to black (0x00), and values above (or equal to) the threshold are changed to white (0xFF, 255 decimal).  


### Basic Threshold
For basic thresholding, a pre-determined value is used for the threshold, typically in the middle of the pixel value range such as 0x7F (127 decimal).  Pixel values are simply determined to be either above/below the threshold.  


### Otsu's Method
Otsu's method is more complex, but ultimately finds a better threshold value. This method searches for the threshold that minimizes the metric known as intra-class variance by using a histogram of the image to split the values into two groups (ideally foreground and background) with the smallest total variance.  


### Chow & Kaneko Method
The Chow & Kaneko method builds on Otsu's method in that it first divides an image into individual regions (typically 9), applies Otsu's method to each individual region, and then uses a weighted threshold based on the nearest 4 regions to each pixel. Regions closer to a pixel will carry more weight.  
Obviously, this method is more processing intensive than strict Otsu's method (so may not be ideal for real-time applications), but will produce a better overall result when light intensity varies across an image.  
The Chow & Kaneko method is also known as local or adaptive thresholding.  

## Image Thumbnail Class
Class for creating a thumbnail size image of a source image.   
