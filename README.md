# PdfSharpXps
PdfSharp C# libraries allowing XPS to PDF export

This repository contains a Visual Studio 2015 / .Net 4.5 build of PdfSharp 1.32 with XPS support.
As far as I could investigate PdfSharp-XPS library is no longer maintained since PdfSharp release 1.32 which means 1.50b3 version won't support PDF export from FlowDocument / FixedDocument / XpsDocument.

Installation is as simple as adding references to PdfSharp\PdfSharp-WPF.csproj and PdfSharp.Xps\PdfSharp.Xps.csproj in your solution.
 
Example of use (saving XpsDocument object to pdf file):
    
    PdfSharp.Xps.XpsConverter.Convert(xpsDocument, pdfFileName, 0);

For other scenarios please refer to XpsConverter.cs converter implementation.
