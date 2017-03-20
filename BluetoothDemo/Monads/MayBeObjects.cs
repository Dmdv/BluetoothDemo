using JetBrains.Annotations;
using System;
using System.Linq;

namespace BluetoothDemo.Monads
{
    public static class MayBeObjects
    {
        /// <summary>
        /// Handle exception with no actions
        /// </summary>
        /// <typeparam name="TResult">Type of source object</typeparam>
        /// <param name="result">Source object for operating</param>
        /// <returns><paramref name="result"/> object</returns>
        public static TResult Catch<TResult>(this Tuple<TResult, Exception> result)
        {
            return result.Item1;
        }

        /// <summary>
        /// Handle exception with <param name="handler"/> action
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="handler"></param>
        /// <returns><paramref name="source"/> object</returns>
        public static TSource Catch<TSource>(
            this Tuple<TSource, Exception> source,
            Action<Exception> handler)
        {
            if (source.Item2 != null)
            {
                handler(source.Item2);
            }

            return source.Item1;
        }

        /// <summary>
        /// Allows to do some <paramref name="action"/> on <paramref name="source"/> if its not null
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <returns><paramref name="source"/> object</returns>
        public static TSource Do<TSource>(
            this TSource source,
            Action<TSource> action)
            where TSource : class
        {
            if (source != default(TSource))
            {
                action(source);
            }

            return source;
        }

        /// <summary>
        /// Allows to do some <paramref name="action"/> on <paramref name="source"/> if its not null
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <returns><paramref name="source"/> object</returns>
        public static TResult DoReturn<TSource, TResult>(
            this TSource source,
            Func<TSource, TResult> action)
            where TSource : class
        {
            return source == default(TSource) ? default(TResult) : action(source);
        }

        public static TResult IfNotNull<TArg, TResult>(
            [CanBeNull] this TArg value,
            [NotNull] Func<TArg, TResult> action,
            [CanBeNull] TResult defaultResult = default(TResult))
            where TArg : class
        {
            return value == default(TArg) ? defaultResult : action(value);
        }

        public static void IfNotNull<TArg>(
            [CanBeNull] this TArg value,
            [NotNull] Action<TArg> action)
            where TArg : class
        {
            if (value != default(TArg))
            {
                action(value);
            }
        }

        /// <summary>
        /// Allows to do <paramref name="action"/> and catch any exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <returns>
        /// Tuple which contains <paramref name="source"/> and info about exception (if it throws)
        /// </returns>
        public static Tuple<TSource, Exception> TryDo<TSource>(
            this TSource source,
            Action<TSource> action)
            where TSource : class
        {
            if (source == default(TSource))
            {
                return new Tuple<TSource, Exception>(null, null);
            }

            try
            {
                action(source);
            }
            catch (Exception ex)
            {
                return new Tuple<TSource, Exception>(source, ex);
            }

            return new Tuple<TSource, Exception>(source, null);
        }

        /// <summary>
        /// Allows to do <paramref name="action"/> and catch exceptions, which handled by <param name="exceptionChecker"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="exceptionChecker">Predicate to determine if exceptions should be handled</param>
        /// <returns>
        /// Tuple which contains <paramref name="source"/> and info about exception (if it throws)
        /// </returns>
        public static Tuple<TSource, Exception> TryDo<TSource>(
            this TSource source,
            Action<TSource> action,
            Func<Exception, bool> exceptionChecker)
            where TSource : class
        {
            if (source == default(TSource))
            {
                return new Tuple<TSource, Exception>(null, null);
            }

            try
            {
                action(source);
            }
            catch (Exception ex)
            {
                if (exceptionChecker(ex))
                {
                    return new Tuple<TSource, Exception>(source, ex);
                }

                throw;
            }

            return new Tuple<TSource, Exception>(source, null);
        }

        /// <summary>
        /// Allows to do <paramref name="action"/> and catch <param name="expectedException"/> exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="expectedException">Array of exception types, which should be handled</param>
        /// <returns>
        /// Tuple which contains <paramref name="source"/> and info about exception (if it throws)
        /// </returns>
        public static Tuple<TSource, Exception> TryDo<TSource>(
            this TSource source,
            Action<TSource> action,
            params Type[] expectedException)
            where TSource : class
        {
            if (source != default(TSource))
            {
                try
                {
                    action(source);
                }
                catch (Exception ex)
                {
                    if (expectedException.Any(exp => exp.IsInstanceOfType(ex)))
                    {
                        return new Tuple<TSource, Exception>(source, ex);
                    }

                    throw;
                }
            }

            return new Tuple<TSource, Exception>(source, null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch any exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TSource, Exception> TryLet<TSource>(
            this TSource source,
            Func<TSource, TSource> action)
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = source;
                try
                {
                    result = action(source);
                    return new Tuple<TSource, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    return new Tuple<TSource, Exception>(result, ex);
                }
            }

            return new Tuple<TSource, Exception>(default(TSource), null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch exceptions, which handled by <param name="exceptionChecker"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="exceptionChecker">Predicate to determine if exceptions should be handled</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TSource, Exception> TryLet<TSource>(
            this TSource source,
            Func<TSource, TSource> action,
            Func<Exception, bool> exceptionChecker)
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = source;
                try
                {
                    result = action(source);
                    return new Tuple<TSource, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    if (exceptionChecker(ex))
                    {
                        return new Tuple<TSource, Exception>(result, ex);
                    }

                    throw;
                }
            }

            return new Tuple<TSource, Exception>(default(TSource), null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch <param name="expectedException"/> exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="expectedException">Array of exception types, which should be handled</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TSource, Exception> TryLet<TSource>(
            this TSource source,
            Func<TSource, TSource> action,
            params Type[] expectedException)
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = source;
                try
                {
                    result = action(source);
                    return new Tuple<TSource, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    if (expectedException.Any(exp => exp.IsInstanceOfType(ex)))
                    {
                        return new Tuple<TSource, Exception>(result, ex);
                    }

                    throw;
                }
            }

            return new Tuple<TSource, Exception>(default(TSource), null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch any exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="defaultResult">Default result</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TResult, Exception> TryLet<TSource, TResult>(
            this TSource source,
            Func<TSource, TResult> action,
            TResult defaultResult = default(TResult))
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = defaultResult;
                try
                {
                    result = action(source);
                    return new Tuple<TResult, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    return new Tuple<TResult, Exception>(result, ex);
                }
            }

            return new Tuple<TResult, Exception>(default(TResult), null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch exceptions, which handled by <param name="exceptionChecker"/>
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="exceptionChecker">Predicate to determine if exceptions should be handled</param>
        /// <param name="defaultResult">Default result</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TResult, Exception> TryLet<TSource, TResult>(
            this TSource source,
            Func<TSource, TResult> action,
            Func<Exception, bool> exceptionChecker,
            TResult defaultResult = default(TResult))
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = defaultResult;
                try
                {
                    result = action(source);
                    return new Tuple<TResult, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    if (exceptionChecker(ex))
                    {
                        return new Tuple<TResult, Exception>(result, ex);
                    }

                    throw;
                }
            }

            return new Tuple<TResult, Exception>(default(TResult), null);
        }

        /// <summary>
        /// Allows to do some conversion of <paramref name="source"/> if its not null and catch <param name="expectedException"/> exceptions
        /// </summary>
        /// <typeparam name="TSource">Type of source object</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="source">Source object for operating</param>
        /// <param name="action">Action which should to do</param>
        /// <param name="defaultResult">Default result</param>
        /// <param name="expectedException">Array of exception types, which should be handled</param>
        /// <returns>Tuple which contains Converted object and info about exception (if it throws)</returns>
        public static Tuple<TResult, Exception> TryLet<TSource, TResult>(
            this TSource source,
            Func<TSource, TResult> action,
            TResult defaultResult = default(TResult),
            params Type[] expectedException)
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = defaultResult;
                try
                {
                    result = action(source);
                    return new Tuple<TResult, Exception>(result, null);
                }
                catch (Exception ex)
                {
                    if (expectedException.Any(exp => exp.IsInstanceOfType(ex)))
                    {
                        return new Tuple<TResult, Exception>(result, ex);
                    }

                    throw;
                }
            }

            return new Tuple<TResult, Exception>(default(TResult), null);
        }

        [Pure]
        public static TResult With<TResult, TSource>(
            [CanBeNull] this TSource source,
            [NotNull] Func<TSource, TResult> evaluator,
            [NotNull] TResult defaultResult = default(TResult))
            where TSource : class
        {
            if (source != default(TSource))
            {
                var result = evaluator(source);
                return result.Equals(default(TResult)) ? defaultResult : result;
            }

            return defaultResult;
        }

        [PublicAPI]
        public static TResult WithString<TResult>(this string input, Func<string, TResult> evaluator)
            where TResult : class
        {
            return string.IsNullOrWhiteSpace(input) ? default(TResult) : evaluator(input);
        }

        [PublicAPI]
        public static bool ReturnSuccess(this object source)
        {
            return source != null;
        }

        public static TI If<TI>(this TI input, Func<TI, bool> evaluator) where TI : class
        {
            if (input == null)
            {
                return default(TI);
            }

            return !evaluator(input) ? default(TI) : input;
        }
    }
}
