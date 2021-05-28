using Microsoft.AspNet.Identity;
using QFinans.Areas.Api.Models;
using QFinans.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace QFinans.CustomFilters
{
    public class LogAttribute : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Stopwatch _stopwatch;

        public LogAttribute()
        {
            _stopwatch = new Stopwatch();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _stopwatch.Stop();  //kronometreyi durdur

            //Log classının alanlarını doldur
            var request = filterContext.HttpContext.Request;    //Gelen requesti al
            var userId = HttpContext.Current.User.Identity.GetUserId();     //login olan kullanıcının alınacağı method; cookie'den, session'dan alınabilir  

            Log l = new Log();
            l.AddDate = DateTime.Now;
            l.Data = SerializeRequest(request); //yukarıda alınan requesti serialize edip, string olarak yazıyorum. 
            l.ExecutionMs = _stopwatch.ElapsedMilliseconds; //çalışma süresi
            l.IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            l.UrlAccessed = request.RawUrl; //erişilen sayfanın ham url'i
            l.UserId = userId;
            db.Log.Add(l);
            db.SaveChanges();
            //db.Dispose();
            //Log classını veritabanına kaydet

            base.OnActionExecuted(filterContext);   //işlem devam etsin
        }

        private string SerializeRequest(HttpRequestBase request)
        {
            string result = null;
            #region Form
            List<string> formVals = new List<string>(); //eğer sayfada bir form varsa, formda gönderilen tüm inputları alıp bir listeye atıyorum.
            if (request.Form.AllKeys != null && request.Form.AllKeys.ToList().Count > 0)
            {
                foreach (string s in request.Form.AllKeys.ToList())
                {
                    formVals.Add(request.Unvalidated().Form[s]);
                }
            }
            #endregion

            result = Json.Encode(new
            {
                request.Form,    //gönderilen formun tamamı
                formVals,   //formdaki inputlara girilen veriler
                request.Browser.Browser,    //kullanıcının tarayıcısı
                request.Browser.IsMobileDevice,     //istek bir mobil cihazdan mı geldi
                request.Browser.Version,    //kullanıcının tarayıcı versiyonu
                request.UserAgent,      //yönlendiren kuruluşu bulmamızı sağlayan useragent bilgisi
                request.UserLanguages,  //tarayıcı dili
                request.UrlReferrer     //yönlendiren sayfayı bulmamızı sağlayan urlreferrer bilgisi
            });     //tüm bu bilgileri serialize edip stringe çeviriyorum

            return result;
        }
    }
}