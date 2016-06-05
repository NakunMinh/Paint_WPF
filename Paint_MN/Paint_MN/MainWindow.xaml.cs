using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Paint_MN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct MN
        {
            public ContentControl sContent;
            public Rectangle sRect;
            public Point sStart;
            public Point sEnd;
        }
        //xac dinh hinh ve
        // 1:line 2:rectange 3:ellipes 4:freestyle 5:eraser 6:add_text 7:star 8:heart 9:arrow
        private int key = 0;

        private Line _line;
        private Rectangle _rectange;
        private Ellipse _ellipes;
        private Line _freestyle;
        private System.Windows.Shapes.Path _star;
        private System.Windows.Shapes.Path _heart;
        private System.Windows.Shapes.Path _arrow;

        private string color;
        private int stroke;

        private int sizeEraser;
        private bool flagSizeEraser = false;

        private Point startPoint;

        private Stack<ImageSource> arrayImage = new Stack<ImageSource>();
        private Stack<ImageSource> tempArrayImage = new Stack<ImageSource>();

        private ContentControl _content;
        private ContentControl _tempcontent;

        private Point tempStart;
        private Point tempEnd;

        private Rectangle newrect;

        MN[] arrayMN = new MN[1];

        public MainWindow()
        {
            InitializeComponent();
        }

        //convert to bitmap
        public BitmapSource ConvertToBitmapSource(UIElement element)
        {
            var target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, PixelFormats.Pbgra32);
            var brush = new VisualBrush(element);
            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();

            drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), new Point(element.RenderSize.Width, element.RenderSize.Height)));
            drawingContext.PushOpacityMask(brush);
            drawingContext.Close();
            target.Render(visual);

            return target;
        }

        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            key = 1;
        }

        private void btnRectange_Click(object sender, RoutedEventArgs e)
        {
            key = 2;
        }

        private void btnEllip_Click(object sender, RoutedEventArgs e)
        {
            key = 3;
        }

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            key = 4;
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //khoi tao gia tri
            startPoint = e.GetPosition(canvas);

            tempStart = startPoint;

            color = cboColor.Text;
            stroke = int.Parse(cboStroke.Text);
            sizeEraser = int.Parse(cboSizeEraser.Text);

            //bien convert color
            var bc = new BrushConverter();

            //if (arrayMN != null)
            //{
            //    arrayMN[0].sRect.IsHitTestVisible = true;
            //    arrayMN[0].sContent.SetValue(Selector.IsSelectedProperty, true);
            //    arrayMN[0].sContent = null;
            //    arrayMN[0].sRect = null;
            //}

            //set gia tri tung truong hop
            switch (key)
            {
                case 1:
                    {
                        _line = new Line();
                        _line.Stroke = (Brush)bc.ConvertFrom(color);
                        _line.StrokeThickness = stroke;
                        canvas.Children.Add(_line);
                    }
                    break;
                case 2:
                    {
                        _rectange = new Rectangle();
                        _rectange.Stroke = (Brush)bc.ConvertFrom(color);
                        _rectange.StrokeThickness = stroke;
                        if (chekFill.IsChecked == true)
                        { _rectange.Fill = (Brush)bc.ConvertFrom(color); }
                        canvas.Children.Add(_rectange);
                    }
                    break;
                case 3:
                    {
                        _ellipes = new Ellipse();
                        _ellipes.Stroke = (Brush)bc.ConvertFrom(color);
                        _ellipes.StrokeThickness = stroke;
                        if (chekFill.IsChecked == true)
                        { _ellipes.Fill = (Brush)bc.ConvertFrom(color); }
                        canvas.Children.Add(_ellipes);
                    }
                    break;
                case 5:
                    {
                        flagSizeEraser = true;
                    }
                    break;
                case 6:
                    {
                        var mn = new FontFamilyConverter();
                        TextBox lb = new TextBox();
                        lb.Text = "Add text....";
                        lb.FontFamily = (FontFamily)mn.ConvertFromString(cboFontFamily.Text);
                        lb.FontSize = int.Parse(cboFontSize.Text);
                        lb.Foreground = (Brush)bc.ConvertFrom(color);
                        lb.BorderBrush = Brushes.White;
                        canvas.Children.Add(lb);
                        Canvas.SetTop(lb, startPoint.Y);
                        Canvas.SetLeft(lb, startPoint.X);
                    }
                    break;
                case 7:
                    {
                        _star = new System.Windows.Shapes.Path();
                        _star.Stroke = (Brush)bc.ConvertFrom(color);
                        _star.StrokeThickness = stroke;
                        if (chekFill.IsChecked == true)
                        { _star.Fill = (Brush)bc.ConvertFrom(color); }
                        canvas.Children.Add(_star);
                    }
                    break;
                case 8:
                    {
                        _heart = new System.Windows.Shapes.Path();
                        _heart.Stroke = (Brush)bc.ConvertFrom(color);
                        _heart.StrokeThickness = stroke;
                        if (chekFill.IsChecked == true)
                        { _heart.Fill = (Brush)bc.ConvertFrom(color); }
                        canvas.Children.Add(_heart);
                    }
                    break;
                case 9:
                    {
                        _arrow = new System.Windows.Shapes.Path();
                        _arrow.Stroke = (Brush)bc.ConvertFrom(color);
                        _arrow.StrokeThickness = stroke;
                        if (chekFill.IsChecked == true)
                        { _arrow.Fill = (Brush)bc.ConvertFrom(color); }
                        canvas.Children.Add(_arrow);
                    }
                    break;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //khoi tao gia tri diem cuoi
            Point EndPoint = e.GetPosition(canvas);

            switch (key)
            {
                case 1:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _line == null)
                            return;
                        _line.X1 = startPoint.X;
                        _line.Y1 = startPoint.Y;
                        _line.X2 = EndPoint.X;
                        _line.Y2 = EndPoint.Y;
                    }
                    break;
                case 2:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _rectange == null)
                            return;
                        var x = Math.Min(EndPoint.X, startPoint.X);
                        var y = Math.Min(EndPoint.Y, startPoint.Y);

                        var w = Math.Max(EndPoint.X, startPoint.X) - x;
                        var h = Math.Max(EndPoint.Y, startPoint.Y) - y;

                        _rectange.Width = w;
                        _rectange.Height = h;
                        Canvas.SetLeft(_rectange, x);
                        Canvas.SetTop(_rectange, y);
                    }
                    break;
                case 3:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _ellipes == null)
                            return;
                        var x = Math.Min(EndPoint.X, startPoint.X);
                        var y = Math.Min(EndPoint.Y, startPoint.Y);

                        var w = Math.Max(EndPoint.X, startPoint.X) - x;
                        var h = Math.Max(EndPoint.Y, startPoint.Y) - y;

                        _ellipes.Width = w;
                        _ellipes.Height = h;
                        Canvas.SetLeft(_ellipes, x);
                        Canvas.SetTop(_ellipes, y);
                    }
                    break;
                case 4:
                    {
                        var bc = new BrushConverter();
                        _freestyle = new Line();
                        _freestyle.Stroke = (Brush)bc.ConvertFrom(color);
                        _freestyle.StrokeThickness = stroke;
                        canvas.Children.Add(_freestyle);
                        if (e.LeftButton == MouseButtonState.Released || _freestyle == null)
                            return;
                        _freestyle.X1 = startPoint.X;
                        _freestyle.Y1 = startPoint.Y;
                        _freestyle.X2 = EndPoint.X;
                        _freestyle.Y2 = EndPoint.Y;

                        startPoint = EndPoint;
                    }
                    break;
                case 5:
                    {
                        if (flagSizeEraser == true)
                        {
                            Rectangle rectEraser = new Rectangle();
                            rectEraser.Fill = Brushes.White;

                            canvas.Children.Add(rectEraser);

                            var x = Math.Min(EndPoint.X, EndPoint.X - 2 * sizeEraser);
                            var y = Math.Min(EndPoint.Y, EndPoint.Y - 2 * sizeEraser);

                            var w = Math.Max(EndPoint.X, EndPoint.X - 2 * sizeEraser) - x;
                            var h = Math.Max(EndPoint.Y, EndPoint.Y - 2 * sizeEraser) - y;

                            rectEraser.Width = w;
                            rectEraser.Height = h;
                            Canvas.SetLeft(rectEraser, x);
                            Canvas.SetTop(rectEraser, y);
                        }
                    }
                    break;
                case 7:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _star == null)
                            return;
                        var x = Math.Min(EndPoint.X, startPoint.X);
                        var y = Math.Min(EndPoint.Y, startPoint.Y);

                        var w = Math.Max(EndPoint.X, startPoint.X) - x;
                        var h = Math.Max(EndPoint.Y, startPoint.Y) - y;

                        _star.Data = Geometry.Parse("M 50,50 l 15,0 l 5,-15 l 5,15 l 15,0 l -10,10 l 4,15 l -15,-9 l -15,9 l 7,-15 Z");

                        _star.Width = w;
                        _star.Height = h;
                        Canvas.SetLeft(_star, x);
                        Canvas.SetTop(_star, y);
                    }
                    break;
                case 8:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _heart == null)
                            return;
                        var x = Math.Min(EndPoint.X, startPoint.X);
                        var y = Math.Min(EndPoint.Y, startPoint.Y);

                        var w = Math.Max(EndPoint.X, startPoint.X) - x;
                        var h = Math.Max(EndPoint.Y, startPoint.Y) - y;

                        _heart.Data = Geometry.Parse("M 241,200 A 20,20 0 0 0 200,240 C 210,250 240,270 240,270 C 240,270 260,260 280,240 A 20,20 0 0 0 239,200");

                        _heart.Width = w;
                        _heart.Height = h;
                        Canvas.SetLeft(_heart, x);
                        Canvas.SetTop(_heart, y);
                    }
                    break;
                case 9:
                    {
                        if (e.LeftButton == MouseButtonState.Released || _arrow == null)
                            return;
                        var x = Math.Min(EndPoint.X, startPoint.X);
                        var y = Math.Min(EndPoint.Y, startPoint.Y);

                        var w = Math.Max(EndPoint.X, startPoint.X) - x;
                        var h = Math.Max(EndPoint.Y, startPoint.Y) - y;

                        _arrow.Data = Geometry.Parse("M20.125,32L0.5,12.375L10.3125,12.375L10.3125,0.5L29.9375,0.5L29.9375,12.375L39.75,12.375z");

                        _arrow.Width = w;
                        _arrow.Height = h;
                        Canvas.SetLeft(_arrow, x);
                        Canvas.SetTop(_arrow, y);
                    }
                    break;
            }

        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //luu hinh anh sau khi ve
            ImageSource imagesource = ConvertToBitmapSource(canvas);
            arrayImage.Push(imagesource);

            tempEnd = e.GetPosition(canvas);

            
            
            _tempcontent = new ContentControl();

            switch (key)
            {
                case 1:
                    { 
                        _line = null; 
                    }
                    break;
                case 2:
                    { 
                        _rectange = null;

                        //xoa previous shape
                        canvas.Children.RemoveRange(canvas.Children.Count - 1, canvas.Children.Count);
                        //tao hinh moi
                        newrect = new Rectangle();
                        arrayMN[0].sRect = newrect;
                        //set thuoc tinh giong hinh cu
                        var bc = new BrushConverter();
                        newrect.StrokeThickness = int.Parse(cboStroke.Text);
                        newrect.Stroke = (Brush)bc.ConvertFrom(color);
                        if (chekFill.IsChecked == true)
                        {
                            newrect.Fill = (Brush)bc.ConvertFrom(color);
                        }
                        newrect.IsHitTestVisible = false;

                        _content = new ContentControl();
                        arrayMN[0].sContent = _content;
                        _content.Content = newrect;

                        arrayMN[0].sStart = tempStart;
                        arrayMN[0].sEnd = tempEnd;

                        var x = Math.Min(tempEnd.X, tempStart.X);
                        var y = Math.Min(tempEnd.Y, tempStart.Y);

                        var w = Math.Max(tempEnd.X, tempStart.X) - x;
                        var h = Math.Max(tempEnd.Y, tempStart.Y) - y;

                        _content.Width = w;
                        _content.Height = h;

                        _content.MinWidth = 0;
                        _content.MinHeight = 0;

                        Canvas.SetLeft(_content, x);
                        Canvas.SetTop(_content, y);
                        _content.SetValue(Selector.IsSelectedProperty, true);
                        _content.Style = (Style)FindResource("DesignerItemStyle");

                        _tempcontent = _content;

                        canvas.Children.Add(_content);
                    }
                    break;
                case 3:
                    { _ellipes = null; } break;
                case 4:
                    { _freestyle = null; } break;
                case 5:
                    { flagSizeEraser = false; } break;
                case 7:
                    { _star = null; } break;
                case 8:
                    { _heart = null; } break;
                case 9:
                    { _arrow = null; } break;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.FileName = "NewFile";
            open.RestoreDirectory = true;
            open.DefaultExt = ".png";
            open.Filter = "Jpeg Files|*.jpg|Bmp Files|*.bmp|PNG Files|*.png|Tiff Files|*.tif|Gif Files|*.gif";
            if (open.ShowDialog() == true)
            {
                string t = open.FileName;
                Image img = new Image();
                ImageSource imgSrc = new BitmapImage(new Uri(t));
                img.Source = imgSrc;
                img.Height = canvas.Height;
                img.Width = canvas.Width;
                canvas.Children.Add(img);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "NewFile";
            save.RestoreDirectory = true;
            save.DefaultExt = ".png";
            save.Filter = "Jpeg Files|*.jpg|Bmp Files|*.bmp|PNG Files|*.png|Tiff Files|*.tif|Gif Files|*.gif";

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(canvas);

            if (save.ShowDialog() == true)
            {
                string t = save.FileName;
                BitmapEncoder encoder;
                string Extension = System.IO.Path.GetExtension(t).ToLower();
                if (Extension == ".gif")
                    encoder = new GifBitmapEncoder();
                else if (Extension == ".png")
                    encoder = new PngBitmapEncoder();
                else if (Extension == ".jpg")
                    encoder = new JpegBitmapEncoder();
                else
                    return;

                encoder.Frames.Add(BitmapFrame.Create(bmp));
                FileStream s = new FileStream(t, FileMode.OpenOrCreate, FileAccess.Write);
                encoder.Save(s);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            key = 5;
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {
            key = 6;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Image image = new Image();
            if (arrayImage.Count != 1)
            {
                ImageSource temp = arrayImage.Pop();
                tempArrayImage.Push(temp);
                image.Source = arrayImage.Peek();
                image.Height = canvas.Height;
                image.Width = canvas.Width;
                canvas.Children.Add(image);
            }       
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            Image image = new Image();
            if (tempArrayImage.Count != 0)
            {
                arrayImage.Push(tempArrayImage.Pop());
                image.Source = arrayImage.Peek();
                image.Height = canvas.Height;
                image.Width = canvas.Width;
                canvas.Children.Add(image);
            }
            
        }

        private void btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double posX = e.GetPosition(null).X;
            double posY = e.GetPosition(null).Y;

            double posX1 = posX - (posX / 4);
            double posY1 = posY - (posY / 4);

            double posX2 = posX + (posX / 4);
            double posY2 = posY;

            double posX3 = posX - (posX / 4);
            double posY3 = posY + (posY - posY / 3);


            Polygon p = new Polygon();
            p.Stroke = Brushes.Black;
            p.Fill = Brushes.LightBlue;
            p.StrokeThickness = 1;
            p.HorizontalAlignment = HorizontalAlignment.Left;
            p.VerticalAlignment = VerticalAlignment.Center;
            p.Points = new PointCollection() { new Point(posX1, posY1), new Point(posX2, posY2), new Point(posX3, posY3), new Point(posX1, posY1) };
            canvas.Children.Add(p);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //
        }

        private void btnStar_Click(object sender, RoutedEventArgs e)
        {
            key = 7;
        }

        private void btnHeart_Click(object sender, RoutedEventArgs e)
        {
            key = 8;
        }

        private void btnArrow_Click(object sender, RoutedEventArgs e)
        {
            key = 9;
        }
    }
}
