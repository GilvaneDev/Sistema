namespace Sige_Erp.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TestExample()
        {
            // Arrange
            int a = 1;
            int b = 1;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(2, result);
        }
    }
}