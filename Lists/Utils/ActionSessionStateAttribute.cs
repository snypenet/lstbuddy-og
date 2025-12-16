using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;

namespace Lists.Utils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ActionSessionStateAttribute : Attribute
    {
        public SessionStateBehavior Behavior { get; private set; }
        public ActionSessionStateAttribute(SessionStateBehavior behavior)
        {
            this.Behavior = behavior;
        }
    }
}
