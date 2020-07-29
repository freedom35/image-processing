# Image Processing Library
Open source image processing library targeting .NET Standard.  
Published package availabe for download on [NuGet.org](https://nuget.org)
<br />

## Image Bytes
Methods for returning the bytes of an image.  
<br />

## Image Thresholding
When applying a threshold to an image, values below the threshold are changed to black (0x00), and values above (or equal to) the threshold are changed to white (0xFF, 255 decimal).

### Basic Threshold
For basic thresholding, a pre-determined value is used for the threshold, typically in the middle of the pixel value range such as 0x7F (127 decimal).  Pixel values are simply determined to be either above/below the threshold. 

### Otsu's Method
Otsu's method is more complex, but ultimately finds a better threshold value. This method searches for the threshold that minimizes the metric known as intra-class variance by using a histogram of the image to split the values into two groups (ideally foreground and background) with the smallest total variance.  
<br />

## Image Combine
Methods for combining multiple images.
<br />

## Image Convert
Methods for converting images to different formats/types.

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