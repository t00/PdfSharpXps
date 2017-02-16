﻿using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;
//using System.Drawing;

namespace PdfSharp.Xps.Rendering
{
    /// <summary>
    /// Constructs a PdfTilingPattern from ImageBrush or VisualBrush.
    /// </summary>
    class TilingPatternBuilder : PatternOrShadingBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TilingPatternBuilder"/> class.
        /// </summary>
        TilingPatternBuilder(DocumentRenderingContext context) :
            base(context)
        { }

        /// <summary>
        /// Builds a tiling pattern from an image brush.
        /// </summary>
        public static PdfTilingPattern BuildFromImageBrush(DocumentRenderingContext context, ImageBrush brush, XMatrix transform)
        {
          TilingPatternBuilder builder = new TilingPatternBuilder(context);
          PdfTilingPattern pattern = builder.BuildPattern(brush, transform);
          return pattern;
        }

        /// <summary>
        /// Builds a PdfTilingPattern pattern from a visual brush.
        /// </summary>
        public static PdfTilingPattern BuildFromVisualBrush(
          DocumentRenderingContext context, VisualBrush brush, XMatrix transform)
        {
          var builder = new TilingPatternBuilder(context);
          PdfTilingPattern pdfPattern = builder.BuildPattern(brush, transform);
          return pdfPattern;
        }

        PdfTilingPattern BuildPattern(ImageBrush brush, XMatrix transform)
        {
          // Bounding box repects viewbox
            //XRect bbox = new XRect(brush.Viewbox.X, brush.Viewbox.Y, brush.Viewbox.Width, brush.Viewbox.Height);
           // double scalex = brush.Viewport.Width / brush.Viewbox.Width * 96 / pdfForm.DpiX;
            //BitmapImage src = new BitmapImage();
            //BitmapSource cropbrush = BitmapSource.Create()
            //CroppedBitmap cropbrush = new CroppedBitmap((BitmapSource)brush.ImageSource, new System.Windows.Int32Rect((int)brush.Viewbox.X, (int)brush.Viewbox.Y, (int)brush.Viewbox.Width, (int)brush.Viewbox.Height));
            //using (var fileStream = new System.IO.FileStream("c:\\fullimage.png", System.IO.FileMode.Create))
            //{
            //    BitmapEncoder encoder = new PngBitmapEncoder();
            //    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)brush.ImageSource));
            //    encoder.Save(fileStream);
            //}
            double scaledpix = ((BitmapSource)brush.ImageSource).DpiX;
            double scaledpiy = ((BitmapSource)brush.ImageSource).DpiY;

            if (brush.Viewbox.X > 0 || brush.Viewbox.Y > 0)
            {
                CroppedBitmap cropbrush = new CroppedBitmap((BitmapSource)brush.ImageSource, new System.Windows.Int32Rect((int)Math.Round((brush.Viewbox.X / 96) * scaledpix), (int)Math.Round((brush.Viewbox.Y / 96) * scaledpiy), (int)Math.Round((brush.Viewbox.Width / 96) * scaledpiy), (int)Math.Round((brush.Viewbox.Height / 96) * scaledpiy)));
                //cropbrush.DpiX = scaledpix;
                //cropbrush.DpiY = scaledpiy;
                brush.ImageSource = cropbrush;
                //brush.Viewport = new System.Windows.Rect(0, 0, brush.Viewport.Width, brush.Viewport.Height);
                //brush.Viewbox = new System.Windows.Rect(0, 0, ((Math.Round((brush.Viewbox.Width / 96) * scaledpiy)) / scaledpiy * 96) , brush.Viewbox.Height);
                brush.Viewbox = new System.Windows.Rect(0, 0, brush.Viewbox.Width, brush.Viewbox.Height);
            }

            //brush = new ImageBrush(cropbrush);

            //BitmapSource
            //BitmapSource. cropbrushbitmap = new BitmapSource(cropbrush);


            //using (var fileStream = new System.IO.FileStream("c:\\cropimage.png", System.IO.FileMode.Create))
            //{
             //   BitmapEncoder encoder = new PngBitmapEncoder();
             //   encoder.Frames.Add(BitmapFrame.Create(cropbrush));
             //   encoder.Save(fileStream);
            //}

            //XRect bbox = new XRect(brush.Viewport.X - brush.Viewbox.X, brush.Viewport.Y - brush.Viewbox.Y, brush.Viewbox.Width, brush.Viewbox.Height);
            //XRect bbox = new XRect((brush.Viewbox.X / 96) * scaledpix, (brush.Viewbox.Y / 96) * scaledpiy, (brush.Viewbox.Width / 96) * scaledpix, (brush.Viewbox.Height / 96) * scaledpiy);
            //XRect bbox = new XRect(0, 0, (brush.Viewport.Width / 96) * scaledpix, (brush.Viewport.Height / 96) * scaledpiy);
            XRect bbox = new XRect(0, 0, brush.Viewport.Width, brush.Viewport.Height);

#if true
          XMatrix matrix = transform;
//this only needs to be a translate and to offset the viewbox viewport difference
          matrix.TranslatePrepend(brush.Viewport.X, brush.Viewport.Y);
          //matrix.TranslatePrepend((brush.Viewport.X - brush.Viewbox.X)+2, (brush.Viewport.Y - brush.Viewbox.Y)+2);

         if (brush.Transform != null)
         {
             matrix.Prepend(new XMatrix(brush.Transform.Value.M11, brush.Transform.Value.M12, brush.Transform.Value.M21,
             brush.Transform.Value.M22, brush.Transform.Value.OffsetX,
             brush.Transform.Value.OffsetY));
         }
#else
      double c = 1;
      XMatrix matrix = new XMatrix(1 * c, 0, 0, 1 * c, brush.Viewport.X * c, brush.Viewport.Y * c); // HACK: 480
      XMatrix t = transform;
      //t.Invert();
      t.Prepend(matrix);
      //t.TranslateAppend(brush.Viewport.X , brush.Viewport.Y);
      //matrix.Append(t);
      matrix = t;
#endif
          double xStep = brush.Viewport.Width+1;// (brush.Viewbox.Width / 96) * scaledpix; //* (brush.TileMode == TileMode.None ? 2 : 1);
          double yStep = brush.Viewport.Height+1;// (brush.Viewbox.Height / 96) * scaledpiy; //* (brush.TileMode == TileMode.None ? 2 : 1);


          // PdfTilingPattern
          //<<
          //  /BBox [0 0 240 120]
          //  /Length 74
          //  /Matrix [0.75 0 0 -0.75 0 480]
          //  /PaintType 1
          //  /PatternType 1
          //  /Resources
          //  <<
          //    /ExtGState
          //    <<
          //      /GS0 10 0 R
          //    >>
          //    /XObject
          //    <<
          //      /Fm0 17 0 R
          //    >>
          //  >>
          //  /TilingType 3
          //  /Type /Pattern
          //  /XStep 480
          //  /YStep 640
          //>>
          //stream
          //  q
          //  0 0 240 120 re
          //  W n
          //  q
          //    2.3999939 0 0 1.1999969 0 0 cm
          //    /GS0 gs
          //    /Fm0 Do
          //  Q
          //Q
          //endstream
          PdfTilingPattern pattern = Context.PdfDocument.Internals.CreateIndirectObject<PdfTilingPattern>();
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.PatternType, 1);  // Tiling
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.PaintType, 1);  // Color
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.TilingType, 3);  // Constant spacing and faster tiling
          pattern.Elements.SetMatrix(PdfTilingPattern.Keys.Matrix, matrix);
          pattern.Elements.SetRectangle(PdfTilingPattern.Keys.BBox, new PdfRectangle(bbox));
          pattern.Elements.SetReal(PdfTilingPattern.Keys.XStep, xStep);
          pattern.Elements.SetReal(PdfTilingPattern.Keys.YStep, yStep);

          // Set extended graphic state like Acrobat do
          PdfExtGState pdfExtGState = Context.PdfDocument.Internals.CreateIndirectObject<PdfExtGState>();
          pdfExtGState.SetDefault1();

          PdfFormXObject pdfForm = BuildForm(brush);
          //XRect viewBoxForm = new XRect(0, 0, 640, 480);

          PdfContentWriter writer = new PdfContentWriter(Context, pattern);
          writer.BeginContentRaw();

          // Acrobat 8 clips to bounding box, so do we
          //writer.WriteClip(bbox);

          XMatrix transformation = new XMatrix();
          //double dx = brush.Viewport.Width / brush.Viewbox.Width * 96 / pdfForm.DpiX;
          //double dy = brush.Viewport.Height / brush.Viewbox.Height * 96 / pdfForm.DpiY;

          double dx = brush.Viewport.Width / brush.Viewbox.Width * 96 / pdfForm.DpiX;
          double dy = brush.Viewport.Height / brush.Viewbox.Height * 96 / pdfForm.DpiY;
          transformation = new XMatrix(dx, 0, 0, dy, 0, 0);
          writer.WriteMatrix(transformation);
          writer.WriteGraphicsState(pdfExtGState);
            
          string name = writer.Resources.AddForm(pdfForm);
          writer.WriteLiteral(name + " Do\n");

          writer.EndContent();

          return pattern;
        }

        /// <summary>
        /// Builds a PdfFormXObject from the specified brush. 
        /// </summary>
        PdfFormXObject BuildForm(ImageBrush brush)
        {
          //<<
          //  /BBox [0 100 100 0]
          //  /Length 65
          //  /Matrix [1 0 0 1 0 0]
          //  /Resources
          //  <<
          //    /ColorSpace
          //    <<
          //      /CS0 15 0 R
          //    >>
          //    /ExtGState
          //    <<
          //      /GS0 10 0 R
          //    >>
          //    /ProcSet [/PDF /ImageC /ImageI]
          //    /XObject
          //    <<
          //      /Im0 16 0 R
          //    >>
          //  >>
          //  /Subtype /Form
          //>>
          //stream
          //  q
          //  0 0 100 100 re
          //  W n
          //  q
          //    /GS0 gs
          //    100 0 0 -100 0 100 cm
          //    /Im0 Do
          //  Q
          //Q
          //endstream
          PdfFormXObject pdfForm = Context.PdfDocument.Internals.CreateIndirectObject<PdfFormXObject>();
          XImage ximage = ImageBuilder.FromImageBrush(Context, brush);
          ximage.Interpolate = false;
          double width = ximage.PixelWidth ;/// ximage.HorizontalResolution) * 96;
          double height = ximage.PixelHeight;// / ximage.HorizontalResolution) *96;
          pdfForm.DpiX = ximage.HorizontalResolution;
          pdfForm.DpiY =  ximage.VerticalResolution;

          // view box in point
          // XRect box = new XRect(brush.Viewbox.X * 0.75, brush.Viewbox.Y * 0.75, brush.Viewbox.Width * 0.75, brush.Viewbox.Height * 0.75);
          //XRect box = new XRect(0, 0, width, height);

          pdfForm.Elements.SetRectangle(PdfFormXObject.Keys.BBox, new PdfRectangle(0, height, width, 0));
          //pdfForm.Elements.SetMatrix(PdfFormXObject.Keys.Matrix, new XMatrix());

          PdfContentWriter writer = new PdfContentWriter(Context, pdfForm);

          Debug.Assert(ximage != null);

          //PdfFormXObject pdfForm = xform.PdfForm;
          pdfForm.Elements.SetMatrix(PdfFormXObject.Keys.Matrix, new XMatrix());

          //formWriter.Size = brush.Viewport.Size;
          writer.BeginContentRaw();

          string imageID = writer.Resources.AddImage(new PdfImage(Context.PdfDocument, ximage));
          //XMatrix matrix = new XMatrix();
          //double scaleX = brush.Viewport.Width / brush.Viewbox.Width * 4 / 3 * ximage.PointWidth;
          //double scaleY = brush.Viewport.Height / brush.Viewbox.Height * 4 / 3 * ximage.PointHeight;
          //matrix.TranslatePrepend(-brush.Viewbox.X, -brush.Viewbox.Y);
          //matrix.ScalePrepend(scaleX, scaleY);
          //matrix.TranslatePrepend(brush.Viewport.X / scaleX, brush.Viewport.Y / scaleY);

          //double scaleX = 96 / ximage.HorizontalResolution;
          //double scaleY = 96 / ximage.VerticalResolution;
          //width *= scaleX;
          //height *= scaleY;
          XMatrix matrix = new XMatrix(width, 0, 0, -height, 0, height);
          writer.WriteLiteral("q\n");
          // TODO:WriteClip(path.Data);
          //formWriter.WriteLiteral("{0:0.###} 0 0 -{1:0.###} {2:0.###} {3:0.###} cm 100 Tz {4} Do Q\n",
          //  matrix.M11, matrix.M22, matrix.OffsetX, matrix.OffsetY + brush.Viewport.Height, imageID);
          writer.WriteMatrix(matrix);
          writer.WriteLiteral(imageID + " Do Q\n");

#if DEBUG
          if (DevHelper.BorderPatterns)
            writer.WriteLiteral("1 1 1 rg 0 0 m {0:0.###} 0 l {0:0.###} {1:0.###} l 0 {1:0.###} l h s\n", width, height);
#endif

          writer.EndContent();

          return pdfForm;
        }

        PdfTilingPattern BuildPattern(VisualBrush brush, XMatrix transform)
        {
          // Bounding box lays always at (0,0)
          XRect bbox = new XRect(0, 0, brush.Viewport.Width, brush.Viewport.Height);

          XMatrix matrix = transform;
          matrix.Prepend(new XMatrix(1, 0, 0, 1, brush.Viewport.X, brush.Viewport.Y));
          if (brush.Transform != null)
          {
            matrix.Prepend(new XMatrix(brush.Transform.Value.M11, brush.Transform.Value.M12,
              brush.Transform.Value.M21,brush.Transform.Value.M22,
              brush.Transform.Value.OffsetX, brush.Transform.Value.OffsetY));
          }

          // Set X Step beyond the viewport if tilemode is set to none since
          // there is no other easy way to turn off tiling - NPJ
          double xStep = brush.Viewport.Width * (brush.TileMode == TileMode.None ? 2 : 1);
          double yStep = brush.Viewport.Height * (brush.TileMode == TileMode.None ? 2 : 1);

          // PdfTilingPattern
          //<<
          //  /BBox [0 0 240 120]
          //  /Length 74
          //  /Matrix [0.75 0 0 -0.75 0 480]
          //  /PaintType 1
          //  /PatternType 1
          //  /Resources
          //  <<
          //    /ExtGState
          //    <<
          //      /GS0 10 0 R
          //    >>
          //    /XObject
          //    <<
          //      /Fm0 17 0 R
          //    >>
          //  >>
          //  /TilingType 3
          //  /Type /Pattern
          //  /XStep 480
          //  /YStep 640
          //>>
          //stream
          //  q
          //  0 0 240 120 re
          //  W n
          //  q
          //    2.3999939 0 0 1.1999969 0 0 cm
          //    /GS0 gs
          //    /Fm0 Do
          //  Q
          //Q
          //endstream
          var pattern = new PdfTilingPattern(Context.PdfDocument);
          Context.PdfDocument.Internals.AddObject(pattern);
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.PatternType, 1);  // Tiling
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.PaintType, 1);  // Color
          pattern.Elements.SetInteger(PdfTilingPattern.Keys.TilingType, 3);  // Constant spacing and faster tiling
          pattern.Elements.SetMatrix(PdfTilingPattern.Keys.Matrix, matrix);
          pattern.Elements.SetRectangle(PdfTilingPattern.Keys.BBox, new PdfRectangle(bbox));
          pattern.Elements.SetReal(PdfTilingPattern.Keys.XStep, xStep);
          pattern.Elements.SetReal(PdfTilingPattern.Keys.YStep, yStep);

          // Set extended graphic state like Acrobat do
          var pdfExtGState = Context.PdfDocument.Internals.CreateIndirectObject<PdfExtGState>();
          pdfExtGState.SetDefault1();

          var pdfForm = BuildForm(brush);

          var writer = new PdfContentWriter(Context, pattern);
          writer.BeginContentRaw();

          // Acrobat 8 clips to bounding box, so do we
          //writer.WriteClip(bbox);

          XMatrix transformation = new XMatrix();
          double dx = brush.Viewport.Width / brush.Viewbox.Width * 96 / pdfForm.DpiX;
          double dy = brush.Viewport.Height / brush.Viewbox.Height * 96 / pdfForm.DpiY;
          transformation = new XMatrix(dx, 0, 0, dy, 0, 0);
          writer.WriteMatrix(transformation);
          writer.WriteGraphicsState(pdfExtGState);

          string name = writer.Resources.AddForm(pdfForm);
          writer.WriteLiteral(name + " Do\n");

          writer.EndContent();

          return pattern;
        }
      
        /// <summary>
        /// Builds a PdfFormXObject from the specified brush. 
        /// </summary>
        PdfFormXObject BuildForm(VisualBrush brush)
        {
          //<<
          //  /BBox [0 100 100 0]
          //  /Length 65
          //  /Matrix [1 0 0 1 0 0]
          //  /Resources
          //  <<
          //    /ColorSpace
          //    <<
          //      /CS0 15 0 R
          //    >>
          //    /ExtGState
          //    <<
          //      /GS0 10 0 R
          //    >>
          //    /ProcSet [/PDF /ImageC /ImageI]
          //    /XObject
          //    <<
          //      /Im0 16 0 R
          //    >>
          //  >>
          //  /Subtype /Form
          //>>
          //stream
          //  q
          //  0 0 100 100 re
          //  W n
          //  q
          //    /GS0 gs
          //    100 0 0 -100 0 100 cm
          //    /Im0 Do
          //  Q
          //Q
          //endstream
          var pdfForm = Context.PdfDocument.Internals
            .CreateIndirectObject<PdfFormXObject>();

          pdfForm.DpiX = 96;
          pdfForm.DpiY = 96;

          // view box
          var box = new PdfRectangle(brush.Viewbox.X,
            brush.Viewbox.Y + brush.Viewbox.Height - 1,
            brush.Viewbox.X + brush.Viewbox.Width - 1, 
            brush.Viewbox.Y);

          pdfForm.Elements.SetRectangle(PdfFormXObject.Keys.BBox, box);

          pdfForm.Elements.SetMatrix(PdfFormXObject.Keys.Matrix, new XMatrix());

          var writer = new PdfContentWriter(Context, pdfForm);

          pdfForm.Elements.SetMatrix(PdfFormXObject.Keys.Matrix, new XMatrix());

          writer.BeginContentRaw();
          writer.WriteLiteral("-100 Tz\n");
          writer.WriteLiteral("q\n");
          writer.WriteVisual(brush.Visual);
          writer.WriteLiteral("Q\n");

#if DEBUG
          if (DevHelper.BorderPatterns)
            writer.WriteLiteral("1 1 1 rg 0 0 m {0:0.###} 0 l {0:0.###} {1:0.###} l 0 {1:0.###} l h s\n",
                                        brush.Viewbox.Width, brush.Viewbox.Height);
#endif

          writer.EndContent();

          return pdfForm;
        }
    }
}
