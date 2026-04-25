 namespace System.Runtime.CompilerServices
 {
     // this type is needed by the compiler to implement interceptors - it doesn't need to
     // come from the runtime itself, though

     [global::System.Diagnostics.Conditional("DEBUG")] // not needed post-build, so: evaporate
     [global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
     sealed file class InterceptsLocationAttribute : global::System.Attribute
     {
         public InterceptsLocationAttribute(int version, string data)
         {
             _ = version;
             _ = data;
         }
     }
 }namespace MinimalInterceptorSourceGenerator{ static file class MyInterceptors{ [global::System.Runtime.CompilerServices.InterceptsLocation(1,"cNKg05kQEjX50VlFyl9i3MIAAABQcm9ncmFtLmNz")] public static void InterceptorMethod(this MyClass mc, global:: System.String s){Console.WriteLine("intercepted!");} } }