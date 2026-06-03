using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ECommerce.Common
{
    public class GeneralResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<Errors>>? Errors { get; set; }

        public static GeneralResult SuccessResult(string message = "Success")
            => new()
            {
                Success = true,
                Message = message
            };

        public static GeneralResult NotFound(string message = "Resource not found")
            => new()
            {
                Success = false,
                Message = message
            };

        public static GeneralResult Fail(
            Dictionary<string, List<Errors>> errors,
            string message = "One or more validation errors occured.")
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors
            };

        public static GeneralResult Fail(string message = "Operation Failed")
            => new()
            {
                Success = false,
                Message = message,
                Errors = null
            };
    }

    public class GeneralResult<T> : GeneralResult
    {
        // FIX: تغيير WhenWritingNull إلى WhenWritingDefault لحل مشكلة Value Types
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }

        public static GeneralResult<T> SuccessResult(T data, string message = "Success")
            => new()
            {
                Success = true,
                Message = message,
                Data = data
            };

        public new static GeneralResult<T> NotFound(string message = "Resource not found")
            => new()
            {
                Success = false,
                Message = message
            };

        public new static GeneralResult<T> Fail(
            Dictionary<string, List<Errors>> errors,
            string message = "One or more validation errors occured.")
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors
            };

        public new static GeneralResult<T> Fail(string message = "Operation Failed")
            => new()
            {
                Success = false,
                Message = message,
                Errors = null
            };
    }
}