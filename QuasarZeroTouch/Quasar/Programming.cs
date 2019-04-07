using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Quasar
{
    /// <summary>
    /// Class for programming related nodes.
    /// </summary>
    public class Programming
    {
        /// <summary>
        /// private constructor
        /// </summary>
        private Programming() { }

        /// <summary>
        /// Execute python script from string. 
        /// Returns value must assign in variable name "Output".
        /// (Eg. Output = YourReturnValues)
        /// </summary>
        /// <param name="PythonString">python script as a string.</param>
        /// <returns name="Output">return objects</returns>
        public static object ExecutePythonString(string PythonString)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            engine.Execute(PythonString, scope);
            var Output = scope.GetVariable("Output");
            return (Output);
        }
    }
}
