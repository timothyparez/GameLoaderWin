using PostSharp.Aspects;
using System;
using System.Reflection;
namespace TeamArg.GameLoader
{
    [Serializable]
    public class RaisePropertyChangedAttribute : OnMethodBoundaryAspect
    {
        private string propertyName;

        public override void OnExit(MethodExecutionArgs args)
        {
            dynamic instance = args.Instance;
            instance.RaisePropertyChanged(propertyName);
        }

        public override bool CompileTimeValidate(MethodBase method)
        {
            if (IsPropertySetter(method))
            {
                propertyName = GetPropertyName(method);
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetPropertyName(MethodBase method)
        {
            return method.Name.Replace("set_", "");
        }

        private bool IsPropertySetter(MethodBase method)
        {
            return method.Name.StartsWith("set_");
        }
    }
}
