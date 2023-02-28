using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Services
{
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected const string Key = nameof(ModelStateTransfer);
    }

    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //only export when ModelState is not valid
            if (!filterContext.ModelState.IsValid)
            {
                //export if we are redirecting
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    Controller? controller = filterContext.Controller as Controller;

                    if (controller != null && filterContext.ModelState != null)
                    {
                        var modelState = ModelStateHelpers.SerialiseModelState(filterContext.ModelState);

                        controller.TempData[Key] = modelState;
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }

    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;

            string? serialisedModelState = controller?.TempData[Key] as string;

            if (serialisedModelState != null)
            {
                //only import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    var modelState = ModelStateHelpers.DeserialiseModelState(serialisedModelState);

                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    //otherwise remove it.
                    controller!.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}