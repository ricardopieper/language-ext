﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LanguageExt
{
    public static partial class Prelude
    {
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, R>(Tuple<T1, T2> self, Func<T1, T2, R> func) =>
            func(self.Item1, self.Item2);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, R>(Tuple<T1, T2, T3> self, Func<T1, T2, T3, R> func) =>
            func(self.Item1, self.Item2, self.Item3);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, R>(Tuple<T1, T2, T3, T4> self, Func<T1, T2, T3, T4, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, R>(Tuple<T1, T2, T3, T4, T5> self, Func<T1, T2, T3, T4, T5, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, T6, R>(Tuple<T1, T2, T3, T4, T5, T6> self, Func<T1, T2, T3, T4, T5, T6, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, T4, T5, T6, T7, R>(Tuple<T1, T2, T3, T4, T5, T6, T7> self, Func<T1, T2, T3, T4, T5, T6, T7, R> func) =>
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2>(Tuple<T1, T2> self, Action<T1, T2> func)
        {
            func(self.Item1, self.Item2);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3>(Tuple<T1, T2, T3> self, Action<T1, T2, T3> func)
        {
            func(self.Item1, self.Item2, self.Item3);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4>(Tuple<T1, T2, T3, T4> self, Action<T1, T2, T3, T4> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5>(Tuple<T1, T2, T3, T4, T5> self, Action<T1, T2, T3, T4, T5> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5, T6>(Tuple<T1, T2, T3, T4, T5, T6> self, Action<T1, T2, T3, T4, T5, T6> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6);
            return Unit.Default;
        }

        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Unit with<T1, T2, T3, T4, T5, T6, T7>(Tuple<T1, T2, T3, T4, T5, T6, T7> self, Action<T1, T2, T3, T4, T5, T6, T7> func)
        {
            func(self.Item1, self.Item2, self.Item3, self.Item4, self.Item5, self.Item6, self.Item7);
            return Unit.Default;
        }

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2> tuple<T1, T2>(T1 item1, T2 item2) =>
            System.Tuple.Create(item1, item2);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3> tuple<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            System.Tuple.Create(item1, item2, item3);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4> tuple<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            System.Tuple.Create(item1, item2, item3, item4);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5> tuple<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            System.Tuple.Create(item1, item2, item3, item4, item5);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5, T6> tuple<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) =>
            System.Tuple.Create(item1, item2, item3, item4, item5, item6);

        [Obsolete("Use 'Tuple'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> tuple<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) =>
            System.Tuple.Create(item1, item2, item3, item4, item5, item6, item7);

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone|ifNoneOrFail' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failure<T>(TryOption<T> tryDel, Func<T> Fail) =>
            tryDel.Failure(Fail);

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone|ifNoneOrFail' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failure<T>(TryOption<T> tryDel, T failValue) =>
            tryDel.Failure(failValue);

        [Obsolete("'failureUnsafe' has been deprecated.  Please use 'ifNoneUnsafe' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failureUnsafe<T>(OptionUnsafe<T> option, Func<T> None) =>
            option.FailureUnsafe(None);

        [Obsolete("'failureUnsafe' has been deprecated.  Please use 'ifNoneUnsafe' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failureUnsafe<T>(OptionUnsafe<T> option, T noneValue) =>
            option.FailureUnsafe(noneValue);

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failure<T>(Option<T> option, Func<T> None) =>
            option.Failure(None);

        [Obsolete("'failure' has been deprecated.  Please use 'ifNone' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T failure<T>(Option<T> option, T noneValue) =>
            option.Failure(noneValue);

        [Obsolete("'failureUnsafe' has been deprecated.  Please use 'ifLeftUnsafe' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R failureUnsafe<L, R>(EitherUnsafe<L, R> either, Func<R> None) =>
            either.FailureUnsafe(None);

        [Obsolete("'failureUnsafe' has been deprecated.  Please use 'ifLeftUnsafe' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R failureUnsafe<L, R>(EitherUnsafe<L, R> either, R noneValue) =>
            either.FailureUnsafe(noneValue);

        [Obsolete("'failure' has been deprecated.  Please use 'ifLeft' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R failure<L, R>(Either<L, R> either, Func<R> None) =>
            either.Failure(None);

        [Obsolete("'failure' has been deprecated.  Please use 'ifLeft' instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R failure<L, R>(Either<L, R> either, R noneValue) =>
            either.Failure(noneValue);

        /// <summary>
        /// Create a queryable
        /// </summary>
        [Obsolete("Use 'Query'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IQueryable<T> query<T>(params T[] items) =>
            toQuery(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>() =>
            LanguageExt.Map.empty<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>(params Tuple<K, V>[] items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Obsolete("Use 'Map'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Map<K, V> map<K, V>(params KeyValuePair<K, V>[] items) =>
            LanguageExt.Map.createRange(from x in items
                                        select Tuple(x.Key, x.Value));

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Obsolete("Use 'List'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> list<T>() =>
            new Lst<T>();

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Obsolete("Use 'List'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> list<T>(params T[] items) =>
            new Lst<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ImmutableArray<T> array<T>() =>
            ImmutableArray.Create<T>();

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ImmutableArray<T> array<T>(T item) =>
            ImmutableArray.Create<T>(item);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Obsolete("Use 'Array'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ImmutableArray<T> array<T>(params T[] items) =>
            ImmutableArray.Create<T>(items);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>() =>
            new Set<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>(T item) =>
            new Set<T>().Add(item);

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Obsolete("Use 'Set'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Set<T> set<T>(params T[] items) =>
            new Set<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Obsolete("Use 'Stack'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IImmutableStack<T> stack<T>() =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Obsolete("Use 'Stack'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IImmutableStack<T> stack<T>(T item) =>
            ImmutableStack.Create<T>();

        /// <summary>
        /// Create an empty IEnumerable T
        /// </summary>
        [Obsolete("Use List.empty")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> empty<T>() =>
            new Lst<T>();

        /// <summary>
        /// Construct a list from head and tail
        /// head becomes the first item in the list
        /// Is lazy
        /// </summary>
        [Obsolete("Use 'Cons'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var item in tail)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Construct a list from head and tail
        /// </summary>
        [Obsolete("Use 'Cons'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Lst<T> cons<T>(this T head, Lst<T> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Lazily generate a range of chars.  
        /// 
        ///   Remarks:
        ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
        /// </summary>
        [Obsolete("Use 'Range'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static CharRange range(char from, char to) =>
            new CharRange(from, to);

        /// <summary>
        /// Lazily generate integers from any number of provided ranges
        /// </summary>
        [Obsolete("Use 'Range'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<int> range(params IntegerRange[] ranges) =>
            from range in ranges
            from i in range
            select i;

        /// <summary>
        /// Lazily generate chars from any number of provided ranges
        /// </summary>
        [Obsolete("Use 'Range'.  All constructor functions are renamed to have their first letter as a capital.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<char> range(params CharRange[] ranges) =>
            from range in ranges
            from c in range
            select c;

        /// <summary>
        /// Projects a value into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T, R>(T value, Func<T, R> project) =>
            project(value);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, R>(T1 value1, T2 value2, Func<T1, T2, R> project) =>
            project(value1, value2);

        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your
        /// expression.  This allows you to keep the expression going.
        /// </summary>
        [Obsolete("'with' has been renamed to 'map', please use that instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static R with<T1, T2, T3, R>(T1 value1, T2 value2, T3 value3, Func<T1, T2, T3, R> project) =>
            project(value1, value2, value3);
    }
}
