using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QFinans.CustomFilters
{
    public class CustomAuth : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                // yetkili olduğu sayfada ise herhangi bir değişiklik yapmıyoruz.
                // yetkili olduğu sayfaya direkt girebilir sonuçta,başka bir sayfaya yönlendirmeyeceğiz.
                base.OnAuthorization(filterContext);
            }
            else
            {
                // orada yetkili değilse ya yetkili olduğu sayfaya geri gönderiyoruz
                // yada yetkisiz olduğuna dair error page i istemciye gönderiyoruz.
                var _urlReferrer = filterContext.HttpContext.Request.UrlReferrer;

                if (_urlReferrer != null)
                {
                    //string _redirectUrl = "~" + _urlReferrer.LocalPath;
                    //filterContext.Result = new RedirectResult(_redirectUrl);
                    filterContext.Result = new RedirectResult("~/CustomAuth/UnAuthorized");
                }
                else
                {
                    // direkt url den talebi göndermiş demektir.
                    filterContext.Result = new RedirectResult("~/CustomAuth/UnAuthorized");
                }
            }
        }
    }
}