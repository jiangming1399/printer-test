using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public static class ImageHelper
{
    /// <summary> 
    /// ��������ͼ 
    /// </summary>
    /// <param name="width">����ͼ���</param> 
    /// <param name="height">����ͼ�߶�</param> 
    /// <param name="mode">��������ͼ�ķ�ʽ:HWָ���߿�����(���ܱ���);Wָ�����߰����� Hָ���ߣ������� Cutָ���߿�ü�(������)</param>���� 
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
            case "HW"://ָ���߿����ţ����ܱ��Σ�
                break;
            case "W"://ָ�����߰������������������������� 
                _toheight = _originalImage.Height * width / _originalImage.Width;
                break;
            case "H"://ָ���ߣ������� 
                _towidth = _originalImage.Width * height / _originalImage.Height;
                break;
            case "Cut"://ָ���߿�ü��������Σ����������������� 
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
            case "MaxHW":// �������а�����������
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
        //�½�һ��bmpͼƬ 
        Bitmap _bitmap = new Bitmap(_towidth, _toheight);
        //�½�һ������ 
        Graphics g = Graphics.FromImage(_bitmap);
        //���ø�������ֵ�� 
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
        //���ø�����,���ٶȳ���ƽ���̶� 
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //��ջ�������͸������ɫ��� 
        g.Clear(Color.Transparent);
        //��ָ��λ�ò��Ұ�ָ����С����ԭͼƬ��ָ������ 
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
}