using ExpressionTrees.Task2.ExpressionMapping.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionTrees.Task2.ExpressionMapping.Tests
{
    [TestClass]
    public class ExpressionMappingTests
    {
        private readonly Foo _foo = new Foo
        {
            FirstName = "Viktor",
            LastName = "Yakubovych",
            Patronymic = "Petrovych",
            Age = 26
        };

        [TestMethod]
        public void Mapper_ShouldMapFieldToFieldWithTheSameName()
        {
            var mapGenerator = new MappingGenerator<Foo, Bar>();
            var mapper = mapGenerator.Generate();

            var res = mapper.Map(_foo);

            Assert.AreEqual("Viktor", res.FirstName);
        }

        [TestMethod]
        public void Mapper_ShouldMapFieldToPropertyWithTheSameName()
        {
            var mapGenerator = new MappingGenerator<Foo, Bar>();
            var mapper = mapGenerator.Generate();

            var res = mapper.Map(_foo);

            Assert.AreEqual("Yakubovych", res.LastName);
        }

        [TestMethod]
        public void Mapper_ShouldMapPropertyToFieldWithTheSameName()
        {
            var mapGenerator = new MappingGenerator<Foo, Bar>();
            var mapper = mapGenerator.Generate();

            var res = mapper.Map(_foo);

            Assert.AreEqual("Petrovych", res.Patronymic);
        }

        [TestMethod]
        public void Mapper_ShouldMapPropertyToPropertyWithTheSameName()
        {
            var mapGenerator = new MappingGenerator<Foo, Bar>();
            var mapper = mapGenerator.Generate();

            var res = mapper.Map(_foo);

            Assert.AreEqual(26, res.Age);
        }

        [TestMethod]
        public void Mapper_ShouldMapSameTypeUsingCustomRules()
        {
            var mapGenerator = new MappingGenerator<Foo,Bar>();
            var mapper = mapGenerator
                .MapMember(b => b.FullName, f => $"{f.FirstName} {f.LastName}")
                .Generate();

            var res = mapper.Map(new Foo
            {
                FirstName = "Viktor",
                LastName = "Yakubovych"
            });

            Assert.AreEqual("Viktor Yakubovych", res.FullName);
        }

        [TestMethod]
        public void Mapper_ShouldMapDifferentTypeUsingCustomRules()
        {
            var mapGenerator = new MappingGenerator<Foo, Bar>();
            var mapper = mapGenerator
                .MapMember(b => b.IsAdult, f => f.Age >= 18)
                .Generate();

            var res = mapper.Map(_foo);

            Assert.AreEqual(true, res.IsAdult);
        }
    }
}
