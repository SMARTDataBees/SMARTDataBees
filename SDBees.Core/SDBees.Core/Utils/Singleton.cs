using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace EXOE.CsharpHelper
{
    //http://www.codeproject.com/Articles/14026/Generic-Singleton-Pattern-using-Reflection-in-C
    /// <summary>
    /// MultiThread supported
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Singleton<T> where T : class
    {
        static volatile T _instance; // Le mot clé volatile indique qu'un champ peut être modifié par plusieurs threads qui s'exécutent simultanément. Les champs qui sont déclarés volatile ne sont pas soumis aux optimisations du compilateur qui supposent l'accès par un seul thread. Cela garantit que la valeur la plus à jour est présente dans le champ à tout moment.
        // Le modificateur volatile est généralement utilisé pour un champ auquel accèdent plusieurs threads sans utiliser l'instruction lock, instruction (Référence C#) pour sérialiser l'accès.
        static object _lock = new object();

        static Singleton()
        { }

        public static T Instance => _instance ?? ResetAndGetInstance();

        private static T ResetAndGetInstance()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    GetInstance();
                }
            }
            return _instance;
        }

        private static void GetInstance()
        {
            ConstructorInfo constructor = null;
            try
            {
                constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null); // Binding flags exclude public constructors.
            }
            catch (Exception excep)
            {
                throw new SingletonException(excep);
            }
            if (constructor == null || constructor.IsAssembly)
            {
                throw new SingletonException(string.Format("A private or protected constructor is missing for '{0}'.", typeof(T).Name));
            }
            _instance = (T)constructor.Invoke(null);
        }
    }

    /// <summary>
    /// Represents errors that occur while creating a singleton.
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/library/ms229064(VS.80).aspx
    /// </remarks>
    [Serializable]
    public class SingletonException
       : Exception
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public SingletonException()
        {
        }

        /// <summary>
        /// Initializes a new instance with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SingletonException(string message)
            : base(message)
        {
            throw new Exception(message);
        }

        /// <summary>
        /// Initializes a new instance with a reference to the inner 
        /// exception that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.
        /// </param>
        public SingletonException(Exception innerException)
            : base(null, innerException)
        {
            throw new Exception(innerException.Message);
        }

        /// <summary>
        /// Initializes a new instance with a specified error message and a 
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, 
        /// or a null reference if no inner exception is specified.
        /// </param>
        public SingletonException(string message, Exception innerException)
            : base(message, innerException)
        {
            throw new Exception(message, innerException);
        }

#if !WindowsCE
        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the 
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains 
        /// contextual information about the source or destination.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The info parameter is null.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">The class name is null or System.Exception.HResult is zero (0).</exception>
        protected SingletonException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}
