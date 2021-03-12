using Halforbit.ObjectTools.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Halforbit.ObjectTools.Tests.Validation
{
    public class ValidationErrorsTests
    {
        [Fact]
        public void Regarding_Require()
        {
            var person = new Person("Steve", string.Empty);

            var result = ValidationErrors.Empty
                .Regarding(person)
                .Require(p => p.FirstName)
                .Require(p => p.LastName);

            Assert.Equal(1, result.Count);

            Assert.Equal("LastName required.", result[0].Message);
        }

        [Fact]
        public void Regarding_Require_Anonymous()
        {
            var person = new Person("Steve", string.Empty);

            var result = ValidationErrors.Empty
                .Regarding(person)
                .Require(p => new
                {
                    p.FirstName,
                    p.LastName
                });

            Assert.Equal(1, result.Count);

            Assert.Equal("LastName required.", result[0].Message);
        }
    }

    public class Person
    {
        public Person(
            string firstName,
            string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }
    }
}
