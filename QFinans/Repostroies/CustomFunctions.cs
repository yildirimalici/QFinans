using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace QFinans.Repostroies
{
    public class CustomFunctions
    {
        public string GenerateQR(string txt)
        {
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(txt, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);

            System.Web.UI.WebControls.Image imgQRCode = new System.Web.UI.WebControls.Image();
            imgQRCode.Height = 150;
            imgQRCode.Width = 150;

            using (Bitmap bitmap = code.GetGraphic(5))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    imgQRCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }

            return imgQRCode.ImageUrl;
        }
    }
}