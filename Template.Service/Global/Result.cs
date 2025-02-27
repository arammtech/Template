using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Service.Global
{
    public class Result
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public Result(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
