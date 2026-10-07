using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Results
{
    public class ResultT<T> : Result
    {
        public T? value { get; }

        public ResultT(bool isSuccess,T? Value, string? error = null) : base(isSuccess, error)
        {
            value = Value;
        }
        public static ResultT<T> Success(T value)
        {
            return new ResultT<T>(true,value);
        } 
        public static ResultT<T> Failure(string error)
        {
            return new ResultT<T>(false,default,error);
        }

    }
}
