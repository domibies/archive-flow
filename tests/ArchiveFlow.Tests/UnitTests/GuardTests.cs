
using FluentAssertions;
using ArchiveFlow.Utilities;

namespace ArchiveFlow.Tests.UnitTests
{
    public class GuardTests
    {
        [Fact]
        public void AgainstNull_ValueIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            string paramName = "param";
            object? value = null;

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainstNull(paramName, value))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AgainstNull_ValueIsNotNull_DoesNotThrowException()
        {
            // Arrange
            string paramName = "param";
            object value = new object();

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainstNull(paramName, value))
                .Should().NotThrow();
        }

        [Fact]
        public void AgainsNullOrWhiteSpace_ValueIsNullOrWhiteSpace_ThrowsArgumentException()
        {
            // Arrange
            string paramName = "param";
            string value = "   ";

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainsNullOrWhiteSpace(paramName, value))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void AgainsNullOrWhiteSpace_ValueIsNotNullOrWhiteSpace_DoesNotThrowException()
        {
            // Arrange
            string paramName = "param";
            string value = "test";

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainsNullOrWhiteSpace(paramName, value))
                .Should().NotThrow();
        }

        [Fact]
        public void AgainstSmallerThan_ValueIsSmallerThanCompare_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string paramName = "param";
            int value = 5;
            int compare = 10;

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainstSmallerThan(paramName, value, compare))
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AgainstSmallerThan_ValueIsEqualToCompare_DoesNotThrowException()
        {
            // Arrange
            string paramName = "param";
            int value = 10;
            int compare = 10;

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainstSmallerThan(paramName, value, compare))
                .Should().NotThrow();
        }

        [Fact]
        public void AgainstSmallerThan_ValueIsGreaterThanCompare_DoesNotThrowException()
        {
            // Arrange
            string paramName = "param";
            int value = 15;
            int compare = 10;

            // Act & Assert
            FluentActions.Invoking(() => Guard.AgainstSmallerThan(paramName, value, compare))
                .Should().NotThrow();
        }

        [Fact]
        public void ShouldStartWith_ValueDoesNotStartWithStart_ThrowsArgumentException()
        {
            // Arrange
            string paramName = "param";
            string value = "test";
            string start = "abc";

            // Act & Assert
            FluentActions.Invoking(() => Guard.ShouldStartWith(paramName, value, start))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldStartWith_ValueStartsWithStart_DoesNotThrowException()
        {
            // Arrange
            string paramName = "param";
            string value = "abcTest";
            string start = "abc";

            // Act & Assert
            FluentActions.Invoking(() => Guard.ShouldStartWith(paramName, value, start))
                .Should().NotThrow();
        }
    }
}
