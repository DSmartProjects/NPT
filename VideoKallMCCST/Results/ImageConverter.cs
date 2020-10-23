using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VideoKallMCCST.Results
{
    class ImageConverter
    {

        async void ConverttoImage(byte[] byteArray)
        {
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(byteArray);
                    await writer.StoreAsync();
                }
                var image = new BitmapImage();
                await image.SetSourceAsync(stream);
                
            }
        }

        //public static async Task<Image> LoadBitmapFromStorageFile(string aFileName)
        //{
        //    StorageFile ImageFile;///= await Globals.picturesLibrary.GetFileAsync(aFileName);

        //    BitmapImage bimg = new BitmapImage();
        //    FileRandomAccessStream stream = null;// = (FileRandomAccessStream)await ImageFile.OpenAsync(FileAccessMode.Read);
        //    bimg.SetSource(stream);

        //    Image img = new Image();
        //    img.Source = bimg;

        //    return img;
        //}

        public async Task SaveImageToFileAsync(string fileName)
        {
            var rtb = new RenderTargetBitmap();
            //  await rtb.RenderAsync(img);

            StorageFolder storageFolder =null;//= await StorageFolder.GetFolderFromPathAsync(path);
            StorageFile storageFile = await storageFolder.CreateFileAsync(fileName);

            if (storageFile == null)
                return;

            var pixels = await rtb.GetPixelsAsync();
            using (IRandomAccessStream stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await
                BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)rtb.PixelWidth, (uint)rtb.PixelHeight, 200, 200, bytes);
                await encoder.FlushAsync();
            }
        }
        
        public async Task<ImageSource> SaveToImageSource(byte[] imageBuffer)
        {
            ImageSource imageSource = null;
            using (MemoryStream stream = new MemoryStream(imageBuffer))
            {
                var ras = stream.AsRandomAccessStream();
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, ras);
                var provider = await decoder.GetPixelDataAsync();
                byte[] buffer = provider.DetachPixelData();
                WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                imageSource = bitmap;

            }
            return imageSource;
        }
        void SavePic(bool der, string strByte)
        {
            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            byte[] strb = HexString2Bytes(strByte);
            //Image img1 = byteArrayToImage(strb);
            //ImageDisplay?.Invoke(image1);
            //img1.sa(@"c:\sujit\testdata.png");

        }


        private static byte[] HexString2Bytes(string hexString)
        {
            int bytesCount = (hexString.Length) / 2;
            byte[] bytes = new byte[bytesCount];
            for (int x = 0; x < bytesCount; ++x)
            {
                bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
            }

            return bytes;
        }
    }
}
