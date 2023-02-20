
```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using XCloud.Core.Extension;
using XCloud.Core.Helper;

namespace XCloud.Common.Images
{
    /// <summary>
    ///用于生成多种样式的验证码
    /// </summary>
    public class DrawVerifyCode
    {
        class VerificationCodeHelper
        {
            public static readonly VerificationCodeHelper Instance = new VerificationCodeHelper();

            private readonly List<char> chars;

            public VerificationCodeHelper()
            {
                var chars = new List<char>();
                Action<char, char> GenerateCharCollection = (start, end) =>
                {
                    for (var i = start; i <= end; ++i)
                    {
                        chars.Add((char)i);
                    }
                };
                GenerateCharCollection('a', 'z');
                GenerateCharCollection('A', 'Z');
                GenerateCharCollection('0', '9');

                chars = chars.Except(new char[] { '0', 'o', 'O', '1', 'l', '9', 'q', 'z', 'Z', '2' }).ToList();

                this.chars = chars;
            }

            public List<Color> Colors => new List<Color>() { Color.Purple, Color.Black, Color.DarkBlue, Color.DarkRed };

            public List<string> Fonts => new List<string>() { "Times New Roman", "MS Mincho", "Gungsuh", "PMingLiU", "Impact" };

            public List<char> Chars => this.chars;
        }

        class CharItem
        {
            public char c { get; set; }
            public Font font { get; set; }
        }

        #region 各种参数
        private readonly Random random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// 噪线的条数，默认5条
        /// </summary>
        public int LineCount { get; set; } = 5;

        /// <summary>
        /// 生成的code
        /// </summary>
        public string Code { get; private set; } = string.Empty;

        /// <summary>
        /// 字体大小，【默认15px】为了使字符居中显示，请设置一个合适的值
        /// </summary>
        public int FontSize { get; set; } = 18;

        /// <summary>
        /// 验证码字符数
        /// </summary>
        public int CharCount { get; set; } = 4;

        private readonly List<Color> colors = VerificationCodeHelper.Instance.Colors;
        private readonly List<string> fonts = VerificationCodeHelper.Instance.Fonts;
        private readonly List<char> chars = VerificationCodeHelper.Instance.Chars;

        /// <summary>
        /// 构造函数，设置response对象
        /// </summary>
        public DrawVerifyCode() { }

        #endregion

        #region 主体程序
        /// <summary>
        /// 获取图片bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetImageBytes()
        {
            return GetImageBytesAndSize().bs;
        }
        /// <summary>
        /// 获取图片bytes和宽高
        /// </summary>
        /// <returns></returns>
        public (byte[] bs, int width, int height) GetImageBytesAndSize()
        {
            if (CharCount <= 0) { throw new Exception("字符数必须大于0"); }
            this.Code = string.Empty;

            var items = Com.Range(CharCount).Select(_ => new CharItem()
            {
                c = random.Choice(chars),
                font = new Font(random.Choice(fonts), FontSize)
            }).ToList();

            //把验证码保存起来
            this.Code = new string(items.Select(x => x.c).ToArray());
            int Height = (int)(items.Select(x => x.font).Max(x => x.Height) * 1.3);
            int Width = (int)(Height * 0.8 * CharCount);
            //获取随机字体，颜色
            using (var bm = new Bitmap(Width, Height))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.Clear(Color.White);
                    using (var ms = new MemoryStream())
                    {
                        //判断是否画噪线
                        if (LineCount > 0)
                        {
                            for (int k = 0; k < LineCount; ++k)
                            {
                                var x1 = random.Next(bm.Width);
                                var y1 = random.Next(bm.Height);
                                var x2 = random.Next(bm.Width);
                                var y2 = random.Next(bm.Height);
                                g.DrawLine(new Pen(random.Choice(colors)), x1, y1, x2, y2);
                            }
                        }
                        //画验证码
                        var i = 0;
                        foreach (var itm in items)
                        {
                            //计算位置
                            var (x, y) = ComputePosition(i++, itm.font, bm);

                            var angle = random.Next(-5, 5);
                            g.RotateTransform(angle);

                            g.DrawString(itm.c.ToString(), itm.font, new SolidBrush(random.Choice(colors)), x, y);

                            g.RotateTransform(-angle);
                        }

                        bm.Save(ms, ImageFormat.Png);
                        return (ms.ToArray(), Width, Height);
                    }
                }
            }
        }

        #endregion

        #region 方法
        private (float x, float y) ComputePosition(int i, Font font, Bitmap bm)
        {
            var font_h = font.Height;
            var font_w = font.Size;
            var box_w = bm.Width / CharCount;

            var x = box_w * i + (box_w - font_w) / 2;

            var y = (bm.Height - font_h) / 2;

            return (x, y);
        }
        #endregion
    }
}


```