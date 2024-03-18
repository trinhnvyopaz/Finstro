using System;
namespace Finstro.Serverless.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class FinstroCustomAttributes : Attribute
    {
        public FinstroCustomAttributes()
        {
        }
    }

    
    public class FisntroServiceCallAttribute : Attribute
    {
        // This is a positional argument
        public FisntroServiceCallAttribute()
        {
      
        }
    }
}
