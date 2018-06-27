
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SDBees.Utils;

namespace SDBees
{
    public class Profiler
    {
        const string indent = "    ";

        const string newline = "\r\n\r\n";

        // Classes ...

        public class Tree
        {
            private List<Node> m_roots;

            private Node m_parent;

            public Tree()
            {
                m_roots = new List<Node>();

                m_parent = null;
            }

            public void Clear()
            {
                m_roots.Clear();

                m_parent = null;
            }

            public void AddNode(Node node)
            {
                m_roots.Add(node);
            }

            public Node GetNode(string name, bool create)
            {
                var result = FindNode(m_parent, name);

                if ((result == null) && create)
                {
                    result = new Node(this, m_parent, name);
                }

                return result;
            }

            public void Start(Node node)
            {
                m_parent = node;
            }

            public void Stop(Node node)
            {
                m_parent = node.Parent;
            }

            public string callgraph()
            {
                var result = "";

                foreach (var node in m_roots)
                {
                    result += node + newline;
                }

                return result;
            }

            public string flatlist()
            {
                var result = "";

                var functions = new Functions();

                foreach (var node in m_roots)
                {
                    node.AddFunctions(functions);
                }

                foreach (var function in functions.Values)
                {
                    result += function.result(functions.TotalTime) + newline;
                }

                return result;
            }

            private Node FindNode(Node parent, string name)
            {
                Node result = null;

                if (parent == null)
                {
                    foreach (var root in m_roots)
                    {
                        if (root.Name == name)
                        {
                            result = root;

                            break;
                        }
                    }
                }
                else
                {
                    result = parent.FindNode(name);
                }

                return result;
            }
        }

        public class Node
        {
            private string m_name;

            private long m_count;

            private Stopwatch m_stopwatch;

            private Tree m_tree;

            private Node m_parent;

            private List<Node> m_children;

            private int m_level;

            private List<string> m_messages = new List<string>();

            public Node(Tree tree, Node parent, string name)
            {
                m_name = name;

                m_count = 0;

                m_stopwatch = new Stopwatch();

                m_tree = tree;

                m_parent = parent;

                if (m_parent == null)
                {
                    m_tree.AddNode(this);
                }
                else
                {
                    m_parent.AddNode(this);
                }

                m_children = new List<Node>();

                m_level = m_parent?.Level + 1 ?? 0;
            }

            public Node Parent
            {
                get
                {
                    return m_parent;
                }
            }

            public int Level
            {
                get
                {
                    return m_level;
                }
            }

            public string Name
            {
                get
                {
                    return m_name;
                }
            }

            public Stopwatch StopWatch
            {
                get
                {
                    return m_stopwatch; 
                }
            }

            public Node FindNode(string name)
            {
                Node result = null;

                foreach (var node in m_children)
                {
                    if (node.Name == name)
                    {
                        result = node;

                        break;
                    }
                }

                return result;
            }

            public void AddNode(Node node)
            {
                m_children.Add(node);
            }

            public void AddFunctions(Functions functions)
            {
                functions.AddFunction(Name, m_count, ElapsedMilliseconds(false));

                foreach (var child in m_children)
                {
                    child.AddFunctions(functions);
                }
            }

            public void Start()
            {
                m_tree.Start(this);

                m_stopwatch.Start();

                m_count++;
            }

            public void Stop()
            {
                m_stopwatch.Stop();

                m_tree.Stop(this);
            }

            public void Log(string message)
            {
                var lastMessage = (0 == m_messages.Count) ? null : m_messages[m_messages.Count - 1];

                if ((lastMessage == null) || (lastMessage.CompareTo(message) != 0))
                {
                    m_messages.Add(message);
                }
            }

            public override string ToString()
            {
                var result = "";

                for (var index = 0; index < m_level; index++)
                {
                    result += indent;
                }

                result += m_name + " : " + (double)ElapsedMilliseconds(false) / 1000 + "s / " + (double)ElapsedMilliseconds(true) / 1000 + "s for #" + m_count;

                result += ListToString(m_messages, "LOG: ", m_level + 1);

                foreach (var child in m_children)
                {
                    result += newline + child;
                }

                return result;
            }

            private long ElapsedMilliseconds(bool includeChildren)
            {
                return includeChildren ? m_stopwatch.ElapsedMilliseconds : m_stopwatch.ElapsedMilliseconds - ElapsedMillisecondsChildren();
            }

            private long ElapsedMillisecondsChildren()
            {
                var result = 0L;

                foreach (var child in m_children)
                {
                    result += child.ElapsedMilliseconds(true);
                }

                return result;
            }
        }

        public class Function
        {
            private string m_name;

            private long m_milliseconds;

            private long m_count;

            public Function(string name)
            {
                m_name = name;

                m_milliseconds = 0;

                m_count = 0;
            }

            public long Time
            {
                get
                {
                    return m_milliseconds;
                }
            }

            public void add(long count, long milliseconds)
            {
                m_count += count;

                m_milliseconds += milliseconds;
            }

            public string result(long totalTime)
            {
                return m_name + " : " + (double)m_milliseconds / 1000 + "s for #" + m_count + " " + m_milliseconds / (double)totalTime * 100.0 + "%";               }        }

        public class Functions
        {
            private Dictionary<string, Function> m_functions = new Dictionary<string, Function>();

            private long m_milliseconds;

            public Functions()
            {
                m_milliseconds = 0;
            }

            public void AddFunction(string name, long count, long time)
            {
                m_functions.TryGetValue(name, out var function);

                if (function == null)
                {
                    function = new Function(name);

                    m_functions.Add(name, function);
                }

                function.add(count, time);

                m_milliseconds += time;
            }

            public long TotalTime
            {
                get
                {
                    return m_milliseconds;
                }
            }

            public List<Function> Values
            {
                get
                {
                    var result = new List<Function>();

                    result.AddRange(m_functions.Values);

                    result.Sort(CompareFunctionsByTime);

                    return result;
                }
            }

            private static int CompareFunctionsByTime(Function x, Function y)
            {
                return -1 * x.Time.CompareTo(y.Time);
            }
        }

        public class Results : Queue<Tree>
        {
            public Results() : base(new Queue<Tree>())
            {
                // empty
            }

            public override string ToString()
            {
                var result = "";

                var count = 1;

                foreach (var tree in ToArray())
                {
                    result += "Callgraph of run #" + count + newline + tree.callgraph() + newline + "Functions of run #" + count + newline + tree.flatlist() + newline;

                    count ++;
                }

                return result;
            }
        }

        // Member variables ...

        private static bool s_enabled;

        private static Results s_results = new Results();

        private static Tree s_tree = new Tree();

        private static Stack<Node> s_nodeStack = new Stack<Node>();

        // Member functions ...

        public static bool Enabled
        {
            get
            {
                return s_enabled;
            }
            set
            {
                s_enabled = value;
            }
        }

        public static bool Empty()
        {
            return Enabled ? s_nodeStack.Count == 0 : true;
        }

        public static void Clear()
        {
            if (Enabled)
            {
                s_results.Clear();

                s_tree.Clear();

                s_nodeStack.Clear();
            }
        }

        public static Results RememberAndOpenNotepad()
        {
            if (Enabled)
            {
                if (s_nodeStack.Count == 0)
                {
                    s_results.Enqueue(s_tree);

                    s_tree = new Tree();

                    s_nodeStack.Clear();

                    WriteResults(s_results, true);
                }
            }

            return s_results;
        }

        public static void Start(string name)
        {
            if (Enabled)
            {
                var node = s_tree.GetNode(name, true);

                if (node != null)
                {
                    node.Start();

                    s_nodeStack.Push(node);
                }
            }
        }
        
        public static void Relay(string name)
        {
            Stop();

            Start(name);
        }

        public static void Stop()
        {
            if (Enabled)
            {
                var node = s_nodeStack.Peek();

                if (node != null)
                {
                    node.Stop();

                    s_nodeStack.Pop();
                }
            }
        }

        public static Results GetResults()
        {
            return s_results;
        }

        public static void Log(string message)
        {
            if (Enabled)
            {
                var node = s_nodeStack.Peek();

                node?.Log(message);
            }
        }

        private static void WriteResults(Results results, bool launchNotpad)
        {
            WriteLines(results.ToString(), "profiler.txt", launchNotpad);
        }

        private static void WriteLines(string lines, string filename, bool launchNotepad)
        {
            var path = Path.Combine(DirectoryTools.GetTempDir(), filename);

            var file = new StreamWriter(path);

            file.WriteLine(lines);

            file.Close();

            if (launchNotepad) Process.Start(path);
        }

        private static string ListToString(List<string> messages, string prefix, int level)
        {
            var result = "";

            var indentation = "";

            for (var index = 0; index < level; index++)
            {
                indentation += indent;
            }

            foreach (var message in messages)
            {
                result += newline + indentation + prefix + message;
            }

            return result;
        }
    }
}
