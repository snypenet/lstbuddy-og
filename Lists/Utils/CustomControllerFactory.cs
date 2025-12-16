using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Lists.Utils
{
    public class CustomControllerFactory : DefaultControllerFactory
    {
        protected override SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return SessionStateBehavior.Default;
            }

            var actionName = requestContext.RouteData.Values["action"].ToString();
            MethodInfo actionMethodInfo;
            actionMethodInfo = controllerType.GetMethod(actionName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (actionMethodInfo != null)
            {
                var actionSessionStateAttr = actionMethodInfo.GetCustomAttributes(typeof(ActionSessionStateAttribute), false)
                                    .OfType<ActionSessionStateAttribute>()
                                    .FirstOrDefault();

                if (actionSessionStateAttr != null)
                {
                    return actionSessionStateAttr.Behavior;
                }
            }
            return base.GetControllerSessionBehavior(requestContext, controllerType);
        }
    }
}