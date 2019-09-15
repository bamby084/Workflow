using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for ImagePropertiesPanel.xaml
    /// </summary>
    public partial class ImagePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private ImageStyle _ImageStyle;
        private string _ImageName = "";
        private int _Pages = 0;

        public ImageStyle ImageStyle
        {
            get => _ImageStyle;
            set => _ImageStyle = value;
        }

        public string ImageName
        {
            get
            {
                return _ImageName + " - Image";
            }

            set
            {
                _ImageName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageName"));
            }
        }

        public ImagePropertiesPanel()
        {
            InitializeComponent();
        }

        public ImagePropertiesPanel(ImageStyle img)
        {
            ImageStyle = img;

            InitializeComponent();

            ImageName = $"{img.Id} - Image";
            {
                Binding binding = new Binding("ImageName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblImageName.SetBinding(Label.ContentProperty, binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            UpDownPage.Value = (Decimal)int.Parse(reader.GetAttribute("Page"));
            chkVariablePageSelection.IsChecked = bool.Parse(reader.GetAttribute("VPSelection"));
            chkMakeTransparency.IsChecked = bool.Parse(reader.GetAttribute("Tansparent"));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Page", UpDownPage.Value.ToString());
            writer.WriteAttributeString("VPSelection", chkVariablePageSelection.IsChecked.ToString());
            writer.WriteAttributeString("Tansparent", chkMakeTransparency.IsChecked.ToString());
        }

        public byte[] LoadImageBytes(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        public BitmapSource LoadBitmapSource(string path)
        {
            using (System.Drawing.Imaging.Metafile emf = new System.Drawing.Imaging.Metafile(path))
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(emf.Width, emf.Height))
            {
                bmp.SetResolution(emf.HorizontalResolution, emf.VerticalResolution);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(emf, 0, 0);
                    return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        public void ShowImageProperties(string fileName)
        {
            lblFile.Content = fileName;
            try
            {
                //using (FileStream ms = File.OpenRead(fileName))
                {
                    char[] split = new char[] { '.' };
                    string extension = fileName.Split(split)[1].ToLower();
                    if (!extension.Equals("wmf") && !extension.Equals("emf") && !extension.Equals("pdf"))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
                        
                        //lblPages.Content = images.Count;
                        lblFormat.Content = image.RawFormat;//info.Format;
                        lblWidth.Content = image.Width +" px";
                        lblHeight.Content = image.Height + " px";
                        lblType.Content = image.PixelFormat;
                        lblImageDPI.Content = (int)(image.HorizontalResolution + 0.5) + ", " + (int)(image.VerticalResolution + 0.5);
                        lblPages.Content = 1;
                        BitmapImage img = new BitmapImage(new Uri(fileName));
                        
                        imgPreview.Source = img;
                        imgPreview.Stretch = Stretch.UniformToFill;
                        imgPreview_3.Stretch = Stretch.UniformToFill;
                        UpDownPage.IsEnabled = false;
                    }
                    else if (extension.Equals("wmf") || extension.Equals("emf"))
                    {
                        BitmapSource image = LoadBitmapSource(fileName);
                        lblFormat.Content = "Wmf";//info.Format;
                        lblWidth.Content = image.Width;
                        lblHeight.Content = image.Height;
                        lblType.Content = image.Format;
                        lblImageDPI.Content = (int)(image.DpiX + 0.5) + ", " + (int)(image.DpiY + 0.5);
                        lblPages.Content = 1;
                        imgPreview.Source = image;
                        imgPreview_3.Source = image;
                        UpDownPage.IsEnabled = false;
                    }
                    else if (extension.Equals("pdf"))
                    {
                        IEnumerable<System.Drawing.Image> imgs = ExtractImagesFromPDF(fileName);
                        lblPages.Content = _Pages;
                        UpDownPage.Maximum = (decimal)_Pages;
                        lblFormat.Content = "Pdf";
                        foreach (System.Drawing.Image img in imgs)
                        {
                            lblWidth.Content = img.Width + " px";
                            lblHeight.Content = img.Height + " px";
                            lblType.Content = img.PixelFormat;
                            lblImageDPI.Content = (int)(img.HorizontalResolution + 0.5) + ", " + (int)(img.VerticalResolution + 0.5);

                            ImageSourceConverter c = new ImageSourceConverter();
                            imgPreview.Source = (ImageSource)c.ConvertFrom(img);
                            
                            break;
                        }

                        
                        
                    }

                    split = new char[] { '\\' };
                    string[] items = fileName.Split(split);
                    ImageName = items[items.Length - 1];
                }

            }
            catch (Exception ex)
            {
            }
        }

        public IEnumerable<System.Drawing.Image> ExtractImagesFromPDF(string sourcePdf)
        {
            var pdf = new PdfReader(sourcePdf);
            var raf = new RandomAccessFileOrArray(sourcePdf);
            try
            {
                _Pages = pdf.NumberOfPages;
                for (int pageNum = 1; pageNum <= pdf.NumberOfPages; pageNum++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNum);
                    PdfObject obj = FindImageInPDFDictionary(pg);
                    if (obj != null)
                    {
                        int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        PdfObject pdfObject = pdf.GetPdfObject(XrefIndex);
                        PdfStream pdfStream = (PdfStream)pdfObject;
                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStream);

                        if (bytes != null)
                        {
                            using (System.IO.MemoryStream memStream = new MemoryStream(bytes))
                            {
                                memStream.Position = 0;
                                System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                                yield return img;
                            }
                        }
                    }
                }
            }
            finally
            {
                pdf.Close();
                raf.Close();
            }
        }

        private PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }
                    }
                }
            }

            return null;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkMakeTransparency_Checked(object sender, RoutedEventArgs e)
        {
            var myBitmap = new Bitmap(lblFile.Content.ToString());
            myBitmap.MakeTransparent();
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(myBitmap.Width, myBitmap.Height))
            {
                bmp.SetResolution(myBitmap.HorizontalResolution, myBitmap.VerticalResolution);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(myBitmap, 0, 0);
                    imgPreview_3.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        private void chkMakeTransparency_Unchecked(object sender, RoutedEventArgs e)
        {
            var myBitmap = new Bitmap(lblFile.Content.ToString());
            //myBitmap.MakeTransparent();
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(myBitmap.Width, myBitmap.Height))
            {
                bmp.SetResolution(myBitmap.HorizontalResolution, myBitmap.VerticalResolution);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.DrawImage(myBitmap, 0, 0);
                    imgPreview_3.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }
    }
}
