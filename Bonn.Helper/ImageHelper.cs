using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Bonn.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="picData"></param>
        /// <returns></returns>
        public byte[] JpgBytetoBmpHrayByte(byte[] picData)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(picData, 0, picData.Length);
            Bitmap image = new Bitmap(ms);
            Bitmap newimage = new Bitmap(image);
            image.Dispose();
            newimage.Save(ms, ImageFormat.Bmp);

            return RgbToGrayScale(newimage);

        }

        /// <summary>
        /// 将源图像灰度化，并转化为8位灰度图像。
        /// </summary>
        /// <param name="original"> 源图像。 </param>
        /// <returns> 8位灰度图像。 </returns>
        public byte[] RgbToGrayScale(Bitmap original)
        {
            if (original != null)
            {
                // 将源图像内存区域锁定
                Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
                System.Drawing.Imaging.BitmapData bmpData = original.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                // 获取图像参数
                int width = bmpData.Width;
                int height = bmpData.Height;
                int stride = bmpData.Stride;  // 扫描线的宽度,比实际图片要大
                int offset = stride - width * 3;  // 显示宽度与扫描线宽度的间隙
                IntPtr ptr = bmpData.Scan0;   // 获取bmpData的内存起始位置的指针
                int scanBytesLength = stride * height;  // 用stride宽度，表示这是内存区域的大小

                // 分别设置两个位置指针，指向源数组和目标数组
                int posScan = 0, posDst = 0;
                byte[] rgbValues = new byte[scanBytesLength];  // 为目标数组分配内存
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, scanBytesLength);  // 将图像数据拷贝到rgbValues中
                // 分配灰度数组
                byte[] grayValues = new byte[width * height]; // 不含未用空间。
                // 计算灰度数组

                byte blue, green, red;



                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        blue = rgbValues[posScan];
                        green = rgbValues[posScan + 1];
                        red = rgbValues[posScan + 2];
                        grayValues[posDst] = (byte)((blue * 0.114 + green * 0.587 + red * 0.299));
                        posScan += 3;
                        posDst++;

                    }
                    // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
                    posScan += offset;
                }

                // 内存解锁
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, scanBytesLength);
                original.UnlockBits(bmpData);  // 解锁内存区域

                return grayValues;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 二值化命令数据数组 转 png图像Byte数组 
        /// </summary>
        public Bitmap JpgPicDataToBitmap(byte[] picData, int picWidth, int picHeight)
        {
            byte[] bmptemp = JpgBytetoBmpHrayByte(picData);
            Bitmap retBitmap = BuiltGrayBitmap(bmptemp, picWidth, picHeight);
            return retBitmap;
        }

        /// <summary>
        /// 二值化命令数据数组 转 png图像Byte数组 
        /// </summary>
        public byte[] JpgPicDataToBitmapArray(byte[] picData, int picWidth, int picHeight)
        {
            byte[] bmptemp = JpgBytetoBmpHrayByte(picData);
            Bitmap retBitmap = BuiltGrayBitmap(bmptemp, picWidth, picHeight);
            return BitmapToByteArray(retBitmap);
        }

        #region 压缩图像转换

        /// <summary>
        /// 压缩图像命令数据数组 转 png图像Byte数组 
        /// </summary>
        /// <param name="picData">压缩图像命令数据</param>
        /// <param name="picWidth">宽</param>
        /// <param name="picHeight">高</param>
        /// <returns>png图像Byte数组</returns>
        public byte[] PicDataToBitmapArray(byte[] picData, int picWidth, int picHeight)
        {
            Bitmap bitmap = PicDataToBitmap(picData, picWidth, picHeight);
            return BitmapToByteArray(bitmap);
        }

        /// <summary>
        /// 压缩图像命令数据数组 转 图像 (注：转换中需要将 Byte数组 转成 Bit数组)
        /// </summary>
        /// <param name="picData">压缩图像命令数据</param>
        /// <param name="picWidth">宽</param>
        /// <param name="picHeight">高</param>
        /// <returns></returns>
        private Bitmap PicDataToBitmap(byte[] picData, int picWidth, int picHeight)
        {
            try
            {
                if (picData.Length < 2 || picWidth < 1 || picHeight < 1)
                    return null;
                byte[] bitPicData = picData.ToArray4BitByte(picWidth, picHeight);

                //增加识别算法
                byte[,] arrayDblByt = bitPicData.ToDoubleArrayByte(picWidth, picHeight);

                Bitmap bitmap = new Bitmap(picWidth, picHeight);
                Graphics graphics = Graphics.FromImage(bitmap);
                for (int iHeight = 0; iHeight < picHeight; iHeight++)
                {
                    for (int iWidth = 0; iWidth < picWidth; iWidth++)
                    {
                        //这里的是灰度读写，三色取值一样
                        byte red = arrayDblByt[iHeight, iWidth];
                        byte green = red;
                        byte blue = red;
                        Color customColor = Color.FromArgb(red, green, blue);
                        SolidBrush shadowBrush = new SolidBrush(customColor);
                        graphics.FillRectangle(shadowBrush, iWidth, iHeight, 1, 1);
                    }
                }
                return bitmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Bitmap 转 Byte数组
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public byte[] BitmapToByteArray(Bitmap bitmap)
        {
            if (bitmap == null)
                return null;
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            return ms.GetBuffer();
        }

        /// <summary>
        /// 用灰度数组新建一个8位灰度图像。
        /// </summary>
        /// <param name="rawValues"> 灰度数组(length = width * height)。 </param>
        /// <param name="width"> 图像宽度。 </param>
        /// <param name="height"> 图像高度。 </param>
        /// <returns> 新建的8位灰度位图。 </returns>
        public Bitmap BuiltGrayBitmap(byte[] rawValues, int width, int height)
        {
            // 新建一个8位灰度位图，并锁定内存区域操作
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // 计算图像参数
            int offset = bmpData.Stride - bmpData.Width;        // 计算每行未用空间字节数
            IntPtr ptr = bmpData.Scan0;                         // 获取首地址
            int scanBytes = bmpData.Stride * bmpData.Height;    // 图像字节数 = 扫描字节数 * 高度
            byte[] grayValues = new byte[scanBytes];            // 为图像数据分配内存

            // 为图像数据赋值
            int posSrc = 0, posScan = 0;                        // rawValues和grayValues的索引
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    grayValues[posScan++] = rawValues[posSrc++];
                }
                // 跳过图像数据每行未用空间的字节，length = stride - width * bytePerPixel
                posScan += offset;
            }

            // 内存解锁
            System.Runtime.InteropServices.Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpData);  // 解锁内存区域

            // 修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette palette;
            // 获取一个Format8bppIndexed格式图像的Palette对象
            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                palette = bmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            // 修改生成位图的索引表
            bitmap.Palette = palette;

            return bitmap;
        }
        #endregion


        /// <summary>
        /// 解析图像数据算法
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="numWid"></param>
        /// <param name="numHei"></param>
        /// <returns></returns>
        public byte[] BitmapArithmetic(byte[] imageData, int numWid, int numHei)
        {
            int byteNum;// 字节数
            if ((numWid * numHei) % 8 == 0)
                byteNum = (numWid * numHei) / 8;
            else
                byteNum = (numWid * numHei) / 8 + 1;
            byte[] temp = new byte[8];//
            //字节换算位
            byte[] numData = new byte[numWid * numHei];// 字节数
            for (int j = 0; j < numData.Length; j++)
                numData[j] = 0xFF;// 初始化
            byte[] byteData = new byte[byteNum];
            for (int j = 0; j < byteData.Length; j++)
                byteData[j] = 0x00;// 初始化
            if (imageData.Length < byteNum)
                Buffer.BlockCopy(imageData, 0, byteData, 0, imageData.Length);
            else
                Buffer.BlockCopy(imageData, 0, byteData, 0, byteNum);
            for (int m = 0; m < byteNum; m++)
            {
                byte tempData = byteData[m];
                for (int k = 0; k < 8; k++)
                {
                    // 换算一个字节为8个图片字节
                    if (((tempData >> (k)) & 0x01) == 1)
                        temp[k] = 0x00;
                    else
                        temp[k] = 0xFF;
                }
                if (numWid * numHei - m * 8 >= 8)
                    Buffer.BlockCopy(temp, 0, numData, m * 8, 8);   // 正好够除以8
                else if (numWid * numHei - m * 8 < 8)
                    Buffer.BlockCopy(temp, 0, numData, m * 8, numWid * numHei - m * 8);       // 取多出的位数
            }
            Bitmap bitmap = BuiltGrayBitmap(numData, numWid, numHei);
            return BitmapToByteArray(bitmap);
        }

        /// <summary>
        /// 将终端上传的图像数据转换为Bitmap图像
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="numWid"></param>
        /// <param name="numHei"></param>
        /// <returns></returns>
        public Bitmap GetBitmap(byte[] imageData, int numWid, int numHei)
        {
            int byteNum;// 字节数
            if ((numWid * numHei) % 8 == 0)
                byteNum = (numWid * numHei) / 8;
            else
                byteNum = (numWid * numHei) / 8 + 1;
            byte[] temp = new byte[8];//
            //字节换算位
            byte[] numData = new byte[numWid * numHei];// 字节数
            for (int j = 0; j < numData.Length; j++)
                numData[j] = 0xFF;// 初始化
            byte[] byteData = new byte[byteNum];
            for (int j = 0; j < byteData.Length; j++)
                byteData[j] = 0x00;// 初始化
            if (imageData.Length < byteNum)
                Buffer.BlockCopy(imageData, 0, byteData, 0, imageData.Length);
            else
                Buffer.BlockCopy(imageData, 0, byteData, 0, byteNum);
            for (int m = 0; m < byteNum; m++)
            {
                byte tempData = byteData[m];
                for (int k = 0; k < 8; k++)
                {
                    // 换算一个字节为8个图片字节
                    if (((tempData >> (k)) & 0x01) == 1)
                        temp[k] = 0x00;
                    else
                        temp[k] = 0xFF;
                }
                if (numWid * numHei - m * 8 >= 8)
                    Buffer.BlockCopy(temp, 0, numData, m * 8, 8);   // 正好够除以8
                else if (numWid * numHei - m * 8 < 8)
                    Buffer.BlockCopy(temp, 0, numData, m * 8, numWid * numHei - m * 8);       // 取多出的位数
            }
            return BuiltGrayBitmap(numData, numWid, numHei);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <returns></returns>
        public int GetImageDataLength(int imageWidth, int imageHeight)
        {
            //图像宽*图像高/ 8，余数不为零加1
            int byteNum;// 字节数
            if ((imageWidth * imageHeight) % 8 == 0)
                byteNum = (imageWidth * imageHeight) / 8;
            else
                byteNum = (imageWidth * imageHeight) / 8 + 1;

            return byteNum;
        }

        /// <summary>
        /// 合并多副图像，中间用白条分隔
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        public Bitmap Combine(List<Bitmap> bitmaps)
        {
            if (bitmaps.Count <= 0)
                throw new Exception();

            int outMapWidth = 0;
            int outMapHeight = bitmaps[0].Height;
            foreach (Bitmap bitmap in bitmaps)
            {
                outMapWidth += bitmap.Width + 5;

                if (bitmap.Height > outMapHeight)
                    outMapHeight = bitmap.Height;
            }
            //最后一张图像右边不间隔，所以总长度减5
            outMapWidth -= 5;
            Bitmap outBitmap = new Bitmap(outMapWidth, outMapHeight);
            Graphics g = Graphics.FromImage(outBitmap);
            g.Clear(Color.White);//背景设置为白色
            int locationX = 0;
            int locationY = 0;
            for (int i = 0; i < bitmaps.Count; i++)
            {
                Bitmap bitmap = bitmaps[i];
                //索引号为1、3、5，实际为第2、4、6幅图像，前面加10像素分隔
                if (i == 1 || i == 3)
                    locationX += 5;
                //将单字的图像再添加到整个图像上
                g.DrawImage(bitmap, locationX, locationY, bitmap.Width, bitmap.Height);
                locationX += bitmap.Width;
            }
            g.Dispose();
            return outBitmap;
        }


        /// <summary>
        /// 裁剪合并图像
        /// </summary>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        public Bitmap CutCombine(List<CutImageInfo> bitmaps)
        {
            if (bitmaps.Count <= 0)
                throw new Exception();

            int outMapWidth = 0;
            int outMapHeight = 0;
            foreach (CutImageInfo bitmap in bitmaps)
            {
                outMapWidth += bitmap.CutWidth;

                if (bitmap.CutHeight > outMapHeight)
                    outMapHeight = bitmap.CutHeight;
            }

            Bitmap outBitmap = new Bitmap(outMapWidth, outMapHeight);
            Graphics g = Graphics.FromImage(outBitmap);
            g.Clear(Color.White);//背景设置为白色
            int locationX = 0;
            int locationY = 0;
            for (int i = 0; i < bitmaps.Count; i++)
            {
                CutImageInfo cutInfo = bitmaps[i];
                Bitmap bitmap = cutInfo.OrgBitmap;
                Rectangle srcRect = new Rectangle();
                srcRect.X = cutInfo.CutLocationX;
                srcRect.Y = cutInfo.CutLocationY;
                srcRect.Width = cutInfo.CutWidth;
                srcRect.Height = cutInfo.CutHeight;
                GraphicsUnit srcUnit = GraphicsUnit.Pixel;
                //将单字的图像再添加到整个图像上
                g.DrawImage(bitmap, locationX, locationY, srcRect, srcUnit);
                locationX += cutInfo.CutWidth;
            }
            g.Dispose();
            return outBitmap;
        }
    }

    /// <summary>
    /// 裁剪图像信息
    /// </summary>
    public class CutImageInfo
    {
        /// <summary>
        /// 原裁剪图像
        /// </summary>
        public Bitmap OrgBitmap;

        /// <summary>
        /// 裁剪起始位置X
        /// </summary>
        public int CutLocationX;

        /// <summary>
        /// 裁剪起始位置Y
        /// </summary>
        public int CutLocationY;

        /// <summary>
        /// 裁剪宽度
        /// </summary>
        public int CutWidth;

        /// <summary>
        /// 裁剪高度
        /// </summary>
        public int CutHeight;
    }
}
