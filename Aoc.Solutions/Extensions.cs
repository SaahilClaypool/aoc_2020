using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Extensions {
    public static class CommonExtensions {
        /// <summary>
        /// Print any object using the system json
        /// NOTE: this might get caught in infinite loops (look how to fix...)
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pretty">print multi line json</param>
        /// <returns></returns>
        public static string ToJson(this object self, bool pretty = false) {
            return JsonSerializer.Serialize(self, new JsonSerializerOptions { WriteIndented = pretty });
        }

        /// <summary>
        /// print file location and object pretty printed
        /// 
        /// https://www.hanselman.com/blog/GettingTheLineNumberAndFileNameFromC.aspx
        /// </summary>
        /// <param name="self">extended object</param>
        /// <param name="pretty">Multi line json</param>
        public static void Dbg(this object self, bool pretty = false) {
            var CallStack = new StackFrame(1, true);
            var fileName = System.IO.Path.GetFileName(CallStack.GetFileName());
            var lineNum = CallStack.GetFileLineNumber();
            var type = self.GetType();

            string representation = self is string @string ? @string : self.ToJson(pretty);
            // Console.WriteLine($"DBG: {fileName}:{lineNum} : {type} - {representation}");
            Console.WriteLine($"DBG: {representation}");
        }

        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) {
            return source.Select((item, index) => (item, index));
        }

        public static Output Then<Input, Output>(this Input input, Func<Input, Output> f) => f(input);

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this List<T> elements) {
            if (elements.Count == 0) {
                yield return new List<T>();
            }
            else {
                foreach (var combo in Combinations(elements.GetRange(1, elements.Count - 1))) {
                    yield return combo;
                    yield return combo.Concat(new List<T> { elements[0] });
                }

            }
        }

        // all combinations of k elements
        public static IEnumerable<IEnumerable<T>> Choose<T>(this IEnumerable<T> elements, int k) {
            if (k == 0) {
                return new[] { Array.Empty<T>() };
            }
            return elements.SelectMany((e, i) =>
                elements.Skip(i + 1) // skip over the ith element
                    .Choose(k - 1) // compute all combinations of the rest
                    .Select(c => (new[] { e }).Concat(c))); // add this element to each combo
        }

        public static IEnumerable<IEnumerable<T>> SplitAt<T>(this IEnumerable<T> items, Func<T, bool> splitter) {
            List<T> block = new();
            foreach (var item in items) {
                if (splitter(item) && block.Count > 0) {
                    yield return block;
                    block = new();
                }
                else {
                    block.Add(item);
                }
            }
            if (block.Count > 0) {
                yield return block;
            }
        }

    }
}
