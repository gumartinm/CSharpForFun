using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chapter9
{
    public partial class Form1 : Form
    {
        delegate T MyFunc<T>();

        public Form1()
        {

            InitializeComponent();

            /**
             * 
             * Listing 9.1 Using an anonymous method to create a delegate instance.
             */
            Console.WriteLine("Listing 9.1 Using an anonymous method to create a delegate instance.");
            Func<string, int> returnLength1;
            returnLength1 = delegate(string text) { return text.Length; };
            Console.WriteLine(returnLength1("Hello"));

            /**
             * 
             * Listing 9.2 A long-winded first lambada expression, similar to an anonymous method.
             */
            Console.WriteLine("Listing 9.2 A long-winded first lambada expression, similar to an anonymous method.");
            Func<string, int> returnLength2;
            returnLength2 = (string text) => { return text.Length; };
            Console.WriteLine(returnLength2("Hello"));

            /**
             * 
             * Listing 9.3 A concise lambda expression.
             */
            Console.WriteLine("Listing 9.3 A concise lambda expression.");
            Func<string, int> returnLength3;
            returnLength3 = text => text.Length;
            Console.WriteLine(returnLength3("Hello"));

            /**
             * 
             * Listing 9.4 Manipulating a  list of films using lambda expressions.
             */
            Console.WriteLine("Listing 9.4 Manipulating a  list of films using lambda expressions.");
            var films = new List<Film>
            {
                new Film { Name = "Jaws", Year = 1975 },
                new Film { Name = "Singing in the Rain", Year = 1952 },
                new Film { Name = "Some like it Hot", Year = 1959 },
                new Film { Name = "The Wizard of Oz", Year = 1939 },
                new Film { Name = "It's a Wonderful Life", Year = 1946 },
                new Film { Name = "American Beauty", Year = 1999 },
                new Film { Name = "High Fidelity", Year = 2000 },
                new Film { Name = "The Usual Suspects", Year = 1995 }
            };

            Action<Film> print = film => Console.WriteLine("Name={0}, Year={1}",
                                                            film.Name, film.Year);
            films.ForEach(print);
            films.FindAll(film => film.Year < 1960)
                 .ForEach(print);
            films.Sort((f1, f2) => f1.Name.CompareTo(f2.Name));
            films.ForEach(print);

            /**
             * 
             * Listing 9.5 Logging events using lambda expressions.
             */
            Console.WriteLine("Listing 9.5 Logging events using lambda expressions.");

            this.button.Text = "Click me";
            this.button.Click += (src, e) => Log("Click", src, e);
            this.button.KeyPress += (src, e) => Log("KeyPress", src, e);
            this.button.MouseClick += (src, e) => Log("MouseClick", src, e);

            /**
             * 
             * Listing 9.6 A simple expression tree, adding 2 and 3.
             */
            Console.WriteLine("Listing 9.6 A simple expression tree, adding 2 and 3.");
            Expression firstArg1 = Expression.Constant(2);
            Expression secondArg1 = Expression.Constant(3);
            Expression add1 = Expression.Add(firstArg1, secondArg1);
            Console.WriteLine(add1);

            /**
             * 
             * Listing 9.7 Compiling an executing an expression tree.
             */
            Console.WriteLine("Listing 9.7 Compiling an executing an expression tree.");
            Expression firstArg2 = Expression.Constant(2);
            Expression secondArg2 = Expression.Constant(3);
            Expression add2 = Expression.Add(firstArg2, secondArg2);

            Func<int> compiled1 = Expression.Lambda<Func<int>>(add2).Compile();
            Console.WriteLine(compiled1());

            /**
             * 
             * Listing 9.8 Using lambda expressions to create expression trees.
             */
            Console.WriteLine("Listing 9.8 Using lambda expressions to create expression trees.");
            Expression<Func<int>> return5 = () => 5;
            Func<int> compiled2 = return5.Compile();
            Console.WriteLine(compiled2());

            /**
             * 
             * Listing 9.9 Demonstration of a more complicated expression tree.
             */
            Console.WriteLine("Listing 9.9 Demonstration of a more complicated expression tree.");
            Expression<Func<string, string, bool>> expression = (x, y) => x.StartsWith(y);
            var compiled3 = expression.Compile();

            Console.WriteLine(compiled3("First", "Second"));
            Console.WriteLine(compiled3("First", "Fir"));

            /**
             * 
             * Listing 9.10 Building a method call expression tree in code.
             */
            Console.WriteLine("Listing 9.10 Building a method call expression tree in code.");
            MethodInfo method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var target = Expression.Parameter(typeof(string), "x");
            var methodArg = Expression.Parameter(typeof(string), "y");
            Expression[] methodArgs = new[] { methodArg };

            Expression call = Expression.Call(target, method, methodArgs);

            var lambdaParameters = new[] { target, methodArg };
            var lambda = Expression.Lambda<Func<string, string, bool>>(call, lambdaParameters);

            var compiled4 = lambda.Compile();

            Console.WriteLine(compiled4("First", "Second"));
            Console.WriteLine(compiled4("First", "Fir"));

            /**
             * 
             * Listing 9.11 Example of code requiring the new type inference rules.
             */
            Console.WriteLine("Listing 9.11 Example of code requiring the new type inference rules.");
            PrintConvertedValue("I am a string", x => x.Length);

            /**
             * 
             * Listing 9.12 Attempting to infer the return type of an anonymous method.
             */
            Console.WriteLine("Listing 9.12 Attempting to infer the return type of an anonymous method.");
            WriteResult(delegate { return 5; });

            /**
             * 
             * Listing 9.13 Code returning an integer or an object depending on the time of day.
             */
            Console.WriteLine("Listing 9.13 Code returning an integer or an object depending on the time of day.");
            WriteResult(delegate
            {
                if (DateTime.Now.Hour < 12)
                {
                    return 10;
                }
                else 
                {
                    return new object();
                }
            });

            /**
             * 
             * Listing 9.14 Flexible type inference comibning information from multiple arguments.
             */
            Console.WriteLine("Listing 9.14 Flexible type inference comibning information from multiple arguments.");
            PrintType(1, new object());

            /**
             * 
             * Listing 9.15 Multistage type inference.
             */
            Console.WriteLine("Listing 9.15 Multistage type inference.");
            ConvertTwice("Another String", text => text.Length, length => Math.Sqrt(length));

            /**
             * 
             * Listing 9.16 Sample of overloading choice influenced by delegate return type.
             */
            Console.WriteLine("Listing 9.16 Sample of overloading choice influenced by delegate return type.");
            Execute(() => 1);
        }

        static void Log(string title, object sender, EventArgs e)
        {
            Console.WriteLine("Event: {0}", title);
            Console.WriteLine(" Sender: {0}", sender);
            Console.WriteLine(" Arguments: {0}", e.GetType());
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(e))
            {
                string name = prop.DisplayName;
                object value = prop.GetValue(e);
                Console.WriteLine("    {0}={1}", name, value);
            }
        }

        static void PrintConvertedValue<TInput, TOutput>(TInput input, Converter<TInput, TOutput> converter)
        {
            Console.WriteLine(converter(input));
        }

        static void WriteResult<T>(MyFunc<T> function)
        {
            Console.WriteLine(function());
        }

        static void PrintType<T>(T first, T second)
        {
            Console.WriteLine(typeof(T));
        }

        static void ConvertTwice<TInput, TMiddle, TOutput>(TInput input, 
                                                           Converter<TInput, TMiddle> firstConversion,
                                                           Converter<TMiddle, TOutput> secondConversion)
        {
            TMiddle middle = firstConversion(input);
            TOutput output = secondConversion(middle);
            Console.WriteLine(output);
        }

        static void Execute(Func<int> action)
        {
            Console.WriteLine("action returns an int: " + action());
        }

        static void Execute(Func<double> action)
        {
            Console.WriteLine("action returns a double: " + action());
        }
    }

    class Film
    {
        public string Name { get; set; }
        public int Year { get; set; }
    }
}
