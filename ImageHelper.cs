using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Cyotek.Drawing.Imaging.ColorReduction;
using Cyotek.Drawing;
public static class ImageHelper
{
    /// <summary> 
    /// 生成缩略图 
    /// </summary>
    /// <param name="width">缩略图宽度</param> 
    /// <param name="height">缩略图高度</param> 
    /// <param name="mode">生成缩略图的方式:HW指定高宽缩放(可能变形);W指定宽，高按比例 H指定高，宽按比例 Cut指定高宽裁减(不变形)</param>　　 
    public static Bitmap MakeThumbnail(Bitmap _originalImage, int width, int height, string mode)
    {
        int _towidth = width == 0 ? _originalImage.Width : width;
        int _toheight = height == 0 ? _originalImage.Height : height; ;
        int x = 0;
        int y = 0;
        int ow = _originalImage.Width;
        int oh = _originalImage.Height;
        switch (mode)
        {
            case "HW"://指定高宽缩放（可能变形）
                break;
            case "W"://指定宽，高按比例　　　　　　　　　　 
                _toheight = _originalImage.Height * width / _originalImage.Width;
                break;
            case "H"://指定高，宽按比例 
                _towidth = _originalImage.Width * height / _originalImage.Height;
                break;
            case "Cut"://指定高宽裁减（不变形）　　　　　　　　 
                if ((double)_originalImage.Width / (double)_originalImage.Height > (double)_towidth / (double)_toheight)
                {
                    oh = _originalImage.Height;
                    ow = _originalImage.Height * _towidth / _toheight;
                    y = 0;
                    x = (_originalImage.Width - ow) / 2;
                }
                else
                {
                    ow = _originalImage.Width;
                    oh = _originalImage.Width * height / _towidth;
                    x = 0;
                    y = (_originalImage.Height - oh) / 2;
                }
                break;
            case "MaxHW":// 比例进行按照自身缩放
                if (_originalImage.Height > _toheight || _originalImage.Width > _towidth)
                {
                    if ((double)_originalImage.Width / (double)_originalImage.Height > (double)_towidth / (double)_toheight)
                    {
                        //_towidth = _towidth;
                        _toheight = (int)(_towidth * ((double)_originalImage.Height / (double)_originalImage.Width));
                    }
                    else
                    {
                        // _toheight = _toheight;
                        _towidth = (int)(_toheight * ((double)_originalImage.Width / (double)_originalImage.Height));
                    }
                }
                else
                {
                    _towidth = _originalImage.Width;
                    _toheight = _originalImage.Height;
                }
                break;
            default:
                break;
        }
        //新建一个bmp图片 
        Bitmap _bitmap = new Bitmap(_towidth, _toheight);
        //新建一个画板 
        Graphics g = Graphics.FromImage(_bitmap);
        //设置高质量插值法 
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //设置高质量,低速度呈现平滑程度 
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //清空画布并以透明背景色填充 
        g.Clear(Color.Transparent);
        //在指定位置并且按指定大小绘制原图片的指定部分 
        g.DrawImage(_originalImage, new Rectangle(0, 0, _towidth, _toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
        try
        {
            return _bitmap;
        }
        catch (System.Exception e)
        {
            throw e;
        }
        finally
        {
            _originalImage.Dispose();
            g.Dispose();
        }
    }

    public static Bitmap DitteringImage(IErrorDiffusion diffusion, Bitmap bitmap)
    {
        List<ArgbColor> colorList = new List<ArgbColor>();
        int height = bitmap.Height;
        int width = bitmap.Width;

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                var pixel = bitmap.GetPixel(w, h);
                colorList.Add(new ArgbColor(pixel.A, pixel.R, pixel.G, pixel.B));
            }
        }

        ArgbColor[] colorArray = colorList.ToArray();

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                var argb = colorArray[h * width + w];
                var bw = ToBWPixel(argb);
                colorArray[h * width + w] = bw;

                diffusion?.Diffuse(colorArray, argb, bw, w, h, width, height);
            }
        }

        Bitmap newbm = new Bitmap(bitmap);
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                var argb = colorArray[h * width + w];
                Color pixel = Color.FromArgb(argb.A, argb.R, argb.G, argb.B);
                newbm.SetPixel(w, h, pixel);
            }
        }

        return newbm;
    }

    public static ArgbColor ToBWPixel(ArgbColor origin)
    {
        uint gray;

        gray = (uint)(0.299 * origin.R + 0.587 * origin.G + 0.114 * origin.B);

        /*
         * I'm leaving the alpha channel untouched instead of making it fully opaque
         * otherwise the transparent areas become fully black, and I was getting annoyed
         * by this when testing images with large swathes of transparency!
         */

        return gray < GrayThreshold ? new ArgbColor(origin.A, 0, 0, 0) : new ArgbColor(origin.A, 255, 255, 255);
    }

    static uint grayThreshold = 128;

    public static uint GrayThreshold { get => grayThreshold; set => grayThreshold = value; }
}