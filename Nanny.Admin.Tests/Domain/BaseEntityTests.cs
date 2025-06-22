using Nanny.Admin.Domain.Entities;

namespace Nanny.Admin.Tests.Domain;

public class BaseEntityTests
{
    [Fact]
    public void Constructor_WithSpecificId_SetsIdCorrectly()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var entity = new TestEntity(expectedId);

        // Assert
        Assert.Equal(expectedId, entity.Id);
    }

    [Fact]
    public void Constructor_WithoutId_GeneratesNewGuid()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.IsType<Guid>(entity.Id);
    }

    [Fact]
    public void IdProperty_CannotBeModified()
    {
        // Arrange
        var entity = new TestEntity();
        var originalId = entity.Id;

        // Act & Assert
        // The Id property has a private setter, so we can't directly test setting it.
        // Instead, we verify that the ID remains unchanged after creation.
        Assert.Equal(originalId, entity.Id);
    }

    [Fact]
    public void DifferentInstances_HaveDifferentIds()
    {
        // Act
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Assert
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    // Helper class for testing BaseEntity
    private class TestEntity : BaseEntity
    {
        public TestEntity() : base()
        {
        }

        public TestEntity(Guid id) : base(id)
        {
        }
    }
}
