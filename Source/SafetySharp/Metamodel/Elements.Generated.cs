//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the 'MetamodelElements.fsx' script.
//     Thursday, 10 April 2014, 12:29:26
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SafetySharp.Metamodel.Declarations
{
    using System;

    public abstract partial class TypeDeclaration : MetamodelElement
    {
        /// <summary>
        ///     Gets the name of the declared type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the namespace the type is declared in.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        protected TypeDeclaration(string name, string @namespace)
            : base()
        {
            Name = name;
            Namespace = @namespace;
            Validate();
        }

        /// <summary>
        ///     Validates the properties of a <see cref="TypeDeclaration" /> instance.
        /// </summary>
        partial void Validate();
    }

    public partial class ClassDeclaration : TypeDeclaration
    {
        /// <summary>
        ///     Gets ...
        /// </summary>
        public bool Members { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">...</param>
        public ClassDeclaration(string name, string @namespace, bool members)
            : base(name, @namespace)
        {
            Members = members;
            Validate();
        }

        /// <summary>
        ///     Validates the properties of a <see cref="ClassDeclaration" /> instance.
        /// </summary>
        partial void Validate();

        /// <summary>
        ///     Replaces the name in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        public ClassDeclaration WithName(string name)
        {
            return Update(name, Namespace, Members);
        }

        /// <summary>
        ///     Replaces the namespace in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ClassDeclaration WithNamespace(string @namespace)
        {
            return Update(Name, @namespace, Members);
        }

        /// <summary>
        ///     Replaces the members in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="members">...</param>
        public ClassDeclaration WithMembers(bool members)
        {
            return Update(Name, Namespace, members);
        }

        /// <summary>
        ///     Returns a new <see cref="ClassDeclaration" /> instance if any properties require an update.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">...</param>
        public ClassDeclaration Update(string name, string @namespace, bool members)
        {
            if (Name != name || Namespace != @namespace || Members != members)
                return new ClassDeclaration(name, @namespace, members);

            return this;
        }
    }

    public partial class ComponentDeclaration : TypeDeclaration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ComponentDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ComponentDeclaration(string name, string @namespace)
            : base(name, @namespace)
        {
            Validate();
        }

        /// <summary>
        ///     Validates the properties of a <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        partial void Validate();

        /// <summary>
        ///     Replaces the name in a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        public ComponentDeclaration WithName(string name)
        {
            return Update(name, Namespace);
        }

        /// <summary>
        ///     Replaces the namespace in a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ComponentDeclaration WithNamespace(string @namespace)
        {
            return Update(Name, @namespace);
        }

        /// <summary>
        ///     Returns a new <see cref="ComponentDeclaration" /> instance if any properties require an update.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ComponentDeclaration Update(string name, string @namespace)
        {
            if (Name != name || Namespace != @namespace)
                return new ComponentDeclaration(name, @namespace);

            return this;
        }
    }

    public abstract partial class MemberDeclaration : MetamodelElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberDeclaration" /> class.
        /// </summary>
        protected MemberDeclaration()
            : base()
        {
        }
    }

    public partial class FieldDeclaration : MemberDeclaration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FieldDeclaration" /> class.
        /// </summary>
        public FieldDeclaration()
            : base()
        {
        }
    }

    public partial class PropertyDeclaration : MemberDeclaration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDeclaration" /> class.
        /// </summary>
        public PropertyDeclaration()
            : base()
        {
        }
    }

    public partial class StateVariableDeclaration : MemberDeclaration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StateVariableDeclaration" /> class.
        /// </summary>
        public StateVariableDeclaration()
            : base()
        {
        }
    }
}

namespace SafetySharp.Metamodel.Expressions
{
    using System;

    public abstract partial class Expression : MetamodelElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Expression" /> class.
        /// </summary>
        protected Expression()
            : base()
        {
        }
    }

    public partial class GuardedCommandExpression : Expression
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuardedCommandExpression" /> class.
        /// </summary>
        public GuardedCommandExpression()
            : base()
        {
        }
    }
}

namespace SafetySharp.Metamodel.Statements
{
    using System;

    public abstract partial class Statement : MetamodelElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Statement" /> class.
        /// </summary>
        protected Statement()
            : base()
        {
        }
    }

    public partial class ExpressionStatement : Statement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionStatement" /> class.
        /// </summary>
        public ExpressionStatement()
            : base()
        {
        }
    }

    public partial class BlockStatement : Statement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BlockStatement" /> class.
        /// </summary>
        public BlockStatement()
            : base()
        {
        }
    }
}
