using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTrees.Task2.ExpressionMapping
{
    public class MappingGenerator<TSource, TDestination>
    {
        private readonly Dictionary<string, LambdaExpression> _mappings =
            new Dictionary<string, LambdaExpression>();

        public MappingGenerator<TSource, TDestination> MapMember<T>(
            Expression<Func<TDestination, T>> destinationMemberExpression,
            Expression<Func<TSource, T>> sourceMappingExpression)
        {
            _mappings.Add(((MemberExpression) destinationMemberExpression.Body).Member.Name, sourceMappingExpression);
            return this;
        }

        public Mapper<TSource, TDestination> Generate()
            => new Mapper<TSource, TDestination>(GetMapFunction().Compile());

        private Expression<Func<TSource, TDestination>> GetMapFunction()
        {
            var sourceInstance = Expression.Parameter(typeof(TSource), "sourceInstance");
            var destinationInstance = Expression.Variable(typeof(TDestination), "destinationInstance");

            var destinationConstructor = GetDestinationConstructor();

            var expressions = new List<Expression>
            {
                sourceInstance,
                Expression.Assign(destinationInstance, Expression.New(destinationConstructor))
            };

            var sourceMembers = GetPublicFieldsAndProperties(typeof(TSource));
            var destinationMembers = GetPublicFieldsAndProperties(typeof(TDestination));

            expressions.AddRange(from destinationMember in destinationMembers
                let sourceValue = GetSourceValueExpression(destinationMember, sourceInstance, sourceMembers)
                where sourceValue != null
                let destinationValue = GetMemberExpression(destinationInstance, destinationMember)
                select Expression.Assign(destinationValue, sourceValue));

            expressions.Add(destinationInstance);

            var body = Expression.Block(new[] {destinationInstance}, expressions);
            return Expression.Lambda<Func<TSource, TDestination>>(body, sourceInstance);
        }

        private Expression GetSourceValueExpression(MemberInfo destinationMember, ParameterExpression sourceInstance,
            IEnumerable<MemberInfo> sourceMembers)
        {
            Expression sourceValue = null;

            if (_mappings.ContainsKey(destinationMember.Name))
                sourceValue = Expression.Invoke(_mappings[destinationMember.Name], sourceInstance);
            else
            {
                var sourceMember = sourceMembers.FirstOrDefault(sp => destinationMember.Name == sp.Name);
                if (sourceMember != null)
                    sourceValue = GetMemberExpression(sourceInstance, sourceMember);
            }

            return sourceValue;
        }

        private static ConstructorInfo GetDestinationConstructor()
            => typeof(TDestination).GetTypeInfo().DeclaredConstructors
                .First(c => !c.IsStatic && c.GetParameters().Length == 0);

        private static IEnumerable<MemberInfo> GetPublicFieldsAndProperties(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(type.GetProperties(bindingFlags)).ToArray();
        }

        private static Expression GetMemberExpression(ParameterExpression instance, MemberInfo member)
            => member is PropertyInfo propertyInfo
                ? Expression.Property(instance, propertyInfo)
                : Expression.Field(instance, (FieldInfo)member);
    }
}
