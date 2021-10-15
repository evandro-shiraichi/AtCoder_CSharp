using System;
using AtCoder.ABC;

namespace AtCoder {
    class Program {
        static void Main(string[] args) {
            while (true) {
                Exec(args);
                Console.WriteLine("============ Retry ============");
                Console.Out.Flush();
            }
        }

        private static void Exec(string[] args) {
            QuestionA
                .Main(args);
        }
    }
}
