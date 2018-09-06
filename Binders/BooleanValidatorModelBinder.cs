using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.ModelBinding;


namespace JrtFS.Binders
{
    /// <summary>
    /// 可以使model参数中的bool值属性能接收1、0这样的数值
    /// </summary>
    public class BooleanValidatorModelBinder : System.Web.Http.ModelBinding.IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(bool) && bindingContext.ModelType != typeof(bool?))
            {
                return false;
            }

            var input = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (input != null && !string.IsNullOrEmpty(input.AttemptedValue))
            {
                bool result;
                if (input.AttemptedValue == "1" || input.AttemptedValue == "true")
                {
                    bindingContext.Model = true;
                    return true;
                }
                if (!bool.TryParse(input.AttemptedValue, out result))
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "是无效的bool值");
                    return false;
                }
            }

            return true;
        }
    }

}