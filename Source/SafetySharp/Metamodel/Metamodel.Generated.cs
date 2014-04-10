//------------------------------------------------------------------------------
// <auto-generated>
//     Generated by the 'Metamodel.fsx' script.
//     Thursday, 10 April 2014, 15:13:57
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SafetySharp.Metamodel.Declarations
{
    using System;
    using System.Collections.Immutable;
    using Utilities;

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
        ///     Gets the declared members of the type.
        /// </summary>
        public ImmutableArray<MemberDeclaration> Members { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        protected TypeDeclaration(string name, string @namespace, ImmutableArray<MemberDeclaration> members)
            : base()
        {
            Validate(name, @namespace, members);
            Name = name;
            Namespace = @namespace;
            Members = members;
        }

        /// <summary>
        ///     Validates all of the given property values.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        partial void Validate(string name, string @namespace, ImmutableArray<MemberDeclaration> members);
    }

    public partial class ClassDeclaration : TypeDeclaration
    {
        /// <summary>
        ///     Gets ...
        /// </summary>
        public bool SomeFlag { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClassDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        /// <param name="someFlag">...</param>
        public ClassDeclaration(string name, string @namespace, ImmutableArray<MemberDeclaration> members, bool someFlag)
            : base(name, @namespace, members)
        {
            Validate(someFlag);
            SomeFlag = someFlag;
        }

        /// <summary>
        ///     Validates all of the given property values.
        /// </summary>
        /// <param name="someFlag">...</param>
        partial void Validate(bool someFlag);

        /// <summary>
        ///     Replaces the name in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        public ClassDeclaration WithName(string name)
        {
            return Update(name, Namespace, Members, SomeFlag);
        }

        /// <summary>
        ///     Replaces the namespace in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ClassDeclaration WithNamespace(string @namespace)
        {
            return Update(Name, @namespace, Members, SomeFlag);
        }

        /// <summary>
        ///     Replaces the members in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="members">The declared members of the type.</param>
        public ClassDeclaration WithMembers(ImmutableArray<MemberDeclaration> members)
        {
            return Update(Name, Namespace, members, SomeFlag);
        }

        /// <summary>
        ///     Replaces the someFlag in a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="someFlag">...</param>
        public ClassDeclaration WithSomeFlag(bool someFlag)
        {
            return Update(Name, Namespace, Members, someFlag);
        }

        /// <summary>
        ///     Adds <paramref name="members" /> to a copy of the <see cref="ClassDeclaration" /> instance.
        /// </summary>
        /// <param name="members">The declared members of the type.</param>
        public ClassDeclaration AddMembers(params MemberDeclaration[] members)
        {
            return WithMembers(Members.AddRange(members));
        }

        /// <summary>
        ///     Returns a new <see cref="ClassDeclaration" /> instance if any properties require an update.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        /// <param name="someFlag">...</param>
        public ClassDeclaration Update(string name, string @namespace, ImmutableArray<MemberDeclaration> members, bool someFlag)
        {
            if (Name != name || Namespace != @namespace || Members != members || SomeFlag != someFlag)
                return new ClassDeclaration(name, @namespace, members, someFlag);

            return this;
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitClassDeclaration(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitClassDeclaration(this);
        }
    }

    public partial class ComponentDeclaration : TypeDeclaration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ComponentDeclaration" /> class.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        public ComponentDeclaration(string name, string @namespace, ImmutableArray<MemberDeclaration> members)
            : base(name, @namespace, members)
        {
        }

        /// <summary>
        ///     Replaces the name in a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        public ComponentDeclaration WithName(string name)
        {
            return Update(name, Namespace, Members);
        }

        /// <summary>
        ///     Replaces the namespace in a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="namespace">The namespace the type is declared in.</param>
        public ComponentDeclaration WithNamespace(string @namespace)
        {
            return Update(Name, @namespace, Members);
        }

        /// <summary>
        ///     Replaces the members in a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="members">The declared members of the type.</param>
        public ComponentDeclaration WithMembers(ImmutableArray<MemberDeclaration> members)
        {
            return Update(Name, Namespace, members);
        }

        /// <summary>
        ///     Adds <paramref name="members" /> to a copy of the <see cref="ComponentDeclaration" /> instance.
        /// </summary>
        /// <param name="members">The declared members of the type.</param>
        public ComponentDeclaration AddMembers(params MemberDeclaration[] members)
        {
            return WithMembers(Members.AddRange(members));
        }

        /// <summary>
        ///     Returns a new <see cref="ComponentDeclaration" /> instance if any properties require an update.
        /// </summary>
        /// <param name="name">The name of the declared type.</param>
        /// <param name="namespace">The namespace the type is declared in.</param>
        /// <param name="members">The declared members of the type.</param>
        public ComponentDeclaration Update(string name, string @namespace, ImmutableArray<MemberDeclaration> members)
        {
            if (Name != name || Namespace != @namespace || Members != members)
                return new ComponentDeclaration(name, @namespace, members);

            return this;
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitComponentDeclaration(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitComponentDeclaration(this);
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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitFieldDeclaration(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitFieldDeclaration(this);
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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitPropertyDeclaration(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitPropertyDeclaration(this);
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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitStateVariableDeclaration(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitStateVariableDeclaration(this);
        }
    }
}

namespace SafetySharp.Metamodel.Expressions
{
    using System;
    using System.Collections.Immutable;
    using Utilities;

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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitGuardedCommandExpression(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitGuardedCommandExpression(this);
        }
    }
}

namespace SafetySharp.Metamodel.Statements
{
    using System;
    using System.Collections.Immutable;
    using Utilities;

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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitExpressionStatement(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitExpressionStatement(this);
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

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override void Accept(MetamodelVisitor visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            visitor.VisitBlockStatement(this);
        }

        /// <summary>
        ///     Accepts <paramref name="visitor" />, calling the type-specific visit method.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="visitor" />.</typeparam>
        /// <param name="visitor">The visitor the type-specific visit method should be invoked on.</param>
        public override TResult Accept<TResult>(MetamodelVisitor<TResult> visitor)
        {
            Assert.ArgumentNotNull(visitor, () => visitor);
            return visitor.VisitBlockStatement(this);
        }
    }
}

namespace SafetySharp.Metamodel
{
    using System;
    using System.Collections.Immutable;
    using Utilities;

    using SafetySharp.Metamodel.Declarations;
    using SafetySharp.Metamodel.Expressions;
    using SafetySharp.Metamodel.Statements;

    public abstract partial class MetamodelVisitor
    {
        /// <summary>
        ///     Visits a metamodel element of type <see cref="ClassDeclaration" />.
        /// </summary>
        /// <param name="classDeclaration">The <see cref="ClassDeclaration" /> instance that should be visited.</param>
        public virtual void VisitClassDeclaration(ClassDeclaration classDeclaration)
        {
            Assert.ArgumentNotNull(classDeclaration, () => classDeclaration);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="ComponentDeclaration" />.
        /// </summary>
        /// <param name="componentDeclaration">The <see cref="ComponentDeclaration" /> instance that should be visited.</param>
        public virtual void VisitComponentDeclaration(ComponentDeclaration componentDeclaration)
        {
            Assert.ArgumentNotNull(componentDeclaration, () => componentDeclaration);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="FieldDeclaration" />.
        /// </summary>
        /// <param name="fieldDeclaration">The <see cref="FieldDeclaration" /> instance that should be visited.</param>
        public virtual void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            Assert.ArgumentNotNull(fieldDeclaration, () => fieldDeclaration);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="PropertyDeclaration" />.
        /// </summary>
        /// <param name="propertyDeclaration">The <see cref="PropertyDeclaration" /> instance that should be visited.</param>
        public virtual void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
        {
            Assert.ArgumentNotNull(propertyDeclaration, () => propertyDeclaration);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="StateVariableDeclaration" />.
        /// </summary>
        /// <param name="stateVariableDeclaration">The <see cref="StateVariableDeclaration" /> instance that should be visited.</param>
        public virtual void VisitStateVariableDeclaration(StateVariableDeclaration stateVariableDeclaration)
        {
            Assert.ArgumentNotNull(stateVariableDeclaration, () => stateVariableDeclaration);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="GuardedCommandExpression" />.
        /// </summary>
        /// <param name="guardedCommandExpression">The <see cref="GuardedCommandExpression" /> instance that should be visited.</param>
        public virtual void VisitGuardedCommandExpression(GuardedCommandExpression guardedCommandExpression)
        {
            Assert.ArgumentNotNull(guardedCommandExpression, () => guardedCommandExpression);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="ExpressionStatement" />.
        /// </summary>
        /// <param name="expressionStatement">The <see cref="ExpressionStatement" /> instance that should be visited.</param>
        public virtual void VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            Assert.ArgumentNotNull(expressionStatement, () => expressionStatement);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="BlockStatement" />.
        /// </summary>
        /// <param name="blockStatement">The <see cref="BlockStatement" /> instance that should be visited.</param>
        public virtual void VisitBlockStatement(BlockStatement blockStatement)
        {
            Assert.ArgumentNotNull(blockStatement, () => blockStatement);
        }
    }

    public abstract partial class MetamodelVisitor<TResult>
    {
        /// <summary>
        ///     Visits a metamodel element of type <see cref="ClassDeclaration" />.
        /// </summary>
        /// <param name="classDeclaration">The <see cref="ClassDeclaration" /> instance that should be visited.</param>
        public virtual TResult VisitClassDeclaration(ClassDeclaration classDeclaration)
        {
            Assert.ArgumentNotNull(classDeclaration, () => classDeclaration);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="ComponentDeclaration" />.
        /// </summary>
        /// <param name="componentDeclaration">The <see cref="ComponentDeclaration" /> instance that should be visited.</param>
        public virtual TResult VisitComponentDeclaration(ComponentDeclaration componentDeclaration)
        {
            Assert.ArgumentNotNull(componentDeclaration, () => componentDeclaration);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="FieldDeclaration" />.
        /// </summary>
        /// <param name="fieldDeclaration">The <see cref="FieldDeclaration" /> instance that should be visited.</param>
        public virtual TResult VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            Assert.ArgumentNotNull(fieldDeclaration, () => fieldDeclaration);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="PropertyDeclaration" />.
        /// </summary>
        /// <param name="propertyDeclaration">The <see cref="PropertyDeclaration" /> instance that should be visited.</param>
        public virtual TResult VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
        {
            Assert.ArgumentNotNull(propertyDeclaration, () => propertyDeclaration);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="StateVariableDeclaration" />.
        /// </summary>
        /// <param name="stateVariableDeclaration">The <see cref="StateVariableDeclaration" /> instance that should be visited.</param>
        public virtual TResult VisitStateVariableDeclaration(StateVariableDeclaration stateVariableDeclaration)
        {
            Assert.ArgumentNotNull(stateVariableDeclaration, () => stateVariableDeclaration);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="GuardedCommandExpression" />.
        /// </summary>
        /// <param name="guardedCommandExpression">The <see cref="GuardedCommandExpression" /> instance that should be visited.</param>
        public virtual TResult VisitGuardedCommandExpression(GuardedCommandExpression guardedCommandExpression)
        {
            Assert.ArgumentNotNull(guardedCommandExpression, () => guardedCommandExpression);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="ExpressionStatement" />.
        /// </summary>
        /// <param name="expressionStatement">The <see cref="ExpressionStatement" /> instance that should be visited.</param>
        public virtual TResult VisitExpressionStatement(ExpressionStatement expressionStatement)
        {
            Assert.ArgumentNotNull(expressionStatement, () => expressionStatement);
            return default(TResult);
        }

        /// <summary>
        ///     Visits a metamodel element of type <see cref="BlockStatement" />.
        /// </summary>
        /// <param name="blockStatement">The <see cref="BlockStatement" /> instance that should be visited.</param>
        public virtual TResult VisitBlockStatement(BlockStatement blockStatement)
        {
            Assert.ArgumentNotNull(blockStatement, () => blockStatement);
            return default(TResult);
        }
    }
}
