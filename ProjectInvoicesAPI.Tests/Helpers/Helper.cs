using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaklaNewAPI.Tests.Helpers
{
    /// <summary>
    /// This class provides helper methods for unit tests
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Get ObjectResult contained value
        /// </summary>
        public static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }
    }
}
