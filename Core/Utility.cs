using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using UniRx;

using Random = System.Random;

namespace Augment
{
    public static class Utility
    {
        private static readonly Random _rand = new Random();

        /// <summary>
        /// Returns a random element in an array.
        /// </summary>
        public static TElement GetRandom<TElement>(this TElement[] array)
        {
            return array[_rand.Next(0, array.Length)];
        }

        public static TElement GetRandom<TElement>(this List<TElement> list)
        {
            return list[_rand.Next(0, list.Count)];
        }

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            while (count-- > 1)
            {
                var i = _rand.Next(count + 1);
                var value = list[i];
                list[i] = list[count];
                list[count] = value;
            }

            return list;
        }

        public static void ForEachReverse<T>(this IReactiveCollection<T> collection, Action<T> action)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
                action(collection[i]);
        }

        public static void ForEachReverse<T>(this IList<T> collection, Action<T> action)
        {
            for (var i = collection.Count - 1; i >= 0; i--)
                action(collection[i]);
        }

        public static void ForEachReverse<T>(this T[] collection, Action<T> action)
        {
            for (var i = collection.Length - 1; i >= 0; i--)
                action(collection[i]);
        }

        public static Component CopyComponent(Component original, GameObject destination)
        {
            var type = original.GetType();
            var copy = destination.AddComponent(type);

            foreach (var field in type.GetFields())
                field.SetValue(copy, field.GetValue(original));

            return copy;
        }

        public static bool PercentageChance(float percent)
        {
            return _rand.NextDouble() < percent;
        }

        public static void Bind(this Button button, Action action)
        {
            Assert.IsNotNull(button);
            Assert.IsNotNull(action);
            button.OnClickAsObservable()
                .Subscribe(_ => action())
                .AddTo(button);
        }

        public static void Bind(this Toggle toggle, Action<bool> action)
        {
            Assert.IsNotNull(toggle);
            Assert.IsNotNull(action);
            toggle.OnValueChangedAsObservable()
                .Subscribe(x =>
                {
                    action(x);
                    toggle.isOn = x;
                })
                .AddTo(toggle);
        }

        public static IDisposable StateChange<T>(this IObservable<T> observable, Action<T, T> action)
        {
            return observable.Pairwise().Subscribe(pair =>
            {
                action(pair.Previous, pair.Current);
            });
        }
    }
}