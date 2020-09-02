# Image Processing Library
Open source image processing library targeting .NET Standard.  
Published package available for download on [NuGet.org](https://nuget.org)  

You are welcome to use/update this software under the terms of the **MIT license**.  
<br />

## Usage
Below are the steps involved in using the Image Processing Library in your own projects.  
The repository also contains an **ImageViewerApp** (WPF) project to demonstrate usage of the Image Processing Library.

Note: Examples are in C#, but the library may also be used in other .NET language projects.

1. Add the **Image Processing Library** NuGet package to your .NET project.  

2. Include the namespace for the Image Processing Library at the top of your code file.  
```csharp
using Freedom35.ImageProcessing;
```

3. Most methods within the library are static, they can be called directly - they do not require instantiating a class. Make the appropriate call to the method associated with the image processing you wish to perform.  
Example:
```csharp
Image newImage = ImageColor.ToNegative(currentImage);
```  

4. If you're only making a single call and do not necessarily want to include the namespace at the top of your code file, methods may also be called by explicitly including the namespace before the method name.  
Example:
```csharp
Image newImage = Freedom35.ImageProcessing.ImageColor.ToNegative(currentImage);
```  
Most methods have a standard method that will return a new image (leaving the original intact), and also a direct method which will alter the original image directly. The **direct methods require using a Bitmap** encoded image. The standard methods will automatically convert the image to a Bitmap (if required) for processing, and return the processed image in the original format.  
Example:
```csharp
// Will apply threshold and return a new image
Image newImage = ImageThreshold.ApplyOtsuMethod(currentImage);
```  
```csharp
// Will apply threshold directly to current image
ImageThreshold.ApplyOtsuMethodDirect(ref currentImage);
``` 

## Image Bytes
Methods for returning the bytes of an image.  
<br />

## Image Thresholding
When applying a threshold to an image, values below the threshold are changed to black (0x00), and values above (or equal to) the threshold are changed to white (0xFF, 255 decimal).

### Basic Threshold
For basic thresholding, a pre-determined value is used for the threshold, typically in the middle of the pixel value range such as 0x7F (127 decimal).  Pixel values are simply determined to be either above/below the threshold. 

### Otsu's Method
Otsu's method is more complex, but ultimately finds a better threshold value. This method searches for the threshold that minimizes the metric known as intra-class variance by using a histogram of the image to split the values into two groups (ideally foreground and background) with the smallest total variance.  

### Chow & Kaneko Method
The Chow & Kaneko method builds on Otsu's method in that it first divides an image into individual regions (typically 9), applies Otsu's method to each individual region, and then uses a weighted threshold based on the nearest 4 regions to each pixel. Regions closer to a pixel will carry more weight.  
Obviously, this method is more processing intensive than strict Otsu's method (so may not be ideal for real-time applications), but will produce a better overall result when light intensity varies across an image.  
The Chow & Kaneko method is also known as local or adaptive thresholding.  
<br />

## Image Combine
Methods for combining multiple images.  
<br />

## Image Convert
Methods for converting images to different formats/types.  
<br />

## Image Color
Methods for manipulating and filtering image colors.  
<br />

## Image Convolution
An image can be manipulated to enhance or isolate features etc. This is a fundamental of image processing and often used as a pre-processing step in machine vision applications.

### Convolution Filters
Types of filters to apply to an image.

#### Edge
Filter to detect the edges within an image, either horizontal, vertical, or both.

#### Smoothing

#### Noise Reduction

#### Sharpen

#### Mexican Hat

#### Laplacian A

#### Laplacian B

#### Sobel Horizontal

#### Sobel Vertical

#### Emboss

## Image Copy

## Image Edit

## Image Encoding