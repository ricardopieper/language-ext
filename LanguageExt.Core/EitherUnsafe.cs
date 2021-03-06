﻿using System;
using System.Collections;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Collections.Immutable;
using System.ComponentModel;

namespace LanguageExt
{
    /// <summary>
    /// EitherUnsafe L R - This is 'unsafe' because L or R can be null.
    /// 
    /// Holds one of two values 'Left' or 'Right'.  Usually 'Left' is considered 'wrong' or 'in error', and
    /// 'Right' is, well, right.  So when the Either is in a Left state, it cancels computations like bind
    /// or map, etc.  So you can see Left as an 'early out, with a message'.  Unlike Option that has None
    /// as its alternative value (i.e. it has an 'early out, but no message').
    /// 
    /// NOTE: If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    public struct EitherUnsafe<L, R> :
        IEither,
        IComparable<EitherUnsafe<L, R>>,
        IComparable<R>,
        IEquatable<EitherUnsafe<L, R>>,
        IEquatable<R>
    {
        readonly R right;
        readonly L left;

        private EitherUnsafe(R right)
        {
            State = EitherState.IsRight;
            left = default(L);
            this.right = right;
        }

        private EitherUnsafe(L left)
        {
            State = EitherState.IsLeft;
            right = default(R);
            this.left = left;
        }

        internal EitherUnsafe(bool bottom)
        {
            State = EitherState.IsBottom;
            right = default(R);
            left = default(L);
        }

        /// <summary>
        /// State of the Either
        /// You can also use:
        ///     IsRight
        ///     IsLeft
        ///     IsBottom
        ///     IsUninitialised
        /// </summary>
        public readonly EitherState State;

        /// <summary>
        /// Is the Either in a Right state?
        /// </summary>
        public bool IsRight =>
            CheckInitialised(State == EitherState.IsRight);

        /// <summary>
        /// Is the Either in a Left state?
        /// </summary>
        public bool IsLeft =>
            CheckInitialised(State == EitherState.IsLeft);

        /// <summary>
        /// Is the Either in a Bottom state?
        /// When the Either is filtered, both Right and Left are meaningless.
        /// 
        /// If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
        /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
        /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
        /// of filtering Either.
        /// 
        /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
        /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
        /// doesn't needlessly break expressions. 
        /// </summary>
        public bool IsBottom =>
            State == EitherState.IsBottom;

        /// <summary>
        /// Is the Either in the uninitialised state?
        /// 
        /// This only occurs because EitherUnsafe is a struct, and you instantiate
        /// using its default ctor.  This can occur if you have an uninitialised 
        /// member variable or an uninitialised array, or just do:
        /// 
        ///      new Either();
        ///      new EitherUnsafe();
        ///
        /// So Either will put itself into an uninitialised state, that will fail
        /// quickly.
        /// </summary>
        public bool IsUninitialised =>
            State == EitherState.IsUninitialised;

        /// <summary>
        /// Implicit conversion operator from R to Either R L
        /// </summary>
        /// <param name="value">Value</param>
        public static implicit operator EitherUnsafe<L, R>(R value) =>
            Right(value);

        /// <summary>
        /// Implicit conversion operator from L to Either R L
        /// </summary>
        /// <param name="value">Value</param>
        public static implicit operator EitherUnsafe<L, R>(L value) =>
            Left(value);

        /// <summary>
        /// Invokes the Right or Left function depending on the state of the Either
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Right">Function to invoke if in a Right state</param>
        /// <param name="Left">Function to invoke if in a Left state</param>
        /// <returns>The return value of the invoked function</returns>
        public Ret MatchUnsafe<Ret>(Func<R, Ret> Right, Func<L, Ret> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        /// <summary>
        /// Invokes the Right or Left action depending on the state of the Either
        /// </summary>
        /// <param name="Right">Action to invoke if in a Right state</param>
        /// <param name="Left">Action to invoke if in a Left state</param>
        /// <returns>Unit</returns>
        public Unit MatchUnsafe(Action<R> Right, Action<L> Left)
        {
            if (IsRight)
            {
                Right(RightValue);
            }
            else
            {
                Left(LeftValue);
            }
            return unit;
        }

        /// <summary>
        /// Executes the Left function if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// </summary>
        /// <param name="Left">Function to generate a Right value if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public R IfLeftUnsafe(Func<R> Left) =>
            MatchUnsafe(identity, _ => Left());

        /// <summary>
        /// Returns the rightValue if the Either is in a Left state.
        /// Returns the Right value if the Either is in a Right state.
        /// <param name="Left">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Right value</returns>
        public R IfLeftUnsafe(R Left) =>
            MatchUnsafe(identity, _ => Left);

        /// <summary>
        /// Invokes the Right action if the Either is in a Right state, otherwise does nothing
        /// <param name="Right">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfRightUnsafe(Action<R> Right)
        {
            if (IsRight)
            {
                Right(right);
            }
            return unit;
        }

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        public EitherUnsafeUnitContext<L, R> Right(Action<R> rightHandler) =>
            new EitherUnsafeUnitContext<L, R>(this, rightHandler);

        /// <summary>
        /// Match Right and return a context.  You must follow this with .Left(...) to complete the match
        /// </summary>
        /// <param name="right">Action to invoke if the Either is in a Right state</param>
        /// <returns>Context that must have Left() called upon it.</returns>
        public EitherUnsafeContext<L, R, Ret> Right<Ret>(Func<R, Ret> rightHandler) =>
            new EitherUnsafeContext<L, R, Ret>(this, rightHandler);

        /// <summary>
        /// Return a string representation of the Either
        /// </summary>
        /// <returns>String representation of the Either</returns>
        public override string ToString() =>
            IsBottom
                ? "Bottom"
                : IsRight
                    ? RightValue == null
                        ? "Right(null)"
                        : String.Format("Right({0})", RightValue)
                    : LeftValue == null
                        ? "Left(null)"
                        : String.Format("Left({0})", LeftValue);

        /// <summary>
        /// Returns a hash code of the wrapped value of the Either
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() =>
            IsBottom
                ? -1
                : IsRight
                    ? RightValue == null
                        ? 0
                        : RightValue.GetHashCode()
                    : LeftValue == null
                        ? 0
                        : LeftValue.GetHashCode();


        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj) =>
            obj is EitherUnsafe<L, R>
                ? map(this, (EitherUnsafe<L, R>)obj, (lhs, rhs) =>
                      lhs.IsLeft && rhs.IsLeft
                          ? lhs.LeftValue == null 
                                ? rhs.LeftValue == null
                                : lhs.LeftValue.Equals(rhs.LeftValue)
                          : lhs.IsLeft || rhs.IsLeft
                              ? false
                              : lhs.RightValue == null
                                    ? rhs.RightValue == null
                                    : lhs.RightValue.Equals(rhs.RightValue))
                : false;

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        public Lst<R> ToList() =>
            toList(AsEnumerable());

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        public ImmutableArray<R> ToArray() =>
            toArray(AsEnumerable());

        /// <summary>
        /// Equality operator override
        /// </summary>
        public static bool operator ==(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator override
        /// </summary>
        public static bool operator !=(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            !lhs.Equals(rhs);

        /// <summary>
        /// Override of the Or operator to be a Left coalescing operator
        /// </summary>
        public static EitherUnsafe<L, R> operator |(EitherUnsafe<L, R> lhs, EitherUnsafe<L, R> rhs) =>
            lhs.IsBottom || rhs.IsBottom
                ? lhs
                : lhs.IsRight
                    ? lhs
                    : rhs;

        /// <summary>
        /// Override of the True operator to return True if the Either is Right
        /// </summary>
        public static bool operator true(EitherUnsafe<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsRight;

        /// <summary>
        /// Override of the False operator to return True if the Either is Left
        /// </summary>
        public static bool operator false(EitherUnsafe<L, R> value) =>
            value.IsBottom
                ? false
                : value.IsLeft;

        public IEnumerable<R> AsEnumerable()
        {
            if (IsRight)
            {
                yield return RightValue;
            }
        }

        public int CompareTo(EitherUnsafe<L, R> other) =>
            IsLeft && other.IsLeft
                ? Comparer<L>.Default.Compare(LeftValue, other.LeftValue)
                : IsRight && other.IsRight
                    ? Comparer<R>.Default.Compare(RightValue, other.RightValue)
                    : IsLeft
                        ? -1
                        : 1;

        /// <summary>
        /// CompareTo override
        /// </summary>
        public int CompareTo(R other) =>
            IsRight
                ? Comparer<R>.Default.Compare(RightValue, other)
                : -1;

        /// <summary>
        /// CompareTo override
        /// </summary>
        public int CompareTo(L other) =>
            IsRight
                ? -1
                : Comparer<L>.Default.Compare(LeftValue, other);

        /// <summary>
        /// CompareTo override
        /// </summary>
        public bool Equals(R other) =>
            IsRight
                ? EqualityComparer<R>.Default.Equals(RightValue, other)
                : false;

        /// <summary>
        /// Equality override
        /// </summary>
        public bool Equals(L other) =>
            IsLeft
                ? EqualityComparer<L>.Default.Equals(LeftValue, other)
                : false;

        /// <summary>
        /// Equality override
        /// </summary>
        public bool Equals(EitherUnsafe<L, R> other) =>
            IsRight
                ? other.Equals(RightValue)
                : other.Equals(LeftValue);

        /// <summary>
        /// Match the Right and Left values but as objects.  This can be useful to avoid reflection.
        /// </summary>
        public TResult MatchUntyped<TResult>(Func<object, TResult> Right, Func<object, TResult> Left) =>
            IsRight
                ? Right(RightValue)
                : Left(LeftValue);

        /// <summary>
        /// Find out the underlying Right type
        /// </summary>
        public Type GetUnderlyingRightType() =>
            typeof(R);

        /// <summary>
        /// Find out the underlying Left type
        /// </summary>
        public Type GetUnderlyingLeftType() =>
            typeof(L);

        private U CheckInitialised<U>(U value) =>
            State == EitherState.IsUninitialised
                ? raise<U>(new EitherNotInitialisedException())
                : value;

        public EitherUnsafe<L, Ret> BindUnsafe<Ret>(Func<R, EitherUnsafe<L, Ret>> binder) =>
            IsRight
                ? binder(RightValue)
                : EitherUnsafe<L, Ret>.Left(LeftValue);
        internal static EitherUnsafe<L, R> Right(R value) =>
            new EitherUnsafe<L, R>(value);

        internal static EitherUnsafe<L, R> Left(L value) =>
            new EitherUnsafe<L, R>(value);

        internal R RightValue =>
            CheckInitialised(
                IsRight
                    ? right
                    : raise<R>(new EitherIsNotRightException())
            );

        internal L LeftValue =>
            CheckInitialised(
                IsLeft
                    ? left
                    : raise<L>(new EitherIsNotLeftException())
            );

        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfLeftUnsafe' instead")]
        public R FailureUnsafe(Func<R> None) =>
            MatchUnsafe(identity, _ => None());

        /// <summary>
        /// Deprecated
        /// </summary>
        [Obsolete("'FailureUnsafe' has been deprecated.  Please use 'IfLeftUnsafe' instead")]
        public R FailureUnsafe(R noneValue) =>
            MatchUnsafe(identity, _ => noneValue);
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct EitherUnsafeContext<L, R, Ret>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Func<R, Ret> rightHandler;

        internal EitherUnsafeContext(EitherUnsafe<L, R> either, Func<R, Ret> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        public Ret Left(Func<L, Ret> Left)
        {
            return matchUnsafe(either, rightHandler, Left);
        }
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct EitherUnsafeUnitContext<L, R>
    {
        readonly EitherUnsafe<L, R> either;
        readonly Action<R> rightHandler;

        internal EitherUnsafeUnitContext(EitherUnsafe<L, R> either, Action<R> rightHandler)
        {
            this.either = either;
            this.rightHandler = rightHandler;
        }

        /// <summary>
        /// Left match
        /// </summary>
        /// <param name="Left">Left handler</param>
        /// <returns>Result of the match</returns>
        public Unit Left(Action<L> Left)
        {
            return matchUnsafe(either, rightHandler, Left);
        }
    }
}

public static class __EitherUnsafeExt
{
    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="mapper">Map function</param>
    /// <returns>Mapped Either</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, UR> Select<L, TR, UR>(this EitherUnsafe<L, TR> self, Func<TR, UR> map) =>
        self.Map(map);

    /// <summary>
    /// Sum of the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>0 if Left, or value of Right</returns>
    public static int Sum<L>(this EitherUnsafe<L, int> self) =>
        self.IsBottom || self.IsLeft
            ? 0
            : self.RightValue;

    /// <summary>
    /// Counts the Either
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to count</param>
    /// <returns>1 if the Either is in a Right state, 0 otherwise.</returns>
    public static int Count<L, R>(this EitherUnsafe<L, R> self) =>
        self.IsBottom || self.IsLeft
            ? 0
            : 1;

    /// <summary>
    /// Iterate the Either
    /// action is invoked if in the Right state
    /// </summary>
    public static Unit Iter<L, R>(this EitherUnsafe<L, R> self, Action<R> action)
    {
        if (self.IsBottom)
        {
            return unit;
        }
        if (self.IsRight)
        {
            action(self.RightValue);
        }
        return unit;
    }

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to forall</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Left state.  
    /// True if the Either is in a Right state and the predicate returns True.  
    /// False otherwise.</returns>
    public static bool ForAll<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? true
            : self.IsRight
                ? pred(self.RightValue)
                : true;

    /// <summary>
    /// Folds the either into an S
    /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
    /// </summary>
    /// <typeparam name="S">State</typeparam>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to fold</param>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Folded state</returns>
    public static S Fold<L, R, S>(this EitherUnsafe<L, R> self, S state, Func<S, R, S> folder) =>
        self.IsBottom
            ? state
            : self.IsRight
                ? folder(state, self.RightValue)
                : state;

    /// <summary>
    /// Invokes a predicate on the value of the Either if it's in the Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to check existence of</param>
    /// <param name="pred">Predicate</param>
    /// <returns>True if the Either is in a Right state and the predicate returns True.  False otherwise.</returns>
    public static bool Exists<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? false
            : self.IsRight
                ? pred(self.RightValue)
                : false;

    /// <summary>
    /// Maps the value in the Either if it's in a Right state
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret">Mapped Either type</typeparam>
    /// <param name="self">Either to map</param>
    /// <param name="mapper">Map function</param>
    /// <returns>Mapped Either</returns>
    public static EitherUnsafe<L, Ret> Map<L, R, Ret>(this EitherUnsafe<L, R> self, Func<R, Ret> mapper) =>
        self.IsBottom
            ? new EitherUnsafe<L, Ret>(true)
            : self.IsRight
                ? RightUnsafe<L, Ret>(mapper(self.RightValue))
                : LeftUnsafe<L, Ret>(self.LeftValue);

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <typeparam name="Ret"></typeparam>
    /// <param name="self"></param>
    /// <param name="binder"></param>
    /// <returns>Bound Either</returns>
    public static EitherUnsafe<L, Ret> Bind<L, R, Ret>(this EitherUnsafe<L, R> self, Func<R, EitherUnsafe<L, Ret>> binder) =>
        self.IsBottom
            ? new EitherUnsafe<L, Ret>(true)
            : self.IsRight
                ? binder(self.RightValue)
                : EitherUnsafe<L, Ret>.Left(self.LeftValue);

    /// <summary>
    /// Filter the Either
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filterd and that
    /// should be checked.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, R> Where<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        Filter(self, pred);

    /// <summary>
    /// Filter the Either
    /// This may give unpredictable results for a filtered value.  The Either won't
    /// return true for IsLeft or IsRight.  IsBottom is True if the value is filterd and that
    /// should be checked.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either to filter</param>
    /// <param name="pred">Predicate function</param>
    /// <returns>If the Either is in the Left state it is returned as-is.  
    /// If in the Right state the predicate is applied to the Right value.
    /// If the predicate returns True the Either is returned as-is.
    /// If the predicate returns False the Either is returned in a 'Bottom' state.  IsLeft will return True, but the value 
    /// of Left = default(L)</returns>
    public static EitherUnsafe<L, R> Filter<L, R>(this EitherUnsafe<L, R> self, Func<R, bool> pred) =>
        self.IsBottom
            ? self
            : matchUnsafe(self,
                Right: t => pred(t) ? EitherUnsafe<L, R>.Right(t) : new EitherUnsafe<L, R>(true),
                Left: l => EitherUnsafe<L, R>.Left(l));

    /// <summary>
    /// Monadic bind function
    /// https://en.wikipedia.org/wiki/Monad_(functional_programming)
    /// </summary>
    /// <returns>Bound Either</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EitherUnsafe<L, V> SelectMany<L, T, U, V>(this EitherUnsafe<L, T> self, Func<T, EitherUnsafe<L, U>> bind, Func<T, U, V> project)
    {
        if (self.IsBottom) return new EitherUnsafe<L, V>(true);
        if (self.IsLeft) return EitherUnsafe<L, V>.Left(self.LeftValue);
        var u = bind(self.RightValue);
        if (u.IsBottom) return new EitherUnsafe<L, V>(true);
        if (u.IsLeft) return EitherUnsafe<L, V>.Left(u.LeftValue);
        return project(self.RightValue, u.RightValue);
    }
}
