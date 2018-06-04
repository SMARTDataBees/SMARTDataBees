//	============================================================================
//
//  .,-:::::   :::.    :::::::..   :::::::.      ...   :::.    :::.
//	,;;;'````'   ;;`;;   ;;;;``;;;;   ;;;'';;'  .;;;;;;;.`;;;;,  `;;;
//	[[[         ,[[ '[[,  [[[,/[[['   [[[__[[\.,[[     \[[,[[[[[. '[[
//	$$$        c$$$cc$$$c $$$$$$c     $$""""Y$$$$$,     $$$$$$ "Y$c$$
//	`88bo,__,o, 888   888,888b "88bo,_88o,,od8P"888,_ _,88P888    Y88
//	"YUMMMMMP"YMM   ""` MMMM   "W" ""YUMMMP"   "YMMMMMP" MMM     YM
//
//	============================================================================
//
//	This file is a part of the Carbon Framework.
//
//	Copyright (C) 2005 Mark (Code6) Belles 
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//	============================================================================

using System;
using System.Diagnostics;
using System.Reflection;

namespace Carbon.Common
{
	/// <summary>
	/// Provides methods for loading Types and creating instances of loaded Types.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public static class TypeUtilities
	{
		#region RequiredConstructorNotFoundException

		/// <summary>
		/// Defines an exception that is generated when a Type to be created does not provide a constructor
		/// that supports a required set of parameters or order.
		/// </summary>
		public sealed class RequiredConstructorNotFoundException : ApplicationException
		{
			private readonly Type _typeToCreate;
			private readonly Type[] _constructorParamTypes;

			/// <summary>
			/// Initializes a new instance of the RequiredConstructorNotFoundException class
			/// </summary>
			/// <param name="typeToCreate">The Type that was to be created</param>
			/// <param name="constructorParamTypes">The array of Types that define the required constructor's prototype</param>
			internal RequiredConstructorNotFoundException(Type typeToCreate, Type[] constructorParamTypes) : 
				base(string.Format("The Type '{0}' does not provide a constructor with the required parameter types.", typeToCreate.FullName))
			{
				_typeToCreate = typeToCreate;
				_constructorParamTypes = constructorParamTypes;
			}

			/// <summary>
			/// Returns the Type that was to be created
			/// </summary>
			public Type TypeToCreate
			{
				get
				{
					return _typeToCreate;
				}
			}

			/// <summary>
			/// Returns an array of Types that define the required constructor's prototype
			/// </summary>
			public Type[] ConstructorParamTypes
			{
				get
				{
					return _constructorParamTypes;
				}
			}
		}

		#endregion        

		#region TypeDoesNotDeriveFromRequiredBaseTypeException

		/// <summary>
		/// Defines an exception that is generated when a Type does not derive from a required base Type.
		/// </summary>
		public sealed class TypeDoesNotDeriveFromRequiredBaseTypeException : Exception
		{
			private readonly Type _type;
			private readonly Type _requiredBaseType;

			/// <summary>
			/// Initializes a new instance of the TypeDoesNotDeriveFromRequiredBaseTypeException class.
			/// </summary>
			/// <param name="type">The Type that should derive from the specified base Type</param>
			/// <param name="requiredBaseType">The base Type the specified Type needs to derive from</param>
			internal TypeDoesNotDeriveFromRequiredBaseTypeException(Type type, Type requiredBaseType) : 
				base(string.Format("The Type '{0}' does not derive from the required base Type '{1}'.", type.FullName, requiredBaseType.FullName))
			{
				_type = type;
				_requiredBaseType = requiredBaseType;
			}

			/// <summary>
			/// Returns the Type that should derive from the specified base Type
			/// </summary>
			public Type Type
			{
				get
				{
					return _type;
				}
			}
			
			/// <summary>
			/// Returns the base Type the specified Type needs to derive from
			/// </summary>
			public Type RequiredBaseType
			{
				get
				{
					return _requiredBaseType;
				}
			}
		}

		#endregion

		/// <summary>
		/// Defines an exception that is generated when a Type does not implement the required interface Type.
		/// </summary>
		public sealed class TypeDoesNotImplementRequiredInterfaceTypeException : Exception
		{
			private readonly Type _type;
			private readonly Type _requiredInterfaceType;

			/// <summary>
			/// Initializes a new instance of the TypeDoesNotDeriveFromRequiredBaseTypeException class.
			/// </summary>
			/// <param name="type">The type that should implement the required interface Type.</param>
			/// <param name="requiredInterfaceType">The interface Type that should be implemented.</param>
			internal TypeDoesNotImplementRequiredInterfaceTypeException(Type type, Type requiredInterfaceType)
				: base(string.Format("The Type '{0}' does not implement the required interface Type '{1}'.", type.FullName, requiredInterfaceType.FullName))
			{
				_type = type;
				_requiredInterfaceType = type;
			}

			/// <summary>
			/// Returns the Type that should implement the specified Type.
			/// </summary>
			public Type Type
			{
				get
				{
					return _type;
				}
			}

			/// <summary>
			/// Returns the required Type the specified Type needs to implement.
			/// </summary>
			public Type RequiredInterfaceType
			{
				get
				{
					return _requiredInterfaceType;
				}
			}
		}

		/// <summary>
		/// Gets the Type with the specified name, specifying whether to perform a case-sensitive 
		/// search and whether to throw an exception if an error occurs while loading the Type.
		/// </summary>
		/// <param name="typeName">The name of the Type to get</param>
		/// <param name="throwOnError">true to throw any exception that occurs, false to ignore any exception that occurs</param>
		/// <param name="ignoreCase">true to perform a case-insensitive search for typeName, false to perform a case-sensitive search for typeName</param>
		/// <returns></returns>
		public static Type GetTypeFromFullyQualifiedName(string typeName, bool throwOnError, bool ignoreCase)
		{
			Type type = null;			
			try
			{
				Log.WriteLine("Provider Type '{0}' registered in App.Config.", typeName);

				type = Type.GetType(typeName, throwOnError, ignoreCase);

				Log.WriteLine("Provider Type '{0}' loaded. Qualified as '{1}'.", type.Name, type.AssemblyQualifiedName);
			}
			catch(TypeLoadException ex)
			{
				Log.WriteLine("Provider Type '{0}' could not be loaded. Ensure the assembly is in the application's path or the .NET GAC and try again. Exception: '{1}'.", typeName, ex.Message);
				
				if (throwOnError)
					throw; // preserve the stack
			}
			return type;
		}

		/// <summary>
		/// Determines whether the current Type derives from the specified Type. 
		/// </summary>
		/// <exception cref="TypeDoesNotDeriveFromRequiredBaseTypeException">TypeDoesNotDeriveFromRequiredBaseTypeException</exception>
		/// <param name="type">The Type to check.</param>
		/// <param name="baseClassType">The base Type the specified Type must derive</param>
		public static void AssertTypeIsSubclassOfBaseType(Type type, Type requiredBaseType)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (requiredBaseType == null)
				throw new ArgumentNullException("requiredBaseType");

			// it better inherit from the base type or actually be the base type
			if (!type.IsSubclassOf(requiredBaseType) || type == requiredBaseType)
				throw new TypeDoesNotDeriveFromRequiredBaseTypeException(type, requiredBaseType);			
		}

		/// <summary>
		/// Determines whether the Type implements the interface Type specified.	
		/// </summary>
		/// <exception cref="TypeDoesNotImplementRequiredInterfaceTypeException">TypeDoesNotImplementRequiredInterfaceTypeException</exception>
		/// <param name="type">The Type to check.</param>
		/// <param name="requiredInterfaceType">The interface Type the specified Type must implement.</param>
		public static void AssertTypeImplementsInterface(Type type, Type requiredInterfaceType)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (requiredInterfaceType == null)
				throw new ArgumentNullException("requiredInterfaceType");

			if (type.GetInterface(requiredInterfaceType.Name, true) == null)
				throw new TypeDoesNotImplementRequiredInterfaceTypeException(type, requiredInterfaceType);
		}

		/// <summary>
		/// Creates an instance of the specified Type
		/// </summary>
		/// <param name="type">The Type to create</param>
		/// <param name="constructorParamTypes">An array of Types that define the constructor to use to create the type</param>
		/// <returns></returns>
		public static object CreateInstanceOfType(Type type, Type[] constructorParamTypes, object[] args)
		{			
			//return Activator.CreateInstance(type, args);

			// look for the required constructor
			ConstructorInfo ci = type.GetConstructor(constructorParamTypes);
			if (ci == null)
				// if the required constructor cannot be found, we cannot continue
				// check the class definition and add the appropriate constructor
				throw new RequiredConstructorNotFoundException(type, constructorParamTypes);

			// create an instance of the specified type
			return ci.Invoke(args);
		}
	}
}
