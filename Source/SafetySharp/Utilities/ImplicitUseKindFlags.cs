﻿using System;

namespace SafetySharp.Utilities
{
	/// <summary>
	///     Specifies how the symbol is used implicitly when marked with <see cref="MeansImplicitUseAttribute" /> or
	///     <see cref="UsedImplicitlyAttribute" />.
	/// </summary>
	/// <remarks>Code generated by JetBrain's ReSharper.</remarks>
	[Flags]
	public enum ImplicitUseKindFlags
	{
		/// <summary>
		///     Indicates that the marked symbol is accessed, assigned, and instantiated implicitly.
		/// </summary>
		Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

		/// <summary>
		///     Indicates that the marked symbol is accessed.
		/// </summary>
		Access = 1,

		/// <summary>
		///     Indicates that the marked symbol is assigned.
		/// </summary>
		Assign = 2,

		/// <summary>
		///     Indicates implicit instantiation of a type with a fixed constructor signature.
		/// </summary>
		InstantiatedWithFixedConstructorSignature = 4,

		/// <summary>
		///     Indicates implicit instantiation of a type without a fixed constructor signature.
		/// </summary>
		InstantiatedNoFixedConstructorSignature = 8,
	}
}