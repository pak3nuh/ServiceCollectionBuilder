using System;

namespace NCaro.ServiceCollectionBuilder.Util
{
    public static class Check
    {
        private delegate Exception ThrowException();
        
        public static void Argument(bool truth)
        {
            Validate(truth,() => new ArgumentException());
        }

        public static void Argument(bool truth, string message, params object[] format)
        {
            Validate(truth, () => new ArgumentException(message.Format(format)));
        }

        public static void State(bool truth, string message, params object[] format)
        {
            Validate(truth, delegate { return new InvalidOperationException(message.Format(format));});//just because
        }

        private static void Validate(bool truth, ThrowException exception)
        {
            if (!truth)
            {
                throw exception();
            }
        }

        private static string Format(this string _this, params object[] arguments)
        {
            return string.Format(_this, arguments);
        }
    }
}