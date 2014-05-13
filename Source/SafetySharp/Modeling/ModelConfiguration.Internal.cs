﻿// The MIT License (MIT)
// 
// Copyright (c) 2014, Institute for Software & Systems Engineering
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SafetySharp.Modeling
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using System.Linq;
	using CSharp.Transformation;
	using Utilities;

	partial class ModelConfiguration
	{
		/// <summary>
		///     The partition root components of the configuration.
		/// </summary>
		private readonly List<Component> _partitionRoots = new List<Component>();

		/// <summary>
		///     Gets a snapshot of the current model configuration state.
		/// </summary>
		internal ModelConfigurationSnapshot GetSnapshot()
		{
			if (_partitionRoots.Count == 0)
				throw new InvalidOperationException("No partition roots have been set for the model configuration.");

			// Ensure that there are no shared components
			var hashSet = new HashSet<Component>();
			foreach (var component in _partitionRoots)
				CollectComponents(hashSet, component);

			return new ModelConfigurationSnapshot(_partitionRoots.Select(root => root.GetSnapshot()).ToImmutableArray());
		}

		/// <summary>
		///     Adds each component in <paramref name="components" /> as the root component of a partition to the model configuration.
		/// </summary>
		/// <param name="components">The components that should be added as root components of partitions.</param>
		partial void AddPartitionRoots(Component[] components)
		{
			Argument.NotNull(components, () => components);
			_partitionRoots.AddRange(components);
		}

		/// <summary>
		///     Recursively analyzes the object tree of <see cref="Component" /> instances and returns <paramref name="component" /> and
		///     all of its sub components.
		/// </summary>
		/// <param name="components">The hash set the components are collected into that is used to check if a component is shared.</param>
		/// <param name="component">The root component from which all sub components should be collected.</param>
		private static void CollectComponents(HashSet<Component> components, Component component)
		{
			const string message = "A component instance of type '{0}' has been found in multiple locations of the component tree.";
			if (!components.Add(component))
				throw new InvalidOperationException(String.Format(message, component.GetType().FullName));

			foreach (var subComponent in component.SubComponents)
				CollectComponents(components, subComponent);
		}
	}
}