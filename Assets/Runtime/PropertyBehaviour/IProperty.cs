using System;

namespace Oculus.Interaction.Bullshiddo
{
    /// <summary>
    /// an interface for an observable value
    /// </summary>
    public interface IProperty
    {
        event Action WhenChanged;
    }

    /// <summary>
    /// an interface for an observable value
    /// </summary>
    public interface IProperty<T> : IProperty
    {
        T Value { get; set; }
    }
}
