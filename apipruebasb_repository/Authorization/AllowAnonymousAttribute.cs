using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apipruebasb_repository.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    { }
}