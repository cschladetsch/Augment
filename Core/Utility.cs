namespace Augment
{
    using System;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using UniRx;
    using Random = System.Random;

    /// <summary>
    /// General utility functionality.
    /// </summary>
    public static class Utility
    {
        private static readonly Random _rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Make an enumerable of all integers between min and max
        /// </summary>
        public static IEnumerable<int> MakeRange(int min, int max)
        {
            for (var i = min; i <= max; ++i)
                yield return i;
        }

        /// <summary>
        /// Clean a path name, which should be done by Path.Combine.
        /// </summary>
        public static string SanitisePath(this string path)
        {
            path = path.Replace("%20", " ");
            path = path.Replace('\\', '/');
            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            if (path.StartsWith("/"))
                path = path.Substring(1);
            return path;
        }

        /// <summary>
        /// Puts the string into the Clipboard.
        /// </summary>
        public static void CopyToClipboard(string str)
        {
            var textEditor = new TextEditor { text = str };
            textEditor.SelectAll();
            textEditor.Copy();
        }

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

        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, _rand.Next(i, list.Count));
        }

        public static void Shuffle<T>(this T[] list)
        {
            for (var i = 0; i < list.Length; i++)
                list.Swap(i, _rand.Next(i, list.Length));
        }

        /// <summary>
        /// Connect this collection to another so that it reacts to additions and removals from the connected collection.
        /// </summary>
        /// <typeparam name="TTarget">The type of the connected collection.</typeparam>
        /// <param name="self">The collection to connect to and watch.</param>
        /// <param name="add">Called when an element is added to the connected collection.</param>
        /// <param name="remove">Called when an element is removed from the connected collection.</param>
        public static void ObserveChanges<TTarget>(
            this IReadOnlyReactiveCollection<TTarget> self,
            Action<CollectionAddEvent<TTarget>> add,
            Action<CollectionRemoveEvent<TTarget>> remove)
        {
            self.ObserveAdd().Subscribe(add);
            self.ObserveRemove().Subscribe(remove);
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
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

        public static IDisposable Bind(this Button button, Action action)
        {
            Assert.IsNotNull(button);
            Assert.IsNotNull(action);
            return button.OnClickAsObservable()
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

        public static IDisposable Bind<T>(this TextMeshProUGUI tmp, IReadOnlyReactiveProperty<T> prop)
        {
            if (tmp != null)
                return prop.Subscribe(x => tmp.text = x.ToString()).AddTo(tmp);

            Debug.LogError("TextMeshPro field not set");
            return null;

        }

        public static IDisposable Bind<T>(this Text text, IReadOnlyReactiveProperty<T> prop)
        {
            if (text != null)
                return prop.Subscribe(x => text.text = x.ToString()).AddTo(text);

            Debug.LogError("Text field not set");
            return null;
        }

        public static void DisposeAll(this IList<IDisposable> disposableCollection)
        {
            foreach (var disposable in disposableCollection)
                disposable.Dispose();

            disposableCollection.Clear();
        }

        public static string MakeFilename(this DateTime when)
        {
            return $"{when:yy-MM-dd}T{when:HH-mm-ss}";
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
